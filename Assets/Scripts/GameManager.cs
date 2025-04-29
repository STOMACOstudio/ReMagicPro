using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool isBlockingPhase = false;
    List<CreatureCard> currentAttackers = new List<CreatureCard>();
    List<CreatureCard> enemyPotentialBlockers = new List<CreatureCard>();

    Dictionary<CreatureCard, CreatureCard> blockingAssignments = new Dictionary<CreatureCard, CreatureCard>();

    CreatureCard selectedAttacker = null;

    public TMP_Text manaPoolText;
    public TMP_Text lifeText;
    public TMP_Text enemyLifeText;
    public TMP_Text enemyHandText;
    public GameObject cardPrefab;
    public Transform playerHandArea;
    public Transform playerGraveyardArea;
    public Transform playerLandArea;
    public Transform playerCreatureArea;
    public Transform enemyLandArea;
    public Transform enemyGraveyardArea;
    public Transform enemyCreatureArea;

    public Button confirmBlockersButton;
    public Button nextTurnButton;

    Player player;
    Player enemy;
    
    bool landPlayedThisTurn = false;
    bool isPlayerTurn = true;

    List<CardVisual> activeCards = new List<CardVisual>();

    void Start()
    {
        player = new Player();
        enemy = new Player(); // create an enemy

        confirmBlockersButton.interactable = false;
        
        // Player deck
        player.Deck.Add(new LandCard { cardName = "Forest" });
        player.Deck.Add(new LandCard { cardName = "Forest" });
        player.Deck.Add(new LandCard { cardName = "Forest" });
        player.Deck.Add(new LandCard { cardName = "Forest" });
        player.Deck.Add(new LandCard { cardName = "Forest" });
        player.Deck.Add(new CreatureCard { cardName = "Domestic Cat", manaCost = 1, power = 1, toughness = 1, baseToughness = 1 });
        player.Deck.Add(new CreatureCard { cardName = "Deep Forest Monkeys", manaCost = 2, power = 2, toughness = 2, baseToughness = 2 });
        player.Deck.Add(new CreatureCard { cardName = "Violent Ape", manaCost = 3, power = 3, toughness = 3, baseToughness = 3 });

        // Enemy deck
        enemy.Deck.Add(new LandCard { cardName = "Mountain" });
        enemy.Deck.Add(new LandCard { cardName = "Mountain" });
        enemy.Deck.Add(new LandCard { cardName = "Mountain" });
        enemy.Deck.Add(new LandCard { cardName = "Mountain" });
        enemy.Deck.Add(new LandCard { cardName = "Mountain" });
        enemy.Deck.Add(new CreatureCard { cardName = "Rabid Dog", manaCost = 1, power = 1, toughness = 1, baseToughness = 1 });
        enemy.Deck.Add(new CreatureCard { cardName = "Wild Ostrich", manaCost = 2, power = 3, toughness = 1, baseToughness = 1 });
        enemy.Deck.Add(new CreatureCard { cardName = "Goblin Puncher", manaCost = 3, power = 3, toughness = 3, baseToughness = 3 });

        //shuffle decks
        ShuffleDeck(player);
        ShuffleDeck(enemy);

        for (int i = 0; i < 5; i++)
        {
            DrawCard(player);
            DrawCard(enemy);
        }

        enemy.Life = 20; // start enemy at 20 life (same as player)

        UpdateUI();

        Debug.Log("Setup complete.");
    }
    
    void EnemyAttemptsToBlock(List<CreatureCard> attackers)
    {
        List<CreatureCard> availableBlockers = new List<CreatureCard>();

        foreach (var card in enemy.Battlefield)
        {
            if (card is CreatureCard c && !c.isTapped && c.blockingThisAttacker == null)
            {
                availableBlockers.Add(c);
            }
        }

        foreach (var attacker in attackers)
        {
            CreatureCard bestBlocker = null;

            foreach (var blocker in availableBlockers)
            {
                bool killsAttacker = blocker.power >= attacker.toughness;
                bool survives = blocker.toughness > attacker.power;

                if (killsAttacker || survives)
                {
                    bestBlocker = blocker;
                    break;
                }
            }

            if (bestBlocker != null)
            {
                // Assign and resolve combat
                Debug.Log($"Enemy blocks {attacker.cardName} with {bestBlocker.cardName}");

                bestBlocker.toughness -= attacker.power;
                attacker.toughness -= bestBlocker.power;

                availableBlockers.Remove(bestBlocker);
            }
            else
            {
                // No one blocked → deal damage to enemy
                enemy.Life -= attacker.power;
                Debug.Log($"{attacker.cardName} hits enemy for {attacker.power} damage!");
            }
        }

        DestroyDeadCreatures(player.Battlefield, player.Graveyard, playerGraveyardArea);
        DestroyDeadCreatures(enemy.Battlefield, enemy.Graveyard, enemyGraveyardArea);

        UpdateUI();

        if (enemy.Life <= 0)
        {
            Debug.Log("You won the game!");
            // TODO: Add victory screen later
        }
    }
    public CardVisual FindCardVisual(Card card)
    {
        foreach (var visual in activeCards)
        {
            if (visual.linkedCard == card)
                return visual;
        }
        return null;
    }
    void DestroyDeadCreatures(List<Card> battlefield, List<Card> graveyard, Transform graveyardArea)
    {
        for (int i = battlefield.Count - 1; i >= 0; i--)
        {
            if (battlefield[i] is CreatureCard creature && creature.toughness <= 0)
            {
                Debug.Log(creature.cardName + " is destroyed!");

                Card card = battlefield[i];

                // ---- Clear blocking links (VERY IMPORTANT) ----
                if (creature.blockingThisAttacker != null)
                {
                    creature.blockingThisAttacker.blockedByThisBlocker = null;
                    creature.blockingThisAttacker = null;
                }
                if (creature.blockedByThisBlocker != null)
                {
                    creature.blockedByThisBlocker.blockingThisAttacker = null;
                    creature.blockedByThisBlocker = null;
                }
                // ------------------------------------------------

                // Untap it and reset toughness before graveyard
                card.isTapped = false;

                if (card is CreatureCard deadCreature)
                {
                    deadCreature.toughness = deadCreature.baseToughness;
                }

                // Move to graveyard
                battlefield.RemoveAt(i);
                graveyard.Add(card);

                // Move visual
                foreach (var visual in activeCards)
                {
                    if (visual.linkedCard == card)
                    {
                        visual.transform.SetParent(graveyardArea, false);
                        visual.transform.localPosition = Vector3.zero; // pile cards
                        break;
                    }
                }
            }
        }
    }
    void ResolveCombat()
    {
        Debug.Log("Resolving combat...");

        foreach (var attacker in currentAttackers)
        {
            if (blockingAssignments.TryGetValue(attacker, out CreatureCard blocker))
            {
                // Blocker and attacker fight
                blocker.toughness -= attacker.power;
                attacker.toughness -= blocker.power;

                Debug.Log(blocker.cardName + " and " + attacker.cardName + " battle!");
            }
            else
            {
                // No blocker ➔ attacker hits player directly
                player.Life -= attacker.power;
                Debug.Log(attacker.cardName + " hits player directly for " + attacker.power + " damage!");
            }
        }

        // Kill dead creatures
        DestroyDeadCreatures(player.Battlefield, player.Graveyard, playerGraveyardArea);
        DestroyDeadCreatures(enemy.Battlefield, enemy.Graveyard, enemyGraveyardArea);

        foreach (var visual in activeCards)
        {
            visual.blockingThisAttacker = null;
            visual.blockedByThisBlocker = null;
            visual.UpdateVisual(); // Force refresh and disable line
        }

        // End blocking phase
        EndEnemyTurn();
    }
    public void ConfirmBlocks()
    {
        if (!isBlockingPhase)
        {
            Debug.LogWarning("Tried to confirm blocks outside of blocking phase!");
            return;
        }

        confirmBlockersButton.interactable = false; // Immediately disable button
        ResolveCombat();
    }
    public void OnLandClicked(CardVisual cardVisual)
    {
        LandCard land = cardVisual.linkedCard as LandCard;

        // Can't untap land if mana already spent
        if (land.manaUsedThisTurn)
        {
            Debug.Log("Mana from this land already used this turn!");
            return;
        }

        if (!land.isTapped)
        {
            land.isTapped = true;
            player.ManaPool += 1;
        }
        else
        {
            land.isTapped = false;
            player.ManaPool -= 1;
            if (player.ManaPool < 0)
                player.ManaPool = 0;
        }

        UpdateUI();
    }
    void MoveCardToBattlefield(CardVisual cardVisual)
    {
        if (cardVisual.linkedCard is LandCard)
        {
            cardVisual.transform.SetParent(playerLandArea, false);
            cardVisual.isInBattlefield = true;
        }
        else if (cardVisual.linkedCard is CreatureCard)
        {
            cardVisual.transform.SetParent(playerCreatureArea, false);
            cardVisual.isInBattlefield = true;

            Button button = cardVisual.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners(); // REMOVE click event temporarily
            }
        }
    }
    void MoveEnemyCardToBattlefield(Card card)
    {
        GameObject cardGO = Instantiate(cardPrefab, (card is LandCard) ? enemyLandArea : enemyCreatureArea);
        CardVisual visual = cardGO.GetComponent<CardVisual>();
        visual.Setup(card, this);
        visual.isInBattlefield = true;
        activeCards.Add(visual);

        visual.UpdateVisual(); // <-- Add this line so that summon sickness is correctly shown immediately!
    }
    public void OnCardClicked(CardVisual cardVisual)
    {
        Card card = cardVisual.linkedCard;

        if (card is LandCard)
        {
            if (!landPlayedThisTurn)
            {
                card.Play(player);
                landPlayedThisTurn = true;
                MoveCardToBattlefield(cardVisual);
                UpdateUI();
            }
            else
            {
                Debug.Log("You already played a land this turn!");
            }
        }
        else if (card is CreatureCard creature)
        {
            if (player.ManaPool >= creature.manaCost)
            {
                creature.Play(player);
                MoveCardToBattlefield(cardVisual);
                foreach (var bfCard in player.Battlefield)
                {
                    if (bfCard is LandCard land && land.isTapped)
                    {
                        land.manaUsedThisTurn = true;
                    }
                }
                UpdateUI();
            }
            else
            {
                Debug.Log("Not enough mana to play creature!");
            }
        }
    }
    void CreateCardVisual(Card card, Transform parentArea)
    {
        GameObject cardGO = Instantiate(cardPrefab, parentArea);
        CardVisual visual = cardGO.GetComponent<CardVisual>();
        visual.Setup(card, this);
        activeCards.Add(visual);
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
    void EnemyTurn()
    {
        Debug.Log("Enemy Turn!");

        // 1. Play land if has one
        foreach (var card in enemy.Hand)
        {
            if (card is LandCard)
            {
                card.Play(enemy);
                MoveEnemyCardToBattlefield(card);
                Debug.Log("Enemy played a land.");
                break;
            }
        }

        // 2. Tap lands for mana
        foreach (var card in enemy.Battlefield)
        {
            if (card is LandCard land && !land.isTapped)
            {
                land.TapForMana(enemy);
            }
        }

        // 3. Play creature if can afford
        foreach (var card in enemy.Hand)
        {
            if (card is CreatureCard creature && enemy.ManaPool >= creature.manaCost)
            {
                creature.Play(enemy);
                MoveEnemyCardToBattlefield(creature);
                Debug.Log("Enemy summoned a creature.");
                UpdateUI(); // Ensures summoning sickness is shown before blockers
                break;
            }
        }

        Debug.Log("Checking enemy creatures to attack:");
        UpdateUI();
        foreach (var card in enemy.Battlefield)
        {
            if (card is CreatureCard creature)
            {
                Debug.Log($"{creature.cardName} - SummoningSickness: {creature.hasSummoningSickness}, Tapped: {creature.isTapped}");
            }
        }

        // 4. Attack you
        confirmBlockersButton.interactable = true;
        currentAttackers.Clear();
        enemyPotentialBlockers.Clear();

        foreach (var card in enemy.Battlefield)
        {
            if (card is CreatureCard creature && !creature.isTapped)
            {
                enemyPotentialBlockers.Add(creature);
            }
        }

        foreach (var card in enemy.Battlefield)
        {
            if (card is CreatureCard creature && !creature.hasSummoningSickness && !creature.isTapped)
            {
                currentAttackers.Add(creature);
                creature.isTapped = true; // attacker taps when attacking
            }
        }

        if (currentAttackers.Count > 0)
        {
            Debug.Log("Enemy is attacking you! Choose blockers.");
            isBlockingPhase = true;

            confirmBlockersButton.interactable = true;  // Already doing this
            nextTurnButton.interactable = false;     // <<< Add this line

            // Enable clicking player creatures
            foreach (var visual in activeCards)
            {
                if (visual.isInBattlefield && visual.linkedCard is CreatureCard)
                {
                    Button button = visual.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(visual.OnClick); // Enable creature click
                    }
                }
            }
        }
        else
            {
                Debug.Log("Enemy has no creatures to attack!");
                EndEnemyTurn(); // New function you will add
            }
    }
    public void TryAssignBlocker(CardVisual cardVisual)
    {
        // 1. Clicked an enemy creature → select as attacker to block
        if (cardVisual.linkedCard is CreatureCard enemyCreature && enemy.Battlefield.Contains(enemyCreature))
        {
            if (!currentAttackers.Contains(enemyCreature))
            {
                Debug.Log("You must select an attacking creature!");
                return;
            }

            selectedAttacker = enemyCreature;
            Debug.Log("Selected attacker: " + enemyCreature.cardName);
            return;
        }

        // 2. Clicked your own creature → assign or unassign as blocker
        if (cardVisual.linkedCard is CreatureCard playerCreature && player.Battlefield.Contains(playerCreature))
        {
            if (playerCreature.isTapped)
            {
                Debug.Log("Tapped creatures can't block.");
                return;
            }

            // Already blocking someone? → Unassign
            if (playerCreature.blockingThisAttacker != null)
            {
                CreatureCard attacker = playerCreature.blockingThisAttacker;
                blockingAssignments.Remove(attacker);

                attacker.blockedByThisBlocker = null;
                playerCreature.blockingThisAttacker = null;

                Debug.Log(playerCreature.cardName + " is no longer blocking " + attacker.cardName);

                CardVisual blockerVisual = FindCardVisual(playerCreature);
                CardVisual attackerVisual = FindCardVisual(attacker);

                if (blockerVisual != null) blockerVisual.UpdateVisual();
                if (attackerVisual != null) attackerVisual.UpdateVisual();

                selectedAttacker = null; // <<< added here too after unassign
                return;
            }

            // No attacker selected
            if (selectedAttacker == null)
            {
                Debug.Log("No attacker selected. First click on an attacker.");
                return;
            }

            // Assign block
            blockingAssignments[selectedAttacker] = playerCreature;
            playerCreature.blockingThisAttacker = selectedAttacker;
            selectedAttacker.blockedByThisBlocker = playerCreature;

            Debug.Log(playerCreature.cardName + " blocks " + selectedAttacker.cardName);

            CardVisual blockerVis = FindCardVisual(playerCreature);
            CardVisual attackerVis = FindCardVisual(selectedAttacker);

            if (blockerVis != null) blockerVis.UpdateVisual();
            if (attackerVis != null) attackerVis.UpdateVisual();

            selectedAttacker = null; // Reset attacker selection after assignment
        }
    }
    void EndEnemyTurn()
    {
        isBlockingPhase = false;
        currentAttackers.Clear();
        blockingAssignments.Clear();
        confirmBlockersButton.interactable = false;
        nextTurnButton.interactable = true;

        foreach (var visual in activeCards)
        {
            if (visual.isInBattlefield && visual.linkedCard is CreatureCard)
            {
                Button button = visual.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners(); // Disable creature click
                }
            }
        }
        
        UpdateUI();
        Debug.Log("Enemy turn ended.");
    }
    public void AttackWithCreatures()
    {
        if (!isPlayerTurn)
        {
            Debug.Log("You can't attack during enemy turn!");
            return;
        }

        List<CreatureCard> attackers = new List<CreatureCard>();

        foreach (var card in player.Battlefield)
        {
            if (card is CreatureCard creature && !creature.hasSummoningSickness && !card.isTapped)
            {
                attackers.Add(creature);
                creature.isTapped = true; // Tap after attack
            }
        }

        if (attackers.Count > 0)
        {
            Debug.Log($"You're attacking with {attackers.Count} creatures.");
            EnemyAttemptsToBlock(attackers);
        }
        else
        {
            Debug.Log("No creatures available to attack!");
        }
    }
    public void NextTurn()
    {
        if (isBlockingPhase)
        {
            Debug.Log("Auto-resolving combat because player didn't confirm blockers.");

            List<CreatureCard> availableBlockers = new List<CreatureCard>();

            foreach (var card in enemy.Battlefield)
            {
                if (card is CreatureCard c && !c.isTapped && c.blockingThisAttacker == null)
                {
                    availableBlockers.Add(c);
                }
            }

            foreach (var attacker in currentAttackers)
            {
                if (attacker.blockedByThisBlocker != null)
                    continue; // already blocked

                CreatureCard bestBlocker = null;

                foreach (var blocker in availableBlockers)
                {
                    bool killsAttacker = blocker.power >= attacker.toughness;
                    bool survives = blocker.toughness > attacker.power;

                    if (killsAttacker || survives)
                    {
                        bestBlocker = blocker;
                        break;
                    }
                }

                if (bestBlocker != null)
                {
                    blockingAssignments[attacker] = bestBlocker;
                    bestBlocker.blockingThisAttacker = attacker;
                    attacker.blockedByThisBlocker = bestBlocker;

                    availableBlockers.Remove(bestBlocker);

                    Debug.Log($"Enemy blocks {attacker.cardName} with {bestBlocker.cardName}");
                }
            }

            ConfirmBlocks();
            return;
        }

        Debug.Log("Ending turn!");

        // Clear mana pool
        if (isPlayerTurn)
            player.ManaPool = 0;
        else
            enemy.ManaPool = 0;

        // Heal damage
        foreach (var card in player.Battlefield)
        {
            if (card is CreatureCard creature)
                creature.toughness = creature.baseToughness;
        }
        foreach (var card in enemy.Battlefield)
        {
            if (card is CreatureCard creature)
                creature.toughness = creature.baseToughness;
        }

        // Switch turn
        isPlayerTurn = !isPlayerTurn;
        Debug.Log(isPlayerTurn ? "It is now YOUR turn!" : "It is now ENEMY turn!");

        UpdateUI();
        StartNewTurn();
    }
    void StartNewTurn()
    {
        Debug.Log("Starting new turn!");

        if (isPlayerTurn)
        {
            // Draw a card
            DrawCard(player);
            
            // Player untap phase
            foreach (var card in player.Battlefield)
            {
                card.isTapped = false;

                if (card is CreatureCard creature)
                {
                    creature.hasSummoningSickness = false;
                }

                if (card is LandCard land)
                {
                    land.manaUsedThisTurn = false; // <<< ADD THIS
                }
            }

            player.ManaPool = 0;
            landPlayedThisTurn = false;
            Debug.Log("Player untap phase complete.");
        }
        else
        {
            // Draw a card
            DrawCard(enemy);

            // Enemy untap phase
            foreach (var card in enemy.Battlefield)
            {
                card.isTapped = false;

                if (card is CreatureCard creature)
                {
                    creature.hasSummoningSickness = false;
                }
            }

            enemy.ManaPool = 0;
            Debug.Log("Enemy untap phase complete.");

            // Enemy automatically plays
            EnemyTurn();
        }

        UpdateUI();
    }
    void DrawCard(Player playerDrawing)
    {
        if (playerDrawing.Deck.Count > 0)
        {
            Card topCard = playerDrawing.Deck[0];
            playerDrawing.Deck.RemoveAt(0);
            playerDrawing.Hand.Add(topCard);

            if (playerDrawing == player)
            {
                CreateCardVisual(topCard, playerHandArea); // Player sees drawn cards
            }

            Debug.Log(playerDrawing == player ? "You draw a card: " + topCard.cardName : "Enemy draws a card.");
        }
        else
        {
            Debug.Log(playerDrawing == player ? "You have no cards to draw." : "Enemy has no cards to draw.");
        }
    }
    void UpdateUI()
    {
        foreach (var visual in activeCards)
        {
            visual.UpdateVisual();
        }

        manaPoolText.text = "Mana Pool: " + player.ManaPool;
        lifeText.text = "Life: " + player.Life;
        enemyLifeText.text = "Enemy Life: " + enemy.Life;
        enemyHandText.text = "Enemy Hand: " + enemy.Hand.Count + " cards";
    }
}