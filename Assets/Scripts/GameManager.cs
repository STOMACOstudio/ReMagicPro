using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public List<CardVisual> activeCardVisuals = new List<CardVisual>();
    public List<CreatureCard> selectedAttackers = new List<CreatureCard>();
    public List<CreatureCard> currentAttackers = new List<CreatureCard>();

    public Dictionary<CreatureCard, CreatureCard> blockingAssignments = new Dictionary<CreatureCard, CreatureCard>();

    public bool isStackBusy = false;

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

            BuildStartingDeck(humanPlayer);

            //BuildStartingDeck(aiPlayer);
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Plains"));
                aiPlayer.Deck.Add(CardFactory.Create("Obstacle"));
                aiPlayer.Deck.Add(CardFactory.Create("Obstacle"));
                aiPlayer.Deck.Add(CardFactory.Create("Obstacle"));
                aiPlayer.Deck.Add(CardFactory.Create("Obstacle"));
                aiPlayer.Deck.Add(CardFactory.Create("Angry Farmer"));
                aiPlayer.Deck.Add(CardFactory.Create("Angry Farmer"));
                aiPlayer.Deck.Add(CardFactory.Create("Angry Farmer"));
                aiPlayer.Deck.Add(CardFactory.Create("Angry Farmer"));
                aiPlayer.Deck.Add(CardFactory.Create("Waterbearer"));
                aiPlayer.Deck.Add(CardFactory.Create("Waterbearer"));
                aiPlayer.Deck.Add(CardFactory.Create("Waterbearer"));
                aiPlayer.Deck.Add(CardFactory.Create("Waterbearer"));
                aiPlayer.Deck.Add(CardFactory.Create("Sphynx Lynx"));
                aiPlayer.Deck.Add(CardFactory.Create("Sphynx Lynx"));
                aiPlayer.Deck.Add(CardFactory.Create("Origin Golem"));
                aiPlayer.Deck.Add(CardFactory.Create("Origin Golem"));
                aiPlayer.Deck.Add(CardFactory.Create("Candlelight"));
                aiPlayer.Deck.Add(CardFactory.Create("Candlelight"));
                aiPlayer.Deck.Add(CardFactory.Create("Potion of Knowledge"));
                aiPlayer.Deck.Add(CardFactory.Create("Potion of Knowledge"));
                aiPlayer.Deck.Add(CardFactory.Create("Mana Rock"));
                aiPlayer.Deck.Add(CardFactory.Create("Mana Rock"));                

            ShuffleDeck(humanPlayer);
            ShuffleDeck(aiPlayer);

            for (int i = 0; i < 7; i++)
            {
                DrawCard(humanPlayer);
                DrawCard(aiPlayer);
            }
        }

    void BuildStartingDeck(Player player)
        {
            if (DeckHolder.SelectedDeck != null && DeckHolder.SelectedDeck.Count > 0)
            {
                foreach (var data in DeckHolder.SelectedDeck)
                {
                    player.Deck.Add(CardFactory.Create(data.cardName));
                }
            }
            else
            {
                Debug.LogWarning("No deck found in DeckHolder. Using fallback test deck.");
                // fallback to your hardcoded test deck if needed
                player.Deck.Add(CardFactory.Create("Plains"));
                player.Deck.Add(CardFactory.Create("Angry Farmer"));
                // etc.
            }
            /*test deck
                humanPlayer.Deck.Add(CardFactory.Create("Forest"));
                humanPlayer.Deck.Add(CardFactory.Create("Forest"));
                humanPlayer.Deck.Add(CardFactory.Create("Forest"));
                humanPlayer.Deck.Add(CardFactory.Create("Forest"));
                humanPlayer.Deck.Add(CardFactory.Create("Swamp"));
                humanPlayer.Deck.Add(CardFactory.Create("Swamp"));
                humanPlayer.Deck.Add(CardFactory.Create("Swamp"));
                humanPlayer.Deck.Add(CardFactory.Create("Swamp"));
                humanPlayer.Deck.Add(CardFactory.Create("Bog Crocodile"));
                humanPlayer.Deck.Add(CardFactory.Create("Bog Crocodile"));
                humanPlayer.Deck.Add(CardFactory.Create("Bog Crocodile"));
                humanPlayer.Deck.Add(CardFactory.Create("River Crocodile"));
                humanPlayer.Deck.Add(CardFactory.Create("River Crocodile"));    
                humanPlayer.Deck.Add(CardFactory.Create("River Crocodile"));*/
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

                card.OnEnterPlay(player);

                visual.transform.SetParent(player == humanPlayer ? playerLandArea : aiLandArea, false);
                visual.isInBattlefield = true;
                visual.UpdateVisual();
            }

            else if (card is CreatureCard creature)
            {
                if (player.ManaPool >= creature.manaCost)
                {
                    player.ManaPool -= creature.manaCost;
                    card.owner = player;
                    if (player == humanPlayer) UpdateUI();
                    player.Hand.Remove(card);
                    player.Battlefield.Add(card);

                    card.OnEnterPlay(player);

                    if (creature.keywordAbilities.Contains(KeywordAbility.Haste))
                        creature.hasSummoningSickness = false;
                    else
                        creature.hasSummoningSickness = true;

                    if (card.entersTapped)
                    {
                        card.isTapped = true;
                        Debug.Log($"{card.cardName} enters tapped.");
                    }

                    visual.transform.SetParent(playerBattlefieldArea, false);
                    visual.isInBattlefield = true;
                    visual.UpdateVisual();
                }
            }

            else if (card is SorceryCard sorcery)
            {
                if (player.ManaPool >= sorcery.manaCost)
                {
                    isStackBusy = true; // BLOCK OTHER ACTIONS WHILE SORCERY IS ON STACK
                    player.ManaPool -= sorcery.manaCost;
                    card.owner = player;
                    player.Hand.Remove(card);
                    UpdateUI();

                    // Move visual to the stack zone
                    visual.transform.SetParent(stackZone, false);
                    visual.transform.localPosition = Vector3.zero;

                    StartCoroutine(ResolveSorceryAfterDelay(sorcery, visual, player));
                }
                else
                {
                    Debug.Log("Not enough mana to cast this sorcery.");
                }
            }
            else if (card is ArtifactCard artifact)
            {
                if (player.ManaPool >= artifact.manaCost)
                {
                    player.ManaPool -= artifact.manaCost;
                    card.owner = player;
                    if (player == humanPlayer) UpdateUI();

                    player.Hand.Remove(card);
                    player.Battlefield.Add(card);
                    card.OnEnterPlay(player);

                    if (artifact.entersTapped)
                    {
                        artifact.isTapped = true;
                        Debug.Log($"{artifact.cardName} enters tapped.");
                    }

                    // Move to battlefield area visually
                    Transform visualParent = player == humanPlayer
                        ? (card is ArtifactCard ? playerArtifactArea : playerBattlefieldArea)
                        : (card is ArtifactCard ? aiArtifactArea : aiBattlefieldArea);

                    visual.transform.SetParent(visualParent, false);

                    visual.isInBattlefield = true;
                    visual.UpdateVisual();
                }
                else
                {
                    Debug.Log("Not enough mana to play this artifact.");
                }
            }
            else
            {
                Debug.LogWarning("Unhandled card type played: " + card.cardName);
            }
        }

    public void TapLandForMana(LandCard land, Player player)
        {
            if (!land.isTapped)
            {
                land.isTapped = true;
                player.ManaPool++;
                if (player == humanPlayer) UpdateUI();
            }
        }

    public void SendToGraveyard(Card card, Player owner)
        {
            bool diedFromBattlefield = owner.Battlefield.Contains(card);

            owner.Battlefield.Remove(card);
            Debug.Log($"{card.cardName} is being sent to the graveyard.");

            if (diedFromBattlefield)
            {
                card.OnLeavePlay(owner);
            }

            card.isTapped = false;
            CardVisual visual = FindCardVisual(card);
            
            //Reset summoning sickness and toughness
            if (card is CreatureCard deadCreature)
            {
                deadCreature.hasSummoningSickness = false;
                deadCreature.toughness = deadCreature.baseToughness;

                if (visual != null)
                {
                    visual.sicknessText.text = "";
                }
            }
            
            // Handle token destruction
            if (card is CreatureCard creature && card.isToken)
            {
                if (visual != null)
                {
                    activeCardVisuals.Remove(visual);
                    Destroy(visual.gameObject);
                }
                return;
            }

            // Visual fallback
            if (visual == null)
            {
                GameObject visualGO = Instantiate(cardPrefab,
                    owner == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
                visual = visualGO.GetComponent<CardVisual>();
                visual.Setup(card, this);
                visual.isInBattlefield = false;
                activeCardVisuals.Add(visual);
            }

            owner.Graveyard.Add(card);

            var graveyardVisual = FindCardVisual(card);
            if (graveyardVisual != null)
            {
                graveyardVisual.isInBattlefield = false;
                graveyardVisual.transform.SetParent(owner == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
                graveyardVisual.transform.localPosition = Vector3.zero;
                graveyardVisual.UpdateVisual();
            }
        }

    public void ResolveCombat()
        {
            foreach (var attacker in currentAttackers)
            {
                CreatureCard blocker = attacker.blockedByThisBlocker;

                if (blocker != null)
                {
                    // Blocker and attacker deal damage to each other
                    blocker.toughness -= attacker.power;
                    attacker.toughness -= blocker.power;

                    Debug.Log($"{attacker.cardName} is blocked by {blocker.cardName}. They deal damage to each other.");

                    // Lifelink: gain life equal to damage dealt to blocker
                    if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink))
                    {
                        Player owner = GetOwnerOfCard(attacker);
                        owner.Life += attacker.power;
                        Debug.Log($"{attacker.cardName} lifelinks {attacker.power} life to {owner}.");
                    }
                }
                else
                {
                    // Attacker goes unblocked
                    if (humanPlayer.Battlefield.Contains(attacker))
                    {
                        aiPlayer.Life -= attacker.power;
                        Debug.Log($"{attacker.cardName} hits AI for {attacker.power} damage!");

                        // Lifelink: gain life equal to damage dealt to AI
                        if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink))
                        {
                            humanPlayer.Life += attacker.power;
                            Debug.Log($"{attacker.cardName} lifelinks {attacker.power} life to Human.");
                        }
                    }
                    else
                    {
                        humanPlayer.Life -= attacker.power;
                        Debug.Log($"{attacker.cardName} hits YOU for {attacker.power} damage!");

                        // Lifelink: gain life equal to damage dealt to Human
                        if (attacker.keywordAbilities.Contains(KeywordAbility.Lifelink))
                        {
                            aiPlayer.Life += attacker.power;
                            Debug.Log($"{attacker.cardName} lifelinks {attacker.power} life to AI.");
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
                    c.blockedByThisBlocker = null;
                }
            }
            foreach (var card in aiPlayer.Battlefield)
            {
                if (card is CreatureCard c)
                {
                    c.blockingThisAttacker = null;
                    c.blockedByThisBlocker = null;
                }
            }

            currentAttackers.Clear();
            selectedAttackerForBlocking = null;

            UpdateUI();
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
                    creature.blockedByThisBlocker = null;
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
    private IEnumerator ResolveSorceryAfterDelay(SorceryCard sorcery, CardVisual visual, Player caster)
        {
            yield return new WaitForSeconds(2f);

            sorcery.ResolveEffect(caster);
            SendToGraveyard(sorcery, caster);

            // Remove old stack visual if it wasn't reused
            if (caster == aiPlayer && visual != null)
            {
                activeCardVisuals.Remove(visual);
                Destroy(visual.gameObject);
            }

            UpdateUI();
            isStackBusy = false; // UNBLOCK ON RESOLVE
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
                creature.isTapped = creature.entersTapped;
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
                humanPlayer.ManaPool++;
                UpdateUI();
            }
        }

    public void TapToLoseLife(CreatureCard creature)
        {
            if (creature.isTapped || creature.hasSummoningSickness)
                return;

            creature.isTapped = true;

            Player opponent = GetOpponentOf(GetOwnerOfCard(creature));
            opponent.Life -= creature.tapLifeLossAmount;

            Debug.Log($"{creature.cardName} tapped: opponent loses {creature.tapLifeLossAmount} life.");
            UpdateUI();
        }
    public void PayToGainAbility(CreatureCard creature)
        {
            if (creature.isTapped) return;

            Player owner = GetOwnerOfCard(creature);
            if (owner.ManaPool < creature.manaToPayToActivate)
            {
                Debug.Log($"{creature.cardName} can't activate: not enough mana.");
                return;
            }

            owner.ManaPool -= creature.manaToPayToActivate;
            creature.keywordAbilities.Add(creature.abilityToGain); // TEMP grant
            Debug.Log($"{creature.cardName} gains {creature.abilityToGain} until end of turn.");

            UpdateUI();
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
                playerLifeText.text = "Life: " + humanPlayer.Life;

            if (enemyLifeText != null)
                enemyLifeText.text = "Enemy Life: " + aiPlayer.Life;

            if (manaPoolText != null)
            {
                manaPoolText.text = "Mana: " + humanPlayer.ManaPool;
            }
        }
}