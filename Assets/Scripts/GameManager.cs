using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    // Temporarily stores the player's selected blocker creature during the
    // Choose Blockers phase. The player first selects their own creature and
    // then clicks an attacking enemy creature to assign the block.
    public CreatureCard selectedBlockerForBlocking = null;

    public Player humanPlayer;
    public Player aiPlayer;
    public TMP_Text manaPoolText;
    public TMP_Text playerLifeText;
    public TMP_Text enemyLifeText;
    public TMP_Text enemyHandText;
    public TMP_Text playerDeckCountText;
    public TMP_Text playerGraveyardCountText;
    public TMP_Text enemyDeckCountText;
    public TMP_Text enemyGraveyardCountText;

    public Transform playerHandArea;
    public Transform playerBattlefieldArea;
    public Transform playerGraveyardArea;
    public Transform playerLandArea;
    public Transform playerArtifactArea;
    public Transform playerEnchantmentArea;

    public Transform stackZone; //shared zone

    public Transform aiBattlefieldArea;
    public Transform aiGraveyardArea;
    public Transform aiLandArea;
    public Transform aiArtifactArea;
    public Transform aiEnchantmentArea;

    public GameObject cardPrefab;
    public GameObject manaVFXPrefab;
    public GameObject bloodSplatPrefab;
    public GameObject deathPlaceholderPrefab;
    public GameObject artifactDeathPrefab;
    public GameObject playerLifeContainer;
    public GameObject enemyLifeContainer;
    public GameObject floatingDamagePrefab;
    public GameObject favouritePopupPrefab;
    public GameObject triggerVFXPrefab;

    // Tracks cumulative life changes during a combat step
    private TMP_Text playerLifeDeltaText;
    private TMP_Text enemyLifeDeltaText;
    private int playerLifeDelta = 0;
    private int enemyLifeDelta = 0;
    private Coroutine playerDeltaRoutine;
    private Coroutine enemyDeltaRoutine;
    // When true, life delta text will not automatically fade out.
    private bool lifeDeltaFadeDeferred = false;

    public ArtifactCard targetingArtifact;
    public EquipmentCard targetingEquipment;
    public CreatureCard targetingCreatureActivated;

    public Sprite blueIcon, whiteIcon, blackIcon, redIcon, greenIcon;

    public Image whiteManaIcon;
    public TMP_Text whiteManaText;
    public Image blueManaIcon;
    public TMP_Text blueManaText;
    public Image blackManaIcon;
    public TMP_Text blackManaText;
    public Image redManaIcon;
    public TMP_Text redManaText;
    public Image greenManaIcon;
    public TMP_Text greenManaText;
    public Image colorlessManaIcon;
    public TMP_Text colorlessManaText;

    // Name of the player's favourite card selected in the deck editor
    private string favouriteCardName;

    public List<CardVisual> activeCardVisuals = new List<CardVisual>();
    public List<CreatureCard> selectedAttackers = new List<CreatureCard>();
    public List<CreatureCard> currentAttackers = new List<CreatureCard>();

    public Dictionary<CreatureCard, List<CreatureCard>> blockingAssignments = new Dictionary<CreatureCard, List<CreatureCard>>();

    public bool isStackBusy = false;
    public int pendingStackEffects = 0;
    public bool gameOver = false;
    public int pendingGraveyardAnimations = 0;
    public bool graveyardViewActive = false;
    public GraveyardUIManager graveyardUIManager;
    public bool preventAllCombatDamageThisTurn = false;

    private bool skipStackWait = false;

    // Tracks cards already moved to the graveyard this turn to
    // prevent duplicate death triggers if CheckDeaths runs again.
    private HashSet<Card> processedDeaths = new HashSet<Card>();

    public SorceryCard targetingSorcery;
    public AuraCard targetingAura;
    public Player targetingPlayer;
    public CardVisual targetingVisual;
    public bool isTargetingMode = false;

    public Card targetingCreature;
    public CardAbility targetingAbility;
    public Card targetingCreatureOptional;
    public CardAbility optionalAbility;
    public Player optionalTargetPlayer;

    private struct TriggeredAbilityContext
    {
        public CardAbility ability;
        public Player owner;
        public Card source;
        public Card target;
        public Card deadCreature;

        public TriggeredAbilityContext(CardAbility ability, Player owner, Card source, Card target, Card deadCreature)
        {
            this.ability = ability;
            this.owner = owner;
            this.source = source;
            this.target = target;
            this.deadCreature = deadCreature;
        }
    }

    private readonly Queue<TriggeredAbilityContext> triggerQueue = new Queue<TriggeredAbilityContext>();
    private bool processingTriggerQueue = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool IsStackActive()
    {
        return isStackBusy || pendingStackEffects > 0;
    }

    public void ResolveStackNow()
    {
        skipStackWait = true;
    }

    public IEnumerator WaitForStackOrSkip(float seconds)
    {
        skipStackWait = false;
        float elapsed = 0f;
        while (elapsed < seconds && !skipStackWait)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        skipStackWait = false;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        humanPlayer = new Player();
        aiPlayer = new Player();

        // Load favourite card from deck editor if one was set
        favouriteCardName = DeckHolder.FavouriteCardName;

        Debug.Log("Loading deck for zone ID: " + BattleData.CurrentZoneId);

        DeckDatabase.BuildStartingDeck(humanPlayer);

        if (!string.IsNullOrEmpty(BattleData.CurrentDeckKey))
        {
            Debug.Log("Loading deck by key: " + BattleData.CurrentDeckKey);
            LoadDeckByKey(aiPlayer, BattleData.CurrentDeckKey);
        }
        else
        {
            Debug.LogWarning("No deckKey set — using fallback starter deck.");
            DeckDatabase.BuildStarterDeck(aiPlayer);
        }

        PutStartingPermanentsOnBattlefield(humanPlayer);
        PutStartingPermanentsOnBattlefield(aiPlayer);

        ShuffleDeck(humanPlayer);
        ShuffleDeck(aiPlayer);

        DrawCards(humanPlayer, 7);
        DrawCards(aiPlayer, 7);
        UpdateUI();
    }

    void PutStartingPermanentsOnBattlefield(Player player)
    {
        if (player.StartingPermanents == null || player.StartingPermanents.Count == 0)
            return;

        foreach (var card in player.StartingPermanents)
        {
            SummonToken(card, player);
        }

        player.StartingPermanents.Clear();
    }

    void Update()
    {
        // Debug helpers for testing win and draw functions. Commented out for release build.
        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     if (!string.IsNullOrEmpty(BattleData.CurrentZoneId))
        //     {
        //         Debug.Log("[DEV] Instant win triggered for zone ID: " + BattleData.CurrentZoneId);
        //         gameOver = true;
        //         UnityEngine.Object.FindFirstObjectByType<WinScreenUI>().ShowWinScreen();
        //     }
        //     else
        //     {
        //         Debug.LogWarning("[DEV] No zone ID found — can't win");
        //     }
        // }

        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     GameManager.Instance.DrawCard(GameManager.Instance.humanPlayer);
        //     Debug.Log("D key pressed — drew a card.");
        // }
    }

    public void ShuffleDeck(Player player)
    {
        for (int i = 0; i < player.Deck.Count; i++)
        {
            Card temp = player.Deck[i];
            int randomIndex = Random.Range(i, player.Deck.Count);
            player.Deck[i] = player.Deck[randomIndex];
            player.Deck[randomIndex] = temp;
        }
    }

    public void DrawCards(Player player, int amount)
    {
        if (amount <= 0)
            return;

        for (int i = 0; i < amount; i++)
        {
            if (gameOver)
                break;

            // only play the draw sound on the first card if this is the human player
            bool playSfx = (player == humanPlayer && i == 0);
            DrawCard(player, playSfx);

            // Drawing from an empty deck ends the game inside DrawCard.
            if (gameOver)
                break;
        }
    }
    public void DrawCard(Player player, bool playSfx = true)
    {
        if (player.Deck.Count == 0)
        {
            if (player == aiPlayer)
            {
                Debug.Log("AI tried to draw from an empty deck — player wins by mill.");
                // Card reward logic temporarily disabled.
                // CardData reward = PlayerCollection.AddRandomCard();
                gameOver = true;
                UnityEngine.Object.FindFirstObjectByType<WinScreenUI>().ShowWinScreen(null);
            }
            else if (player == humanPlayer)
            {
                Debug.Log("Player tried to draw from an empty deck — player loses by mill.");
                gameOver = true;
                UnityEngine.Object.FindFirstObjectByType<WinScreenUI>().ShowLoseScreen();
            }
            return;
        }

        Card card = player.Deck[0];
        player.Deck.RemoveAt(0);
        player.Hand.Add(card);

        if (player == humanPlayer)
        {
            GameObject obj = Instantiate(cardPrefab, playerHandArea);
            CardVisual visual = obj.GetComponent<CardVisual>();

            CardData sourceData = CardDatabase.GetCardData(card.cardName);
            visual.Setup(card, this, sourceData);

            activeCardVisuals.Add(visual);
        }

        NotifyCardDrawn(player, 1);
        NotifyOpponentDraw(player);
        if (player == humanPlayer && playSfx)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.drawCard);
        }
    }

    public void PlayCard(Player player, CardVisual visual)
    {
        if (IsStackActive())
        {
            Debug.Log("A spell is already on the stack. Please wait.");
            return;
        }

        Card card = visual.linkedCard;
        if (!CanPlayCardNow(player, card))
            return;

        if (card is LandCard land)
        {
            TryPlayLand(player, visual, land);
            return;
        }

        if (card is CreatureCard creature)
        {
            TryCastCreature(player, visual, card, creature);
            return;
        }

        if (card is SorceryCard sorcery)
        {
            TryCastSorcery(player, visual, card, sorcery);
            return;
        }

        if (card is ArtifactCard artifact)
        {
            TryCastArtifact(player, visual, card, artifact);
            return;
        }

        if (card is AuraCard aura)
        {
            Debug.Log("Aura requires target — entering targeting mode.");
            BeginAuraTargetSelection(aura, player, visual);
            return;
        }

        if (card is EnchantmentCard enchantment)
        {
            TryCastEnchantment(player, visual, card, enchantment);
            return;
        }

        Debug.LogWarning("Unhandled card type played: " + card.cardName);
    }

    private bool CanPlayCardNow(Player player, Card card)
    {
        // MTG timing guard for non-instant card types in this project:
        // lands and permanent/sorcery spells can only be played on your own main phase.
        if (TurnSystem.Instance != null)
        {
            bool isPlayersTurn = (TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human && player == humanPlayer) ||
                                 (TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.AI && player == aiPlayer);
            bool isMainPhase = TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 ||
                               TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2;
            CardData cardData = CardDatabase.GetCardData(card.cardName);
            bool isInstantSpell = cardData != null && cardData.cardType == CardType.Instant;
            bool requiresMainPhaseTiming = card is LandCard ||
                                          card is CreatureCard ||
                                          (card is SorceryCard && !isInstantSpell) ||
                                          card is ArtifactCard ||
                                          card is EnchantmentCard ||
                                          card is AuraCard;

            if (requiresMainPhaseTiming && (!isPlayersTurn || !isMainPhase))
            {
                Debug.Log("This card can only be played during its controller's own main phase.");
                return false;
            }
        }

        if (IsOnlyCastCreatureSpellsActive() && !(card is CreatureCard) && !(card is LandCard))
        {
            Debug.Log("Anti-Magic Grid prevents casting non-creature spells.");
            return false;
        }

        return true;
    }

    private void TryPlayLand(Player player, CardVisual visual, LandCard land)
    {
        if (player.hasPlayedLandThisTurn)
        {
            Debug.Log("You already played a land this turn!");
            return;
        }

        player.Battlefield.Add(land);
        player.Hand.Remove(land);
        player.hasPlayedLandThisTurn = true;

        if (land.entersTapped || IsAllPermanentsEnterTappedActive())
        {
            land.isTapped = true;
            Debug.Log($"{land.cardName} enters tapped (static effect or base).");
        }

        land.OnEnterPlay(player);
        NotifyLandEntered(land, player);

        visual.transform.SetParent(player == humanPlayer ? playerLandArea : aiLandArea, false);
        visual.isInBattlefield = true;
        visual.UpdateVisual();
        SoundManager.Instance.PlaySound(SoundManager.Instance.cardPlay);

        AwardFavouriteCardCoins(land, player);
    }

    private void TryCastCreature(Player player, CardVisual visual, Card card, CreatureCard creature)
    {
        var cost = BuildSpellCost(card, player);
        int reduction = GetCreatureCostReduction(player);

        CardData data = CardDatabase.GetCardData(card.cardName);
        if (data != null && data.subtypes.Contains("Beast"))
            reduction += GetBeastCreatureCostReduction(player);

        ApplyColorlessReduction(cost, reduction);

        if (!TryMoveSpellToStack(player, visual, card, cost, player == humanPlayer))
        {
            Debug.Log("Not enough colored mana to cast this creature.");
            return;
        }

        StartCoroutine(ResolveCreatureAfterDelay(creature, visual, player));
    }

    private void TryCastSorcery(Player player, CardVisual visual, Card card, SorceryCard sorcery)
    {
        if (sorcery.requiresTarget)
        {
            Debug.Log("This sorcery requires a target — entering targeting mode.");
            BeginTargetSelection(sorcery, player, visual);
            return;
        }

        var cost = BuildSpellCost(sorcery, player);
        if (!TryMoveSpellToStack(player, visual, card, cost, true))
        {
            Debug.Log("Not enough colored mana to cast this sorcery.");
            return;
        }

        StartCoroutine(ResolveSorceryAfterDelay(sorcery, visual, player));
    }

    private void TryCastArtifact(Player player, CardVisual visual, Card card, ArtifactCard artifact)
    {
        var cost = BuildSpellCost(artifact, player);
        CardData artData = CardDatabase.GetCardData(card.cardName);
        int reduction = (artData != null && artData.subtypes.Contains("Potion")) ? GetPotionCostReduction(player) : 0;
        ApplyColorlessReduction(cost, reduction);

        if (!TryMoveSpellToStack(player, visual, card, cost, player == humanPlayer))
        {
            Debug.Log("Not enough colored mana to play this artifact.");
            return;
        }

        StartCoroutine(ResolveArtifactAfterDelay(artifact, visual, player));
    }

    private void TryCastEnchantment(Player player, CardVisual visual, Card card, EnchantmentCard enchantment)
    {
        var cost = BuildSpellCost(enchantment, player);
        if (!TryMoveSpellToStack(player, visual, card, cost, player == humanPlayer))
        {
            Debug.Log("Not enough colored mana to play this enchantment.");
            return;
        }

        StartCoroutine(ResolveEnchantmentAfterDelay(enchantment, visual, player));
    }

    private Dictionary<string, int> BuildSpellCost(Card card, Player player)
    {
        var cost = GetManaCostBreakdown(card.manaCost, card.color);
        int tax = GetOpponentSpellTax(player);
        if (tax > 0)
        {
            if (!cost.ContainsKey("Colorless"))
                cost["Colorless"] = 0;

            cost["Colorless"] += tax;
        }

        return cost;
    }

    private void ApplyColorlessReduction(Dictionary<string, int> cost, int reduction)
    {
        if (reduction > 0 && cost.ContainsKey("Colorless"))
            cost["Colorless"] = Mathf.Max(0, cost["Colorless"] - reduction);
    }

    private bool TryMoveSpellToStack(Player player, CardVisual visual, Card card, Dictionary<string, int> cost, bool shouldUpdateUi)
    {
        if (!player.ColoredMana.CanPay(cost))
            return false;

        isStackBusy = true;
        player.ColoredMana.Pay(cost);

        if (card.hasXCost)
        {
            card.xValue = player.ColoredMana.Total();
            if (card.xValue > 0)
                player.ColoredMana.SpendGeneric(card.xValue);
        }

        card.owner = player;
        player.Hand.Remove(card);

        if (shouldUpdateUi)
            UpdateUI();

        visual.transform.SetParent(stackZone, false);
        visual.isInStack = true;
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;
        visual.transform.localScale = Vector3.one;
        SoundManager.Instance.PlaySound(SoundManager.Instance.cardPlay);
        return true;
    }

    public void TapLandForMana(LandCard land, Player player)
        {
            if (land.isTapped)
                return;

            land.isTapped = true;

            var colors = CardDatabase.GetCardData(land.cardName).color;
            string color = (colors != null && colors.Count > 0) ? colors[0] : "Colorless";

            switch (color)
            {
                case "White": player.ColoredMana.White++; break;
                case "Blue": player.ColoredMana.Blue++; break;
                case "Black": player.ColoredMana.Black++; break;
                case "Red": player.ColoredMana.Red++; break;
                case "Green": player.ColoredMana.Green++; break;
                default:
                    Debug.LogWarning($"Unknown land color for mana: {color}");
                    break;
            }

            if (player == humanPlayer)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.tap_for_mana);
                ShowManaVFX(land);
                UpdateUI();
            }
        }

    private void ShowManaVFX(LandCard land)
        {
            CardVisual visual = FindCardVisual(land);
            if (visual == null)
            {
                Debug.LogWarning("No visual found for land card " + land.cardName);
                return;
            }

            Vector3 spawnPos = visual.transform.position;
            spawnPos.z = 0f;

            Sprite iconSprite = GetManaIconForCardName(land.cardName);

            GameObject vfx = Instantiate(manaVFXPrefab, spawnPos, Quaternion.identity);
            vfx.GetComponentInChildren<SpriteRenderer>().sprite = iconSprite;

            Debug.Log("Spawning mana VFX at: " + spawnPos);
        }

    private Sprite GetManaIconForCardName(string cardName)
        {
            CardData data = CardDatabase.GetCardData(cardName);
            if (data == null)
            {
                Debug.LogWarning("No card data found for: " + cardName);
                return null;
            }

            string primaryColor = (data.color != null && data.color.Count > 0) ? data.color[0] : "None";

            switch (primaryColor)
            {
                case "Blue": return blueIcon;
                case "White": return whiteIcon;
                case "Black": return blackIcon;
                case "Red": return redIcon;
                case "Green": return greenIcon;
                default:
                    Debug.LogWarning("Unknown color: " + data.color);
                    return null;
            }
        }

    public void SendToGraveyard(Card card, Player owner, bool fromStack = false)
        {
            if (processedDeaths.Contains(card))
                return;

            processedDeaths.Add(card);

            bool diedFromBattlefield = owner.Battlefield.Contains(card);
            bool discardedFromHand = owner.Hand.Contains(card);
            Player graveyardOwner = card.owner ?? owner;

            if (fromStack)
            {
                Debug.Log($"{card.cardName} is going to the graveyard from the stack — skipping VFX.");

                CardVisual stackVisual = FindCardVisual(card);
                if (stackVisual != null)
                {
                    activeCardVisuals.Remove(stackVisual);
                    Destroy(stackVisual.gameObject);
                }

                if (!card.isToken)
                {
                GameObject visualGO = Instantiate(cardPrefab,
                    graveyardOwner == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
                CardVisual stackGraveyardVisual = visualGO.GetComponent<CardVisual>();
                stackGraveyardVisual.Setup(card, this);
                stackGraveyardVisual.transform.localPosition = Vector3.zero;
                stackGraveyardVisual.UpdateGraveyardVisual();
                // Ensure logo and count remain on top
                // Last card in should appear on top, so send to end of hierarchy
                stackGraveyardVisual.transform.SetAsLastSibling();
                EnsureGraveyardCounterOnTop(graveyardOwner);

                activeCardVisuals.Add(stackGraveyardVisual);
            }

            graveyardOwner.Graveyard.Add(card);
            UpdateUI();
            return;
        }


            owner.Battlefield.Remove(card);
            owner.Hand.Remove(card);

            bool removeFromGameOnDeath = card.exileSelfOnDeath && diedFromBattlefield;

            if (diedFromBattlefield && card is CreatureCard deadCreature)
            {
                currentAttackers.Remove(deadCreature);
                selectedAttackers.Remove(deadCreature);
                if (selectedBlockerForBlocking == deadCreature)
                    selectedBlockerForBlocking = null;

                foreach (var creature in humanPlayer.Battlefield.Concat(aiPlayer.Battlefield).OfType<CreatureCard>())
                {
                    creature.blockedByThisBlocker.Remove(deadCreature);
                    if (creature.blockingThisAttacker == deadCreature)
                        creature.blockingThisAttacker = null;

                    var vis = FindCardVisual(creature);
                    if (vis != null)
                        vis.UpdateVisual();
                }
            }

            if (discardedFromHand)
            {
                NotifyOpponentDiscard(owner);
                NotifyPlayerDiscard(owner);
            }

            Debug.Log($"{card.cardName} is being sent to the graveyard.");

            if (card is CreatureCard && (diedFromBattlefield || discardedFromHand))
            {
                NotifyCreatureDiesOrDiscarded(card, owner);
            }
            if (card is CreatureCard && diedFromBattlefield)
            {
                NotifyCreatureDies(card, owner);
            }

            if (diedFromBattlefield)
            {
                card.OnLeavePlay(owner);
                if (card is LandCard)
                    NotifyLandLeft(card, owner);

                if (card is CreatureCard leftCreature)
                {
                    foreach (var player in new[] { humanPlayer, aiPlayer })
                    {
                        var attached = player.Battlefield
                            .OfType<AuraCard>()
                            .Where(a => a.attachedTo == leftCreature)
                            .ToList();

                        foreach (var aura in attached)
                            SendToGraveyard(aura, player);

                        var equips = player.Battlefield
                            .OfType<EquipmentCard>()
                            .Where(e => e.equippedTo == leftCreature)
                            .ToList();

                        foreach (var eq in equips)
                        {
                            eq.Unequip();
                            FindCardVisual(eq)?.UpdateVisual();
                        }
                    }
                }
            }

            card.isTapped = false;

            CardVisual visual = FindCardVisual(card); // <-- Moved up
            if (visual != null)
                visual.EnableTargetingHighlight(false); // ensure highlight removed
            if (visual != null && visual.tapIcon != null)
                visual.tapIcon.SetActive(false);

            if (discardedFromHand && visual != null)
            {
                StartCoroutine(ShowHandDiscardVFX(card, owner, visual));
                return;
            }

            if (card is CreatureCard thisDeadCreature)
            {
                thisDeadCreature.hasSummoningSickness = false;
                thisDeadCreature.toughness = thisDeadCreature.baseToughness;

                if (visual != null)
                {
                    visual.sicknessText.text = "";
                }

                if (card.isToken && diedFromBattlefield)
                {
                    if (visual != null)
                    {
                        StartCoroutine(ShowDeathVFXAndDelayLayout(card, owner, visual, removeFromGame: removeFromGameOnDeath));
                    }
                    return;
                }

                if (diedFromBattlefield && visual != null)
                {
                    StartCoroutine(ShowDeathVFXAndDelayLayout(card, owner, visual, removeFromGame: removeFromGameOnDeath));
                    return;
                }
            }

            if (card is ArtifactCard && diedFromBattlefield && visual != null)
            {
                StartCoroutine(ShowDeathVFXAndDelayLayout(card, owner, visual, artifactDeathPrefab, removeFromGameOnDeath));
                return;
            }

            if (removeFromGameOnDeath)
            {
                if (visual != null)
                {
                    activeCardVisuals.Remove(visual);
                    Destroy(visual.gameObject);
                }

                Debug.Log($"{card.cardName} is removed from the game after dying.");
                UpdateUI();
                return;
            }

            // Fallback: create graveyard visual normally
            CardVisual graveyardVisual = FindCardVisual(card);
            if (graveyardVisual == null)
            {
                GameObject visualGO = Instantiate(cardPrefab,
                    graveyardOwner == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
                graveyardVisual = visualGO.GetComponent<CardVisual>();
                graveyardVisual.Setup(card, this);
                activeCardVisuals.Add(graveyardVisual);
            }

            graveyardVisual.transform.SetParent(graveyardOwner == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
            graveyardVisual.transform.localPosition = Vector3.zero;
            graveyardVisual.UpdateGraveyardVisual();
            // Ensure graveyard UI elements stay above the cards
            // Newest card should appear on top
            graveyardVisual.transform.SetAsLastSibling();
            EnsureGraveyardCounterOnTop(graveyardOwner);

            graveyardOwner.Graveyard.Add(card);
            UpdateUI();
        }

    public (int playerDamage, int aiDamage) ResolveCombat()
    {
        int playerDamage = 0;
        int aiDamage = 0;

        foreach (var attacker in currentAttackers)
        {
            // Clamp negative power to zero when dealing damage and handle damage prevention
            int attackerDamage = preventAllCombatDamageThisTurn || attacker.keywordAbilities.Contains(KeywordAbility.CantDealCombatDamage)
                ? 0
                : Mathf.Max(attacker.power, 0);
            var blockers = attacker.blockedByThisBlocker;

            if (blockers != null && blockers.Count > 0)
            {
                int remainingDamage = attackerDamage;
                int totalDamageFromBlockers = 0;

                bool attackerHasTrample = attacker.keywordAbilities.Contains(KeywordAbility.Trample);
                for (int i = 0; i < blockers.Count; i++)
                {
                    var blocker = blockers[i];
                    bool attackerProtected = blocker.color.Any(c => attacker.keywordAbilities.Contains(ProtectionUtils.GetProtectionKeyword(c)));
                    bool blockerProtected = attacker.color.Any(c => blocker.keywordAbilities.Contains(ProtectionUtils.GetProtectionKeyword(c)));

                    int damageToBlocker = 0;
                    if (!blockerProtected)
                    {
                        bool isLastBlocker = i == blockers.Count - 1;
                        if (!attackerHasTrample && isLastBlocker)
                            damageToBlocker = remainingDamage;
                        else
                            damageToBlocker = Mathf.Min(remainingDamage, blocker.toughness);

                        blocker.TakeDamage(damageToBlocker);
                        if (attacker.keywordAbilities.Contains(KeywordAbility.Deathtouch) && damageToBlocker > 0)
                            blocker.Kill();
                        remainingDamage -= damageToBlocker;
                    }

                    int damageFromBlocker = (preventAllCombatDamageThisTurn || attackerProtected || blocker.keywordAbilities.Contains(KeywordAbility.CantDealCombatDamage))
                        ? 0
                        : Mathf.Max(blocker.power, 0);
                    if (!attackerProtected)
                    {
                        totalDamageFromBlockers += damageFromBlocker;
                        if (blocker.keywordAbilities.Contains(KeywordAbility.Deathtouch) && damageFromBlocker > 0)
                            attacker.Kill();
                    }

                    if (damageToBlocker > 0 || damageFromBlocker > 0)
                        SoundManager.Instance.PlaySound(SoundManager.Instance.impact);

                    Debug.Log($"{attacker.cardName} is blocked by {blocker.cardName}.");

                    if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink) && damageToBlocker > 0)
                    {
                        Player owner = GetOwnerOfCard(attacker);
                        GameManager.Instance.TryGainLife(owner, damageToBlocker);
                        Debug.Log($"{attacker.cardName} lifelinks {damageToBlocker} life to {owner}.");
                    }

                    if (blocker.keywordAbilities.Contains(KeywordAbility.Lifelink) && damageFromBlocker > 0)
                    {
                        Player blockerOwner = GetOwnerOfCard(blocker);
                        GameManager.Instance.TryGainLife(blockerOwner, damageFromBlocker);
                        Debug.Log($"{blocker.cardName} lifelinks {damageFromBlocker} life to {blockerOwner}.");
                    }
                }

                attacker.TakeDamage(totalDamageFromBlockers);

                if (attacker.keywordAbilities.Contains(KeywordAbility.Trample) && remainingDamage > 0)
                {
                    if (humanPlayer.Battlefield.Contains(attacker))
                    {
                        aiPlayer.Life -= remainingDamage;
                        aiDamage += remainingDamage;
                        Debug.Log($"{attacker.cardName} tramples over for {remainingDamage} damage!");
                        NotifyCombatDamageToPlayer(attacker, aiPlayer);
                    }
                    else
                    {
                        humanPlayer.Life -= remainingDamage;
                        playerDamage += remainingDamage;
                        Debug.Log($"{attacker.cardName} tramples YOU for {remainingDamage} damage!");
                        NotifyCombatDamageToPlayer(attacker, humanPlayer);
                    }

                    if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink))
                    {
                        GameManager.Instance.TryGainLife(GetOwnerOfCard(attacker), remainingDamage);
                        Debug.Log($"{attacker.cardName} lifelinks {remainingDamage} trample damage.");
                    }
                }
            }
            else
            {
                // Attacker goes unblocked
                if (humanPlayer.Battlefield.Contains(attacker))
                {
                    aiPlayer.Life -= attackerDamage;
                    aiDamage += attackerDamage;
                    NotifyCombatDamageToPlayer(attacker, aiPlayer);

                    // Lifelink: gain life equal to damage dealt to AI
                    if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink))
                    {
                        Player owner = humanPlayer.Battlefield.Contains(attacker) ? humanPlayer : aiPlayer;
                        GameManager.Instance.TryGainLife(owner, attackerDamage);
                        Debug.Log($"{attacker.cardName} lifelinks {attackerDamage} life to {(owner == humanPlayer ? "Human" : "AI")}.");
                    }
                }
                else
                {
                    humanPlayer.Life -= attackerDamage;
                    playerDamage += attackerDamage;
                    NotifyCombatDamageToPlayer(attacker, humanPlayer);

                    // Lifelink: gain life equal to damage dealt to Human
                    if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink))
                    {
                        GameManager.Instance.TryGainLife(aiPlayer, attackerDamage);
                        Debug.Log($"{attacker.cardName} lifelinks {attackerDamage} life to AI.");
                    }
                }
            }
        }

        foreach (var creature in humanPlayer.Battlefield.Concat(aiPlayer.Battlefield).OfType<CreatureCard>())
        {
            if (creature.cardName == "Undead Army" &&
                (currentAttackers.Contains(creature) || creature.blockingThisAttacker != null))
            {
                creature.AddMinusOneCounter();
                var vis = FindCardVisual(creature);
                if (vis != null) vis.UpdateVisual();
            }
        }

        CheckDeaths(humanPlayer);
        CheckDeaths(aiPlayer);

        // Cleanup combat assignments
        foreach (var card in humanPlayer.Battlefield)
        {
            if (card is CreatureCard c)
            {
                c.blockingThisAttacker = null;
                c.blockedByThisBlocker.Clear();
            }
        }
        foreach (var card in aiPlayer.Battlefield)
        {
            if (card is CreatureCard c)
            {
                c.blockingThisAttacker = null;
                c.blockedByThisBlocker.Clear();
            }
        }

        currentAttackers.Clear();
        selectedBlockerForBlocking = null;
        UpdateUI();
        CheckForGameEnd();
        return (playerDamage, aiDamage);
    }

    public void CheckDeaths(Player player)
    {
        List<Card> toGrave = new List<Card>();
        foreach (var card in player.Battlefield)
        {
            if (card is CreatureCard c && c.toughness <= 0)
            {
                if (!processedDeaths.Contains(c))
                    toGrave.Add(c);
            }
        }

        foreach (var card in toGrave)
        {
            SendToGraveyard(card, player);
        }
    }

    public void ResetDeathTracking()
    {
        processedDeaths.Clear();
    }

    public void ResetPermanents(Player player, bool clearSummoningSickness = true)
    {
        foreach (var card in player.Battlefield)
        {
            bool cantUntap = false;
            if (card is CreatureCard checkCreature)
                cantUntap = checkCreature.keywordAbilities.Contains(KeywordAbility.CantUntap);
            else if (card.keywordAbilities != null)
                cantUntap = card.keywordAbilities.Contains(KeywordAbility.CantUntap);

            if (!cantUntap)
                card.isTapped = false;

            if (card is CreatureCard creature)
            {
                if (clearSummoningSickness)
                    creature.hasSummoningSickness = false;
                creature.RecalculateStats();
                creature.blockingThisAttacker = null;
                creature.blockedByThisBlocker.Clear();
            }

            var visual = FindCardVisual(card);
            if (visual != null)
            {
                visual.UpdateVisual(); // Just call once, it's enough
            }
        }

        player.hasPlayedLandThisTurn = false; // Also reset land play

        foreach (var visual in activeCardVisuals)
        {
            visual.UpdateVisual();
        }

    }

    public void ResetDamage(Player player)
    {
        foreach (var card in player.Battlefield)
        {
            if (card is CreatureCard creature)
            {
                creature.ResetDamage();
            }
        }
    }

    public CardVisual FindCardVisual(Card card)
        {
            return activeCardVisuals.Find(cv => cv.linkedCard == card);
        }

    public void RemoveCreatureFromCombatIfNeeded(CreatureCard creature)
    {
        if (creature == null)
            return;

        currentAttackers.Remove(creature);
        selectedAttackers.Remove(creature);

        if (selectedBlockerForBlocking == creature)
            selectedBlockerForBlocking = null;

        blockingAssignments.Remove(creature);

        foreach (var assignment in blockingAssignments.Values)
            assignment.Remove(creature);

        foreach (var other in humanPlayer.Battlefield.Concat(aiPlayer.Battlefield).OfType<CreatureCard>())
        {
            other.blockedByThisBlocker.Remove(creature);
            if (other.blockingThisAttacker == creature)
                other.blockingThisAttacker = null;

            var otherVisual = FindCardVisual(other);
            if (otherVisual != null)
                otherVisual.UpdateVisual();
        }

        creature.blockingThisAttacker = null;
        creature.blockedByThisBlocker.Clear();

        var visual = FindCardVisual(creature);
        if (visual != null)
            visual.UpdateVisual();
    }

    private SorceryCard GetAIUnsummonInHand()
    {
        if (aiPlayer == null)
            return null;

        return aiPlayer.Hand
            .OfType<SorceryCard>()
            .FirstOrDefault(card => card.cardName == "Unsummon" &&
                                    CardDatabase.GetCardData(card.cardName)?.cardType == CardType.Instant);
    }

    private SorceryCard GetAIGiantGrowthInHand()
    {
        if (aiPlayer == null)
            return null;

        return aiPlayer.Hand
            .OfType<SorceryCard>()
            .FirstOrDefault(card => card.cardName == "Giant Growth" &&
                                    CardDatabase.GetCardData(card.cardName)?.cardType == CardType.Instant);
    }

    private SorceryCard GetAIChargeInHand()
    {
        if (aiPlayer == null)
            return null;

        return aiPlayer.Hand
            .OfType<SorceryCard>()
            .FirstOrDefault(card => card.cardName == "Charge" &&
                                    CardDatabase.GetCardData(card.cardName)?.cardType == CardType.Instant);
    }

    private SorceryCard GetAIHolyDayInHand()
    {
        if (aiPlayer == null)
            return null;

        return aiPlayer.Hand
            .OfType<SorceryCard>()
            .FirstOrDefault(card => card.cardName == "Holy Day" &&
                                    CardDatabase.GetCardData(card.cardName)?.cardType == CardType.Instant);
    }

    private bool IsLikelyCreatureRemovalSpell(SorceryCard spell, CreatureCard target)
    {
        if (spell == null || target == null)
            return false;

        bool directDestroy = spell.destroyTargetIfTypeMatches &&
                             (spell.requiredTargetType == SorceryCard.TargetType.Creature ||
                              spell.requiredTargetType == SorceryCard.TargetType.CreatureOrPlayer);

        int maxDamage = spell.damageToTargetMax > 0 ? spell.damageToTargetMax : spell.damageToTarget;
        bool damageCouldKill = maxDamage > 0;

        bool appliesNegativeCounters = spell.addXMinusOneCounters;

        return directDestroy || damageCouldKill || appliesNegativeCounters;
    }

    public bool TryAICastUnsummon(CreatureCard target, string reason)
    {
        if (target == null || aiPlayer == null)
            return false;

        if (GetOwnerOfCard(target) == null || GetOwnerOfCard(target).Battlefield.Contains(target) == false)
            return false;

        SorceryCard unsummon = GetAIUnsummonInHand();
        if (unsummon == null)
            return false;

        var cost = GetManaCostBreakdown(unsummon.manaCost, unsummon.color);
        int tax = GetOpponentSpellTax(aiPlayer);
        if (tax > 0)
        {
            if (!cost.ContainsKey("Colorless"))
                cost["Colorless"] = 0;
            cost["Colorless"] += tax;
        }

        bool canPay = TurnSystem.Instance != null
            ? TurnSystem.Instance.TryEnsureAIManaForCost(cost)
            : aiPlayer.ColoredMana.CanPay(cost);

        if (!canPay || !aiPlayer.ColoredMana.CanPay(cost))
            return false;

        return TryAICastInstantOnStack(
            unsummon,
            target,
            $"[AI] Casts Unsummon targeting {target.cardName} ({reason}).",
            cost);
    }

    private bool TryAICastInstantOnStack(SorceryCard instant, Card target, string logMessage, Dictionary<string, int> cost)
    {
        if (instant == null || aiPlayer == null)
            return false;

        aiPlayer.ColoredMana.Pay(cost);
        aiPlayer.Hand.Remove(instant);
        instant.owner = aiPlayer;
        instant.chosenTarget = target;
        instant.chosenPlayerTarget = null;

        if (!string.IsNullOrEmpty(logMessage))
            Debug.Log(logMessage);

        GameObject obj = Instantiate(cardPrefab, stackZone);
        CardVisual visual = obj.GetComponent<CardVisual>();
        CardData data = CardDatabase.GetCardData(instant.cardName);
        visual.Setup(instant, this, data);

        visual.transform.localPosition = Vector3.zero;
        visual.transform.SetParent(stackZone, false);
        visual.isInStack = true;

        if (!activeCardVisuals.Contains(visual))
            activeCardVisuals.Add(visual);

        UpdateUI();
        SoundManager.Instance.PlaySound(SoundManager.Instance.cardPlay);

        isStackBusy = true;
        if (TurnSystem.Instance != null)
        {
            TurnSystem.Instance.waitingToResumeAI = true;
            TurnSystem.Instance.lastPhaseBeforeStack = TurnSystem.Instance.currentPhase;
        }

        StartCoroutine(ResolveSorceryAfterDelay(instant, visual, aiPlayer));
        return true;
    }

    private bool TryAICastGiantGrowth(CreatureCard target, string reason)
    {
        if (target == null || aiPlayer == null)
            return false;

        if (GetOwnerOfCard(target) != aiPlayer || !aiPlayer.Battlefield.Contains(target))
            return false;

        SorceryCard giantGrowth = GetAIGiantGrowthInHand();
        if (giantGrowth == null)
            return false;

        var cost = GetManaCostBreakdown(giantGrowth.manaCost, giantGrowth.color);
        int tax = GetOpponentSpellTax(aiPlayer);
        if (tax > 0)
        {
            if (!cost.ContainsKey("Colorless"))
                cost["Colorless"] = 0;
            cost["Colorless"] += tax;
        }

        bool canPay = TurnSystem.Instance != null
            ? TurnSystem.Instance.TryEnsureAIManaForCost(cost)
            : aiPlayer.ColoredMana.CanPay(cost);

        if (!canPay || !aiPlayer.ColoredMana.CanPay(cost))
            return false;

        return TryAICastInstantOnStack(
            giantGrowth,
            target,
            $"[AI] Casts Giant Growth targeting {target.cardName} ({reason}).",
            cost);
    }

    public bool TryAICastGiantGrowthForCombat(bool aiIsAttacker)
    {
        if (aiPlayer == null || currentAttackers == null || currentAttackers.Count == 0)
            return false;

        CreatureCard bestTarget = null;
        CreatureCard bestEnemy = null;
        int bestScore = int.MinValue;

        if (aiIsAttacker)
        {
            foreach (var attacker in currentAttackers)
            {
                if (attacker == null || GetOwnerOfCard(attacker) != aiPlayer)
                    continue;

                foreach (var blocker in attacker.blockedByThisBlocker ?? new List<CreatureCard>())
                {
                    if (blocker == null || GetOwnerOfCard(blocker) != humanPlayer)
                        continue;

                    bool attackerKillsNow = attacker.power >= blocker.toughness;
                    bool attackerKillsWithGrowth = attacker.power + 3 >= blocker.toughness;
                    bool blockerKillsNow = blocker.power >= attacker.toughness;
                    bool blockerKillsAfterGrowth = blocker.power >= attacker.toughness + 3;

                    if (!attackerKillsNow && attackerKillsWithGrowth)
                    {
                        int score = 10;
                        if (blockerKillsNow && !blockerKillsAfterGrowth)
                            score += 5;
                        score += CardDatabase.GetCardData(blocker.cardName)?.manaCost ?? 0;

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestTarget = attacker;
                            bestEnemy = blocker;
                        }
                    }
                }
            }

            if (bestTarget != null)
                return TryAICastGiantGrowth(bestTarget, $"winning combat versus {bestEnemy.cardName}");

            var unblockedAttacker = currentAttackers
                .Where(a => a != null && GetOwnerOfCard(a) == aiPlayer && (a.blockedByThisBlocker == null || a.blockedByThisBlocker.Count == 0))
                .OrderByDescending(a => a.power)
                .ThenByDescending(a => a.toughness)
                .ThenByDescending(a => CardDatabase.GetCardData(a.cardName)?.manaCost ?? 0)
                .FirstOrDefault();

            if (unblockedAttacker != null)
                return TryAICastGiantGrowth(unblockedAttacker, "pushing extra unblocked combat damage");

            return false;
        }

        foreach (var attacker in currentAttackers)
        {
            if (attacker == null || GetOwnerOfCard(attacker) != humanPlayer)
                continue;

            foreach (var blocker in attacker.blockedByThisBlocker ?? new List<CreatureCard>())
            {
                if (blocker == null || GetOwnerOfCard(blocker) != aiPlayer)
                    continue;

                bool blockerKillsNow = blocker.power >= attacker.toughness;
                bool blockerKillsWithGrowth = blocker.power + 3 >= attacker.toughness;
                bool attackerKillsNow = attacker.power >= blocker.toughness;
                bool attackerKillsAfterGrowth = attacker.power >= blocker.toughness + 3;

                if (!blockerKillsNow && blockerKillsWithGrowth)
                {
                    int score = 10;
                    if (attackerKillsNow && !attackerKillsAfterGrowth)
                        score += 5;
                    score += CardDatabase.GetCardData(attacker.cardName)?.manaCost ?? 0;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestTarget = blocker;
                        bestEnemy = attacker;
                    }
                }
            }
        }

        if (bestTarget == null)
            return false;

        return TryAICastGiantGrowth(bestTarget, $"winning block versus {bestEnemy.cardName}");
    }

    private bool TryAICastCharge(string reason)
    {
        if (aiPlayer == null)
            return false;

        SorceryCard charge = GetAIChargeInHand();
        if (charge == null)
            return false;

        var cost = GetManaCostBreakdown(charge.manaCost, charge.color);
        int tax = GetOpponentSpellTax(aiPlayer);
        if (tax > 0)
        {
            if (!cost.ContainsKey("Colorless"))
                cost["Colorless"] = 0;
            cost["Colorless"] += tax;
        }

        bool canPay = TurnSystem.Instance != null
            ? TurnSystem.Instance.TryEnsureAIManaForCost(cost)
            : aiPlayer.ColoredMana.CanPay(cost);

        if (!canPay || !aiPlayer.ColoredMana.CanPay(cost))
            return false;

        return TryAICastInstantOnStack(
            charge,
            null,
            $"[AI] Casts Charge ({reason}).",
            cost);
    }

    public bool TryAICastHolyDay(string reason)
    {
        if (aiPlayer == null)
            return false;

        SorceryCard holyDay = GetAIHolyDayInHand();
        if (holyDay == null)
            return false;

        var cost = GetManaCostBreakdown(holyDay.manaCost, holyDay.color);
        int tax = GetOpponentSpellTax(aiPlayer);
        if (tax > 0)
        {
            if (!cost.ContainsKey("Colorless"))
                cost["Colorless"] = 0;
            cost["Colorless"] += tax;
        }

        bool canPay = TurnSystem.Instance != null
            ? TurnSystem.Instance.TryEnsureAIManaForCost(cost)
            : aiPlayer.ColoredMana.CanPay(cost);

        if (!canPay || !aiPlayer.ColoredMana.CanPay(cost))
            return false;

        return TryAICastInstantOnStack(
            holyDay,
            null,
            $"[AI] Casts Holy Day ({reason}).",
            cost);
    }

    public bool TryAICastChargeForCombat(bool aiIsAttacker)
    {
        if (aiPlayer == null || currentAttackers == null || currentAttackers.Count == 0)
            return false;

        if (!aiIsAttacker)
            return false;

        var aiAttackers = currentAttackers
            .Where(attacker => attacker != null && GetOwnerOfCard(attacker) == aiPlayer)
            .ToList();

        if (aiAttackers.Count == 0)
            return false;

        bool hasUnblockedAttacker = aiAttackers.Any(attacker => attacker.blockedByThisBlocker == null || attacker.blockedByThisBlocker.Count == 0);
        if (hasUnblockedAttacker)
            return TryAICastCharge("pushing extra combat damage after blockers were confirmed");

        foreach (var attacker in aiAttackers)
        {
            foreach (var blocker in attacker.blockedByThisBlocker ?? new List<CreatureCard>())
            {
                if (blocker == null || GetOwnerOfCard(blocker) != humanPlayer)
                    continue;

                bool attackerKillsWithCharge = attacker.power + 1 >= blocker.toughness;
                bool attackerKillsWithoutCharge = attacker.power >= blocker.toughness;
                bool blockerKillsAttacker = blocker.power >= attacker.toughness;
                bool blockerKillsAttackerAfterCharge = blocker.power >= attacker.toughness + 1;

                if ((!attackerKillsWithoutCharge && attackerKillsWithCharge) ||
                    (blockerKillsAttacker && !blockerKillsAttackerAfterCharge))
                {
                    return TryAICastCharge($"improving combat outcome versus {blocker.cardName}");
                }
            }
        }

        return false;
    }

    public bool TryAICastUnsummonOnStrongestAttacker()
    {
        if (TurnSystem.Instance == null || TurnSystem.Instance.currentPlayer != TurnSystem.PlayerType.Human)
            return false;

        var target = currentAttackers
            .Where(attacker => attacker != null && GetOwnerOfCard(attacker) == humanPlayer)
            .OrderByDescending(attacker => attacker.power)
            .ThenByDescending(attacker => attacker.toughness)
            .ThenByDescending(attacker => CardDatabase.GetCardData(attacker.cardName)?.manaCost ?? 0)
            .FirstOrDefault();

        if (target == null)
            return false;

        return TryAICastUnsummon(target, "opponent declared attackers");
    }

    public bool TryAICastUnsummonOnStrongestBlocker()
    {
        if (TurnSystem.Instance == null || TurnSystem.Instance.currentPlayer != TurnSystem.PlayerType.AI)
            return false;

        var target = currentAttackers
            .Where(attacker => attacker != null && GetOwnerOfCard(attacker) == aiPlayer)
            .SelectMany(attacker => attacker.blockedByThisBlocker ?? new List<CreatureCard>())
            .Where(blocker => blocker != null && GetOwnerOfCard(blocker) == humanPlayer)
            .OrderByDescending(blocker => blocker.toughness)
            .ThenByDescending(blocker => blocker.power)
            .ThenByDescending(blocker => CardDatabase.GetCardData(blocker.cardName)?.manaCost ?? 0)
            .FirstOrDefault();

        if (target == null)
            return false;

        return TryAICastUnsummon(target, "clearing a blocker before damage");
    }

    public void TryAIDefensiveUnsummonResponse(SorceryCard incomingSpell, Card target, Player caster)
    {
        if (caster == aiPlayer || incomingSpell == null)
            return;

        if (!(target is CreatureCard creature))
            return;

        if (GetOwnerOfCard(creature) != aiPlayer)
            return;

        if (!IsLikelyCreatureRemovalSpell(incomingSpell, creature))
            return;

        TryAICastUnsummon(creature, $"responding to {incomingSpell.cardName}");
    }

    public IEnumerator ResolveSorceryAfterDelay(SorceryCard sorcery, CardVisual visual, Player caster)
        {
            yield return WaitForStackOrSkip(2f);

            // PREVENT executing if required target is missing
            if (sorcery.requiresTarget && sorcery.chosenTarget == null && sorcery.chosenPlayerTarget == null)
            {
                Debug.LogWarning($"[ResolveSorceryAfterDelay] {sorcery.cardName} requires a target, but none was set. Aborting cast.");

                // Destroy visual
                if (visual != null)
                {
                    GameManager.Instance.activeCardVisuals.Remove(visual);
                    GameObject.Destroy(visual.gameObject);
                }

                isStackBusy = false;
                yield break;
            }

            if (sorcery.chosenTarget != null)
            {
                TryAIDefensiveUnsummonResponse(sorcery, sorcery.chosenTarget, caster);
                sorcery.ResolveEffect(caster, sorcery.chosenTarget);
            }
            else if (sorcery.chosenPlayerTarget != null)
            {
                sorcery.ResolveEffectOnPlayer(caster, sorcery.chosenPlayerTarget);
            }
            else
            {
                sorcery.ResolveEffect(caster);
            }

            SendToGraveyard(sorcery, caster, fromStack: true);
            AwardFavouriteCardCoins(sorcery, caster);

            if (caster == aiPlayer && visual != null)
            {
                activeCardVisuals.Remove(visual);
                Destroy(visual.gameObject);
            }

            UpdateUI();
            isStackBusy = false;
            CheckForGameEnd();

            if (caster == aiPlayer && TurnSystem.Instance.waitingToResumeAI && pendingStackEffects == 0)
            {
                Debug.Log("Resuming AI phase after stack.");
                TurnSystem.Instance.waitingToResumeAI = false;
                TurnSystem.Instance.RunSpecificPhase(TurnSystem.Instance.lastPhaseBeforeStack);
            }
        }

    public IEnumerator ResolveCreatureAfterDelay(CreatureCard creature, CardVisual visual, Player caster)
        {
            yield return WaitForStackOrSkip(2f);

            caster.Battlefield.Add(creature);

            if (creature.keywordAbilities.Contains(KeywordAbility.Haste))
                creature.hasSummoningSickness = false;
            else
                creature.hasSummoningSickness = true;

            if (creature.entersTapped || IsAllPermanentsEnterTappedActive())
            {
                creature.isTapped = true;
                Debug.Log($"{creature.cardName} enters tapped (due to static effect).");
            }

            Transform battlefield = caster == humanPlayer ? playerBattlefieldArea : aiBattlefieldArea;
            visual.transform.SetParent(battlefield, false);
            visual.isInStack = false;
            visual.isInBattlefield = true;
            if (!activeCardVisuals.Contains(visual))
                activeCardVisuals.Add(visual);
            visual.UpdateVisual();

            creature.OnEnterPlay(caster);

            if (caster == aiPlayer && creature.abilities != null)
            {
                foreach (var ability in creature.abilities)
                {
                    if (ability.timing == TriggerTiming.OnEnter && ability.requiresTarget)
                    {
                        Player opponent = GetOpponentOf(caster);
                        Card target = opponent.Battlefield
                            .Where(c =>
                                (ability.requiredTargetType == SorceryCard.TargetType.Creature && c is CreatureCard creatureT &&
                                    !(ability.excludeArtifactCreatures && creatureT.color.Contains("Artifact"))) ||
                                (ability.requiredTargetType == SorceryCard.TargetType.Artifact && c is ArtifactCard) ||
                                (ability.requiredTargetType == SorceryCard.TargetType.Enchantment && c is EnchantmentCard) ||
                                (ability.requiredTargetType == SorceryCard.TargetType.Land && c is LandCard))
                            .OrderByDescending(c => CardDatabase.GetCardData(c.cardName)?.manaCost ?? 0)
                            .FirstOrDefault();

                        if (target != null)
                        {
                            QueueTriggeredAbility(ability, caster, creature, target);
                            Debug.Log($"[AI ETB] {creature.cardName} targets {target.cardName}");
                        }
                    }
                }
            }

            NotifyCreatureEntered(creature, caster);
            if (creature.color.Contains("Artifact"))
                NotifyArtifactEntered(creature, caster);

            AwardFavouriteCardCoins(creature, caster);

            SoundManager.Instance.PlaySound(SoundManager.Instance.playCreature);

            UpdateUI();
            isStackBusy = false;
            CheckForGameEnd();

            if (caster == aiPlayer && TurnSystem.Instance.waitingToResumeAI && pendingStackEffects == 0)
            {
                Debug.Log("Resuming AI phase after stack.");
                TurnSystem.Instance.waitingToResumeAI = false;
                TurnSystem.Instance.RunSpecificPhase(TurnSystem.Instance.lastPhaseBeforeStack);
            }
        }

    public IEnumerator ResolveArtifactAfterDelay(ArtifactCard artifact, CardVisual visual, Player caster)
        {
            yield return WaitForStackOrSkip(2f);

            caster.Battlefield.Add(artifact);

            if (artifact.entersTapped || IsAllPermanentsEnterTappedActive())
            {
                artifact.isTapped = true;
                Debug.Log($"{artifact.cardName} enters tapped (due to static effect).");
            }

            Transform area = caster == humanPlayer ? playerArtifactArea : aiArtifactArea;
            visual.transform.SetParent(area, false);
            visual.isInStack = false;
            visual.isInBattlefield = true;
            if (!activeCardVisuals.Contains(visual))
                activeCardVisuals.Add(visual);
            visual.UpdateVisual();

            artifact.OnEnterPlay(caster);
            NotifyArtifactEntered(artifact, caster);

            AwardFavouriteCardCoins(artifact, caster);

            SoundManager.Instance.PlaySound(SoundManager.Instance.playArtifact);

            UpdateUI();
            isStackBusy = false;
            CheckForGameEnd();

            if (caster == aiPlayer && TurnSystem.Instance.waitingToResumeAI && pendingStackEffects == 0)
            {
                Debug.Log("Resuming AI phase after stack.");
                TurnSystem.Instance.waitingToResumeAI = false;
                TurnSystem.Instance.RunSpecificPhase(TurnSystem.Instance.lastPhaseBeforeStack);
            }
        }

    public IEnumerator ResolveEnchantmentAfterDelay(EnchantmentCard enchantment, CardVisual visual, Player caster)
        {
            yield return WaitForStackOrSkip(2f);

            caster.Battlefield.Add(enchantment);

            if (enchantment.entersTapped || IsAllPermanentsEnterTappedActive())
            {
                enchantment.isTapped = true;
                Debug.Log($"{enchantment.cardName} enters tapped (due to static effect).");
            }

            Transform area = caster == humanPlayer ? playerEnchantmentArea : aiEnchantmentArea;
            visual.transform.SetParent(area, false);
            visual.isInStack = false;
            visual.isInBattlefield = true;
            if (!activeCardVisuals.Contains(visual))
                activeCardVisuals.Add(visual);
            visual.UpdateVisual();

            enchantment.OnEnterPlay(caster);
            NotifyEnchantmentEntered(enchantment, caster);

            AwardFavouriteCardCoins(enchantment, caster);

            SoundManager.Instance.PlaySound(SoundManager.Instance.playArtifact);

            UpdateUI();
            isStackBusy = false;
            CheckForGameEnd();

            if (caster == aiPlayer && TurnSystem.Instance.waitingToResumeAI && pendingStackEffects == 0)
            {
                Debug.Log("Resuming AI phase after stack.");
                TurnSystem.Instance.waitingToResumeAI = false;
                TurnSystem.Instance.RunSpecificPhase(TurnSystem.Instance.lastPhaseBeforeStack);
            }
        }

        public IEnumerator ResolveAuraAfterDelay(AuraCard aura, CardVisual visual, Player caster)
        {
            yield return WaitForStackOrSkip(2f);

        caster.Battlefield.Add(aura);
        aura.OnEnterPlay(caster);
        NotifyEnchantmentEntered(aura, caster);

        AwardFavouriteCardCoins(aura, caster);

            if (!caster.Battlefield.Contains(aura))
            {
                // Aura may have destroyed itself via its effect
                UpdateUI();
                isStackBusy = false;
                if (caster == aiPlayer && TurnSystem.Instance.waitingToResumeAI && pendingStackEffects == 0)
                {
                    TurnSystem.Instance.waitingToResumeAI = false;
                    TurnSystem.Instance.RunSpecificPhase(TurnSystem.Instance.lastPhaseBeforeStack);
                }
                yield break;
            }

            if (aura.entersTapped || IsAllPermanentsEnterTappedActive())
            {
                aura.isTapped = true;
                Debug.Log($"{aura.cardName} enters tapped (due to static effect).");
            }

            Transform area = caster == humanPlayer ? playerEnchantmentArea : aiEnchantmentArea;
            if (visual != null)
            {
                visual.transform.SetParent(area, false);
                visual.isInStack = false;
                visual.isInBattlefield = true;
                if (!activeCardVisuals.Contains(visual))
                    activeCardVisuals.Add(visual);
                visual.UpdateVisual();
            }

            SoundManager.Instance.PlaySound(SoundManager.Instance.playArtifact);

            UpdateUI();
            isStackBusy = false;
            CheckForGameEnd();

            if (caster == aiPlayer && TurnSystem.Instance.waitingToResumeAI && pendingStackEffects == 0)
            {
                Debug.Log("Resuming AI phase after stack.");
                TurnSystem.Instance.waitingToResumeAI = false;
                TurnSystem.Instance.RunSpecificPhase(TurnSystem.Instance.lastPhaseBeforeStack);
            }
        }

    public IEnumerator ResolveTriggeredAbilityOnStack(
        CardAbility ability,
        Player owner,
        Card source,
        Card target,
        Card deadCreature = null)
    {
        yield return new WaitUntil(() => !isStackBusy);
        isStackBusy = true;
        TurnSystem.Instance.lastPhaseBeforeStack = TurnSystem.Instance.currentPhase;

        GameObject stackObj = Instantiate(cardPrefab, stackZone);
        CardVisual stackVisual = stackObj.GetComponent<CardVisual>();
        stackVisual.Setup(source, this);
        stackVisual.isInStack = true;
        stackVisual.UpdateVisual();
        stackVisual.transform.localPosition = Vector3.zero;
        stackVisual.transform.localRotation = Quaternion.identity;
        stackVisual.transform.localScale = Vector3.one;

        GameObject triggerVFX = null;
        if (triggerVFXPrefab != null)
        {
            triggerVFX = Instantiate(triggerVFXPrefab, stackObj.transform);
            RectTransform rt = triggerVFX.GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = Vector2.zero;
        }

        yield return WaitForStackOrSkip(2f);

        Card previousDead = lastDeadCreature;
        if (deadCreature != null)
            lastDeadCreature = deadCreature;

        Card previousSource = lastAbilitySource;
        lastAbilitySource = source;

        int oldLife = owner.Life;
        ability.effect?.Invoke(owner, target);
        int gained = owner.Life - oldLife;
        if (gained > 0)
        {
            ShowFloatingHeal(gained, owner == humanPlayer ? playerLifeContainer : enemyLifeContainer);
        }

        if (deadCreature != null)
            lastDeadCreature = previousDead;

        lastAbilitySource = previousSource;

        CheckDeaths(humanPlayer);
        CheckDeaths(aiPlayer);
        UpdateUI();
        optionalTargetPlayer = null;

        if (triggerVFX != null)
            Destroy(triggerVFX);
        Destroy(stackObj);
        isStackBusy = false;
        pendingStackEffects = Mathf.Max(0, pendingStackEffects - 1);
        CheckForGameEnd();

        if (owner == aiPlayer && TurnSystem.Instance.waitingToResumeAI && pendingStackEffects == 0)
        {
            Debug.Log("Resuming AI phase after stack.");
            TurnSystem.Instance.waitingToResumeAI = false;
            TurnSystem.Instance.RunSpecificPhase(TurnSystem.Instance.lastPhaseBeforeStack);
        }
    }

    public void QueueArtifactActivatedAbility(ArtifactCard artifact, ActivatedAbility ability, Player controller, Card target = null)
    {
        pendingStackEffects++;
        StartCoroutine(ResolveArtifactActivatedAbilityOnStack(artifact, ability, controller, target));
    }

    public void QueueEquipmentEquipAbility(EquipmentCard equipment, CreatureCard target, Player controller)
    {
        pendingStackEffects++;
        StartCoroutine(ResolveEquipmentEquipAbilityOnStack(equipment, target, controller));
    }

    public IEnumerator ResolveArtifactActivatedAbilityOnStack(ArtifactCard artifact, ActivatedAbility ability, Player controller, Card target = null)
    {
        yield return new WaitUntil(() => !isStackBusy);
        isStackBusy = true;
        TurnSystem.Instance.lastPhaseBeforeStack = TurnSystem.Instance.currentPhase;

        GameObject stackObj = Instantiate(cardPrefab, stackZone);
        CardVisual stackVisual = stackObj.GetComponent<CardVisual>();
        stackVisual.Setup(artifact, this);
        stackVisual.isInStack = true;
        stackVisual.UpdateVisual();
        stackVisual.transform.localPosition = Vector3.zero;
        stackVisual.transform.localRotation = Quaternion.identity;
        stackVisual.transform.localScale = Vector3.one;

        GameObject triggerVFX = null;
        if (triggerVFXPrefab != null)
        {
            triggerVFX = Instantiate(triggerVFXPrefab, stackObj.transform);
            RectTransform rt = triggerVFX.GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = Vector2.zero;
        }

        yield return WaitForStackOrSkip(2f);

        try
        {
            switch (ability)
            {
                case ActivatedAbility.TapToGainLife:
                    TryGainLife(controller, 1);
                    break;
                case ActivatedAbility.TapToPlague:
                    humanPlayer.Life -= artifact.plagueAmount;
                    aiPlayer.Life -= artifact.plagueAmount;
                    ShowFloatingDamage(artifact.plagueAmount, playerLifeContainer);
                    ShowFloatingDamage(artifact.plagueAmount, enemyLifeContainer);
                    SoundManager.Instance.PlaySound(SoundManager.Instance.plague);
                    ShowBloodSplatVFX(artifact);
                    break;
                case ActivatedAbility.SacrificeForLife:
                    TryGainLife(controller, artifact.lifeToGain);
                    SoundManager.Instance.PlaySound(SoundManager.Instance.drink);
                    SoundManager.Instance.PlaySound(SoundManager.Instance.gain_life);
                    break;
                case ActivatedAbility.SacrificeToDrawCards:
                    DrawCards(controller, artifact.cardsToDraw);
                    break;
                case ActivatedAbility.TapToPlayRandomPotion:
                    SearchLibraryForRandomPotionToBattlefield(controller);
                    break;
                case ActivatedAbility.TapToDrawCards:
                    DrawCards(controller, artifact.cardsToDraw);
                    break;
                case ActivatedAbility.DealDamageToCreature:
                    if (target is CreatureCard targetCreature)
                    {
                        targetCreature.TakeDamage(artifact.damageToCreature);
                        Card asCard = artifact;
                        if (asCard is CreatureCard srcCreature &&
                            srcCreature.keywordAbilities.Contains(KeywordAbility.Deathtouch) &&
                            artifact.damageToCreature > 0)
                        {
                            targetCreature.Kill();
                        }
                    }
                    break;
                case ActivatedAbility.BuffTargetCreature:
                    if (target is CreatureCard buffTarget)
                    {
                        buffTarget.AddTemporaryBuff(artifact.buffPower, artifact.buffToughness);
                        CardVisual tVis = FindCardVisual(buffTarget);
                        if (tVis != null)
                            tVis.UpdateVisual();
                    }
                    break;
                case ActivatedAbility.TapTargetArtifactCreatureOrLand:
                    if (target != null)
                    {
                        target.isTapped = true;
                        CardVisual targetVisual = FindCardVisual(target);
                        if (targetVisual != null)
                            targetVisual.UpdateVisual();
                    }
                    break;
            }

            CheckDeaths(humanPlayer);
            CheckDeaths(aiPlayer);
            UpdateUI();
        }
        finally
        {
            if (triggerVFX != null)
                Destroy(triggerVFX);
            Destroy(stackObj);
            isStackBusy = false;
            pendingStackEffects = Mathf.Max(0, pendingStackEffects - 1);
            CheckForGameEnd();

            if (controller == aiPlayer && TurnSystem.Instance.waitingToResumeAI && pendingStackEffects == 0)
            {
                Debug.Log("Resuming AI phase after stack.");
                TurnSystem.Instance.waitingToResumeAI = false;
                TurnSystem.Instance.RunSpecificPhase(TurnSystem.Instance.lastPhaseBeforeStack);
            }
        }
    }

    public void QueueCreatureActivatedAbility(CreatureCard creature, ActivatedAbility ability, Player controller, Card target = null, Player playerTarget = null)
    {
        pendingStackEffects++;
        StartCoroutine(ResolveCreatureActivatedAbilityOnStack(creature, ability, controller, target, playerTarget));
    }

    public IEnumerator ResolveCreatureActivatedAbilityOnStack(CreatureCard creature, ActivatedAbility ability, Player controller, Card target = null, Player playerTarget = null)
    {
        yield return new WaitUntil(() => !isStackBusy);
        isStackBusy = true;
        TurnSystem.Instance.lastPhaseBeforeStack = TurnSystem.Instance.currentPhase;

        GameObject stackObj = Instantiate(cardPrefab, stackZone);
        CardVisual stackVisual = stackObj.GetComponent<CardVisual>();
        stackVisual.Setup(creature, this);
        stackVisual.isInStack = true;
        stackVisual.UpdateVisual();
        stackVisual.transform.localPosition = Vector3.zero;
        stackVisual.transform.localRotation = Quaternion.identity;
        stackVisual.transform.localScale = Vector3.one;

        GameObject triggerVFX = null;
        if (triggerVFXPrefab != null)
        {
            triggerVFX = Instantiate(triggerVFXPrefab, stackObj.transform);
            RectTransform rt = triggerVFX.GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = Vector2.zero;
        }

        yield return WaitForStackOrSkip(2f);

        switch (ability)
        {
            case ActivatedAbility.PayToGainAbility:
                PayToGainAbility(creature);
                break;
            case ActivatedAbility.PayToBuffSelf:
                PayToBuffSelf(creature);
                break;
            case ActivatedAbility.TapToLoseLife:
                Player opponent = GetOpponentOf(controller);
                opponent.Life -= creature.tapLifeLossAmount;
                if (controller == humanPlayer)
                    ShowFloatingDamage(creature.tapLifeLossAmount, enemyLifeContainer);
                else
                    ShowFloatingDamage(creature.tapLifeLossAmount, playerLifeContainer);
                SoundManager.Instance.PlaySound(SoundManager.Instance.plague);
                ShowBloodSplatVFX(creature);
                break;
            case ActivatedAbility.TapToDealDamageAnyTarget:
                if (target is CreatureCard targetCreature)
                {
                    targetCreature.TakeDamage(creature.damageToCreature);
                    if (creature.keywordAbilities.Contains(KeywordAbility.Deathtouch) && creature.damageToCreature > 0)
                        targetCreature.Kill();
                }
                else if (playerTarget != null)
                {
                    playerTarget.Life -= creature.damageToCreature;
                    ShowFloatingDamage(creature.damageToCreature,
                        playerTarget == humanPlayer ? playerLifeContainer : enemyLifeContainer);
                }
                break;
            case ActivatedAbility.TapToDestroyPower4OrGreater:
                if (target is CreatureCard powerCreature && powerCreature.power >= 4)
                {
                    Player targetOwner = GetOwnerOfCard(powerCreature);
                    if (targetOwner != null && !powerCreature.keywordAbilities.Contains(KeywordAbility.Indestructible))
                        SendToGraveyard(powerCreature, targetOwner);
                }
                break;
            case ActivatedAbility.TapToCreateToken:
                string tokenName = creature.tokenToCreate;
                Card token = CardFactory.Create(tokenName);
                if (token != null)
                {
                    if (tokenName == "Autonomous Miner")
                        SoundManager.Instance.PlaySound(SoundManager.Instance.miner);
                    SummonToken(token, controller);
                    Debug.Log($"{creature.cardName} created a {tokenName} token.");
                }
                else
                {
                    Debug.LogError($"Failed to create token: {tokenName}");
                }
                break;
            case ActivatedAbility.ReturnSelfFromGraveyard:
                ReturnCreatureFromGraveyardToBattlefield(controller, creature);
                break;
            case ActivatedAbility.ReturnSelfFromGraveyardToHand:
                ReturnCreatureFromGraveyardToHand(controller, creature);
                break;
        }

        CheckDeaths(humanPlayer);
        CheckDeaths(aiPlayer);
        UpdateUI();

        if (triggerVFX != null)
            Destroy(triggerVFX);
        Destroy(stackObj);
        isStackBusy = false;
        pendingStackEffects = Mathf.Max(0, pendingStackEffects - 1);
        CheckForGameEnd();

        if (controller == aiPlayer && TurnSystem.Instance.waitingToResumeAI && pendingStackEffects == 0)
        {
            Debug.Log("Resuming AI phase after stack.");
            TurnSystem.Instance.waitingToResumeAI = false;
            TurnSystem.Instance.RunSpecificPhase(TurnSystem.Instance.lastPhaseBeforeStack);
        }
    }

    public IEnumerator ResolveEquipmentEquipAbilityOnStack(EquipmentCard equipment, CreatureCard target, Player controller)
    {
        yield return new WaitUntil(() => !isStackBusy);
        isStackBusy = true;
        TurnSystem.Instance.lastPhaseBeforeStack = TurnSystem.Instance.currentPhase;

        GameObject stackObj = Instantiate(cardPrefab, stackZone);
        CardVisual stackVisual = stackObj.GetComponent<CardVisual>();
        stackVisual.Setup(equipment, this);
        stackVisual.isInStack = true;
        stackVisual.UpdateVisual();
        stackVisual.transform.localPosition = Vector3.zero;
        stackVisual.transform.localRotation = Quaternion.identity;
        stackVisual.transform.localScale = Vector3.one;

        GameObject triggerVFX = null;
        if (triggerVFXPrefab != null)
        {
            triggerVFX = Instantiate(triggerVFXPrefab, stackObj.transform);
            RectTransform rt = triggerVFX.GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = Vector2.zero;
        }

        yield return WaitForStackOrSkip(2f);

        try
        {
            bool equipmentOnBattlefield = equipment != null && controller != null && controller.Battlefield.Contains(equipment);
            bool validTarget = target != null &&
                               controller != null &&
                               GetOwnerOfCard(target) == controller &&
                               controller.Battlefield.Contains(target) &&
                               !target.isDead;

            if (equipmentOnBattlefield && validTarget)
            {
                equipment.Equip(target);
                FindCardVisual(equipment)?.UpdateVisual();
                FindCardVisual(target)?.UpdateVisual();
            }
            else
            {
                Debug.Log("Equip ability fizzled due to missing source or invalid target.");
            }

            CheckDeaths(humanPlayer);
            CheckDeaths(aiPlayer);
            UpdateUI();
        }
        finally
        {
            if (triggerVFX != null)
                Destroy(triggerVFX);
            Destroy(stackObj);
            isStackBusy = false;
            pendingStackEffects = Mathf.Max(0, pendingStackEffects - 1);
            CheckForGameEnd();

            if (controller == aiPlayer && TurnSystem.Instance.waitingToResumeAI && pendingStackEffects == 0)
            {
                Debug.Log("Resuming AI phase after stack.");
                TurnSystem.Instance.waitingToResumeAI = false;
                TurnSystem.Instance.RunSpecificPhase(TurnSystem.Instance.lastPhaseBeforeStack);
            }
        }
    }

    public void SummonToken(Card tokenCard, Player owner)
    {
        if (tokenCard == null)
        {
            Debug.LogError("Tried to summon a null token.");
            return;
        }

        tokenCard.owner = owner;

        if (tokenCard is CreatureCard creature)
        {
            creature.hasSummoningSickness = true;

            if (creature.entersTapped || GameManager.Instance.IsAllPermanentsEnterTappedActive())
            {
                creature.isTapped = true;
                Debug.Log($"{creature.cardName} enters tapped (due to static effect).");
            }
        }

        owner.Battlefield.Add(tokenCard);

        // Choose the correct battlefield area based on card type
        Transform area;
        if (tokenCard is LandCard)
            area = owner == humanPlayer ? playerLandArea : aiLandArea;
        else if (tokenCard is ArtifactCard)
            area = owner == humanPlayer ? playerArtifactArea : aiArtifactArea;
        else if (tokenCard is EnchantmentCard)
            area = owner == humanPlayer ? playerEnchantmentArea : aiEnchantmentArea;
        else
            area = owner == humanPlayer ? playerBattlefieldArea : aiBattlefieldArea;

        // Create visual and link it
        GameObject visualGO = Instantiate(cardPrefab, area);
        CardVisual visual = visualGO.GetComponent<CardVisual>();

        visual.Setup(tokenCard, this);
        visual.isInBattlefield = true;
        activeCardVisuals.Add(visual);
        visual.UpdateVisual();

        tokenCard.OnEnterPlay(owner);  // Run ETB triggers (last)
        if (tokenCard is CreatureCard)
            NotifyCreatureEntered(tokenCard, owner);
        if (tokenCard is LandCard)
            NotifyLandEntered(tokenCard, owner);
        if ((tokenCard is ArtifactCard) ||
            (tokenCard is CreatureCard cc && cc.color.Contains("Artifact")))
        {
            NotifyArtifactEntered(tokenCard, owner);
        }
        if (tokenCard is EnchantmentCard)
        {
            NotifyEnchantmentEntered(tokenCard, owner);
        }
    }
    public Player GetOpponentOf(Player player)
    {
        return player == humanPlayer ? aiPlayer : humanPlayer;
    }

    public void ReturnRandomLandFromGraveyard(Player player)
    {
        var lands = player.Graveyard.OfType<LandCard>().ToList();
        if (lands.Count == 0)
            return;

        Card chosen = lands[Random.Range(0, lands.Count)];
        player.Graveyard.Remove(chosen);
        player.Hand.Add(chosen);

        if (player == humanPlayer)
        {
            GameObject obj = Instantiate(cardPrefab, playerHandArea);
            CardVisual visual = obj.GetComponent<CardVisual>();
            CardData data = CardDatabase.GetCardData(chosen.cardName);
            visual.Setup(chosen, this, data);
            activeCardVisuals.Add(visual);
        }
        else if (enemyHandText != null)
        {
            enemyHandText.text = "Hand: " + player.Hand.Count;
        }

        RefreshGraveyardVisuals(player);
        UpdateUI();
    }

    public void ReturnRandomCreatureFromGraveyard(Player player)
    {
        var creatures = player.Graveyard.OfType<CreatureCard>().ToList();
        if (creatures.Count == 0)
            return;

        Card chosen = creatures[Random.Range(0, creatures.Count)];
        player.Graveyard.Remove(chosen);
        player.Hand.Add(chosen);

        if (player == humanPlayer)
        {
            GameObject obj = Instantiate(cardPrefab, playerHandArea);
            CardVisual visual = obj.GetComponent<CardVisual>();
            CardData data = CardDatabase.GetCardData(chosen.cardName);
            visual.Setup(chosen, this, data);
            activeCardVisuals.Add(visual);
        }
        else if (enemyHandText != null)
        {
            enemyHandText.text = "Hand: " + player.Hand.Count;
        }

        RefreshGraveyardVisuals(player);
        UpdateUI();
    }

    public void ReturnRandomNonCreatureArtifactFromGraveyard(Player player)
    {
        var artifacts = player.Graveyard
            .Where(card => card is ArtifactCard && !(card is CreatureCard))
            .ToList();
        if (artifacts.Count == 0)
            return;

        Card chosen = artifacts[Random.Range(0, artifacts.Count)];
        player.Graveyard.Remove(chosen);
        player.Hand.Add(chosen);

        if (player == humanPlayer)
        {
            GameObject obj = Instantiate(cardPrefab, playerHandArea);
            CardVisual visual = obj.GetComponent<CardVisual>();
            CardData data = CardDatabase.GetCardData(chosen.cardName);
            visual.Setup(chosen, this, data);
            activeCardVisuals.Add(visual);
        }
        else if (enemyHandText != null)
        {
            enemyHandText.text = "Hand: " + player.Hand.Count;
        }

        RefreshGraveyardVisuals(player);
        UpdateUI();
    }

    public void ReturnRandomPotionFromGraveyardToBattlefield(Player player)
    {
        var potions = player.Graveyard
            .Where(card => card.subtypes.Contains("Potion"))
            .ToList();
        if (potions.Count == 0)
            return;

        Card chosen = potions[Random.Range(0, potions.Count)];
        player.Graveyard.Remove(chosen);
        player.Battlefield.Add(chosen);

        if (chosen is CreatureCard creature)
        {
            creature.hasSummoningSickness = true;
            if (creature.entersTapped || IsAllPermanentsEnterTappedActive())
            {
                creature.isTapped = true;
                Debug.Log($"{creature.cardName} enters tapped (due to static effect).");
            }
        }
        else if (chosen is ArtifactCard artifact)
        {
            if (artifact.entersTapped || IsAllPermanentsEnterTappedActive())
            {
                artifact.isTapped = true;
                Debug.Log($"{artifact.cardName} enters tapped (due to static effect).");
            }
        }

        GameObject obj = Instantiate(cardPrefab,
            player == humanPlayer ?
                (chosen is ArtifactCard ? playerArtifactArea : playerBattlefieldArea) :
                (chosen is ArtifactCard ? aiArtifactArea : aiBattlefieldArea));
        CardVisual visual = obj.GetComponent<CardVisual>();
        CardData data = CardDatabase.GetCardData(chosen.cardName);
        visual.Setup(chosen, this, data);
        visual.isInBattlefield = true;
        activeCardVisuals.Add(visual);
        visual.UpdateVisual();

        chosen.OnEnterPlay(player);
        if (chosen is CreatureCard)
            NotifyCreatureEntered(chosen, player);
        if (chosen is LandCard)
            NotifyLandEntered(chosen, player);
        if ((chosen is ArtifactCard) || (chosen is CreatureCard cc && cc.color.Contains("Artifact")))
            NotifyArtifactEntered(chosen, player);
        if (chosen is EnchantmentCard)
            NotifyEnchantmentEntered(chosen, player);

        RefreshGraveyardVisuals(player);
        UpdateUI();
    }

    public void SearchLibraryForRandomBasicLandToBattlefieldTapped(Player player)
    {
        var basicLands = player.Deck
            .Where(card =>
            {
                CardData data = CardDatabase.GetCardData(card.cardName);
                return CardData.IsBasicLand(data);
            })
            .ToList();

        if (basicLands.Count == 0)
        {
            ShuffleDeck(player);
            UpdateUI();
            return;
        }

        Card chosen = basicLands[Random.Range(0, basicLands.Count)];
        player.Deck.Remove(chosen);
        player.Battlefield.Add(chosen);

        if (chosen is LandCard land)
            land.isTapped = true;

        Transform parent = player == humanPlayer ? playerLandArea : aiLandArea;
        GameObject obj = Instantiate(cardPrefab, parent);
        CardVisual visual = obj.GetComponent<CardVisual>();
        CardData chosenData = CardDatabase.GetCardData(chosen.cardName);
        visual.Setup(chosen, this, chosenData);
        visual.isInBattlefield = true;
        activeCardVisuals.Add(visual);
        visual.UpdateVisual();

        chosen.OnEnterPlay(player);
        NotifyLandEntered(chosen, player);

        Debug.Log($"{player} searches for a random basic land ({chosen.cardName}) and puts it onto the battlefield tapped.");

        ShuffleDeck(player);
        UpdateUI();
    }

    public void SearchLibraryForRandomPotion(Player player)
    {
        var potions = player.Deck
            .Where(card => card.subtypes.Contains("Potion"))
            .ToList();
        if (potions.Count == 0)
        {
            ShuffleDeck(player);
            return;
        }

        Card chosen = potions[Random.Range(0, potions.Count)];
        player.Deck.Remove(chosen);
        player.Hand.Add(chosen);

        if (player == humanPlayer)
        {
            GameObject obj = Instantiate(cardPrefab, playerHandArea);
            CardVisual visual = obj.GetComponent<CardVisual>();
            CardData data = CardDatabase.GetCardData(chosen.cardName);
            visual.Setup(chosen, this, data);
            activeCardVisuals.Add(visual);
        }
        else if (enemyHandText != null)
        {
            enemyHandText.text = "Hand: " + player.Hand.Count;
        }

        ShuffleDeck(player);
        UpdateUI();
    }

    public void SearchLibraryForRandomPotionToBattlefield(Player player)
    {
        var potions = player.Deck
            .Where(card => card.subtypes.Contains("Potion"))
            .ToList();
        if (potions.Count == 0)
        {
            ShuffleDeck(player);
            UpdateUI();
            return;
        }

        Card chosen = potions[Random.Range(0, potions.Count)];
        player.Deck.Remove(chosen);
        player.Battlefield.Add(chosen);

        if (chosen is CreatureCard creature)
        {
            creature.hasSummoningSickness = true;
            if (creature.entersTapped || IsAllPermanentsEnterTappedActive())
            {
                creature.isTapped = true;
                Debug.Log($"{creature.cardName} enters tapped (due to static effect).");
            }
        }
        else if (chosen is ArtifactCard artifact)
        {
            if (artifact.entersTapped || IsAllPermanentsEnterTappedActive())
            {
                artifact.isTapped = true;
                Debug.Log($"{artifact.cardName} enters tapped (due to static effect).");
            }
        }

        GameObject obj = Instantiate(cardPrefab,
            player == humanPlayer ?
                (chosen is ArtifactCard ? playerArtifactArea : playerBattlefieldArea) :
                (chosen is ArtifactCard ? aiArtifactArea : aiBattlefieldArea));
        CardVisual visual = obj.GetComponent<CardVisual>();
        CardData data = CardDatabase.GetCardData(chosen.cardName);
        visual.Setup(chosen, this, data);
        visual.isInBattlefield = true;
        activeCardVisuals.Add(visual);
        visual.UpdateVisual();

        chosen.OnEnterPlay(player);
        if (chosen is CreatureCard)
            NotifyCreatureEntered(chosen, player);
        if (chosen is LandCard)
            NotifyLandEntered(chosen, player);
        if ((chosen is ArtifactCard) || (chosen is CreatureCard cc && cc.color.Contains("Artifact")))
            NotifyArtifactEntered(chosen, player);
        if (chosen is EnchantmentCard)
            NotifyEnchantmentEntered(chosen, player);

        ShuffleDeck(player);
        UpdateUI();
    }

    public IEnumerator RevealUntilCreature(Player player)
    {
        List<Card> revealedNonCreatures = new List<Card>();
        List<CardVisual> visuals = new List<CardVisual>();
        Card creatureCard = null;
        CardVisual creatureVisual = null;

        int index = 0;
        float spacing = 150f;

        while (player.Deck.Count > 0)
        {
            Card top = player.Deck[0];
            player.Deck.RemoveAt(0);

            GameObject obj = Instantiate(cardPrefab, stackZone);
            obj.transform.localPosition = new Vector3(index * spacing, 0f, 0f);
            CardVisual visual = obj.GetComponent<CardVisual>();
            CardData data = CardDatabase.GetCardData(top.cardName);
            visual.Setup(top, this, data);
            visual.isInStack = true;
            visuals.Add(visual);

            if (top is CreatureCard)
            {
                creatureCard = top;
                creatureVisual = visual;
                break;
            }
            else
            {
                revealedNonCreatures.Add(top);
            }

            index++;
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);

        if (creatureCard != null)
        {
            player.Hand.Add(creatureCard);
            if (player == humanPlayer)
            {
                creatureVisual.transform.SetParent(playerHandArea, false);
                creatureVisual.isInStack = false;
                activeCardVisuals.Add(creatureVisual);
            }
            else
            {
                Destroy(creatureVisual.gameObject);
                if (enemyHandText != null)
                    enemyHandText.text = "Hand: " + player.Hand.Count;
            }
        }

        foreach (var card in revealedNonCreatures)
            player.Deck.Add(card);

        foreach (var visual in visuals)
        {
            if (!(visual.linkedCard is CreatureCard))
                Destroy(visual.gameObject);
        }

        ShuffleDeck(player);
        UpdateUI();
        pendingStackEffects = Mathf.Max(0, pendingStackEffects - 1);
    }

    public IEnumerator RevealUntilLand(Player player)
    {
        List<Card> revealedNonLands = new List<Card>();
        List<CardVisual> visuals = new List<CardVisual>();
        Card landCard = null;
        CardVisual landVisual = null;

        int index = 0;
        float spacing = 150f;

        while (player.Deck.Count > 0)
        {
            Card top = player.Deck[0];
            player.Deck.RemoveAt(0);

            GameObject obj = Instantiate(cardPrefab, stackZone);
            obj.transform.localPosition = new Vector3(index * spacing, 0f, 0f);
            CardVisual visual = obj.GetComponent<CardVisual>();
            CardData data = CardDatabase.GetCardData(top.cardName);
            visual.Setup(top, this, data);
            visual.isInStack = true;
            visuals.Add(visual);

            if (top is LandCard)
            {
                landCard = top;
                landVisual = visual;
                break;
            }
            else
            {
                revealedNonLands.Add(top);
            }

            index++;
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);

        if (landCard != null)
        {
            player.Hand.Add(landCard);
            if (player == humanPlayer)
            {
                landVisual.transform.SetParent(playerHandArea, false);
                landVisual.isInStack = false;
                activeCardVisuals.Add(landVisual);
            }
            else
            {
                Destroy(landVisual.gameObject);
                if (enemyHandText != null)
                    enemyHandText.text = "Hand: " + player.Hand.Count;
            }
        }

        foreach (var card in revealedNonLands)
            player.Deck.Add(card);

        foreach (var visual in visuals)
        {
            if (!(visual.linkedCard is LandCard))
                Destroy(visual.gameObject);
        }

        ShuffleDeck(player);
        UpdateUI();
        pendingStackEffects = Mathf.Max(0, pendingStackEffects - 1);
    }

    public void ReturnRandomInstantOrSorceryFromGraveyard(Player player)
    {
        var spells = player.Graveyard
            .Where(card => card is SorceryCard)
            .ToList();
        if (spells.Count == 0)
            return;

        Card chosen = spells[Random.Range(0, spells.Count)];
        player.Graveyard.Remove(chosen);
        player.Hand.Add(chosen);

        if (player == humanPlayer)
        {
            GameObject obj = Instantiate(cardPrefab, playerHandArea);
            CardVisual visual = obj.GetComponent<CardVisual>();
            CardData data = CardDatabase.GetCardData(chosen.cardName);
            visual.Setup(chosen, this, data);
            activeCardVisuals.Add(visual);
        }
        else if (enemyHandText != null)
        {
            enemyHandText.text = "Hand: " + player.Hand.Count;
        }

        RefreshGraveyardVisuals(player);
        UpdateUI();
    }

    public void ReturnRandomCreatureFromGraveyardToBattlefield(Player player, int maxManaCost)
    {
        var creatures = player.Graveyard
            .OfType<CreatureCard>()
            .Where(c => c.manaCost <= maxManaCost)
            .ToList();
        if (creatures.Count == 0)
            return;

        Card chosen = creatures[Random.Range(0, creatures.Count)];
        player.Graveyard.Remove(chosen);
        player.Battlefield.Add(chosen);

        if (chosen is CreatureCard creature)
        {
            creature.hasSummoningSickness = true;
            if (creature.entersTapped || IsAllPermanentsEnterTappedActive())
            {
                creature.isTapped = true;
                Debug.Log($"{creature.cardName} enters tapped (due to static effect).");
            }
        }

        GameObject obj = Instantiate(cardPrefab, player == humanPlayer ? playerBattlefieldArea : aiBattlefieldArea);
        CardVisual visual = obj.GetComponent<CardVisual>();
        CardData data = CardDatabase.GetCardData(chosen.cardName);
        visual.Setup(chosen, this, data);
        visual.isInBattlefield = true;
        activeCardVisuals.Add(visual);
        visual.UpdateVisual();

        chosen.OnEnterPlay(player);
        if (chosen is CreatureCard)
            NotifyCreatureEntered(chosen, player);
        if (chosen is LandCard)
            NotifyLandEntered(chosen, player);
        if ((chosen is ArtifactCard) || (chosen is CreatureCard cc && cc.color.Contains("Artifact")))
            NotifyArtifactEntered(chosen, player);
        if (chosen is EnchantmentCard)
            NotifyEnchantmentEntered(chosen, player);

        RefreshGraveyardVisuals(player);
        UpdateUI();
    }

    public void ReturnRandomZombieFromGraveyardToBattlefield(Player player)
    {
        var zombies = player.Graveyard
            .OfType<CreatureCard>()
            .Where(c => c.subtypes.Contains("Zombie"))
            .ToList();
        if (zombies.Count == 0)
            return;

        Card chosen = zombies[Random.Range(0, zombies.Count)];
        player.Graveyard.Remove(chosen);
        player.Battlefield.Add(chosen);

        if (chosen is CreatureCard creature)
        {
            creature.hasSummoningSickness = true;
            if (creature.entersTapped || IsAllPermanentsEnterTappedActive())
            {
                creature.isTapped = true;
                Debug.Log($"{creature.cardName} enters tapped (due to static effect).");
            }
        }

        GameObject obj = Instantiate(cardPrefab, player == humanPlayer ? playerBattlefieldArea : aiBattlefieldArea);
        CardVisual visual = obj.GetComponent<CardVisual>();
        CardData data = CardDatabase.GetCardData(chosen.cardName);
        visual.Setup(chosen, this, data);
        visual.isInBattlefield = true;
        activeCardVisuals.Add(visual);
        visual.UpdateVisual();

        chosen.OnEnterPlay(player);
        if (chosen is CreatureCard)
            NotifyCreatureEntered(chosen, player);
        if (chosen is LandCard)
            NotifyLandEntered(chosen, player);
        if ((chosen is ArtifactCard) || (chosen is CreatureCard cc && cc.color.Contains("Artifact")))
            NotifyArtifactEntered(chosen, player);
        if (chosen is EnchantmentCard)
            NotifyEnchantmentEntered(chosen, player);

        RefreshGraveyardVisuals(player);
        UpdateUI();
    }

    public void ReturnCreatureFromGraveyardToBattlefield(Player player, CreatureCard creature)
    {
        if (creature == null || !player.Graveyard.Contains(creature))
            return;

        player.Graveyard.Remove(creature);
        player.Battlefield.Add(creature);

        creature.hasSummoningSickness = true;
        if (creature.entersTapped || IsAllPermanentsEnterTappedActive())
        {
            creature.isTapped = true;
            Debug.Log($"{creature.cardName} enters tapped from graveyard" +
                      (IsAllPermanentsEnterTappedActive() && !creature.entersTapped ?
                       " due to global effect" :
                       "") + ".");
        }

        GameObject obj = Instantiate(cardPrefab, player == humanPlayer ? playerBattlefieldArea : aiBattlefieldArea);
        CardVisual visual = obj.GetComponent<CardVisual>();
        CardData data = CardDatabase.GetCardData(creature.cardName);
        visual.Setup(creature, this, data);
        visual.isInBattlefield = true;
        activeCardVisuals.Add(visual);
        visual.UpdateVisual();

        creature.OnEnterPlay(player);
        NotifyCreatureEntered(creature, player);
        if (creature.color.Contains("Artifact"))
            NotifyArtifactEntered(creature, player);

        RefreshGraveyardVisuals(player);
        if (graveyardViewActive && graveyardUIManager != null)
            graveyardUIManager.Open(player.Graveyard);
        UpdateUI();
    }

    public void ReturnCreatureFromGraveyardToHand(Player player, CreatureCard creature)
    {
        if (creature == null || !player.Graveyard.Contains(creature))
            return;

        player.Graveyard.Remove(creature);
        player.Hand.Add(creature);

        if (player == humanPlayer)
        {
            GameObject obj = Instantiate(cardPrefab, playerHandArea);
            CardVisual visual = obj.GetComponent<CardVisual>();
            CardData data = CardDatabase.GetCardData(creature.cardName);
            visual.Setup(creature, this, data);
            activeCardVisuals.Add(visual);
        }

        RefreshGraveyardVisuals(player);
        if (graveyardViewActive && graveyardUIManager != null)
            graveyardUIManager.Open(player.Graveyard);
        UpdateUI();
    }

    public void TapCardForMana(CreatureCard creature)
    {
        if (creature == null || creature.isTapped)
            return;

        creature.isTapped = true;
        Player owner = GetOwnerOfCard(creature) ?? humanPlayer;
        owner.ColoredMana.AddMana(creature.GetActivationColor());
        UpdateUI();
    }

    public void TapToLoseLife(CreatureCard creature)
    {
        if (creature.isTapped || creature.hasSummoningSickness)
            return;

        creature.isTapped = true;

        Player owner = GetOwnerOfCard(creature);            // FIXED: declare 'owner'
        Player opponent = GetOpponentOf(owner);

        opponent.Life -= creature.tapLifeLossAmount;
        Debug.Log($"{creature.cardName} tapped: opponent loses {creature.tapLifeLossAmount} life.");

        SoundManager.Instance.PlaySound(SoundManager.Instance.plague);
        ShowBloodSplatVFX(creature);

        if (owner == humanPlayer)
            ShowFloatingDamage(creature.tapLifeLossAmount, enemyLifeContainer);
        else
            ShowFloatingDamage(creature.tapLifeLossAmount, playerLifeContainer);

        UpdateUI();
        CheckForGameEnd();
    }

    public void ShowBloodSplatVFX(Card card)
    {
        Debug.Log("ShowBloodSplatVFX triggered on: " + card.cardName);

        CardVisual visual = FindCardVisual(card);
        if (visual == null)
        {
            Debug.LogWarning("No visual found for card " + card.cardName);
            return;
        }

        Vector3 spawnPos = visual.transform.position;
        spawnPos.z = 0f;
        spawnPos += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);

        GameObject vfx = Instantiate(bloodSplatPrefab, spawnPos, Quaternion.identity);
        Destroy(vfx, 1.5f);
        Debug.Log("Spawned blood VFX at " + spawnPos);
    }

    public void PayToGainAbility(CreatureCard creature)
    {
        if (creature.isTapped) return;
        if (creature.temporaryKeywordAbilities.Contains(creature.abilityToGain))
        {
            Debug.Log($"{creature.cardName} already gained {creature.abilityToGain} this turn.");
            return;
        }

        Player owner = GetOwnerOfCard(creature);
        int cost = creature.manaToPayToActivate;
        string abilityColor = creature.GetActivationColor();

        if (!owner.ColoredMana.SpendColor(abilityColor, cost))
        {
            if (ManaColorUtility.NormalizeColor(abilityColor) == "Colorless")
            {
                Debug.Log($"Not enough mana to activate {creature.cardName}'s ability.");
            }
            else
            {
                Debug.Log($"Not enough {ManaColorUtility.GetDisplayName(abilityColor)} mana to activate {creature.cardName}'s ability.");
            }
            return;
        }

            if (!creature.keywordAbilities.Contains(creature.abilityToGain))
                creature.keywordAbilities.Add(creature.abilityToGain);
            if (!creature.temporaryKeywordAbilities.Contains(creature.abilityToGain))
                creature.temporaryKeywordAbilities.Add(creature.abilityToGain);
            Debug.Log($"{creature.cardName} gains {creature.abilityToGain} until end of turn.");
            var visual = FindCardVisual(creature);
            if (visual != null)
            {
                visual.UpdateVisual();
                if (CardHoverPreview.Instance != null &&
                    CardHoverPreview.Instance.CurrentCard == creature)
                {
                    CardHoverPreview.Instance.ShowCard(creature);
                }
            }
            UpdateUI();
    }

    public void PayToBuffSelf(CreatureCard creature)
    {
        Player owner = GetOwnerOfCard(creature);
        int cost = creature.manaToPayToActivate;
        string abilityColor = creature.GetActivationColor();

        if (!owner.ColoredMana.SpendColor(abilityColor, cost))
        {
            if (ManaColorUtility.NormalizeColor(abilityColor) == "Colorless")
            {
                Debug.Log($"Not enough mana to activate {creature.cardName}'s ability.");
            }
            else
            {
                Debug.Log($"Not enough {ManaColorUtility.GetDisplayName(abilityColor)} mana to activate {creature.cardName}'s ability.");
            }
            return;
        }

            int powerBuff = creature.buffPower == 0 && creature.buffToughness == 0 ? 1 : creature.buffPower;
            int toughnessBuff = creature.buffPower == 0 && creature.buffToughness == 0 ? 0 : creature.buffToughness;

            creature.AddTemporaryBuff(powerBuff, toughnessBuff);
            var vis = FindCardVisual(creature);
            if (vis != null)
                vis.UpdateVisual();

            Debug.Log($"{creature.cardName} gets +{powerBuff}/+{toughnessBuff} until end of turn.");
            UpdateUI();
    }

    public Player GetOwnerOfCard(Card card)
    {
        if (humanPlayer.Battlefield.Contains(card) ||
            humanPlayer.Hand.Contains(card) ||
            humanPlayer.Graveyard.Contains(card) ||
            humanPlayer.Deck.Contains(card))
        {
            return humanPlayer;
        }

        if (aiPlayer.Battlefield.Contains(card) ||
            aiPlayer.Hand.Contains(card) ||
            aiPlayer.Graveyard.Contains(card) ||
            aiPlayer.Deck.Contains(card))
        {
            return aiPlayer;
        }

        if (card.owner != null)
            return card.owner;

        Debug.LogWarning($"[GetOwnerOfCard] Couldn't find owner of {card.cardName}");
        return null;
    }

    public Player GetControllerOfCard(Card card)
    {
        if (humanPlayer.Battlefield.Contains(card))
            return humanPlayer;
        if (aiPlayer.Battlefield.Contains(card))
            return aiPlayer;
        return null;
    }

    public void ChangeController(Card card, Player newController)
    {
        Player current = GetControllerOfCard(card);
        if (current == newController || newController == null || current == null)
            return;

        current.Battlefield.Remove(card);
        newController.Battlefield.Add(card);

        CardVisual visual = FindCardVisual(card);
        if (visual != null)
        {
            Transform area;
            if (card is LandCard)
                area = newController == humanPlayer ? playerLandArea : aiLandArea;
            else if (card is ArtifactCard)
                area = newController == humanPlayer ? playerArtifactArea : aiArtifactArea;
            else if (card is EnchantmentCard)
                area = newController == humanPlayer ? playerEnchantmentArea : aiEnchantmentArea;
            else
                area = newController == humanPlayer ? playerBattlefieldArea : aiBattlefieldArea;

            visual.transform.SetParent(area, false);
            visual.UpdateVisual();
        }

        HandleControlChange(card, newController);
        UpdateUI();
    }

    private void HandleControlChange(Card card, Player newController)
    {
        if (card is CreatureCard creature && card.cardName == "Untamed Unicorn")
        {
            int plains = newController.Battlefield.Count(c => c.cardName == "Plains");
            creature.basePower = plains;
            creature.baseToughness = plains;
            creature.RecalculateStats();
            CheckDeaths(newController);
        }
    }

    public void UpdateUI()
    {
        foreach (var visual in activeCardVisuals)
        {
            if (visual.isInBattlefield)
                visual.UpdateVisual();
        }

        if (enemyHandText != null)
            enemyHandText.text = "Hand: " + aiPlayer.Hand.Count;

        if (playerLifeText != null)
            playerLifeText.text = "" + humanPlayer.Life;

        if (enemyLifeText != null)
            enemyLifeText.text = "" + aiPlayer.Life;

        if (playerDeckCountText != null)
            playerDeckCountText.text = "" + humanPlayer.Deck.Count;
        if (playerGraveyardCountText != null)
            playerGraveyardCountText.text = "" + humanPlayer.Graveyard.Count;
        if (enemyDeckCountText != null)
            enemyDeckCountText.text = "" + aiPlayer.Deck.Count;
        if (enemyGraveyardCountText != null)
            enemyGraveyardCountText.text = "" + aiPlayer.Graveyard.Count;

        if (manaPoolText != null)
        {
            // Replace mana text with icon updates
            UpdateManaIcon(whiteManaIcon, whiteManaText, humanPlayer.ColoredMana.White);
            UpdateManaIcon(blueManaIcon, blueManaText, humanPlayer.ColoredMana.Blue);
            UpdateManaIcon(blackManaIcon, blackManaText, humanPlayer.ColoredMana.Black);
            UpdateManaIcon(redManaIcon, redManaText, humanPlayer.ColoredMana.Red);
            UpdateManaIcon(greenManaIcon, greenManaText, humanPlayer.ColoredMana.Green);
            UpdateManaIcon(colorlessManaIcon, colorlessManaText, humanPlayer.ColoredMana.Colorless);
        }
    }

    private void UpdateManaIcon(Image icon, TMP_Text label, int amount)
        {
            if (icon != null)
                icon.color = (amount > 0) ? Color.white : Color.black;

            if (label != null)
                label.text = amount.ToString();
        }

    // Ensures the graveyard count text stays above newly added cards
    private void EnsureGraveyardCounterOnTop(Player owner)
    {
        if (owner == humanPlayer)
        {
            if (playerGraveyardCountText != null)
                playerGraveyardCountText.transform.SetAsLastSibling();
        }
        else if (owner == aiPlayer)
        {
            if (enemyGraveyardCountText != null)
                enemyGraveyardCountText.transform.SetAsLastSibling();
        }
    }

    public void RefreshGraveyardVisuals(Player player)
        {
            var graveyardVisuals = activeCardVisuals
                .Where(cv => cv.isInGraveyard && GetOwnerOfCard(cv.linkedCard) == player)
                .ToList();

            foreach (var visual in graveyardVisuals)
            {
                activeCardVisuals.Remove(visual);
                Destroy(visual.gameObject);
            }

            foreach (var card in player.Graveyard)
            {
                if (card.isToken) continue;

                GameObject visualGO = Instantiate(cardPrefab,
                    player == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
                CardVisual graveyardVisual = visualGO.GetComponent<CardVisual>();
                graveyardVisual.Setup(card, this);
                graveyardVisual.transform.localPosition = Vector3.zero;
                graveyardVisual.UpdateGraveyardVisual();
                // Keep UI overlay elements above the cards
                // Newest card should appear on top
                graveyardVisual.transform.SetAsLastSibling();
                EnsureGraveyardCounterOnTop(player);

                activeCardVisuals.Add(graveyardVisual);
            }
        }

    public void ShowPlayerGraveyard()
        {
            if (graveyardUIManager != null)
                graveyardUIManager.Open(humanPlayer.Graveyard);
        }

    public void ShowOpponentGraveyard()
        {
            if (graveyardUIManager != null)
                graveyardUIManager.Open(aiPlayer.Graveyard);
        }

    public void ClosePlayerGraveyard()
        {
            if (graveyardUIManager != null)
                graveyardUIManager.Close();
        }

    public void WinBattle()
    {
        // 2D map zone unlock flow is deprecated while we transition to the 3D world.
        // Keep this logic disabled to avoid warning spam when battles no longer map to a zone ID.
        // if (!string.IsNullOrEmpty(BattleData.CurrentZoneId))
        // {
        //     Debug.Log("Player won battle at zone ID: " + BattleData.CurrentZoneId);
        //     PlayerPrefs.SetString("LastCompletedZone", BattleData.CurrentZoneId);
        //     PlayerPrefs.Save();
        // }
        // else
        // {
        //     Debug.LogWarning("No zone ID found when trying to WinBattle.");
        // }

        // Flag the merchant inventory to refresh on the next visit
        PlayerPrefs.SetInt("RefreshMerchant", 1);
        PlayerPrefs.Save();

        ReturnToPreviousScene(applyWinEffects: true);
    }

    public void ReturnToPreviousScene(bool applyWinEffects)
    {
        string returnSceneName = BattleData.GetReturnScene();

        if (BattleData.IsBattleOpenedAdditively)
        {
            Scene returnScene = SceneManager.GetSceneByName(returnSceneName);
            if (returnScene.IsValid() && returnScene.isLoaded)
            {
                BattleData.ResumeReturnScene();
                EventSystemUtility.EnableOnlyForScene(returnScene);
                EventSystemUtility.EnsureSingleAudioListener(returnScene);
                SceneManager.SetActiveScene(returnScene);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (applyWinEffects)
                {
                    BattleData.TriggeringPlatform?.ApplyCrippledState();
                    BattleData.TriggeringPlatform?.PlayPostBeginnerBattleWinSubtitlesIfNeeded();
                }

                BattleData.TriggeringPlatform = null;
                BattleData.IsBattleOpenedAdditively = false;
                BattleData.ReturnSceneName = null;
                SceneManager.UnloadSceneAsync("GameScene");
                return;
            }
        }

        BattleData.ResumeReturnScene();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (applyWinEffects)
        {
            BattleData.TriggeringPlatform?.ApplyCrippledState();
            BattleData.TriggeringPlatform?.PlayPostBeginnerBattleWinSubtitlesIfNeeded();
        }

        BattleData.TriggeringPlatform = null;
        BattleData.IsBattleOpenedAdditively = false;
        BattleData.ReturnSceneName = null;
        SceneManager.LoadScene(returnSceneName);
    }

    void PickRandomBeginnerDeck(Player ai)
    {
        var decks = new System.Action<Player>[]
        {
                    DeckDatabase.BuildWhiteBeginnerDeck,
                    DeckDatabase.BuildBlueBeginnerDeck,
                    DeckDatabase.BuildBlackBeginnerDeck,
                    DeckDatabase.BuildRedBeginnerDeck,
                    DeckDatabase.BuildGreenBeginnerDeck,
                    DeckDatabase.BuildRuinsDeck
        };

        decks[Random.Range(0, decks.Length)](ai);
    }

    void PickRandomAdvancedDeck(Player ai)
    {
        var decks = new System.Action<Player>[]
        {
                    DeckDatabase.BuildWhiteAdvancedDeck,
                    DeckDatabase.BuildBlueAdvancedDeck,
                    DeckDatabase.BuildBlackAdvancedDeck,
                    DeckDatabase.BuildRedAdvancedDeck,
                    DeckDatabase.BuildGreenAdvancedDeck
        };

        decks[Random.Range(0, decks.Length)](ai);
    }

    void LoadDeckByKey(Player ai, string key)
    {
        switch (key)
        {
            // STARTER + BOSS
            case "Deck_Starter":
                DeckDatabase.BuildStarterDeck(ai);
                break;
            case "Deck_Farmer":
                DeckDatabase.BuildFarmerDeck(ai);
                break;
            case "Deck_Guard":
                DeckDatabase.BuildGuardDeck(ai);
                break;
            case "Deck_Monk":
                DeckDatabase.BuildMonkDeck(ai);
                break;
            case "Deck_Corpse":
                DeckDatabase.BuildCorpseDeck(ai);
                break;
            case "Deck_Fisher":
                DeckDatabase.BuildFisherDeck(ai);
                break;
            case "Deck_Gipsy":
                DeckDatabase.BuildGipsyDeck(ai);
                break;
            case "Deck_OldWomanDruid":
                DeckDatabase.BuildOldWomanDruidDeck(ai);
                break;
            case "Deck_PhantomWarrior":
                DeckDatabase.BuildPhantomWarriorDeck(ai);
                break;
            case "Deck_Boss":
                DeckDatabase.BuildBossDeck(ai);
                break;

            // BEGINNER (6)
            case "Deck_Shore":
                DeckDatabase.BuildBlueBeginnerDeck(ai);
                break;
            case "Deck_Camp":
                DeckDatabase.BuildRedBeginnerDeck(ai);
                break;
            case "Deck_Graveyard":
                DeckDatabase.BuildBlackBeginnerDeck(ai);
                break;
            case "Deck_Thicket":
                DeckDatabase.BuildGreenBeginnerDeck(ai);
                break;
            case "Deck_Village":
                DeckDatabase.BuildWhiteBeginnerDeck(ai);
                break;
            case "Deck_Ruins":
                DeckDatabase.BuildRuinsDeck(ai);
                break;

            // ADVANCED (5)
            case "Deck_Church":
                DeckDatabase.BuildWhiteAdvancedDeck(ai);
                break;
            case "Deck_Tower":
                DeckDatabase.BuildBlueAdvancedDeck(ai);
                break;
            case "Deck_Hut":
                DeckDatabase.BuildBlackAdvancedDeck(ai);
                break;
            case "Deck_Nest":
                DeckDatabase.BuildRedAdvancedDeck(ai);
                break;
            case "Deck_Woods":
                DeckDatabase.BuildGreenAdvancedDeck(ai);
                break;

            default:
                Debug.LogWarning("Unknown deck key: " + key + " — using fallback.");
                DeckDatabase.BuildStarterDeck(ai);
                break;
        }
    }

    public bool IsAllPermanentsEnterTappedActive()
    {
        return humanPlayer.Battlefield.Concat(aiPlayer.Battlefield)
            .Any(card => card.keywordAbilities != null &&
                        card.keywordAbilities.Contains(KeywordAbility.AllPermanentsEnterTapped));
    }

    public bool IsLifeGainPrevented()
    {
        return humanPlayer.Battlefield.Concat(aiPlayer.Battlefield)
            .Any(card => card.keywordAbilities != null &&
                        card.keywordAbilities.Contains(KeywordAbility.NoLifeGain));
    }

    public bool IsOnlyCastCreatureSpellsActive()
    {
        return humanPlayer.Battlefield.Concat(aiPlayer.Battlefield)
            .Any(card => card.keywordAbilities != null &&
                        card.keywordAbilities.Contains(KeywordAbility.OnlyCastCreatureSpells));
    }

    public bool IsHasteCreaturesOnlyBlockedByHasteActive(Player player)
    {
        return player.Battlefield.Any(card => card.keywordAbilities != null &&
            card.keywordAbilities.Contains(KeywordAbility.HasteCreaturesOnlyBlockedByHaste));
    }

    public int GetCreatureCostReduction(Player player)
    {
        return player.Battlefield.Count(card => card.keywordAbilities != null &&
            card.keywordAbilities.Contains(KeywordAbility.CreatureSpellsCostOneLess));
    }

    public int GetBeastCreatureCostReduction(Player player)
    {
        return player.Battlefield.Count(card => card.keywordAbilities != null &&
            card.keywordAbilities.Contains(KeywordAbility.BeastCreatureSpellsCostOneLess));
    }

    public int GetPotionCostReduction(Player player)
    {
        return player.Battlefield.Count(card => card.keywordAbilities != null &&
            card.keywordAbilities.Contains(KeywordAbility.PotionSpellsCostOneLess));
    }

    public int GetOpponentSpellTax(Player player)
    {
        Player opponent = GetOpponentOf(player);
        return opponent.Battlefield.Count(card => card.keywordAbilities != null &&
            card.keywordAbilities.Contains(KeywordAbility.OpponentSpellsCostOneMore));
    }

    public void TryGainLife(Player player, int amount, bool showVFX = true)
    {
        if (amount <= 0 || IsLifeGainPrevented())
            return;

        player.Life += amount;
        UpdateUI();

        NotifyLifeGain(player, amount);

        if (showVFX)
        {
            if (player == humanPlayer)
                ShowFloatingHeal(amount, playerLifeContainer);
            else
                ShowFloatingHeal(amount, enemyLifeContainer);
        }
    }



    public void BeginTargetSelection(SorceryCard sorcery, Player caster, CardVisual visual)
        {
            targetingSorcery = sorcery;
            targetingPlayer = caster;
            targetingVisual = visual;
            isTargetingMode = true;

            if (!sorcery.requiresTarget)
            {
                Debug.LogWarning("BeginTargetSelection called for non-targeting sorcery.");
                return;
            }

            // Highlight the selected card
            if (visual != null)
                visual.EnableTargetingHighlight(true);

            // Check if any valid targets exist (but do not highlight anything)
            foreach (var cv in activeCardVisuals)
            {
                if (cv == null || cv.linkedCard == null)
                    continue;

                Card target = cv.linkedCard;

                bool correctType =
                    (sorcery.requiredTargetType == SorceryCard.TargetType.Creature && target is CreatureCard) ||
                    (sorcery.requiredTargetType == SorceryCard.TargetType.TappedCreature && target is CreatureCard tc && tc.isTapped) ||
                    (sorcery.requiredTargetType == SorceryCard.TargetType.Land && target is LandCard) ||
                    (sorcery.requiredTargetType == SorceryCard.TargetType.Artifact && sorcery.IsValidArtifactTarget(target)) ||
                    (sorcery.requiredTargetType == SorceryCard.TargetType.Enchantment && target is EnchantmentCard) ||
                    (sorcery.requiredTargetType == SorceryCard.TargetType.CreatureOrPlayer && target is CreatureCard);

                bool isOnBattlefield = GetOwnerOfCard(target)?.Battlefield.Contains(target) == true;

                bool colorMatches = true;
                if (!string.IsNullOrEmpty(sorcery.requiredTargetColor))
                {
                    CardData data = CardDatabase.GetCardData(target.cardName);
                    colorMatches = data != null && data.color.Contains(targetingSorcery.requiredTargetColor);
                }

                bool excludedColorMatches = true;
                if (!string.IsNullOrEmpty(sorcery.excludedTargetColor))
                {
                    CardData data = CardDatabase.GetCardData(target.cardName);
                    excludedColorMatches = data == null || !data.color.Contains(sorcery.excludedTargetColor);
                }

                bool nonTokenMatches = !(sorcery.requireNonTokenTarget && target is CreatureCard creatureTarget && creatureTarget.isToken);
                bool nonArtifactMatches = !(sorcery.excludeArtifactCreatures && target is CreatureCard artifactCreature && artifactCreature.color.Contains("Artifact"));

                if (correctType && isOnBattlefield && colorMatches && excludedColorMatches && nonTokenMatches && nonArtifactMatches && !IsProtectedFromSpell(target))
                {
                    // Valid target exists, but no visual feedback is shown
                }
            }
        }

    private IEnumerator ResolveTargetedSorceryAfterDelay(Card target, Player caster, SorceryCard sorcery, CardVisual visual)
    {
        yield return WaitForStackOrSkip(2f);

        // Give AI a chance to protect a threatened creature with Unsummon.
        TryAIDefensiveUnsummonResponse(sorcery, target, caster);

        // The card-specific ResolveEffect(target) already invokes the general
        // ResolveEffect method internally. Calling it again here caused cards
        // such as Forced Mummification to create two zombies and Stain of Rot
        // to make the opponent lose 4 life instead of 2. Run it only once.
        sorcery.ResolveEffect(caster, target);
        SendToGraveyard(sorcery, caster, fromStack: true);

        if (caster == aiPlayer && visual != null)
        {
            activeCardVisuals.Remove(visual);
            Destroy(visual.gameObject);
        }

        UpdateUI();
        isStackBusy = false;
    }

    public void CompleteTargetSelection(CardVisual targetVisual)
    {
            Card chosen = targetVisual.linkedCard;

            // Creature destroy ability (power 4 or greater)
            if (targetingCreatureActivated != null &&
                targetingCreatureActivated.activatedAbilities.Contains(ActivatedAbility.TapToDestroyPower4OrGreater))
            {
                if (chosen is CreatureCard targetCreature &&
                    targetCreature.power >= 4 &&
                    GetOwnerOfCard(targetCreature)?.Battlefield.Contains(targetCreature) == true)
                {
                    Player controller = targetingPlayer;
                    QueueCreatureActivatedAbility(targetingCreatureActivated, ActivatedAbility.TapToDestroyPower4OrGreater, controller, targetCreature);
                    UpdateUI();
                }
                else
                {
                    Debug.Log("Invalid target. Must target a creature with power 4 or greater.");
                    targetingCreatureActivated.isTapped = false;
                }

                targetingCreatureActivated = null;
                targetingPlayer = null;
                targetingVisual = null;
                isTargetingMode = false;
                return;
            }

            // Creature ping ability (any target)
            if (targetingCreatureActivated != null &&
                targetingCreatureActivated.activatedAbilities.Contains(ActivatedAbility.TapToDealDamageAnyTarget))
            {
                if (chosen is CreatureCard targetCreature &&
                    GetOwnerOfCard(targetCreature)?.Battlefield.Contains(targetCreature) == true)
                {
                    Player controller = targetingPlayer;
                    string color = targetingCreatureActivated.GetActivationColor();
                    int cost = targetingCreatureActivated.manaToPayToActivate;

                    if (!controller.ColoredMana.HasEnough(color, cost))
                    {
                        Debug.LogWarning("Not enough mana to activate creature ability.");
                        CancelTargeting();
                        return;
                    }

                    controller.ColoredMana.SpendColor(color, cost);
                    QueueCreatureActivatedAbility(targetingCreatureActivated, ActivatedAbility.TapToDealDamageAnyTarget, controller, targetCreature);
                    UpdateUI();
                }
                else
                {
                    Debug.Log("Invalid target. Creature effect canceled.");
                    targetingCreatureActivated.isTapped = false;
                }

                targetingCreatureActivated = null;
                targetingPlayer = null;
                targetingVisual = null;
                isTargetingMode = false;
                return;
            }

            // Artifact damage ability
            if (targetingArtifact != null &&
                targetingArtifact.activatedAbilities.Contains(ActivatedAbility.DealDamageToCreature))
            {
                if (chosen is CreatureCard targetCreature &&
                    GetOwnerOfCard(targetCreature)?.Battlefield.Contains(targetCreature) == true)
                {
                    Player controller = targetingPlayer;
                    int remaining = targetingArtifact.manaToPayToActivate;

                    remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Colorless, remaining);
                    remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.White, remaining);
                    remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Blue, remaining);
                    remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Black, remaining);
                    remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Red, remaining);
                    remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Green, remaining);

                    if (remaining > 0)
                    {
                        Debug.LogWarning("Not enough mana to activate artifact.");
                        CancelTargeting();
                        return;
                    }

                    ArtifactCard artifact = targetingArtifact;
                    targetingArtifact.isTapped = true;
                    SendToGraveyard(targetingArtifact, controller);
                    QueueArtifactActivatedAbility(artifact, ActivatedAbility.DealDamageToCreature, controller, targetCreature);
                    UpdateUI();
                }
                else
                {
                    Debug.Log("Invalid target. Artifact effect canceled.");
                    targetingArtifact.isTapped = false;
                }

                targetingArtifact = null;
                targetingPlayer = null;
                targetingVisual = null;
                isTargetingMode = false;
                return;
            }
        // Artifact buff ability
        if (targetingArtifact != null &&
            targetingArtifact.activatedAbilities.Contains(ActivatedAbility.BuffTargetCreature))
        {
            if (chosen is CreatureCard targetCreature &&
                GetOwnerOfCard(targetCreature)?.Battlefield.Contains(targetCreature) == true)
            {
                Player controller = targetingPlayer;
                int remaining = targetingArtifact.manaToPayToActivate;

                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Colorless, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.White, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Blue, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Black, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Red, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Green, remaining);

                if (remaining > 0)
                {
                    Debug.LogWarning("Not enough mana to activate artifact.");
                    CancelTargeting();
                    return;
                }

                ArtifactCard artifact = targetingArtifact;
                targetingArtifact.isTapped = true;
                SendToGraveyard(targetingArtifact, controller);
                QueueArtifactActivatedAbility(artifact, ActivatedAbility.BuffTargetCreature, controller, targetCreature);
                UpdateUI();
            }
            else
            {
                Debug.Log("Invalid target. Artifact effect canceled.");
                targetingArtifact.isTapped = false;
            }

            targetingArtifact = null;
            targetingPlayer = null;
            targetingVisual = null;
            isTargetingMode = false;
            return;
        }
        // Artifact tap-target ability
        if (targetingArtifact != null &&
            targetingArtifact.activatedAbilities.Contains(ActivatedAbility.TapTargetArtifactCreatureOrLand))
        {
            bool isValidTarget = (chosen is ArtifactCard || chosen is CreatureCard || chosen is LandCard) &&
                                 GetOwnerOfCard(chosen)?.Battlefield.Contains(chosen) == true;

            if (isValidTarget)
            {
                Player controller = targetingPlayer;
                int remaining = targetingArtifact.manaToPayToActivate;

                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Colorless, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.White, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Blue, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Black, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Red, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Green, remaining);

                if (remaining > 0)
                {
                    Debug.LogWarning("Not enough mana to activate artifact.");
                    CancelTargeting();
                    return;
                }

                ArtifactCard artifact = targetingArtifact;
                targetingArtifact.isTapped = true;
                QueueArtifactActivatedAbility(artifact, ActivatedAbility.TapTargetArtifactCreatureOrLand, controller, chosen);
                UpdateUI();
            }
            else
            {
                Debug.Log("Invalid target. Artifact effect canceled.");
                targetingArtifact.isTapped = false;
            }

            targetingArtifact = null;
            targetingPlayer = null;
            targetingVisual = null;
            isTargetingMode = false;
            return;
        }

        // Equipment equip ability
        if (targetingEquipment != null &&
            targetingEquipment.activatedAbilities.Contains(ActivatedAbility.Equip))
        {
            if (chosen is CreatureCard targetCreature &&
                GetOwnerOfCard(targetCreature) == targetingPlayer &&
                targetingPlayer.Battlefield.Contains(targetCreature))
            {
                Player controller = targetingPlayer;
                int remaining = targetingEquipment.EquipCost;

                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Colorless, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.White, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Blue, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Black, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Red, remaining);
                remaining -= Player.ManaPool.SpendFromPool(ref controller.ColoredMana.Green, remaining);

                if (remaining > 0)
                {
                    Debug.LogWarning("Not enough mana to equip artifact.");
                    CancelTargeting();
                    return;
                }

                EquipmentCard equipment = targetingEquipment;
                QueueEquipmentEquipAbility(equipment, targetCreature, controller);

                UpdateUI();
            }
            else
            {
                Debug.Log("Invalid target for equipment.");
            }

            targetingEquipment = null;
            targetingPlayer = null;
            targetingVisual = null;
            isTargetingMode = false;
            return;
        }
        // Aura casting
        if (targetingAura != null)
        {
            Card targetCard = chosen;
            bool correctType =
                (targetingAura.requiredTargetType == SorceryCard.TargetType.Creature && targetCard is CreatureCard) ||
                (targetingAura.requiredTargetType == SorceryCard.TargetType.TappedCreature && targetCard is CreatureCard tc && tc.isTapped) ||
                (targetingAura.requiredTargetType == SorceryCard.TargetType.Artifact && IsArtifactPermanent(targetCard));
            Player targetOwner = GetOwnerOfCard(targetCard);
            bool isOnBattlefield = targetOwner?.Battlefield.Contains(targetCard) == true;
            bool correctController = !targetingAura.targetMustBeControlledCreature || targetOwner == targetingPlayer;

            if (!correctType || !isOnBattlefield || !correctController)
            {
                Debug.Log("Invalid target for aura.");
                CancelTargeting();
                return;
            }

            var cost = GetManaCostBreakdown(targetingAura.manaCost, targetingAura.color);
            int tax = GetOpponentSpellTax(targetingPlayer);
            if (tax > 0)
            {
                if (!cost.ContainsKey("Colorless"))
                    cost["Colorless"] = 0;
                cost["Colorless"] += tax;
            }
            if (!targetingPlayer.ColoredMana.CanPay(cost))
            {
                Debug.LogWarning("Not enough mana to cast aura.");
                CancelTargeting();
                return;
            }

            isStackBusy = true;
            targetingPlayer.ColoredMana.Pay(cost);
            targetingPlayer.Hand.Remove(targetingAura);

            targetingAura.owner = targetingPlayer;
            targetingAura.attachedTo = targetCard;
            UpdateUI();

            if (targetingVisual != null)
            {
                targetingVisual.transform.SetParent(stackZone, false);
                targetingVisual.isInStack = true;
                targetingVisual.transform.localPosition = Vector3.zero;
                targetingVisual.transform.localRotation = Quaternion.identity;
                targetingVisual.transform.localScale = Vector3.one;
                targetingVisual.EnableTargetingHighlight(false);
            }
            SoundManager.Instance.PlaySound(SoundManager.Instance.cardPlay);

            StartCoroutine(ResolveAuraAfterDelay(targetingAura, targetingVisual, targetingPlayer));

            targetingAura = null;
            targetingPlayer = null;
            targetingVisual = null;
            isTargetingMode = false;
            return;
        }
        // Creature ETB targeting
        if (targetingCreature != null && targetingAbility != null)
        {
                Card target = targetVisual.linkedCard;

                bool correctType =
                    (targetingAbility.requiredTargetType == SorceryCard.TargetType.Creature && target is CreatureCard creature &&
                        !(targetingAbility.excludeArtifactCreatures && creature.color.Contains("Artifact"))) ||
                    (targetingAbility.requiredTargetType == SorceryCard.TargetType.Land && target is LandCard) ||
                    (targetingAbility.requiredTargetType == SorceryCard.TargetType.Artifact && target is ArtifactCard) ||
                    (targetingAbility.requiredTargetType == SorceryCard.TargetType.Enchantment && target is EnchantmentCard);

                bool isOnBattlefield = GetOwnerOfCard(target)?.Battlefield.Contains(target) == true;

                if (!correctType || !isOnBattlefield)
                {
                    Debug.LogWarning($"Invalid target: {target.cardName} does not match ETB type.");
                    CancelTargeting();
                    return;
                }

                Debug.Log($"ETB target selected: {target.cardName}");
                targetingAbility.effect?.Invoke(GetOwnerOfCard(targetingCreature), target); // You'll update effect type later if needed

                UpdateUI();
                CheckDeaths(humanPlayer);
                CheckDeaths(aiPlayer);

                targetingCreature = null;
                targetingAbility = null;
                targetingVisual = null;
                isTargetingMode = false;
                return;
            }
            // Sorcery fallback
            if (targetingSorcery != null)
            {
                // Validate type
                bool correctType =
                    (targetingSorcery.requiredTargetType == SorceryCard.TargetType.Creature && chosen is CreatureCard creatureT &&
                        !(targetingSorcery.excludeArtifactCreatures && creatureT.color.Contains("Artifact")) &&
                        !(targetingSorcery.requireNonTokenTarget && creatureT.isToken)) ||
                    (targetingSorcery.requiredTargetType == SorceryCard.TargetType.Land && chosen is LandCard) ||
                    (targetingSorcery.requiredTargetType == SorceryCard.TargetType.Artifact && targetingSorcery.IsValidArtifactTarget(chosen)) ||
                    (targetingSorcery.requiredTargetType == SorceryCard.TargetType.Enchantment && chosen is EnchantmentCard) ||
                    (targetingSorcery.requiredTargetType == SorceryCard.TargetType.CreatureOrPlayer && chosen is CreatureCard creatureOrPlayer &&
                        !(targetingSorcery.requireNonTokenTarget && creatureOrPlayer.isToken));

                // Validate color
                bool colorMatches = true;
                if (!string.IsNullOrEmpty(targetingSorcery.requiredTargetColor))
                {
                    CardData data = CardDatabase.GetCardData(chosen.cardName);
                    colorMatches = data != null && data.color.Contains(targetingSorcery.requiredTargetColor);
                }

                bool excludedColorMatches = true;
                if (!string.IsNullOrEmpty(targetingSorcery.excludedTargetColor))
                {
                    CardData data = CardDatabase.GetCardData(chosen.cardName);
                    excludedColorMatches = data == null || !data.color.Contains(targetingSorcery.excludedTargetColor);
                }

                if (!correctType || !colorMatches || !excludedColorMatches)
                {
                    Debug.LogWarning($"Invalid target: {chosen.cardName} does not match type or color requirements.");
                    return;
                }

                // Pay mana before resolving
                var cost = GetManaCostBreakdown(targetingSorcery.manaCost, targetingSorcery.color);
                int tax = GetOpponentSpellTax(targetingPlayer);
                if (tax > 0)
                {
                    if (!cost.ContainsKey("Colorless"))
                        cost["Colorless"] = 0;
                    cost["Colorless"] += tax;
                }
                if (!targetingPlayer.ColoredMana.CanPay(cost))
                {
                    Debug.LogWarning("Not enough mana to cast targeted sorcery.");
                    CancelTargeting();
                    return;
                }

                targetingPlayer.ColoredMana.Pay(cost);
                if (targetingSorcery.hasXCost)
                {
                    targetingSorcery.xValue = targetingPlayer.ColoredMana.Total();
                    if (targetingSorcery.xValue > 0)
                        targetingPlayer.ColoredMana.SpendGeneric(targetingSorcery.xValue);
                }
                targetingPlayer.Hand.Remove(targetingSorcery);
                UpdateUI();

                targetingSorcery.chosenTarget = chosen;

                Debug.Log($"Target selected: {chosen.cardName}");

                targetingVisual.transform.SetParent(stackZone, false);
                targetingVisual.isInStack = true;
                targetingVisual.transform.localPosition = Vector3.zero;
                targetingVisual.transform.localRotation = Quaternion.identity;
                targetingVisual.transform.localScale = Vector3.one;
                SoundManager.Instance.PlaySound(SoundManager.Instance.cardPlay);

                if (targetingVisual != null)
                    targetingVisual.EnableTargetingHighlight(false);

                StartCoroutine(ResolveTargetedSorceryAfterDelay(chosen, targetingPlayer, targetingSorcery, targetingVisual));

                targetingSorcery = null;
                targetingPlayer = null;
                targetingVisual = null;
                isTargetingMode = false;
            }
        }

    public void CancelTargeting()
        {
            foreach (var cv in activeCardVisuals)
                cv.EnableTargetingHighlight(false); // turn off all

            if (targetingArtifact != null)
            {
                targetingArtifact.isTapped = false; // untap if ability was aborted
                FindCardVisual(targetingArtifact)?.UpdateVisual();
            }

            if (targetingCreatureActivated != null)
            {
                targetingCreatureActivated.isTapped = false;
                FindCardVisual(targetingCreatureActivated)?.UpdateVisual();
            }

            targetingArtifact = null;
            targetingCreatureActivated = null;
            targetingEquipment = null;
            targetingSorcery = null;
            targetingAura = null;
            targetingPlayer = null;

            if (targetingVisual != null)
                targetingVisual.EnableTargetingHighlight(false); // turn off highlight

            targetingVisual = null;
            isTargetingMode = false;
            isStackBusy = false;

            UpdateUI();
        }

    public void CompletePlayerTargetSelection(Player targetPlayer)
    {
        if (targetingCreatureActivated != null &&
            targetingCreatureActivated.activatedAbilities.Contains(ActivatedAbility.TapToDealDamageAnyTarget))
        {
            if (targetingPlayer == null)
            {
                Debug.LogWarning("CompletePlayerTargetSelection called without a controlling player.");
                CancelTargeting();
                return;
            }

            string color = targetingCreatureActivated.GetActivationColor();
            int activationCost = targetingCreatureActivated.manaToPayToActivate;

            if (!targetingPlayer.ColoredMana.HasEnough(color, activationCost))
            {
                Debug.LogWarning("Not enough mana to activate creature ability.");
                CancelTargeting();
                return;
            }

            targetingPlayer.ColoredMana.SpendColor(color, activationCost);
            QueueCreatureActivatedAbility(targetingCreatureActivated, ActivatedAbility.TapToDealDamageAnyTarget, targetingPlayer, null, targetPlayer);

            if (targetingVisual != null)
                targetingVisual.EnableTargetingHighlight(false);

            targetingCreatureActivated = null;
            targetingPlayer = null;
            targetingVisual = null;
            isTargetingMode = false;
            UpdateUI();
            return;
        }

        // Edge case: accidentally triggered for non-sorcery
        if (targetingSorcery == null)
        {
            Debug.LogWarning("CompletePlayerTargetSelection called but no sorcery is being resolved.");
            isTargetingMode = false;
            targetingPlayer = null;
            targetingVisual = null;
            isStackBusy = false;
            UpdateUI();
            return;
        }

        if (targetingPlayer == null)
        {
            Debug.LogWarning("CompletePlayerTargetSelection called without a casting player.");
            CancelTargeting();
            return;
        }

        // Ensure mana is paid before resolving on the stack
        var cost = GetManaCostBreakdown(targetingSorcery.manaCost, targetingSorcery.color);
        int tax = GetOpponentSpellTax(targetingPlayer);
        if (tax > 0)
        {
            if (!cost.ContainsKey("Colorless"))
                cost["Colorless"] = 0;
            cost["Colorless"] += tax;
        }

        if (!targetingPlayer.ColoredMana.CanPay(cost))
        {
            Debug.LogWarning("Not enough mana to cast targeted sorcery.");
            CancelTargeting();
            return;
        }

        targetingPlayer.ColoredMana.Pay(cost);
        if (targetingSorcery.hasXCost)
        {
            targetingSorcery.xValue = targetingPlayer.ColoredMana.Total();
            if (targetingSorcery.xValue > 0)
                targetingPlayer.ColoredMana.SpendGeneric(targetingSorcery.xValue);
        }

        targetingPlayer.Hand.Remove(targetingSorcery);

        if (targetingVisual != null)
            targetingVisual.EnableTargetingHighlight(false);

        // Move visual to stack
        targetingVisual.transform.SetParent(stackZone, false);
            targetingVisual.isInStack = true;
            targetingVisual.transform.localPosition = Vector3.zero;
            targetingVisual.transform.localRotation = Quaternion.identity;
            targetingVisual.transform.localScale = Vector3.one;
            SoundManager.Instance.PlaySound(SoundManager.Instance.cardPlay);

            StartCoroutine(ResolveTargetedSorceryOnPlayerAfterDelay(targetPlayer, targetingPlayer, targetingSorcery, targetingVisual));

            if (targetingPlayer == aiPlayer && targetingVisual != null)
            {
                activeCardVisuals.Remove(targetingVisual);
                Destroy(targetingVisual.gameObject);
            }

            isTargetingMode = false;
            targetingSorcery = null;
            targetingPlayer = null;
            targetingVisual = null;

            UpdateUI();
            isStackBusy = false;
        }

    private bool IsProtectedFromSpell(Card card)
        {
            if (card is CreatureCard creature && targetingSorcery != null)
            {
                KeywordAbility protection = ProtectionUtils.GetProtectionKeyword(targetingSorcery.PrimaryColor);
                return creature.keywordAbilities.Contains(protection);
            }

            return false;
        }

    private IEnumerator ResolveTargetedSorceryOnPlayerAfterDelay(Player targetPlayer, Player caster, SorceryCard sorcery, CardVisual visual)
        {
            yield return WaitForStackOrSkip(2f);

            sorcery.ResolveEffectOnPlayer(caster, targetPlayer);
            SendToGraveyard(sorcery, caster, fromStack: true);

            if (caster == aiPlayer && visual != null)
            {
                activeCardVisuals.Remove(visual);
                Destroy(visual.gameObject);
            }

            UpdateUI();
            isStackBusy = false;
        }
    
    public Dictionary<string, int> GetManaCostBreakdown(int totalCost, List<string> color)
        {
            Dictionary<string, int> breakdown = new Dictionary<string, int>();

            // Treat empty color or "Artifact" as fully colorless
            if (color == null || color.Count == 0 || (color.Count == 1 && color[0] == "Artifact"))
            {
                breakdown["Colorless"] = totalCost;
            }
            else
            {
                foreach (string c in color)
                {
                    if (c == "Artifact") continue; // Don't treat Artifact as colored mana
                    if (!breakdown.ContainsKey(c))
                        breakdown[c] = 0;
                    breakdown[c]++;
                }

                int coloredCount = breakdown.Values.Sum();
                int generic = totalCost - coloredCount;
                if (generic > 0)
                    breakdown["Colorless"] = generic;
            }

            return breakdown;
        }


    public void BeginTargetingWithArtifactDamage(ArtifactCard artifact, Player player, CardVisual visual)
    {
        targetingArtifact = artifact; // << Store the artifact being used
        targetingSorcery = null;
        targetingPlayer = player;
        targetingVisual = visual;
        isTargetingMode = true;

        artifact.isTapped = true; // show the potion tapped while selecting target
        FindCardVisual(artifact)?.UpdateVisual();

        Debug.Log("Targeting creature to deal damage with artifact.");
    }

    public bool IsArtifactPermanent(Card card)
    {
        return card is ArtifactCard || (card is CreatureCard creature && creature.color.Contains("Artifact"));
    }

    public void BeginTargetingWithArtifactBuff(ArtifactCard artifact, Player player, CardVisual visual)
    {
        targetingArtifact = artifact;
        targetingSorcery = null;
        targetingAura = null;
        targetingPlayer = player;
        targetingVisual = visual;
        isTargetingMode = true;

        artifact.isTapped = true; // show the potion tapped while selecting target
        FindCardVisual(artifact)?.UpdateVisual();

        Debug.Log("Targeting creature to buff with artifact.");
    }

    public void BeginTargetingWithArtifactTap(ArtifactCard artifact, Player player, CardVisual visual)
    {
        targetingArtifact = artifact;
        targetingSorcery = null;
        targetingAura = null;
        targetingEquipment = null;
        targetingPlayer = player;
        targetingVisual = visual;
        isTargetingMode = true;

        artifact.isTapped = true; // show tapped state while selecting target
        FindCardVisual(artifact)?.UpdateVisual();

        Debug.Log("Targeting artifact, creature, or land to tap with artifact.");
    }

    public void BeginTargetingWithCreatureDestroyPower4OrGreater(CreatureCard creature, Player player, CardVisual visual)
    {
        targetingCreatureActivated = creature;
        targetingArtifact = null;
        targetingSorcery = null;
        targetingAura = null;
        targetingEquipment = null;
        targetingPlayer = player;
        targetingVisual = visual;
        isTargetingMode = true;

        creature.isTapped = true;
        FindCardVisual(creature)?.UpdateVisual();

        Debug.Log("Targeting a creature with power 4 or greater for creature ability.");
    }

    public void BeginTargetingWithCreatureDamageAnyTarget(CreatureCard creature, Player player, CardVisual visual)
    {
        targetingCreatureActivated = creature;
        targetingArtifact = null;
        targetingSorcery = null;
        targetingAura = null;
        targetingEquipment = null;
        targetingPlayer = player;
        targetingVisual = visual;
        isTargetingMode = true;

        creature.isTapped = true;
        FindCardVisual(creature)?.UpdateVisual();

        Debug.Log("Targeting any target for creature ability.");
    }

    public void BeginAuraTargetSelection(AuraCard aura, Player caster, CardVisual visual)
    {
        targetingAura = aura;
        targetingSorcery = null;
        targetingArtifact = null;
        targetingEquipment = null;
        targetingPlayer = caster;
        targetingVisual = visual;
        isTargetingMode = true;

        // Highlight the aura being cast so the player has feedback similar to
        // sorceries while choosing a target
        if (visual != null)
            visual.EnableTargetingHighlight(true);
    }

    public void BeginEquipmentTargetSelection(EquipmentCard equipment, Player player, CardVisual visual)
    {
        targetingEquipment = equipment;
        targetingArtifact = null;
        targetingSorcery = null;
        targetingAura = null;
        targetingPlayer = player;
        targetingVisual = visual;
        isTargetingMode = true;
    }

    public IEnumerator ResolveArtifactDamageAfterDelay(CardVisual targetVisual, Card targetCard)
        {
            yield return new WaitForSeconds(0.4f);

            if (targetCard is CreatureCard creature &&
                GetOwnerOfCard(creature)?.Battlefield.Contains(creature) == true &&
                targetingArtifact != null)
            {
                creature.TakeDamage(targetingArtifact.damageToCreature);
                Card asCard = targetingArtifact;
                if (asCard is CreatureCard srcCreature &&
                    srcCreature.keywordAbilities.Contains(KeywordAbility.Deathtouch) &&
                    targetingArtifact.damageToCreature > 0)
                {
                    creature.Kill();
                }
                Debug.Log($"{targetingArtifact.cardName} dealt {targetingArtifact.damageToCreature} damage to {creature.cardName}");
                targetingArtifact.isTapped = true;
                SendToGraveyard(targetingArtifact, targetingPlayer);
                CheckDeaths(humanPlayer);
                CheckDeaths(aiPlayer);
                UpdateUI();
            }
            else
            {
                Debug.Log("Invalid or missing target — damage not applied.");
                targetingArtifact.isTapped = false; // Optionally untap
            }

            targetingArtifact = null;
            targetingPlayer = null;
            targetingVisual = null;
            isTargetingMode = false;
        }

    public void BeginTargetSelectionForCreature(Card creature, Player owner, CardAbility ability)
        {
            targetingCreature = creature;
            targetingAbility = ability;
            isTargetingMode = true;

            bool foundValidTarget = false;

            foreach (var cv in activeCardVisuals)
            {
                if (cv == null || cv.linkedCard == null)
                    continue;

                Card target = cv.linkedCard;

                bool correctType =
                    (ability.requiredTargetType == SorceryCard.TargetType.Creature && target is CreatureCard) ||
                    (ability.requiredTargetType == SorceryCard.TargetType.Artifact && target is ArtifactCard) ||
                    (ability.requiredTargetType == SorceryCard.TargetType.Enchantment && target is EnchantmentCard) ||
                    (ability.requiredTargetType == SorceryCard.TargetType.Land && target is LandCard);

                bool isOnBattlefield = GetOwnerOfCard(target)?.Battlefield.Contains(target) == true;

                if (correctType && isOnBattlefield)
                {
                    foundValidTarget = true;
                    break;
                }
            }

            if (!foundValidTarget)
            {
                Debug.Log("No valid targets for creature ETB — skipping ability.");
                targetingCreature = null;
                targetingAbility = null;
                isTargetingMode = false;
            }
            else
            {
                Debug.Log("ETB ability requires target — enter targeting mode.");
            }
        }

        public bool HasValidTargetForAbility(CardAbility ability)
            {
                List<Card> battlefieldCards = new List<Card>();
                battlefieldCards.AddRange(humanPlayer.Battlefield);
                battlefieldCards.AddRange(aiPlayer.Battlefield);

                foreach (Card target in battlefieldCards)
                {
                    bool correctType =
                        (ability.requiredTargetType == SorceryCard.TargetType.Creature && target is CreatureCard) ||
                        (ability.requiredTargetType == SorceryCard.TargetType.Artifact && target is ArtifactCard) ||
                        (ability.requiredTargetType == SorceryCard.TargetType.Enchantment && target is EnchantmentCard) ||
                        (ability.requiredTargetType == SorceryCard.TargetType.Land && target is LandCard);

                    if (correctType)
                        return true;
                }

                return false;
            }

        public void BeginOptionalTargetSelectionAfterEntry(Card creature, Player owner, CardAbility ability)
            {
                targetingCreatureOptional = creature;
                optionalAbility = ability;
                optionalTargetPlayer = null;
                pendingStackEffects++;
                isTargetingMode = true;
                targetingVisual = FindCardVisual(creature); // Optional, for visual link

                Debug.Log($"Optional ETB targeting started for {creature.cardName}. Click a valid target if you want to use the ability.");
            }

        public void CancelOptionalTargeting()
            {
                if (targetingCreatureOptional != null)
                {
                    Debug.Log("Optional targeting cancelled.");
                    targetingCreatureOptional = null;
                    optionalAbility = null;
                    optionalTargetPlayer = null;
                    pendingStackEffects = Mathf.Max(0, pendingStackEffects - 1);
                    isTargetingMode = false;
                    targetingVisual = null;
                }
            }

        public void ResolveOptionalTargeting(Card target)
        {
            if (targetingCreatureOptional == null || optionalAbility == null)
                return;

            var ability = optionalAbility;
            var source = targetingCreatureOptional;
            Player owner = GetOwnerOfCard(source);

            targetingCreatureOptional = null;
            optionalAbility = null;
            isTargetingMode = false;
            targetingVisual = null;

            QueueTriggeredAbility(ability, owner, source, target);
            pendingStackEffects = Mathf.Max(0, pendingStackEffects - 1);
        }

        public void ResolveOptionalPlayerTargeting(Player target)
        {
            if (targetingCreatureOptional == null || optionalAbility == null)
                return;

            optionalTargetPlayer = target;
            var ability = optionalAbility;
            var source = targetingCreatureOptional;
            Player owner = GetOwnerOfCard(source);

            targetingCreatureOptional = null;
            optionalAbility = null;
            isTargetingMode = false;
            targetingVisual = null;

            QueueTriggeredAbility(ability, owner, source, null);
            pendingStackEffects = Mathf.Max(0, pendingStackEffects - 1);
        }

        public void QueueTriggeredAbility(CardAbility ability, Player owner, Card source, Card target = null, Card deadCreature = null)
        {
            triggerQueue.Enqueue(new TriggeredAbilityContext(ability, owner, source, target, deadCreature));
            pendingStackEffects++;
            if (!processingTriggerQueue)
                StartCoroutine(ProcessTriggerQueue());
        }

        private IEnumerator ProcessTriggerQueue()
        {
            processingTriggerQueue = true;
            while (triggerQueue.Count > 0)
            {
                var ctx = triggerQueue.Dequeue();
                yield return StartCoroutine(ResolveTriggeredAbilityOnStack(ctx.ability, ctx.owner, ctx.source, ctx.target, ctx.deadCreature));
            }
            processingTriggerQueue = false;
        }

        public void DeferLifeDeltaFade(bool defer)
        {
            lifeDeltaFadeDeferred = defer;
            if (defer)
            {
                if (playerDeltaRoutine != null)
                {
                    StopCoroutine(playerDeltaRoutine);
                    playerDeltaRoutine = null;
                }
                if (enemyDeltaRoutine != null)
                {
                    StopCoroutine(enemyDeltaRoutine);
                    enemyDeltaRoutine = null;
                }
            }
        }

        private void UpdateLifeDelta(GameObject target, int change)
            {
                bool isPlayer = target == playerLifeContainer;
                bool isEnemy = target == enemyLifeContainer;
                if (!isPlayer && !isEnemy)
                    return;

                TMP_Text txt = isPlayer ? playerLifeDeltaText : enemyLifeDeltaText;
                int total = isPlayer ? playerLifeDelta : enemyLifeDelta;
                total += change;

                if (txt == null)
                {
                    if (floatingDamagePrefab == null)
                    {
                        Debug.LogError("Missing floatingDamagePrefab!");
                        return;
                    }

                    GameObject obj = Instantiate(floatingDamagePrefab);
                    obj.transform.SetParent(GameObject.Find("Canvas").transform, false);

                    RectTransform canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
                    RectTransform targetRect = target.GetComponent<RectTransform>();
                    RectTransform rt = obj.GetComponent<RectTransform>();

                    Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, targetRect.position);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, Camera.main, out Vector2 localPoint);
                    rt.anchoredPosition = localPoint;

                    rt.localScale = Vector3.one;
                    rt.sizeDelta = new Vector2(100, 40);

                    txt = obj.GetComponent<TMP_Text>();

                    if (isPlayer)
                        playerLifeDeltaText = txt;
                    else
                        enemyLifeDeltaText = txt;
                }

                txt.fontSize = 48;
                txt.enableAutoSizing = false;
                txt.text = (total > 0 ? "+" : "") + total;
                txt.color = total > 0 ? Color.green : Color.red;

                if (isPlayer)
                {
                    playerLifeDelta = total;
                    if (!lifeDeltaFadeDeferred)
                    {
                        if (playerDeltaRoutine != null)
                            StopCoroutine(playerDeltaRoutine);
                        playerDeltaRoutine = StartCoroutine(DelayFinalize(target));
                    }
                }
                else
                {
                    enemyLifeDelta = total;
                    if (!lifeDeltaFadeDeferred)
                    {
                        if (enemyDeltaRoutine != null)
                            StopCoroutine(enemyDeltaRoutine);
                        enemyDeltaRoutine = StartCoroutine(DelayFinalize(target));
                    }
                }
            }

        public void ResetLifeDeltas()
            {
                if (playerLifeDeltaText != null)
                {
                    Destroy(playerLifeDeltaText.gameObject);
                    playerLifeDeltaText = null;
                }
                if (enemyLifeDeltaText != null)
                {
                    Destroy(enemyLifeDeltaText.gameObject);
                    enemyLifeDeltaText = null;
                }
                playerLifeDelta = 0;
                enemyLifeDelta = 0;
            }

        public void FinalizeLifeDeltas()
            {
                lifeDeltaFadeDeferred = false;
                if (playerLifeDeltaText != null)
                    StartCoroutine(FadeAndFloatText(playerLifeDeltaText.gameObject, true));
                if (enemyLifeDeltaText != null)
                    StartCoroutine(FadeAndFloatText(enemyLifeDeltaText.gameObject, false));

                playerLifeDeltaText = null;
                enemyLifeDeltaText = null;
                if (playerDeltaRoutine != null)
                {
                    StopCoroutine(playerDeltaRoutine);
                    playerDeltaRoutine = null;
                }
                if (enemyDeltaRoutine != null)
                {
                    StopCoroutine(enemyDeltaRoutine);
                    enemyDeltaRoutine = null;
                }
                playerLifeDelta = 0;
                enemyLifeDelta = 0;
            }

        private IEnumerator DelayFinalize(GameObject target)
            {
                yield return new WaitForSeconds(1.5f);
                if (target == playerLifeContainer && playerLifeDeltaText != null)
                {
                    StartCoroutine(FadeAndFloatText(playerLifeDeltaText.gameObject, true));
                    playerLifeDeltaText = null;
                    playerLifeDelta = 0;
                    playerDeltaRoutine = null;
                }
                else if (target == enemyLifeContainer && enemyLifeDeltaText != null)
                {
                    StartCoroutine(FadeAndFloatText(enemyLifeDeltaText.gameObject, false));
                    enemyLifeDeltaText = null;
                    enemyLifeDelta = 0;
                    enemyDeltaRoutine = null;
                }
            }
        
        public void ShowFloatingDamage(int amount, GameObject target)
            {
                if (target == playerLifeContainer || target == enemyLifeContainer)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.dealDamage);
                    UpdateLifeDelta(target, -amount);
                    return;
                }

                if (floatingDamagePrefab == null)
                {
                    Debug.LogError("Missing floatingDamagePrefab!");
                    return;
                }

                GameObject obj = Instantiate(floatingDamagePrefab);
                obj.transform.SetParent(GameObject.Find("Canvas").transform, false);

                RectTransform canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
                RectTransform targetRect = target.GetComponent<RectTransform>();
                RectTransform rt = obj.GetComponent<RectTransform>();

                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, targetRect.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, Camera.main, out Vector2 localPoint);
                rt.anchoredPosition = localPoint;

                rt.localScale = Vector3.one;
                rt.sizeDelta = new Vector2(100, 40);

                TMP_Text text = obj.GetComponent<TMP_Text>();
                text.fontSize = 48;
                text.enableAutoSizing = false;
                text.text = "-" + amount;
                text.color = Color.red;

                SoundManager.Instance.PlaySound(SoundManager.Instance.dealDamage);

                StartCoroutine(FadeAndFloatText(obj, target == playerLifeContainer));
            }
        
        public void ShowFloatingHeal(int amount, GameObject target)
        {
            Debug.Log($"ShowFloatingHeal called: amount={amount}, target={target.name}");

                if (target == playerLifeContainer || target == enemyLifeContainer)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.gain_life);
                    UpdateLifeDelta(target, amount);
                    return;
                }

                if (floatingDamagePrefab == null)
                {
                    Debug.LogError("Missing floatingDamagePrefab!");
                    return;
                }

                GameObject obj = Instantiate(floatingDamagePrefab);
                obj.transform.SetParent(GameObject.Find("Canvas").transform, false);

                RectTransform canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
                RectTransform targetRect = target.GetComponent<RectTransform>();
                RectTransform rt = obj.GetComponent<RectTransform>();

                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, targetRect.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, Camera.main, out Vector2 localPoint);
                rt.anchoredPosition = localPoint;

                rt.localScale = Vector3.one;
                rt.sizeDelta = new Vector2(100, 40);

                TMP_Text text = obj.GetComponent<TMP_Text>();
                text.fontSize = 48;
                text.enableAutoSizing = false;
                text.text = "+" + amount;
                text.color = Color.green;

                SoundManager.Instance.PlaySound(SoundManager.Instance.gain_life); // use appropriate sound

                StartCoroutine(FadeAndFloatText(obj, target == playerLifeContainer));
            }

        private void ShowFavouritePopup()
        {
            if (favouritePopupPrefab == null || playerLifeContainer == null)
                return;

            GameObject obj = Instantiate(favouritePopupPrefab);
            obj.transform.SetParent(GameObject.Find("Canvas").transform, false);

            RectTransform canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
            RectTransform targetRect = playerLifeContainer.GetComponent<RectTransform>();
            RectTransform rt = obj.GetComponent<RectTransform>();

            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, targetRect.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, Camera.main, out Vector2 localPoint);
            rt.anchoredPosition = localPoint;

            obj.AddComponent<FavouritePopupVFX>();
        }
        
        private IEnumerator FadeAndFloatText(GameObject obj, bool floatUp)
            {
                RectTransform rt = obj.GetComponent<RectTransform>();
                TMP_Text text = obj.GetComponent<TMP_Text>();
                Vector3 startPos = rt.localPosition;
                float t = 0f;
                float direction = floatUp ? 1f : -1f;

                Color baseColor = text.color;

                while (t < 1.25f)
                {
                    t += Time.deltaTime;
                    rt.localPosition = startPos + new Vector3(0, t * 20f * direction, 0);
                    text.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1 - t * 0.8f);
                    yield return null;
                }

                Destroy(obj);
                yield break;
            }

        private IEnumerator MoveCard(Transform tf, Vector3 start, Vector3 end, float duration)
            {
                Canvas cardCanvas = tf.GetComponentInChildren<Canvas>();
                int originalOrder = 0;
                bool originalOverride = false;
                if (cardCanvas != null)
                {
                    originalOrder = cardCanvas.sortingOrder;
                    originalOverride = cardCanvas.overrideSorting;
                    cardCanvas.overrideSorting = true;
                    cardCanvas.sortingOrder = 100;
                }

                float t = 0f;
                while (t < duration)
                {
                    if (tf == null) yield break;
                    t += Time.deltaTime;
                    tf.position = Vector3.Lerp(start, end, t / duration);
                    yield return null;
                }

                if (tf != null)
                    tf.position = end;

                if (cardCanvas != null)
                {
                    cardCanvas.sortingOrder = originalOrder;
                    cardCanvas.overrideSorting = originalOverride;
                }
            }

        public (int playerDamage, int aiDamage) ResolveCombatForAttacker(CreatureCard attacker)
            {
                int playerDamage = 0;
                int aiDamage = 0;

                // Clamp negative power to zero when dealing damage and respect damage prevention
                int attackerDamage = preventAllCombatDamageThisTurn || attacker.keywordAbilities.Contains(KeywordAbility.CantDealCombatDamage)
                    ? 0
                    : Mathf.Max(attacker.power, 0);

                var blockers = attacker.blockedByThisBlocker;

                if (blockers != null && blockers.Count > 0)
                {
                    int remainingDamage = attackerDamage;
                    int totalDamageFromBlockers = 0;

                    bool attackerHasTrample = attacker.keywordAbilities.Contains(KeywordAbility.Trample);
                    for (int i = 0; i < blockers.Count; i++)
                    {
                        var blocker = blockers[i];
                        bool attackerProtected = blocker.color.Any(c => attacker.keywordAbilities.Contains(ProtectionUtils.GetProtectionKeyword(c)));
                        bool blockerProtected = attacker.color.Any(c => blocker.keywordAbilities.Contains(ProtectionUtils.GetProtectionKeyword(c)));

                        int damageToBlocker = 0;
                        if (!blockerProtected)
                        {
                            bool isLastBlocker = i == blockers.Count - 1;
                            if (!attackerHasTrample && isLastBlocker)
                                damageToBlocker = remainingDamage;
                            else
                                damageToBlocker = Mathf.Min(remainingDamage, blocker.toughness);

                            blocker.TakeDamage(damageToBlocker);
                            if (attacker.keywordAbilities.Contains(KeywordAbility.Deathtouch) && damageToBlocker > 0)
                                blocker.Kill();
                            remainingDamage -= damageToBlocker;
                        }

                        int damageFromBlocker = (preventAllCombatDamageThisTurn || attackerProtected || blocker.keywordAbilities.Contains(KeywordAbility.CantDealCombatDamage))
                            ? 0
                            : Mathf.Max(blocker.power, 0);
                        if (!attackerProtected)
                        {
                            totalDamageFromBlockers += damageFromBlocker;
                            if (blocker.keywordAbilities.Contains(KeywordAbility.Deathtouch) && damageFromBlocker > 0)
                                attacker.Kill();
                        }

                        if (damageToBlocker > 0 || damageFromBlocker > 0)
                            SoundManager.Instance.PlaySound(SoundManager.Instance.impact);

                        if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink) && damageToBlocker > 0)
                        {
                            Player owner = GetOwnerOfCard(attacker);
                            TryGainLife(owner, damageToBlocker);
                        }

                        if (blocker.keywordAbilities.Contains(KeywordAbility.Lifelink) && damageFromBlocker > 0)
                        {
                            Player blockerOwner = GetOwnerOfCard(blocker);
                            TryGainLife(blockerOwner, damageFromBlocker);
                        }
                    }

                    attacker.TakeDamage(totalDamageFromBlockers);

                    if (attacker.keywordAbilities.Contains(KeywordAbility.Trample) && remainingDamage > 0)
                    {
                        if (humanPlayer.Battlefield.Contains(attacker))
                        {
                            aiPlayer.Life -= remainingDamage;
                            aiDamage += remainingDamage;
                            NotifyCombatDamageToPlayer(attacker, aiPlayer);
                        }
                        else
                        {
                            humanPlayer.Life -= remainingDamage;
                            playerDamage += remainingDamage;
                            NotifyCombatDamageToPlayer(attacker, humanPlayer);
                        }

                        if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink))
                        {
                            TryGainLife(GetOwnerOfCard(attacker), remainingDamage);
                        }
                    }
                }
                else
                {
                    if (humanPlayer.Battlefield.Contains(attacker))
                    {
                        aiPlayer.Life -= attackerDamage;
                        aiDamage += attackerDamage;
                        NotifyCombatDamageToPlayer(attacker, aiPlayer);

                        if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink))
                        {
                            Player owner = humanPlayer.Battlefield.Contains(attacker) ? humanPlayer : aiPlayer;
                            TryGainLife(owner, attackerDamage);
                        }
                    }
                    else
                    {
                        humanPlayer.Life -= attackerDamage;
                        playerDamage += attackerDamage;
                        NotifyCombatDamageToPlayer(attacker, humanPlayer);

                        if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink))
                        {
                            TryGainLife(aiPlayer, attackerDamage);
                        }
                    }
                }

                return (playerDamage, aiDamage);
            }

        public IEnumerator ResolveCombatWithAnimations()
            {
                ResetLifeDeltas();
                foreach (var attacker in new List<CreatureCard>(currentAttackers))
                {
                    CardVisual attackerVisual = FindCardVisual(attacker);
                    if (attackerVisual == null)
                        continue;

                    Vector3 startPos = attackerVisual.transform.position;
                    Vector3 targetPos = startPos;

                    if (attacker.blockedByThisBlocker != null && attacker.blockedByThisBlocker.Count > 0)
                    {
                        var blockerVisual = FindCardVisual(attacker.blockedByThisBlocker[0]);
                        if (blockerVisual != null)
                            targetPos = blockerVisual.transform.position;
                    }
                    else
                    {
                        Transform targetLife = humanPlayer.Battlefield.Contains(attacker) ? enemyLifeContainer.transform : playerLifeContainer.transform;
                        targetPos = targetLife.position;
                    }

                    yield return StartCoroutine(MoveCard(attackerVisual.transform, startPos, targetPos, 0.15f));
                    yield return new WaitForSeconds(0.05f);

                    var (pd, ad) = ResolveCombatForAttacker(attacker);

                    if (pd > 0)
                        ShowFloatingDamage(pd, playerLifeContainer);
                    if (ad > 0)
                        ShowFloatingDamage(ad, enemyLifeContainer);

                    if (activeCardVisuals.Contains(attackerVisual))
                    {
                        yield return StartCoroutine(MoveCard(attackerVisual.transform, attackerVisual.transform.position, startPos, 0.15f));
                    }

                    yield return new WaitForSeconds(0.05f);
                }

                foreach (var creature in humanPlayer.Battlefield.Concat(aiPlayer.Battlefield).OfType<CreatureCard>())
                {
                    if (creature.cardName == "Undead Army" &&
                        (currentAttackers.Contains(creature) || creature.blockingThisAttacker != null))
                    {
                        creature.AddMinusOneCounter();
                        var vis = FindCardVisual(creature);
                        if (vis != null) vis.UpdateVisual();
                    }
                }

                CheckDeaths(humanPlayer);
                CheckDeaths(aiPlayer);

                foreach (var card in humanPlayer.Battlefield)
                {
                    if (card is CreatureCard c)
                    {
                        c.blockingThisAttacker = null;
                        c.blockedByThisBlocker.Clear();
                    }
                }
                foreach (var card in aiPlayer.Battlefield)
                {
                    if (card is CreatureCard c)
                    {
                        c.blockingThisAttacker = null;
                        c.blockedByThisBlocker.Clear();
                    }
                }

                currentAttackers.Clear();
                selectedBlockerForBlocking = null;
                UpdateUI();
                FinalizeLifeDeltas();
            }

        private IEnumerator ShowDeathVFXAndDelayLayout(Card card, Player owner, CardVisual visual, GameObject overridePrefab = null, bool removeFromGame = false)
        {
            if (visual != null)
                visual.EnableTargetingHighlight(false); // ensure highlight removed

                pendingGraveyardAnimations++;

                // 1. Create a placeholder object in the same layout slot
            GameObject placeholderPrefab = overridePrefab != null ? overridePrefab : deathPlaceholderPrefab;
            GameObject placeholder = Instantiate(placeholderPrefab, visual.transform.parent);
                placeholder.transform.SetSiblingIndex(visual.transform.GetSiblingIndex());
                placeholder.transform.localScale = visual.transform.localScale;
                placeholder.transform.localPosition = visual.transform.localPosition + placeholder.transform.localPosition;

                // 2. Remove the visual instantly, preserving layout slot
                activeCardVisuals.Remove(visual);
                Destroy(visual.gameObject);

                // 3. Wait for VFX duration
                yield return new WaitForSeconds(1.5f); // Match blood splat prefab's lifespan

                // 4. Now destroy the placeholder — this causes the layout to update
                Destroy(placeholder);

                // 5. Create graveyard visual (if not a token and not removed from game)
                if (!card.isToken && !removeFromGame)
                {
                    GameObject visualGO = Instantiate(cardPrefab,
                        owner == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
                    CardVisual graveyardVisual = visualGO.GetComponent<CardVisual>();
                    graveyardVisual.Setup(card, this);
                    graveyardVisual.transform.SetParent(owner == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
                    graveyardVisual.transform.localPosition = Vector3.zero;
                    graveyardVisual.UpdateGraveyardVisual();
                    // Ensure overlay elements appear above
                    // Place newest card on top of pile
                    graveyardVisual.transform.SetAsLastSibling();
                    EnsureGraveyardCounterOnTop(owner);

                    activeCardVisuals.Add(graveyardVisual);
                }

                // 6. Move to graveyard data list (skip for tokens and cards removed from game)
                if (!card.isToken && !removeFromGame)
                    owner.Graveyard.Add(card);
                else if (removeFromGame)
                    Debug.Log($"{card.cardName} is removed from the game after dying.");
                UpdateUI();
                pendingGraveyardAnimations--;
            }

        private IEnumerator ShowHandDiscardVFX(Card card, Player owner, CardVisual visual)
            {
                if (visual != null)
                    visual.EnableTargetingHighlight(false); // ensure highlight removed

                // 1. Create placeholder
                Transform parent = visual.transform.parent;
                GameObject placeholder = Instantiate(deathPlaceholderPrefab, parent);
                placeholder.transform.SetSiblingIndex(visual.transform.GetSiblingIndex());
                placeholder.transform.localScale = visual.transform.localScale;
                placeholder.transform.localPosition = visual.transform.localPosition;

                // 2. Remove the visual (like battlefield death)
                GameManager.Instance.activeCardVisuals.Remove(visual);
                Destroy(visual.gameObject);

                // 3. Wait for VFX duration (even shorter than battlefield)
                yield return new WaitForSeconds(0.5f);

                // 4. Destroy placeholder to allow layout rebuild
                Destroy(placeholder);

                // 5. Create graveyard visual
                if (!card.isToken)
                {
                    GameObject visualGO = Instantiate(GameManager.Instance.cardPrefab,
                        owner == GameManager.Instance.humanPlayer
                            ? GameManager.Instance.playerGraveyardArea
                            : GameManager.Instance.aiGraveyardArea);
                    CardVisual graveyardVisual = visualGO.GetComponent<CardVisual>();
                    graveyardVisual.Setup(card, GameManager.Instance);
                    graveyardVisual.transform.localPosition = Vector3.zero;
                    graveyardVisual.UpdateGraveyardVisual();
                    // Keep UI overlay elements on top
                    // Maintain newest card on top of pile
                    graveyardVisual.transform.SetAsLastSibling();
                    EnsureGraveyardCounterOnTop(owner);

                    GameManager.Instance.activeCardVisuals.Add(graveyardVisual);
                }

                // 6. Move to graveyard data list (skip for tokens)
                if (!card.isToken)
                    owner.Graveyard.Add(card);
                UpdateUI();
            }


        private void ForEachTriggeredAbilityOnBattlefield(TriggerTiming timing, System.Action<Player, Card, CardAbility> action)
        {
            foreach (var player in new[] { humanPlayer, aiPlayer })
            {
                foreach (var card in player.Battlefield.ToList())
                {
                    foreach (var ability in card.abilities)
                    {
                        if (ability.timing == timing && ability.effect != null)
                            action(player, card, ability);
                    }
                }
            }
        }

        private void ExecuteAbilityWithHealFeedback(Player player, Card source, CardAbility ability)
        {
            int oldLife = player.Life;
            ability.effect.Invoke(player, source);
            int gained = player.Life - oldLife;
            if (gained > 0)
            {
                ShowFloatingHeal(gained,
                    player == humanPlayer ? playerLifeContainer : enemyLifeContainer);
            }
        }

        public void NotifyArtifactEntered(Card artifact, Player controller)
        {
            lastEnteredArtifact = artifact;
            ForEachTriggeredAbilityOnBattlefield(TriggerTiming.OnArtifactEnter, (player, card, ability) =>
            {
                if (card == artifact)
                    return;

                ExecuteAbilityWithHealFeedback(player, card, ability);
            });
            lastEnteredArtifact = null;
        }

        public void NotifyEnchantmentEntered(Card enchantment, Player controller)
        {
            ForEachTriggeredAbilityOnBattlefield(TriggerTiming.OnEnchantmentEnter, (player, card, ability) =>
            {
                if (card == enchantment)
                    return;

                ExecuteAbilityWithHealFeedback(player, card, ability);
            });
        }

        public void NotifyLandEntered(Card land, Player controller)
        {
            ForEachTriggeredAbilityOnBattlefield(TriggerTiming.OnLandEnter, (player, card, ability) =>
            {
                ExecuteAbilityWithHealFeedback(player, card, ability);
            });
        }

        public void NotifyCreatureEntered(Card creature, Player controller)
        {
            if (!(creature is CreatureCard))
                return;

            lastEnteredCreature = creature;
            ForEachTriggeredAbilityOnBattlefield(TriggerTiming.OnCreatureEnter, (player, card, ability) =>
            {
                ability.effect.Invoke(player, card);
            });
            lastEnteredCreature = null;
        }

        public void NotifyLandLeft(Card land, Player controller)
        {
            ForEachTriggeredAbilityOnBattlefield(TriggerTiming.OnLandLeave, (player, card, ability) =>
            {
                ExecuteAbilityWithHealFeedback(player, card, ability);
            });
        }

        public int lastLifeGainedAmount = 0;

        public void NotifyLifeGain(Player player, int amount)
        {
            lastLifeGainedAmount = amount;
            foreach (var card in player.Battlefield.ToList())
            {
                foreach (var ability in card.abilities)
                {
                    if (ability.timing == TriggerTiming.OnLifeGain && ability.effect != null)
                        ExecuteAbilityWithHealFeedback(player, card, ability);
                }
            }
            lastLifeGainedAmount = 0;
        }

        public int lastCardsDrawnAmount = 0;

        public Player lastDiscardingPlayer = null;

        public Card lastDeadCreature = null;

        public Card lastEnteredCreature = null;

        public Card lastEnteredArtifact = null;

        // Tracks the source card of the ability currently being resolved.
        public Card lastAbilitySource = null;

        public void NotifyCardDrawn(Player player, int amount)
        {
            lastCardsDrawnAmount = amount;
            foreach (var card in player.Battlefield.ToList())
            {
                foreach (var ability in card.abilities)
                {
                    if (ability.timing == TriggerTiming.OnCardDraw && ability.effect != null)
                    {
                        ability.effect.Invoke(player, card);
                    }
                }
            }
            lastCardsDrawnAmount = 0;
        }

        public void NotifyOpponentDraw(Player drawingPlayer)
        {
            foreach (var player in new[] { humanPlayer, aiPlayer })
            {
                if (player == drawingPlayer)
                    continue;

                foreach (var card in player.Battlefield.ToList())
                {
                    foreach (var ability in card.abilities)
                    {
                        if (ability.timing == TriggerTiming.OnOpponentDraw && ability.effect != null)
                        {
                            ability.effect.Invoke(player, card);
                        }
                    }
                }
            }
        }

        public void NotifyOpponentDiscard(Player discardingPlayer)
        {
            foreach (var player in new[] { humanPlayer, aiPlayer })
            {
                if (player == discardingPlayer)
                    continue;

                foreach (var card in player.Battlefield.ToList())
                {
                    foreach (var ability in card.abilities)
                    {
                        if (ability.timing == TriggerTiming.OnOpponentDiscard && ability.effect != null)
                        {
                            ability.effect.Invoke(player, card);
                        }
                    }
                }
            }
        }

        public void NotifyPlayerDiscard(Player discardingPlayer)
        {
            lastDiscardingPlayer = discardingPlayer;
            foreach (var player in new[] { humanPlayer, aiPlayer })
            {
                foreach (var card in player.Battlefield.ToList())
                {
                    foreach (var ability in card.abilities)
                    {
                        if (ability.timing == TriggerTiming.OnPlayerDiscard && ability.effect != null)
                        {
                            ability.effect.Invoke(player, card);
                        }
                    }
                }
            }
            lastDiscardingPlayer = null;
        }

        public void NotifyCreatureDiesOrDiscarded(Card creature, Player owner)
        {
            if (!(creature is CreatureCard))
                return;

            foreach (var player in new[] { humanPlayer, aiPlayer })
            {
                foreach (var card in player.Battlefield.ToList())
                {
                    foreach (var ability in card.abilities)
                    {
                        if (ability.timing == TriggerTiming.OnCreatureDiesOrDiscarded && ability.effect != null)
                        {
                            QueueTriggeredAbility(ability, player, card, card, creature);
                        }
                    }
                }
            }
        }

        public void NotifyCreatureDies(Card creature, Player owner)
        {
            if (!(creature is CreatureCard))
                return;

            foreach (var player in new[] { humanPlayer, aiPlayer })
            {
                foreach (var card in player.Battlefield.ToList())
                {
                    foreach (var ability in card.abilities)
                    {
                        if (ability.timing == TriggerTiming.OnCreatureDies && ability.effect != null)
                        {
                            if (ability.triggerOnlyOnAttachedCreatureDeath)
                            {
                                if (!(card is AuraCard aura) || aura.attachedTo != creature)
                                    continue;
                            }

                            QueueTriggeredAbility(ability, player, card, card, creature);
                        }
                    }
                }
            }
        }

        public void NotifyCombatDamageToPlayer(CreatureCard attacker, Player target)
        {
            foreach (var player in new[] { humanPlayer, aiPlayer })
            {
                foreach (var card in player.Battlefield.ToList())
                {
                    foreach (var ability in card.abilities)
                    {
                        if (ability.timing == TriggerTiming.OnCombatDamageToPlayer && ability.effect != null)
                        {
                            ability.effect.Invoke(player, card);
                        }
                    }
                }
            }
        }

        public void NotifyCreatureBlocks(CreatureCard blocker, CreatureCard attacker)
        {
            if (blocker == null || attacker == null)
                return;

            Player owner = GetOwnerOfCard(blocker);
            if (owner == null)
                return;

            foreach (var ability in blocker.abilities)
            {
                if (ability.timing == TriggerTiming.OnBlock && ability.effect != null)
                {
                    QueueTriggeredAbility(ability, owner, blocker, attacker);
                }
            }
        }

        public void GainLife(Player player, int amount)
        {
            TryGainLife(player, amount);
        }

        private void AwardFavouriteCardCoins(Card card, Player caster)
        {
            if (caster == humanPlayer && !string.IsNullOrEmpty(favouriteCardName) && card.cardName == favouriteCardName)
            {
                CoinsManager.AddCoins(5);
                ShowFavouritePopup();
            }
        }

        public void CheckForGameEnd()
        {
            if (gameOver)
                return;

            if (aiPlayer.Life <= 0 && humanPlayer.Life <= 0)
            {
                Debug.Log("Both players died — draw counts as a loss for the human player.");
                gameOver = true;
                if (TurnSystem.Instance != null)
                    TurnSystem.Instance.StopAllCoroutines();
                UnityEngine.Object.FindFirstObjectByType<WinScreenUI>().ShowLoseScreen();
            }
            else if (aiPlayer.Life <= 0)
            {
                Debug.Log("AI defeated — player wins!");
                // Card reward logic temporarily disabled.
                // CardData reward = PlayerCollection.AddRandomCard();
                gameOver = true;
                if (TurnSystem.Instance != null)
                    TurnSystem.Instance.StopAllCoroutines();
                UnityEngine.Object.FindFirstObjectByType<WinScreenUI>().ShowWinScreen(null);
            }
            else if (humanPlayer.Life <= 0)
            {
                Debug.Log("Human player defeated — game lost.");
                gameOver = true;
                if (TurnSystem.Instance != null)
                    TurnSystem.Instance.StopAllCoroutines();
                UnityEngine.Object.FindFirstObjectByType<WinScreenUI>().ShowLoseScreen();
            }
        }
}
