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

    public Transform aiBattlefieldArea;
    public Transform aiGraveyardArea;
    public Transform aiLandArea;

    public GameObject cardPrefab;

    public List<CardVisual> activeCardVisuals = new List<CardVisual>();
    public List<CreatureCard> selectedAttackers = new List<CreatureCard>();
    public List<CreatureCard> currentAttackers = new List<CreatureCard>();

    public Dictionary<CreatureCard, CreatureCard> blockingAssignments = new Dictionary<CreatureCard, CreatureCard>();

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
        aiPlayer.Deck.Add(CardFactory.Create("Mountain"));
        aiPlayer.Deck.Add(CardFactory.Create("Mountain"));
        aiPlayer.Deck.Add(CardFactory.Create("Mountain"));
        aiPlayer.Deck.Add(CardFactory.Create("Mountain"));
        aiPlayer.Deck.Add(CardFactory.Create("Flying Pig"));
        aiPlayer.Deck.Add(CardFactory.Create("Rabid Dog"));
        aiPlayer.Deck.Add(CardFactory.Create("Great Boulder"));
        aiPlayer.Deck.Add(CardFactory.Create("Wild Ostrich"));
        aiPlayer.Deck.Add(CardFactory.Create("Goblin Puncher"));
        aiPlayer.Deck.Add(CardFactory.Create("Glassmole"));

        ShuffleDeck(humanPlayer);
        ShuffleDeck(aiPlayer);

        for (int i = 0; i < 5; i++)
        {
            DrawCard(humanPlayer);
            DrawCard(aiPlayer);
        }
    }

    void BuildStartingDeck(Player player)
    {
        /*//test white deck
        humanPlayer.Deck.Add(CardFactory.Create("Plains"));
        humanPlayer.Deck.Add(CardFactory.Create("Plains"));
        humanPlayer.Deck.Add(CardFactory.Create("Plains"));
        humanPlayer.Deck.Add(CardFactory.Create("Plains"));
        humanPlayer.Deck.Add(CardFactory.Create("Plains"));
        humanPlayer.Deck.Add(CardFactory.Create("Waterbearer"));
        humanPlayer.Deck.Add(CardFactory.Create("Virgins Procession"));
        humanPlayer.Deck.Add(CardFactory.Create("Virgins Procession"));
        humanPlayer.Deck.Add(CardFactory.Create("Virgins Procession"));
        humanPlayer.Deck.Add(CardFactory.Create("Angry Farmer"));
        humanPlayer.Deck.Add(CardFactory.Create("Gallant Lord"));
        humanPlayer.Deck.Add(CardFactory.Create("Skyhunter Unicorn"));
        humanPlayer.Deck.Add(CardFactory.Create("Realm Protector"));
        humanPlayer.Deck.Add(CardFactory.Create("Skyhunter Unicorn"));*/
        
        //test blue deck
        humanPlayer.Deck.Add(CardFactory.Create("Island"));
        humanPlayer.Deck.Add(CardFactory.Create("Island"));
        humanPlayer.Deck.Add(CardFactory.Create("Island"));
        humanPlayer.Deck.Add(CardFactory.Create("Island"));
        humanPlayer.Deck.Add(CardFactory.Create("Island"));
        humanPlayer.Deck.Add(CardFactory.Create("Lucky Fisherman"));
        humanPlayer.Deck.Add(CardFactory.Create("Lucky Fisherman"));
        humanPlayer.Deck.Add(CardFactory.Create("Lucky Fisherman"));
        humanPlayer.Deck.Add(CardFactory.Create("Lucky Fisherman"));
        /*humanPlayer.Deck.Add(CardFactory.Create("Angry Farmer"));
        humanPlayer.Deck.Add(CardFactory.Create("Gallant Lord"));
        humanPlayer.Deck.Add(CardFactory.Create("Skyhunter Unicorn"));
        humanPlayer.Deck.Add(CardFactory.Create("Realm Protector"));
        humanPlayer.Deck.Add(CardFactory.Create("Skyhunter Unicorn"));*/

        /*//test black deck
        humanPlayer.Deck.Add(CardFactory.Create("Swamp"));
        humanPlayer.Deck.Add(CardFactory.Create("Swamp"));
        humanPlayer.Deck.Add(CardFactory.Create("Swamp"));
        humanPlayer.Deck.Add(CardFactory.Create("Swamp"));
        humanPlayer.Deck.Add(CardFactory.Create("Swamp"));
        humanPlayer.Deck.Add(CardFactory.Create("Famished Crow"));
        humanPlayer.Deck.Add(CardFactory.Create("Limping Corpse"));
        humanPlayer.Deck.Add(CardFactory.Create("Giant Rat"));
        humanPlayer.Deck.Add(CardFactory.Create("Giant Crow"));
        humanPlayer.Deck.Add(CardFactory.Create("Bog Mosquito"));*/

        /* //test green deck
        humanPlayer.Deck.Add(CardFactory.Create("Forest"));
        humanPlayer.Deck.Add(CardFactory.Create("Forest"));
        humanPlayer.Deck.Add(CardFactory.Create("Forest"));
        humanPlayer.Deck.Add(CardFactory.Create("Forest"));
        humanPlayer.Deck.Add(CardFactory.Create("Forest"));
        humanPlayer.Deck.Add(CardFactory.Create("Domestic Cat"));
        humanPlayer.Deck.Add(CardFactory.Create("Deep Forest Monkeys"));
        humanPlayer.Deck.Add(CardFactory.Create("Violent Ape"));
        humanPlayer.Deck.Add(CardFactory.Create("Obstacle"));
        humanPlayer.Deck.Add(CardFactory.Create("Origin Golem"));*/

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
            visual.Setup(card, this);
            activeCardVisuals.Add(visual);
        }
    }

    public void PlayCard(Player player, CardVisual visual)
    {
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
                if (player == humanPlayer) UpdateUI();
                player.Hand.Remove(card);
                player.Battlefield.Add(card);

                card.OnEnterPlay(player); // ✅ Trigger OnEnterPlay

                if (creature.keywordAbilities.Contains(KeywordAbility.Haste))
                    creature.hasSummoningSickness = false;
                else
                    creature.hasSummoningSickness = true;

                if (creature.entersTapped)
                    creature.isTapped = true;

                visual.transform.SetParent(playerBattlefieldArea, false);
                visual.isInBattlefield = true;
                visual.UpdateVisual();
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
        owner.Battlefield.Remove(card);
        card.OnLeavePlay(owner);
        owner.Graveyard.Add(card);
        card.isTapped = false;

        if (card is CreatureCard creature)
        {
            creature.hasSummoningSickness = false;
            creature.toughness = creature.baseToughness;
            creature.blockingThisAttacker = null;
            creature.blockedByThisBlocker = null;
        }

        CardVisual visual = FindCardVisual(card);
        visual.UpdateVisual();

        if (visual != null)
        {
            visual.isInBattlefield = false;
            visual.transform.SetParent(owner == humanPlayer ? playerGraveyardArea : aiGraveyardArea);
            visual.transform.localPosition = Vector3.zero;
            visual.UpdateVisual();
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
            }
            else
            {
                // Attacker goes unblocked
                if (humanPlayer.Battlefield.Contains(attacker))
                {
                    aiPlayer.Life -= attacker.power;
                    Debug.Log($"{attacker.cardName} hits AI for {attacker.power} damage!");
                }
                else
                {
                    humanPlayer.Life -= attacker.power;
                    Debug.Log($"{attacker.cardName} hits YOU for {attacker.power} damage!");
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

    void CheckDeaths(Player player)
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