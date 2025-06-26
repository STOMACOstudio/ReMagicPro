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
    public CreatureCard selectedAttackerForBlocking = null;

    public Player humanPlayer;
    public Player aiPlayer;
    public TMP_Text manaPoolText;
    public TMP_Text playerLifeText;
    public TMP_Text enemyLifeText;
    public TMP_Text enemyHandText;

    public Transform playerHandArea;
    public Transform playerBattlefieldArea;
    public Transform playerGraveyardArea;
    public Transform playerLandArea;
    public Transform playerArtifactArea;

    public Transform stackZone; //shared zone

    public Transform aiBattlefieldArea;
    public Transform aiGraveyardArea;
    public Transform aiLandArea;
    public Transform aiArtifactArea;

    public GameObject cardPrefab;
    public GameObject manaVFXPrefab;
    public GameObject bloodSplatPrefab;
    public GameObject deathPlaceholderPrefab;
    public GameObject playerLifeContainer;
    public GameObject enemyLifeContainer;
    public GameObject floatingDamagePrefab;

    public ArtifactCard targetingArtifact;

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

    public List<CardVisual> activeCardVisuals = new List<CardVisual>();
    public List<CreatureCard> selectedAttackers = new List<CreatureCard>();
    public List<CreatureCard> currentAttackers = new List<CreatureCard>();

    public Dictionary<CreatureCard, List<CreatureCard>> blockingAssignments = new Dictionary<CreatureCard, List<CreatureCard>>();

    public bool isStackBusy = false;

    public SorceryCard targetingSorcery;
    public Player targetingPlayer;
    public CardVisual targetingVisual;
    public bool isTargetingMode = false;

    public Card targetingCreature;
    public CardAbility targetingAbility;
    public Card targetingCreatureOptional;
    public CardAbility optionalAbility;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        humanPlayer = new Player();
        aiPlayer = new Player();

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

        ShuffleDeck(humanPlayer);
        ShuffleDeck(aiPlayer);

        for (int i = 0; i < 7; i++)
        {
            DrawCard(humanPlayer);
            DrawCard(aiPlayer);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!string.IsNullOrEmpty(BattleData.CurrentZoneId))
            {
                Debug.Log("[DEV] Instant win triggered for zone ID: " + BattleData.CurrentZoneId);
                FindObjectOfType<WinScreenUI>().ShowWinScreen();
            }
            else
            {
                Debug.LogWarning("[DEV] No zone ID found — can't win");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            GameManager.Instance.DrawCard(GameManager.Instance.humanPlayer);
            Debug.Log("D key pressed — drew a card.");
        }
    }

    void ShuffleDeck(Player player)
    {
        for (int i = 0; i < player.Deck.Count; i++)
        {
            Card temp = player.Deck[i];
            int randomIndex = Random.Range(i, player.Deck.Count);
            player.Deck[i] = player.Deck[randomIndex];
            player.Deck[randomIndex] = temp;
        }
    }
    public void DrawCard(Player player)
    {
        if (player.Deck.Count == 0) return;
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
    }

    public void PlayCard(Player player, CardVisual visual)
        {
            if (isStackBusy)
            {
                Debug.Log("A spell is already on the stack. Please wait.");
                return;
            }

            Card card = visual.linkedCard;

            if (card is LandCard)
            {
                if (player.hasPlayedLandThisTurn)
                {
                    Debug.Log("You already played a land this turn!");
                    return;
                }

                player.Battlefield.Add(card);
                player.Hand.Remove(card);
                player.hasPlayedLandThisTurn = true;

                if (card.entersTapped || GameManager.Instance.IsAllPermanentsEnterTappedActive())
                {
                    card.isTapped = true;
                    Debug.Log($"{card.cardName} enters tapped (static effect or base).");
                }

                card.OnEnterPlay(player);
                NotifyLandEntered(card, player);

                visual.transform.SetParent(player == humanPlayer ? playerLandArea : aiLandArea, false);
                visual.isInBattlefield = true;
                visual.UpdateVisual();
                SoundManager.Instance.PlaySound(SoundManager.Instance.cardPlay);

            }

            else if (card is CreatureCard creature)
            {
                var cost = GetManaCostBreakdown(card.manaCost, card.color);
                if (player.ColoredMana.CanPay(cost))
                {
                    player.ColoredMana.Pay(cost);
                    card.owner = player;
                    if (player == humanPlayer) UpdateUI();
                    player.Hand.Remove(card);
                    player.Battlefield.Add(card);

                    card.OnEnterPlay(player);
                    if (card.color.Contains("Artifact"))
                        NotifyArtifactEntered(card, player);

                    if (creature.keywordAbilities.Contains(KeywordAbility.Haste))
                        creature.hasSummoningSickness = false;
                    else
                        creature.hasSummoningSickness = true;

                    if (card.entersTapped || IsAllPermanentsEnterTappedActive())
                    {
                        card.isTapped = true;
                        Debug.Log($"{card.cardName} enters tapped (due to static effect).");
                    }

                    visual.transform.SetParent(player == humanPlayer ? playerBattlefieldArea : aiBattlefieldArea, false);
                    visual.isInBattlefield = true;
                    visual.UpdateVisual();
                    SoundManager.Instance.PlaySound(SoundManager.Instance.playCreature);
                }
                else
                {
                    Debug.Log("Not enough colored mana to cast this creature.");
                }
            }

            else if (card is SorceryCard sorcery)
            {
                if (sorcery.requiresTarget)
                {
                    Debug.Log("This sorcery requires a target — entering targeting mode.");
                    BeginTargetSelection(sorcery, player, visual);
                    return;
                }

                var cost = GetManaCostBreakdown(sorcery.manaCost, sorcery.color);
                if (player.ColoredMana.CanPay(cost))
                {
                    isStackBusy = true; // BLOCK OTHER ACTIONS WHILE SORCERY IS ON STACK
                    player.ColoredMana.Pay(cost);
                    card.owner = player;
                    player.Hand.Remove(card);
                    UpdateUI();

                    // Move visual to the stack zone
                    visual.transform.SetParent(stackZone, false);
                    visual.transform.localPosition = Vector3.zero;
                    SoundManager.Instance.PlaySound(SoundManager.Instance.cardPlay);
                    StartCoroutine(ResolveSorceryAfterDelay(sorcery, visual, player));
                }
                else
                {
                    Debug.Log("Not enough colored mana to cast this sorcery.");
                }
            }
            else if (card is ArtifactCard artifact)
            {
                var cost = GetManaCostBreakdown(artifact.manaCost, artifact.color);
                if (player.ColoredMana.CanPay(cost))
                {
                    player.ColoredMana.Pay(cost);
                    card.owner = player;
                    if (player == humanPlayer) UpdateUI();

                    player.Hand.Remove(card);
                    player.Battlefield.Add(card);
                    card.OnEnterPlay(player);
                    NotifyArtifactEntered(card, player);

                    if (artifact.entersTapped || GameManager.Instance.IsAllPermanentsEnterTappedActive())
                    {
                        artifact.isTapped = true;
                        Debug.Log($"{artifact.cardName} enters tapped (due to static effect).");
                    }

                    // Move to battlefield area visually
                    Transform visualParent = player == humanPlayer
                        ? (card is ArtifactCard ? playerArtifactArea : playerBattlefieldArea)
                        : (card is ArtifactCard ? aiArtifactArea : aiBattlefieldArea);

                    visual.transform.SetParent(visualParent, false);

                    visual.isInBattlefield = true;
                    visual.UpdateVisual();
                    SoundManager.Instance.PlaySound(SoundManager.Instance.playArtifact);
                }
                else
                {
                    Debug.Log("Not enough colored mana to play this artifact.");
                }
            }
            else
            {
                Debug.LogWarning("Unhandled card type played: " + card.cardName);
            }
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
            bool diedFromBattlefield = owner.Battlefield.Contains(card);
            bool discardedFromHand = owner.Hand.Contains(card);

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
                        owner == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
                    CardVisual stackGraveyardVisual = visualGO.GetComponent<CardVisual>();
                    stackGraveyardVisual.Setup(card, this);
                    stackGraveyardVisual.transform.localPosition = Vector3.zero;
                    stackGraveyardVisual.UpdateGraveyardVisual();

                    activeCardVisuals.Add(stackGraveyardVisual);
                }

                owner.Graveyard.Add(card);
                return;
            }


            owner.Battlefield.Remove(card);
            owner.Hand.Remove(card);
            Debug.Log($"{card.cardName} is being sent to the graveyard.");

            if (diedFromBattlefield)
            {
                card.OnLeavePlay(owner);
                if (card is LandCard)
                    NotifyLandLeft(card, owner);
            }

            card.isTapped = false;

            CardVisual visual = FindCardVisual(card); // <-- Moved up
            if (visual != null && visual.tapIcon != null)
                visual.tapIcon.SetActive(false);

            if (discardedFromHand && visual != null)
            {
                StartCoroutine(ShowHandDiscardVFX(card, owner, visual));
                return;
            }

            if (card is CreatureCard deadCreature)
            {
                deadCreature.hasSummoningSickness = false;
                deadCreature.toughness = deadCreature.baseToughness;

                if (visual != null)
                {
                    visual.sicknessText.text = "";
                }

                if (card.isToken && diedFromBattlefield)
                {
                    if (visual != null)
                    {
                        StartCoroutine(ShowDeathVFXAndDelayLayout(card, owner, visual));
                    }
                    return;
                }

                if (diedFromBattlefield && visual != null)
                {
                    StartCoroutine(ShowDeathVFXAndDelayLayout(card, owner, visual));
                    return;
                }
            }

            // Fallback: create graveyard visual normally
            CardVisual graveyardVisual = FindCardVisual(card);
            if (graveyardVisual == null)
            {
                GameObject visualGO = Instantiate(cardPrefab,
                    owner == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
                graveyardVisual = visualGO.GetComponent<CardVisual>();
                graveyardVisual.Setup(card, this);
                activeCardVisuals.Add(graveyardVisual);
            }

            graveyardVisual.transform.SetParent(owner == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
            graveyardVisual.transform.localPosition = Vector3.zero;
            graveyardVisual.UpdateGraveyardVisual();

            owner.Graveyard.Add(card);
        }

    public (int playerDamage, int aiDamage) ResolveCombat()
    {
        int playerDamage = 0;
        int aiDamage = 0;

        foreach (var attacker in currentAttackers)
        {
            var blockers = attacker.blockedByThisBlocker;

            if (blockers != null && blockers.Count > 0)
            {
                int remainingDamage = attacker.power;
                int totalDamageFromBlockers = 0;

                foreach (var blocker in blockers)
                {
                    bool attackerProtected = attacker.color.Any(c => blocker.keywordAbilities.Contains(GetProtectionKeyword(c)));
                    bool blockerProtected = blocker.color.Any(c => attacker.keywordAbilities.Contains(GetProtectionKeyword(c)));

                    int damageToBlocker = 0;
                    if (!blockerProtected)
                    {
                        damageToBlocker = Mathf.Min(remainingDamage, blocker.toughness);
                        blocker.toughness -= damageToBlocker;
                        remainingDamage -= damageToBlocker;
                    }

                    int damageFromBlocker = attackerProtected ? 0 : blocker.power;
                    if (!attackerProtected)
                        totalDamageFromBlockers += damageFromBlocker;

                    Debug.Log($"{attacker.cardName} is blocked by {blocker.cardName}.");

                    if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink) && damageToBlocker > 0)
                    {
                        Player owner = GetOwnerOfCard(attacker);
                        owner.Life += damageToBlocker;
                        Debug.Log($"{attacker.cardName} lifelinks {damageToBlocker} life to {owner}.");

                        if (owner == humanPlayer)
                            ShowFloatingHeal(damageToBlocker, playerLifeContainer);
                        else
                            ShowFloatingHeal(damageToBlocker, enemyLifeContainer);
                    }

                    if (blocker.keywordAbilities.Contains(KeywordAbility.Lifelink) && damageFromBlocker > 0)
                    {
                        Player blockerOwner = GetOwnerOfCard(blocker);
                        blockerOwner.Life += damageFromBlocker;
                        Debug.Log($"{blocker.cardName} lifelinks {damageFromBlocker} life to {blockerOwner}.");

                        if (blockerOwner == humanPlayer)
                            ShowFloatingHeal(damageFromBlocker, playerLifeContainer);
                        else
                            ShowFloatingHeal(damageFromBlocker, enemyLifeContainer);
                    }
                }

                attacker.toughness -= totalDamageFromBlockers;

                if (attacker.keywordAbilities.Contains(KeywordAbility.Trample) && remainingDamage > 0)
                {
                    if (humanPlayer.Battlefield.Contains(attacker))
                    {
                        aiPlayer.Life -= remainingDamage;
                        aiDamage += remainingDamage;
                        Debug.Log($"{attacker.cardName} tramples over for {remainingDamage} damage!");
                    }
                    else
                    {
                        humanPlayer.Life -= remainingDamage;
                        playerDamage += remainingDamage;
                        Debug.Log($"{attacker.cardName} tramples YOU for {remainingDamage} damage!");
                    }

                    if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink))
                    {
                        GetOwnerOfCard(attacker).Life += remainingDamage;
                        Debug.Log($"{attacker.cardName} lifelinks {remainingDamage} trample damage.");

                        if (attacker.owner == humanPlayer)
                            ShowFloatingHeal(remainingDamage, playerLifeContainer);
                        else
                            ShowFloatingHeal(remainingDamage, enemyLifeContainer);
                    }
                }
            }
            else
            {
                // Attacker goes unblocked
                if (humanPlayer.Battlefield.Contains(attacker))
                {
                    aiPlayer.Life -= attacker.power;
                    aiDamage += attacker.power;

                    // Lifelink: gain life equal to damage dealt to AI
                    if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink))
                    {
                        Player owner = humanPlayer.Battlefield.Contains(attacker) ? humanPlayer : aiPlayer;
                        owner.Life += attacker.power;
                        Debug.Log($"{attacker.cardName} lifelinks {attacker.power} life to {(owner == humanPlayer ? "Human" : "AI")}.");

                        if (owner == humanPlayer)
                            ShowFloatingHeal(attacker.power, playerLifeContainer);
                        else
                            ShowFloatingHeal(attacker.power, enemyLifeContainer);
                    }
                }
                else
                {
                    humanPlayer.Life -= attacker.power;
                    playerDamage += attacker.power;

                    // Lifelink: gain life equal to damage dealt to Human
                    if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink))
                    {
                        aiPlayer.Life += attacker.power;
                        Debug.Log($"{attacker.cardName} lifelinks {attacker.power} life to AI.");
                        ShowFloatingHeal(attacker.power, enemyLifeContainer); // ← ADD THIS
                    }
                }
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
        selectedAttackerForBlocking = null;
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
                toGrave.Add(c);
            }
        }

        foreach (var card in toGrave)
        {
            SendToGraveyard(card, player);
        }
    }

    public void ResetPermanents(Player player)
    {
        foreach (var card in player.Battlefield)
        {
            card.isTapped = false;

            if (card is CreatureCard creature)
            {
                creature.hasSummoningSickness = false;
                creature.toughness = creature.baseToughness;
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
                creature.toughness = creature.baseToughness;
            }
        }
    }

    public CardVisual FindCardVisual(Card card)
        {
            return activeCardVisuals.Find(cv => cv.linkedCard == card);
        }
    
    public IEnumerator ResolveSorceryAfterDelay(SorceryCard sorcery, CardVisual visual, Player caster)
        {
            yield return new WaitForSeconds(2f);

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

            if (caster == aiPlayer && visual != null)
            {
                activeCardVisuals.Remove(visual);
                Destroy(visual.gameObject);
            }

            UpdateUI();
            isStackBusy = false;
            CheckForGameEnd();

            if (caster == aiPlayer && TurnSystem.Instance.waitingToResumeAI)
            {
                Debug.Log("Resuming AI phase after stack.");
                TurnSystem.Instance.waitingToResumeAI = false;
                TurnSystem.Instance.RunSpecificPhase(TurnSystem.Instance.lastPhaseBeforeStack);
            }
        }

    public void SummonToken(Card tokenCard, Player owner)
    {
        if (tokenCard == null)
        {
            Debug.LogError("Tried to summon a null token.");
            return;
        }

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

        // Create visual and link it
        GameObject visualGO = Instantiate(cardPrefab, owner == humanPlayer ? playerBattlefieldArea : aiBattlefieldArea);
        CardVisual visual = visualGO.GetComponent<CardVisual>();

        visual.Setup(tokenCard, this);
        visual.isInBattlefield = true;
        activeCardVisuals.Add(visual);
        visual.UpdateVisual();

        tokenCard.OnEnterPlay(owner);  // Run ETB triggers (last)
        if (tokenCard is LandCard)
            NotifyLandEntered(tokenCard, owner);
        if ((tokenCard is ArtifactCard) ||
            (tokenCard is CreatureCard cc && cc.color.Contains("Artifact")))
        {
            NotifyArtifactEntered(tokenCard, owner);
        }
    }
    public Player GetOpponentOf(Player player)
    {
        return player == humanPlayer ? aiPlayer : humanPlayer;
    }

    public void TapCardForMana(CreatureCard creature)
    {
        if (!creature.isTapped)
        {
            creature.isTapped = true;
            humanPlayer.ColoredMana.Colorless++;
            UpdateUI();
        }
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

        Player owner = GetOwnerOfCard(creature);
        int remaining = creature.manaToPayToActivate;

        if (owner.ColoredMana.Total() >= remaining)
        {
            // Spend colorless first
            int useColorless = Mathf.Min(owner.ColoredMana.Colorless, remaining);
            owner.ColoredMana.Colorless -= useColorless;
            remaining -= useColorless;

            // Spend from WUBRG
            remaining -= SpendFromPool(ref owner.ColoredMana.White, remaining);
            remaining -= SpendFromPool(ref owner.ColoredMana.Blue, remaining);
            remaining -= SpendFromPool(ref owner.ColoredMana.Black, remaining);
            remaining -= SpendFromPool(ref owner.ColoredMana.Red, remaining);
            remaining -= SpendFromPool(ref owner.ColoredMana.Green, remaining);

            if (remaining > 0)
            {
                Debug.LogWarning("PayToGainAbility: Not enough mana despite Total() check.");
                return;
            }

            creature.keywordAbilities.Add(creature.abilityToGain);
            Debug.Log($"{creature.cardName} gains {creature.abilityToGain} until end of turn.");
            UpdateUI();
        }
        else
        {
            Debug.Log($"Not enough mana to activate {creature.cardName}'s ability.");
            return;
        }
    }

    public Player GetOwnerOfCard(Card card)
    {
        if (humanPlayer.Battlefield.Contains(card)) return humanPlayer;
        if (aiPlayer.Battlefield.Contains(card)) return aiPlayer;

        Debug.LogWarning($"[GetOwnerOfCard] Couldn't find owner of {card.cardName}");
        return null;
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

    public void WinBattle()
    {
        if (!string.IsNullOrEmpty(BattleData.CurrentZoneId))
        {
            Debug.Log("Player won battle at zone ID: " + BattleData.CurrentZoneId);
            PlayerPrefs.SetString("LastCompletedZone", BattleData.CurrentZoneId);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("No zone ID found when trying to WinBattle.");
        }

        SceneManager.LoadScene("MapScene");
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

    private KeywordAbility GetProtectionKeyword(string color)
    {
        return color switch
        {
            "White" => KeywordAbility.ProtectionFromWhite,
            "Blue" => KeywordAbility.ProtectionFromBlue,
            "Black" => KeywordAbility.ProtectionFromBlack,
            "Red" => KeywordAbility.ProtectionFromRed,
            "Green" => KeywordAbility.ProtectionFromGreen,
            _ => KeywordAbility.None
        };
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
                    (sorcery.requiredTargetType == SorceryCard.TargetType.Land && target is LandCard) ||
                    (sorcery.requiredTargetType == SorceryCard.TargetType.Artifact && target is ArtifactCard) ||
                    (sorcery.requiredTargetType == SorceryCard.TargetType.CreatureOrPlayer && target is CreatureCard);

                bool isOnBattlefield = GetOwnerOfCard(target)?.Battlefield.Contains(target) == true;

                bool colorMatches = true;
                if (!string.IsNullOrEmpty(sorcery.requiredTargetColor))
                {
                    CardData data = CardDatabase.GetCardData(target.cardName);
                    colorMatches = data != null && data.color.Contains(targetingSorcery.requiredTargetColor);
                }

                if (correctType && isOnBattlefield && colorMatches && !IsProtectedFromSpell(target))
                {
                    // Valid target exists, but no visual feedback is shown
                }
            }
        }

    private IEnumerator ResolveTargetedSorceryAfterDelay(Card target, Player caster, SorceryCard sorcery, CardVisual visual)
    {
        yield return new WaitForSeconds(2f);

        sorcery.ResolveEffect(caster, target);   // run effect on the target
        sorcery.ResolveEffect(caster);          // ALSO run general effect (life loss, tokens, etc.)
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

            // Artifact damage ability
            if (targetingArtifact != null &&
                targetingArtifact.activatedAbilities.Contains(ActivatedAbility.DealDamageToCreature))
            {
                if (chosen is CreatureCard targetCreature &&
                    GetOwnerOfCard(targetCreature)?.Battlefield.Contains(targetCreature) == true)
                {
                    Player controller = targetingPlayer;
                    int remaining = targetingArtifact.manaToPayToActivate;

                    remaining -= SpendFromPool(ref controller.ColoredMana.Colorless, remaining);
                    remaining -= SpendFromPool(ref controller.ColoredMana.White, remaining);
                    remaining -= SpendFromPool(ref controller.ColoredMana.Blue, remaining);
                    remaining -= SpendFromPool(ref controller.ColoredMana.Black, remaining);
                    remaining -= SpendFromPool(ref controller.ColoredMana.Red, remaining);
                    remaining -= SpendFromPool(ref controller.ColoredMana.Green, remaining);

                    if (remaining > 0)
                    {
                        Debug.LogWarning("Not enough mana to activate artifact.");
                        CancelTargeting();
                        return;
                    }

                    targetCreature.toughness -= targetingArtifact.damageToCreature;
                    Debug.Log($"{targetingArtifact.cardName} deals {targetingArtifact.damageToCreature} to {targetCreature.cardName}");

                    targetingArtifact.isTapped = true;
                    SendToGraveyard(targetingArtifact, controller);

                    UpdateUI();
                    CheckDeaths(humanPlayer);
                    CheckDeaths(aiPlayer);
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
            // Creature ETB targeting
            if (targetingCreature != null && targetingAbility != null)
            {
                Card target = targetVisual.linkedCard;

                bool correctType =
                    (targetingAbility.requiredTargetType == SorceryCard.TargetType.Creature && target is CreatureCard) ||
                    (targetingAbility.requiredTargetType == SorceryCard.TargetType.Land && target is LandCard) ||
                    (targetingAbility.requiredTargetType == SorceryCard.TargetType.Artifact && target is ArtifactCard);

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
                    (targetingSorcery.requiredTargetType == SorceryCard.TargetType.Creature && chosen is CreatureCard) ||
                    (targetingSorcery.requiredTargetType == SorceryCard.TargetType.Land && chosen is LandCard) ||
                    (targetingSorcery.requiredTargetType == SorceryCard.TargetType.Artifact && chosen is ArtifactCard) ||
                    (targetingSorcery.requiredTargetType == SorceryCard.TargetType.CreatureOrPlayer && chosen is CreatureCard);

                // Validate color
                bool colorMatches = true;
                if (!string.IsNullOrEmpty(targetingSorcery.requiredTargetColor))
                {
                    CardData data = CardDatabase.GetCardData(chosen.cardName);
                    colorMatches = data != null && data.color.Contains(targetingSorcery.requiredTargetColor);
                }

                if (!correctType || !colorMatches)
                {
                    Debug.LogWarning($"Invalid target: {chosen.cardName} does not match type or color requirements.");
                    return;
                }

                // Pay mana before resolving
                var cost = GetManaCostBreakdown(targetingSorcery.manaCost, targetingSorcery.color);
                if (!targetingPlayer.ColoredMana.CanPay(cost))
                {
                    Debug.LogWarning("Not enough mana to cast targeted sorcery.");
                    CancelTargeting();
                    return;
                }

                targetingPlayer.ColoredMana.Pay(cost);
                targetingPlayer.Hand.Remove(targetingSorcery);
                UpdateUI();

                targetingSorcery.chosenTarget = chosen;

                Debug.Log($"Target selected: {chosen.cardName}");

                targetingVisual.transform.SetParent(stackZone, false);
                targetingVisual.transform.localPosition = Vector3.zero;
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

            targetingArtifact = null;
            targetingSorcery = null;
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

            if (targetingVisual != null)
                targetingVisual.EnableTargetingHighlight(false);

            // Move visual to stack
            targetingVisual.transform.SetParent(stackZone, false);
            targetingVisual.transform.localPosition = Vector3.zero;
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
                KeywordAbility protection = targetingSorcery.GetProtectionKeyword(targetingSorcery.PrimaryColor);
                return creature.keywordAbilities.Contains(protection);
            }

            return false;
        }

    private IEnumerator ResolveTargetedSorceryOnPlayerAfterDelay(Player targetPlayer, Player caster, SorceryCard sorcery, CardVisual visual)
        {
            yield return new WaitForSeconds(2f);

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

    private int SpendFromPool(ref int pool, int needed)
        {
            int spent = Mathf.Min(pool, needed);
            pool -= spent;
            return spent;
        }

    public void BeginTargetingWithArtifactDamage(ArtifactCard artifact, Player player, CardVisual visual)
        {
            targetingArtifact = artifact; // << Store the artifact being used
            targetingSorcery = null;
            targetingPlayer = player;
            targetingVisual = visual;
            isTargetingMode = true;

            Debug.Log("Targeting creature to deal damage with artifact.");
        }

    public IEnumerator ResolveArtifactDamageAfterDelay(CardVisual targetVisual, Card targetCard)
        {
            yield return new WaitForSeconds(0.4f);

            if (targetCard is CreatureCard creature &&
                GetOwnerOfCard(creature)?.Battlefield.Contains(creature) == true &&
                targetingArtifact != null)
            {
                creature.toughness -= targetingArtifact.damageToCreature;
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
                isTargetingMode = true;
                targetingVisual = FindCardVisual(creature); // Optional, for visual link

                Debug.Log($"Optional ETB targeting started for {creature.cardName}. Click an artifact if you want to destroy it.");
            }

        public void CancelOptionalTargeting()
            {
                if (targetingCreatureOptional != null)
                {
                    Debug.Log("Optional targeting cancelled.");
                    targetingCreatureOptional = null;
                    optionalAbility = null;
                    isTargetingMode = false;
                    targetingVisual = null;
                }
            }
        
        public void ShowFloatingDamage(int amount, GameObject target)
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

        private IEnumerator ShowDeathVFXAndDelayLayout(Card card, Player owner, CardVisual visual)
            {

                // 1. Create a placeholder object in the same layout slot
                GameObject placeholder = Instantiate(deathPlaceholderPrefab, visual.transform.parent);
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

                // 5. Create graveyard visual (if not a token)
                if (!card.isToken)
                {
                    GameObject visualGO = Instantiate(cardPrefab,
                        owner == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
                    CardVisual graveyardVisual = visualGO.GetComponent<CardVisual>();
                    graveyardVisual.Setup(card, this);
                    graveyardVisual.transform.SetParent(owner == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
                    graveyardVisual.transform.localPosition = Vector3.zero;
                    graveyardVisual.UpdateGraveyardVisual();

                    activeCardVisuals.Add(graveyardVisual);
                }

                // 6. Move to graveyard data list
                owner.Graveyard.Add(card);
            }

        private IEnumerator ShowHandDiscardVFX(Card card, Player owner, CardVisual visual)
            {
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

                    GameManager.Instance.activeCardVisuals.Add(graveyardVisual);
                }

                // 6. Move to graveyard data list
                owner.Graveyard.Add(card);
            }

        public void NotifyArtifactEntered(Card artifact, Player controller)
        {
            foreach (var player in new[] { humanPlayer, aiPlayer })
            {
                foreach (var card in player.Battlefield.ToList())
                {
                    foreach (var ability in card.abilities)
                    {
                        if (ability.timing == TriggerTiming.OnArtifactEnter && ability.effect != null)
                        {
                            int oldLife = player.Life;
                            ability.effect.Invoke(player, artifact);
                            int gained = player.Life - oldLife;
                            if (gained > 0)
                            {
                                ShowFloatingHeal(gained,
                                    player == humanPlayer ? playerLifeContainer : enemyLifeContainer);
                            }
                        }
                    }
                }
            }
        }

        public void NotifyLandEntered(Card land, Player controller)
        {
            foreach (var player in new[] { humanPlayer, aiPlayer })
            {
                foreach (var card in player.Battlefield.ToList())
                {
                    foreach (var ability in card.abilities)
                    {
                        if (ability.timing == TriggerTiming.OnLandEnter && ability.effect != null)
                        {
                            int oldLife = player.Life;
                            ability.effect.Invoke(player, card);
                            int gained = player.Life - oldLife;
                            if (gained > 0)
                            {
                                ShowFloatingHeal(gained,
                                    player == humanPlayer ? playerLifeContainer : enemyLifeContainer);
                            }
                        }
                    }
                }
            }
        }

        public void NotifyLandLeft(Card land, Player controller)
        {
            foreach (var player in new[] { humanPlayer, aiPlayer })
            {
                foreach (var card in player.Battlefield.ToList())
                {
                    foreach (var ability in card.abilities)
                    {
                        if (ability.timing == TriggerTiming.OnLandLeave && ability.effect != null)
                        {
                            int oldLife = player.Life;
                            ability.effect.Invoke(player, card);
                            int gained = player.Life - oldLife;
                            if (gained > 0)
                            {
                                ShowFloatingHeal(gained,
                                    player == humanPlayer ? playerLifeContainer : enemyLifeContainer);
                            }
                        }
                    }
                }
            }
        }

        public void GainLife(Player player, int amount)
        {
            player.Life += amount;
            UpdateUI();

            if (player == humanPlayer)
                ShowFloatingHeal(amount, playerLifeContainer);
            else
                ShowFloatingHeal(amount, enemyLifeContainer);
        }

        public void CheckForGameEnd()
        {
            if (aiPlayer.Life <= 0)
            {
                Debug.Log("AI defeated — player wins!");
                FindObjectOfType<WinScreenUI>().ShowWinScreen();
            }
            else if (humanPlayer.Life <= 0)
            {
                Debug.Log("Human player defeated — game lost.");
                FindObjectOfType<WinScreenUI>().ShowLoseScreen();
            }
        }
}