using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }

    public enum TurnPhase
    {
        StartTurn,
        Untap,
        Upkeep,
        Draw,
        Main1,
        EnterCombat,
        ChooseAttackers,
        ConfirmAttackers,
        ChooseBlockers,
        ConfirmBlockers,
        Damage,
        Main2,
        EndTurn
    }

    public enum PlayerType
    {
        Human,
        AI
    }

    public PlayerType currentPlayer = PlayerType.Human;
    public TurnPhase currentPhase = TurnPhase.StartTurn;

    public bool waitingForPlayerInput = false;
    public TMP_Text phaseText;

    [Header("Buttons")]
    public Button nextPhaseButton;
    public Button confirmAttackersButton;
    public Button confirmBlockersButton;
    public Button attackAllButton;
    public Button clearAttackersButton;

    public TurnPhase lastPhaseBeforeStack;
    public bool waitingToResumeAI = false;

    private Coroutine damageCoroutine;

    void Start()
        {
            Instance = this;

            nextPhaseButton.onClick.AddListener(NextPhaseButton);
            confirmAttackersButton.onClick.AddListener(ConfirmAttackers);
            confirmBlockersButton.onClick.AddListener(ConfirmBlockers);
            attackAllButton.onClick.AddListener(SelectAllEligibleAttackers);
            clearAttackersButton.onClick.AddListener(ClearAllSelectedAttackers);

            confirmAttackersButton.gameObject.SetActive(false);
            confirmBlockersButton.gameObject.SetActive(false);
            attackAllButton.gameObject.SetActive(false);
            clearAttackersButton.gameObject.SetActive(false);

            BeginTurn(PlayerType.Human);
        }

    void Update()
        {
            if (currentPlayer == PlayerType.AI && !waitingForPlayerInput && !GameManager.Instance.isStackBusy)
            {
                RunCurrentPhase();
            }

            if (nextPhaseButton != null)
            {
                bool allowNext = currentPlayer == PlayerType.Human &&
                                waitingForPlayerInput &&
                                !GameManager.Instance.isStackBusy &&
                                currentPhase != TurnPhase.ConfirmAttackers &&
                                currentPhase != TurnPhase.ConfirmBlockers &&
                                currentPhase != TurnPhase.ChooseAttackers;

                nextPhaseButton.interactable = allowNext;

                TMP_Text label = nextPhaseButton.GetComponentInChildren<TMP_Text>();
                if (label != null)
                {
                    label.text = (currentPhase == TurnPhase.Main2) ? "END TURN" : "NEXT PHASE";
                }
            }

            // Handle spacebar shortcut
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (currentPlayer == PlayerType.Human &&
                    waitingForPlayerInput &&
                    !GameManager.Instance.isStackBusy &&
                    currentPhase != TurnPhase.ConfirmAttackers &&
                    currentPhase != TurnPhase.ConfirmBlockers &&
                    currentPhase != TurnPhase.ChooseAttackers)
                {
                    NextPhaseButton();
                }
            }
        }

    public void NextPhaseButton()
        {
            if (currentPlayer == PlayerType.Human && waitingForPlayerInput)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.buttonClick);

                if (GameManager.Instance.isTargetingMode)
                {
                    Debug.Log("Canceled targeting because player pressed Next Phase.");
                    GameManager.Instance.CancelTargeting();
                }
                if (GameManager.Instance.targetingCreatureOptional != null)
                {
                    Debug.Log("Canceled optional ETB targeting because player pressed Next Phase.");
                    GameManager.Instance.CancelOptionalTargeting();
                }

                waitingForPlayerInput = false;
                HideAllConfirmButtons();
                AdvancePhase();
            }
        }

    public void ConfirmAttackers()
        {
            if (waitingForPlayerInput)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.buttonClick);
                waitingForPlayerInput = false;
                confirmAttackersButton.gameObject.SetActive(false);
                attackAllButton.gameObject.SetActive(false);
                clearAttackersButton.gameObject.SetActive(false);

                // Use only manually selected attackers
                GameManager.Instance.currentAttackers.Clear();
                GameManager.Instance.currentAttackers.AddRange(GameManager.Instance.selectedAttackers);
                GameManager.Instance.selectedAttackers.Clear();

                foreach (var creature in GameManager.Instance.currentAttackers)
                {
                    Debug.Log("Attacker declared: " + creature.cardName);
                }

                AdvancePhase();
            }
        }

    public void ConfirmBlockers()
        {
            if (waitingForPlayerInput)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.buttonClick);
                waitingForPlayerInput = false;
                confirmBlockersButton.gameObject.SetActive(false);
                AdvancePhase();
                GameManager.Instance.UpdateUI();
            }
        }

    void HideAllConfirmButtons()
        {
            confirmAttackersButton.gameObject.SetActive(false);
            confirmBlockersButton.gameObject.SetActive(false);
            attackAllButton.gameObject.SetActive(false);
            clearAttackersButton.gameObject.SetActive(false);
        }

    public void BeginTurn(PlayerType player)
        {
            currentPlayer = player;
            currentPhase = TurnPhase.StartTurn;
            Debug.Log($"\n=== {player} TURN START ===");
            AdvancePhase();
        }

    void AdvancePhase()
        {
            GameManager.Instance.UpdateUI();

            // Empty mana at the end of each phase
            var player = currentPlayer == PlayerType.Human
                ? GameManager.Instance.humanPlayer
                : GameManager.Instance.aiPlayer;

            player.ColoredMana.Clear();

            if (currentPlayer == PlayerType.Human)
                GameManager.Instance.UpdateUI();

            currentPhase++;
            RunCurrentPhase();
        }

    void RunCurrentPhase()
        {
            Debug.Log($"[Phase] {currentPlayer} - {currentPhase}");

            if (GameManager.Instance.isStackBusy)
            {
                Debug.Log("AI cast a sorcery — stack is busy. Will resume after.");
                lastPhaseBeforeStack = currentPhase;
                waitingToResumeAI = true;
                return;
            }

            string label = $"{currentPlayer} - {currentPhase}";
            if (phaseText != null)
                phaseText.text = label;

            switch (currentPhase)
            {
                case TurnPhase.Untap:
                    Debug.Log("→ Untapping all permanents.");
                    var p = (currentPlayer == PlayerType.Human) ? GameManager.Instance.humanPlayer : GameManager.Instance.aiPlayer;
                    p.hasPlayedLandThisTurn = false;
                    GameManager.Instance.ResetPermanents(currentPlayer == PlayerType.Human ? GameManager.Instance.humanPlayer : GameManager.Instance.aiPlayer);
                    AdvancePhase();
                    break;

                case TurnPhase.Upkeep:
                    Debug.Log("→ Upkeep phase.");
                    var player = currentPlayer == PlayerType.Human
                        ? GameManager.Instance.humanPlayer
                        : GameManager.Instance.aiPlayer;

                    foreach (var card in player.Battlefield.ToList())
                    {
                        foreach (var ability in card.abilities)
                        {
                            if (ability.timing == TriggerTiming.OnUpkeep && ability.effect != null)
                            {
                                Debug.Log($"[Upkeep Trigger] {card.cardName} triggers OnUpkeep.");

                                int oldLife = player.Life;
                                ability.effect.Invoke(player, card);
                                int gained = player.Life - oldLife;

                                if (gained > 0)
                                {
                                    GameManager.Instance.ShowFloatingHeal(
                                        gained,
                                        player == GameManager.Instance.humanPlayer
                                            ? GameManager.Instance.playerLifeContainer
                                            : GameManager.Instance.enemyLifeContainer
                                    );
                                }
                            }
                        }
                    }

                    AdvancePhase();
                    break;

                case TurnPhase.Draw:
                    Debug.Log("→ Drawing a card.");
                    
                    var drawPlayer = currentPlayer == PlayerType.Human
                        ? GameManager.Instance.humanPlayer
                        : GameManager.Instance.aiPlayer;

                    GameManager.Instance.DrawCard(drawPlayer);
                    GameManager.Instance.UpdateUI();

                    AdvancePhase();
                    break;

                case TurnPhase.Main1:
                case TurnPhase.Main2:
                    if (currentPlayer == PlayerType.Human)
                    {
                        Debug.Log("→ Main Phase: Play land or cast spells.");
                        waitingForPlayerInput = true;
                    }
                    else
                    {
                        Debug.Log("→ AI Main Phase: Playing cards.");

                        Player ai = GameManager.Instance.aiPlayer;

                        // Play 1 land if possible
                        if (!ai.hasPlayedLandThisTurn)
                        {
                            for (int i = 0; i < ai.Hand.Count; i++)
                            {
                                if (ai.Hand[i] is LandCard)
                                {
                                    Card land = ai.Hand[i];

                                    land.Play(ai);
                                    ai.Hand.Remove(land);
                                    
                                    if (land.entersTapped || GameManager.Instance.IsAllPermanentsEnterTappedActive())
                                    {
                                        land.isTapped = true;
                                        Debug.Log($"{land.cardName} (AI) enters tapped (static effect or base).");
                                    }

                                    GameObject obj = GameObject.Instantiate(GameManager.Instance.cardPrefab, GameManager.Instance.aiLandArea);
                                    CardVisual visual = obj.GetComponent<CardVisual>();
                                    visual.Setup(land, GameManager.Instance);
                                    visual.isInBattlefield = true;
                                    GameManager.Instance.activeCardVisuals.Add(visual);

                                    Debug.Log("AI played land: " + land.cardName);
                                    ai.hasPlayedLandThisTurn = true;
                                    break;
                                }
                            }
                        }
                        


                        // Play as many cards as AI can afford
                        bool playedCard = true;

                        while (playedCard && !GameManager.Instance.isStackBusy)
                        {
                            playedCard = false;

                            for (int i = 0; i < ai.Hand.Count; i++)
                            {
                                Card card = ai.Hand[i];

                                if (GameManager.Instance.IsOnlyCastCreatureSpellsActive() && !(card is CreatureCard))
                                    continue;
                                
                                if (card is CreatureCard creature)
                                {
                                    var cost = GameManager.Instance.GetManaCostBreakdown(creature.manaCost, creature.color);
                                    int reduction = GameManager.Instance.GetCreatureCostReduction(ai);
                                    if (reduction > 0 && cost.ContainsKey("Colorless"))
                                        cost["Colorless"] = Mathf.Max(0, cost["Colorless"] - reduction);
                                    if (EnsureManaForCost(ai, cost))
                                    {
                                        ai.ColoredMana.Pay(cost);
                                       ai.Hand.Remove(card);
                                       ai.Battlefield.Add(card);
                                       card.OnEnterPlay(ai);
                                       if (card.color.Contains("Artifact"))
                                           GameManager.Instance.NotifyArtifactEntered(card, ai);

                                        if (card.entersTapped || GameManager.Instance.IsAllPermanentsEnterTappedActive())
                                        {
                                            card.isTapped = true;
                                            Debug.Log($"{card.cardName} (AI) enters tapped (static effect or base).");
                                        }

                                        if (creature.abilities != null)
                                        {
                                            foreach (var ability in creature.abilities)
                                            {
                                                if (ability.timing == TriggerTiming.OnEnter && ability.requiresTarget)
                                                {
                                                    Player opponent = GameManager.Instance.GetOpponentOf(ai);

                                                    Card target = opponent.Battlefield
                                                        .Where(c =>
                                                            (ability.requiredTargetType == SorceryCard.TargetType.Creature && c is CreatureCard creatureT &&
                                                                !(ability.excludeArtifactCreatures && creatureT.color.Contains("Artifact"))) ||
                                                            (ability.requiredTargetType == SorceryCard.TargetType.Artifact && c is ArtifactCard) ||
                                                            (ability.requiredTargetType == SorceryCard.TargetType.Land && c is LandCard))
                                                        .OrderByDescending(c => CardDatabase.GetCardData(c.cardName)?.manaCost ?? 0)
                                                        .FirstOrDefault();

                                                    if (target != null)
                                                    {
                                                        ability.effect?.Invoke(ai, target);
                                                        Debug.Log($"[AI ETB] {creature.cardName} destroys {target.cardName}");
                                                        GameManager.Instance.CheckDeaths(GameManager.Instance.humanPlayer);
                                                        GameManager.Instance.CheckDeaths(GameManager.Instance.aiPlayer);
                                                        GameManager.Instance.UpdateUI();
                                                    }
                                                }
                                            }
                                        }

                                       GameObject obj = GameObject.Instantiate(GameManager.Instance.cardPrefab, GameManager.Instance.aiBattlefieldArea);
                                       CardVisual visual = obj.GetComponent<CardVisual>();
                                       visual.Setup(card, GameManager.Instance);
                                       visual.isInBattlefield = true;
                                       GameManager.Instance.activeCardVisuals.Add(visual);

                                        creature.hasSummoningSickness = !creature.keywordAbilities.Contains(KeywordAbility.Haste);

                                        Debug.Log($"AI played creature: {card.cardName}");
                                        playedCard = true;
                                        break;
                                    }
                                }
                                else if (card is SorceryCard sorcery)
                                {
                                    var cost = GameManager.Instance.GetManaCostBreakdown(sorcery.manaCost, sorcery.color);
                                    if (EnsureManaForCost(ai, cost))
                                    {
                                        if (sorcery.requiredTargetType == SorceryCard.TargetType.Creature &&
                                            sorcery.destroyTargetIfTypeMatches)
                                        {
                                            Player opponent = GameManager.Instance.GetOpponentOf(ai);

                                            // Pick enemy creature with the highest mana cost
                                            var target = opponent.Battlefield
                                                .OfType<CreatureCard>()
                                                .Where(c => !(sorcery.excludeArtifactCreatures && c.color.Contains("Artifact")))
                                                .OrderByDescending(c =>
                                                {
                                                    var data = CardDatabase.GetCardData(c.cardName);
                                                    return data != null ? data.manaCost : 0;
                                                })
                                                .FirstOrDefault();

                                            if (target != null)
                                            {
                                                sorcery.chosenTarget = target;
                                                sorcery.chosenPlayerTarget = null;

                                                Debug.Log($"AI targets {target.cardName} with {sorcery.cardName} (highest cost creature).");
                                            }
                                        }
                                        else if (sorcery.requiredTargetType == SorceryCard.TargetType.Artifact &&
                                                sorcery.destroyTargetIfTypeMatches)
                                        {
                                            Player opponent = GameManager.Instance.GetOpponentOf(ai);

                                            var target = opponent.Battlefield
                                                .OfType<ArtifactCard>()
                                                .OrderByDescending(c =>
                                                {
                                                    var data = CardDatabase.GetCardData(c.cardName);
                                                    return data != null ? data.manaCost : 0;
                                                })
                                                .FirstOrDefault();

                                            if (target != null)
                                            {
                                                sorcery.chosenTarget = target;
                                                sorcery.chosenPlayerTarget = null;
                                                Debug.Log($"AI targets {target.cardName} with {sorcery.cardName} (highest cost artifact).");
                                            }
                                        }
                                        else if (sorcery.requiredTargetType == SorceryCard.TargetType.Land &&
                                                sorcery.destroyTargetIfTypeMatches)
                                        {
                                            Player opponent = GameManager.Instance.GetOpponentOf(ai);

                                            var target = opponent.Battlefield
                                                .OfType<LandCard>()
                                                .OrderByDescending(c =>
                                                {
                                                    var data = CardDatabase.GetCardData(c.cardName);
                                                    return data != null ? data.manaCost : 0;
                                                })
                                                .FirstOrDefault();

                                            if (target != null)
                                            {
                                                sorcery.chosenTarget = target;
                                                sorcery.chosenPlayerTarget = null;
                                                Debug.Log($"AI targets {target.cardName} with {sorcery.cardName} (highest cost land).");
                                            }
                                        }

                                        bool canTarget = sorcery.requiredTargetType == SorceryCard.TargetType.CreatureOrPlayer && sorcery.damageToTarget > 0;

                                        if (canTarget)
                                        {
                                            int damage = sorcery.damageToTarget;
                                            Player opponent = GameManager.Instance.humanPlayer;

                                            // Get enemy creatures
                                            List<CreatureCard> enemyCreatures = opponent.Battlefield
                                                .OfType<CreatureCard>()
                                                .Where(c => c.toughness > 0)
                                                .ToList();

                                            // 1. Kill opponent
                                            if (opponent.Life <= damage)
                                            {
                                                sorcery.chosenTarget = null;
                                                sorcery.chosenPlayerTarget = opponent;
                                            }
                                            // 2. Killable creature
                                            else
                                            {
                                                var killable = enemyCreatures.FirstOrDefault(c => c.toughness <= damage);
                                                if (killable != null)
                                                {
                                                    sorcery.chosenTarget = killable;
                                                }
                                                else
                                                {
                                                    // 3. Fallback: damage opponent
                                                    sorcery.chosenTarget = null;
                                                    sorcery.chosenPlayerTarget = opponent;

                                                }
                                            }
                                        }

                                        if (sorcery.requiresTarget && sorcery.chosenTarget == null && sorcery.chosenPlayerTarget == null)
                                        {
                                            Debug.Log($"[AI] Skipping {sorcery.cardName} — no valid target.");
                                            continue; // Go to next card
                                        }
                                        
                                        ai.ColoredMana.Pay(cost);
                                        ai.Hand.Remove(sorcery);
                                        sorcery.owner = ai;

                                        GameObject obj = GameObject.Instantiate(GameManager.Instance.cardPrefab, GameManager.Instance.stackZone);
                                        CardVisual visual = obj.GetComponent<CardVisual>();
                                        CardData data = CardDatabase.GetCardData(sorcery.cardName);
                                        visual.Setup(sorcery, GameManager.Instance, data);

                                        visual.transform.localPosition = Vector3.zero;
                                        visual.transform.SetParent(GameManager.Instance.stackZone, false);

                                        GameManager.Instance.UpdateUI();
                                        SoundManager.Instance.PlaySound(SoundManager.Instance.cardPlay);

                                        GameManager.Instance.isStackBusy = true;
                                        TurnSystem.Instance.waitingToResumeAI = true;
                                        TurnSystem.Instance.lastPhaseBeforeStack = currentPhase;

                                        GameManager.Instance.StartCoroutine(GameManager.Instance.ResolveSorceryAfterDelay(sorcery, visual, ai));

                                        playedCard = true;
                                        break;
                                    }   
                                }
                                else if (card is ArtifactCard artifact)
                                {
                                    var cost = GameManager.Instance.GetManaCostBreakdown(artifact.manaCost, artifact.color);
                                    if (EnsureManaForCost(ai, cost))
                                    {
                                        ai.ColoredMana.Pay(cost);
                                        ai.Hand.Remove(card);
                                        ai.Battlefield.Add(card);
                                        card.OnEnterPlay(ai);
                                        GameManager.Instance.NotifyArtifactEntered(card, ai);

                                        if (card.entersTapped || GameManager.Instance.IsAllPermanentsEnterTappedActive())
                                        {
                                            card.isTapped = true;
                                            Debug.Log($"{card.cardName} (AI) enters tapped (static effect or base).");
                                        }

                                        GameObject obj = GameObject.Instantiate(GameManager.Instance.cardPrefab, GameManager.Instance.aiArtifactArea);
                                        CardVisual visual = obj.GetComponent<CardVisual>();
                                        visual.Setup(card, GameManager.Instance);
                                        visual.isInBattlefield = true;
                                        GameManager.Instance.activeCardVisuals.Add(visual);

                                        Debug.Log($"AI played artifact: {card.cardName}");
                                        playedCard = true;
                                        break;
                                    }
                                }
                            }
                        }

                        foreach (var card in ai.Battlefield)
                        {
                            if (card is CreatureCard creature &&
                                !creature.isTapped &&
                                (!creature.hasSummoningSickness || creature.keywordAbilities.Contains(KeywordAbility.Haste)))
                            {
                                // TAP TO LOSE LIFE
                                if (creature.activatedAbilities.Contains(ActivatedAbility.TapToLoseLife))
                                {
                                    GameManager.Instance.TapToLoseLife(creature);
                                    GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();
                                }

                                // TAP TO CREATE MINER
                                if (creature.activatedAbilities.Contains(ActivatedAbility.TapToCreateToken))
                                {
                                    int cost = creature.manaToPayToActivate;

                                    if (EnsureManaForCost(ai, new Dictionary<string, int> { {"Colorless", cost} }))
                                    {
                                        ai.ColoredMana.Pay(new Dictionary<string, int> { {"Colorless", cost} });

                                        creature.isTapped = true;

                                        string tokenName = creature.tokenToCreate;
                                        Card token = CardFactory.Create(tokenName);
                                        if (token != null)
                                        {
                                            GameManager.Instance.SummonToken(token, ai);
                                            Debug.Log($"AI created a {tokenName} token.");
                                        }
                                        else
                                        {
                                            Debug.LogError($"AI failed to create token: {tokenName}");
                                        }

                                        GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();
                                    }
                                    else
                                    {
                                        Debug.Log($"AI can't create token — not enough total mana ({ai.ColoredMana.Total()}/{cost}).");
                                    }
                                }

                            if (creature.activatedAbilities.Contains(ActivatedAbility.PayToGainAbility) &&
                                !creature.keywordAbilities.Contains(creature.abilityToGain))
                            {
                                int cost = creature.manaToPayToActivate;
                                string color = creature.PrimaryColor;
                                var abilityCost = new Dictionary<string, int>();

                                if (!string.IsNullOrEmpty(color) && color != "Artifact")
                                {
                                    abilityCost[color] = 1;
                                    if (cost > 1)
                                        abilityCost["Colorless"] = cost - 1;
                                }
                                else
                                {
                                    abilityCost["Colorless"] = cost;
                                }

                                if (EnsureManaForCost(ai, abilityCost))
                                {
                                    GameManager.Instance.PayToGainAbility(creature);
                                    GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();
                                }
                            }
                            }
                        }

                        foreach (var card in ai.Battlefield.ToList()) // .ToList() because we may remove during iteration
                        {
                            if (card is ArtifactCard artifact && !artifact.isTapped)
                            {
                                if (artifact.activatedAbilities.Contains(ActivatedAbility.TapToGainLife))
                                {
                                    artifact.isTapped = true;
                                    GameManager.Instance.TryGainLife(ai, 1);
                                    Debug.Log($"AI taps {artifact.cardName} to gain 1 life.");
                                    GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                }
                                else if (artifact.activatedAbilities.Contains(ActivatedAbility.TapToPlague))
                                {
                                    artifact.isTapped = true;
                                    GameManager.Instance.humanPlayer.Life -= artifact.plagueAmount;
                                    GameManager.Instance.aiPlayer.Life -= artifact.plagueAmount;
                                    GameManager.Instance.CheckForGameEnd();
                                    GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                    GameManager.Instance.UpdateUI();
                                }
                                else if (artifact.activatedAbilities.Contains(ActivatedAbility.SacrificeForLife))
                                {
                                    var abilityCost = new Dictionary<string, int> { {"Colorless", artifact.manaToPayToActivate} };
                                    if (EnsureManaForCost(ai, abilityCost))
                                    {
                                        ai.ColoredMana.Pay(abilityCost);
                                        GameManager.Instance.TryGainLife(ai, artifact.lifeToGain);
                                        artifact.isTapped = true;
                                        GameManager.Instance.SendToGraveyard(artifact, ai);
                                        Debug.Log($"AI sacrifices {artifact.cardName} to gain {artifact.lifeToGain} life.");
                                        GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                        GameManager.Instance.UpdateUI();
                                    }
                                }
                                else if (artifact.activatedAbilities.Contains(ActivatedAbility.SacrificeToDrawCards))
                                {
                                    var abilityCost = new Dictionary<string, int> { {"Colorless", artifact.manaToPayToActivate} };
                                    if (EnsureManaForCost(ai, abilityCost))
                                    {
                                        ai.ColoredMana.Pay(abilityCost);
                                        artifact.isTapped = true;

                                        for (int i = 0; i < artifact.cardsToDraw; i++)
                                        {
                                            GameManager.Instance.DrawCard(ai);
                                        }

                                        GameManager.Instance.SendToGraveyard(artifact, ai);
                                        Debug.Log($"AI sacrifices {artifact.cardName} to draw {artifact.cardsToDraw} card(s).");
                                        GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                        GameManager.Instance.UpdateUI();
                                    }
                                }
                            }
                        }

                        GameManager.Instance.UpdateUI(); // update UI after all actions
                        if (GameManager.Instance.isStackBusy)
                        {
                            Debug.Log("AI cast a sorcery — waiting... will not advance phase until resolved.");
                            return; // just wait
                        }

                        AdvancePhase();
                        break;
                    }
                    break;

                case TurnPhase.EnterCombat:
                    Debug.Log("→ Entering Combat.");
                    AdvancePhase();
                    break;

                case TurnPhase.ChooseAttackers:
                    if (currentPlayer == PlayerType.Human)
                    {
                        Debug.Log("→ Choose attackers.");
                        attackAllButton.gameObject.SetActive(true);
                        clearAttackersButton.gameObject.SetActive(true);
                        waitingForPlayerInput = true;
                        confirmAttackersButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("→ AI chooses attackers.");
                        GameManager.Instance.currentAttackers.Clear();

                        Player ai = GameManager.Instance.aiPlayer;
                        Player human = GameManager.Instance.humanPlayer;

                        var potentialAttackers = new List<CreatureCard>();

                        foreach (var card in ai.Battlefield)
                        {
                            if (card is CreatureCard creature &&
                                !creature.isTapped &&
                                !creature.hasSummoningSickness &&
                                !creature.keywordAbilities.Contains(KeywordAbility.Defender))
                            {
                                potentialAttackers.Add(creature);
                            }
                        }

                        int totalPower = potentialAttackers.Sum(c => c.power);

                        bool goForLethal = totalPower >= human.Life;

                        const int lowLifeThreshold = 5;
                        bool lowLifeNeedsDefense = ai.Life <= lowLifeThreshold &&
                            human.Battlefield.OfType<CreatureCard>().Any();

                        foreach (var creature in potentialAttackers)
                        {
                            bool attack = goForLethal;

                            if (!attack)
                            {
                                if (lowLifeNeedsDefense && !creature.keywordAbilities.Contains(KeywordAbility.Vigilance))
                                {
                                    attack = false; // stay back to block
                                }
                                else
                                {
                                    // Determine if creature can be profitably blocked
                                    var possibleBlockers = new List<CreatureCard>();
                                    foreach (var oppCard in human.Battlefield)
                                    {
                                        if (oppCard is CreatureCard blocker && BlockerCanBlockAttacker(blocker, creature, human))
                                            possibleBlockers.Add(blocker);
                                    }

                                    if (possibleBlockers.Count == 0)
                                    {
                                        attack = true; // Unblockable
                                    }
                                    else
                                    {
                                        // Pick the cheapest blocker (by power+toughness) that can block
                                        var best = possibleBlockers.OrderBy(b => b.power + b.baseToughness).First();

                                        if (creature.power >= best.toughness)
                                        {
                                            if (creature.toughness > best.power)
                                            {
                                                attack = true; // kill and survive
                                            }
                                            else
                                            {
                                                int creatureValue = creature.power + creature.baseToughness;
                                                int blockerValue = best.power + best.baseToughness;
                                                if (creatureValue <= blockerValue)
                                                    attack = true; // acceptable trade
                                            }
                                        }
                                    }
                                }
                            }

                            if (attack)
                            {
                                if (!creature.keywordAbilities.Contains(KeywordAbility.Vigilance))
                                    creature.isTapped = true;

                                GameManager.Instance.currentAttackers.Add(creature);
                                GameManager.Instance.FindCardVisual(creature)?.swordIcon?.SetActive(true);
                                Debug.Log($"AI declares attacker: {creature.cardName}");
                                GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();
                            }
                        }

                        AdvancePhase(); // skip straight to ConfirmAttackers (AI skips confirm)
                    }
                    break;

                case TurnPhase.ConfirmAttackers:
                    if (currentPlayer == PlayerType.Human)
                    {
                        if (waitingForPlayerInput)
                        {
                            Debug.Log("→ Confirm attackers.");
                            confirmAttackersButton.gameObject.SetActive(true);
                        }
                        else
                        {
                            AdvancePhase();
                        }
                    }
                    else
                    {
                        Debug.Log("→ Skipping attacker confirmation (AI turn).");
                        AdvancePhase();
                    }
                    break;

                case TurnPhase.ChooseBlockers:
                    if (GameManager.Instance.currentAttackers.Count == 0)
                    {
                        Debug.Log("→ No attackers. Skipping blockers phase.");
                        waitingForPlayerInput = false;
                        confirmBlockersButton.gameObject.SetActive(false);
                        AdvancePhase();
                        break;
                    }

                    if (currentPlayer == PlayerType.Human)
                    {
                        Debug.Log("→ AI is assigning blockers as defender.");

                        Player ai = GameManager.Instance.aiPlayer;
                        Player human = GameManager.Instance.humanPlayer;
                        var attackers = GameManager.Instance.currentAttackers.OrderByDescending(a => a.power).ToList();
                        var availableBlockers = new List<CreatureCard>();
                        int remainingDamage = attackers.Sum(a => a.power);
                        int projectedLife = ai.Life;

                        // Gather untapped blockers that can block
                        foreach (var card in ai.Battlefield)
                        {
                            if (card is CreatureCard c &&
                                !c.isTapped &&
                                !c.keywordAbilities.Contains(KeywordAbility.CantBlock))
                            {
                                availableBlockers.Add(c);
                            }
                        }

                        // Assign blockers to attackers prioritizing survival/trades
                        foreach (var attacker in attackers)
                        {
                            var blocker = ChooseBestBlocker(attacker, availableBlockers, projectedLife, remainingDamage);
                            if (blocker != null)
                            {
                                if (!GameManager.Instance.blockingAssignments.ContainsKey(attacker))
                                    GameManager.Instance.blockingAssignments[attacker] = new List<CreatureCard>();
                                GameManager.Instance.blockingAssignments[attacker].Add(blocker);
                                blocker.blockingThisAttacker = attacker;
                                attacker.blockedByThisBlocker.Add(blocker);

                                Debug.Log($"AI blocks {attacker.cardName} with {blocker.cardName}");

                                availableBlockers.Remove(blocker);
                                remainingDamage -= attacker.power;
                            }
                            else
                            {
                                projectedLife -= attacker.power;
                                remainingDamage -= attacker.power;
                            }
                        }

                        AdvancePhase(); // Proceed to ConfirmBlockers (or Damage)
                    }
                    else
                    {
                        Debug.Log("→ Player chooses blockers.");
                        waitingForPlayerInput = true;
                        confirmBlockersButton.gameObject.SetActive(true);
                    }
                    break;

                case TurnPhase.ConfirmBlockers:
                    if (currentPlayer == PlayerType.AI)
                    {
                        if (waitingForPlayerInput)
                        {
                            Debug.Log("→ Player confirms blockers.");
                            confirmBlockersButton.gameObject.SetActive(true);
                        }
                        else
                        {
                            AdvancePhase();
                        }
                    }
                    else
                    {
                        Debug.Log("→ Skipping blocker confirmation (Player turn).");
                        AdvancePhase();
                    }
                    break;

                case TurnPhase.Damage:
                    Debug.Log("→ Resolving combat damage.");
                    if (damageCoroutine != null)
                    {
                        StopCoroutine(damageCoroutine);
                        damageCoroutine = null;
                    }

                    damageCoroutine = StartCoroutine(WaitToShowCombatDamage());
                    break;

                case TurnPhase.EndTurn:
                    Debug.Log("→ Ending turn.");

                    // Heal all creatures
                    GameManager.Instance.ResetDamage(GameManager.Instance.humanPlayer);
                    GameManager.Instance.ResetDamage(GameManager.Instance.aiPlayer);

                    // Reference the correct player before swapping turn
                    Player endingPlayer = currentPlayer == PlayerType.Human
                        ? GameManager.Instance.humanPlayer
                        : GameManager.Instance.aiPlayer;

                    // Remove temporary keyword abilities
                    foreach (var card in endingPlayer.Battlefield)
                    {
                        if (card is CreatureCard creature)
                        {
                            if (creature.activatedAbilities != null &&
                                creature.activatedAbilities.Contains(ActivatedAbility.PayToGainAbility))
                            {
                                if (creature.keywordAbilities.Contains(creature.abilityToGain))
                                {
                                    creature.keywordAbilities.Remove(creature.abilityToGain);
                                    Debug.Log($"{creature.cardName} loses {creature.abilityToGain} at end of turn.");
                                }
                            }

                            if (creature.temporaryKeywordAbilities.Count > 0)
                            {
                                foreach (var temp in new List<KeywordAbility>(creature.temporaryKeywordAbilities))
                                {
                                    if (creature.keywordAbilities.Contains(temp))
                                    {
                                        creature.keywordAbilities.Remove(temp);
                                        Debug.Log($"{creature.cardName} loses {temp} at end of turn.");
                                    }
                                }

                                creature.temporaryKeywordAbilities.Clear();

                                var visual = GameManager.Instance.FindCardVisual(card);
                                if (visual != null)
                                    visual.UpdateVisual();
                            }
                        }
                    }

                    // Only after cleanup, begin next turn
                    if (endingPlayer.extraTurns > 0)
                    {
                        endingPlayer.extraTurns--;
                        Debug.Log($"{currentPlayer} gets an extra turn.");
                        BeginTurn(currentPlayer);
                    }
                    else
                    {
                        BeginTurn(currentPlayer == PlayerType.Human ? PlayerType.AI : PlayerType.Human);
                    }
                    break;
            }
        }

        private IEnumerator WaitAndAdvancePhase()
            {
                yield return new WaitUntil(() => !GameManager.Instance.isStackBusy);
                
                // Wait a frame to ensure any triggered UI changes or effects have finished
                yield return null;

                AdvancePhase(); // <-- This must always be called
            }
        
        private bool IsLandwalkPreventingBlock(CreatureCard attacker, Player defender)
            {
                foreach (var ability in attacker.keywordAbilities)
                {
                    if (ability == KeywordAbility.Plainswalk &&
                        defender.Battlefield.Any(card => card is LandCard land && land.cardName.ToLower().Contains("plains")))
                        return true;
                    if (ability == KeywordAbility.Islandwalk &&
                        defender.Battlefield.Any(card => card is LandCard land && land.cardName.ToLower().Contains("island")))
                        return true;
                    if (ability == KeywordAbility.Swampwalk &&
                        defender.Battlefield.Any(card => card is LandCard land && land.cardName.ToLower().Contains("swamp")))
                        return true;
                    if (ability == KeywordAbility.Mountainwalk &&
                        defender.Battlefield.Any(card => card is LandCard land && land.cardName.ToLower().Contains("mountain")))
                        return true;
                    if (ability == KeywordAbility.Forestwalk &&
                        defender.Battlefield.Any(card => card is LandCard land && land.cardName.ToLower().Contains("forest")))
                        return true;
                }
                return false;
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

        private bool BlockerCanBlockAttacker(CreatureCard blocker, CreatureCard attacker, Player defender)
            {
                if (blocker.isTapped) return false;
                if (blocker.keywordAbilities.Contains(KeywordAbility.CantBlock)) return false;

                if (attacker.keywordAbilities.Contains(KeywordAbility.Flying) &&
                    !blocker.keywordAbilities.Contains(KeywordAbility.Flying) &&
                    !blocker.keywordAbilities.Contains(KeywordAbility.Reach))
                    return false;

                if (blocker.keywordAbilities.Contains(KeywordAbility.CanOnlyBlockFlying) &&
                    !attacker.keywordAbilities.Contains(KeywordAbility.Flying))
                    return false;

                if (IsLandwalkPreventingBlock(attacker, defender))
                    return false;

                if (blocker.color.Any(c => attacker.keywordAbilities.Contains(GetProtectionKeyword(c))))
                    return false;

                return true;
            }

        private CreatureCard ChooseBestBlocker(CreatureCard attacker, List<CreatureCard> candidates, int remainingLife, int remainingDamage)
            {
                var possible = candidates.Where(b => BlockerCanBlockAttacker(b, attacker, GameManager.Instance.aiPlayer)).ToList();
                if (possible.Count == 0) return null;

                // Prefer blocker that kills attacker and survives
                var killSurvive = possible
                    .Where(b => b.power >= attacker.toughness && b.toughness > attacker.power)
                    .OrderBy(b => b.power)
                    .FirstOrDefault();
                if (killSurvive != null) return killSurvive;

                // Safe block that survives damage
                var safe = possible
                    .Where(b => b.toughness > attacker.power)
                    .OrderBy(b => b.toughness)
                    .FirstOrDefault();
                if (safe != null) return safe;

                // Trade if it can kill attacker
                var trade = possible
                    .Where(b => b.power >= attacker.toughness)
                    .OrderBy(b => b.power + b.baseToughness)
                    .FirstOrDefault();
                if (trade != null) return trade;

                // Block anything to avoid lethal damage
                if (remainingDamage >= remainingLife)
                    return possible.OrderByDescending(b => b.toughness).First();

                return null;
            }
        
        public void ContinueAIAfterStack()
            {
                if (currentPlayer == PlayerType.AI)
                {
                    Debug.Log("AI stack resolved — resuming AI turn.");
                    RunCurrentPhase();
                }
            }

        public void RunSpecificPhase(TurnPhase phase)
            {
                currentPhase = phase;
                RunCurrentPhase();
            }
        
        private int SpendFromPool(ref int pool, int needed)
            {
                int spent = Mathf.Min(pool, needed);
                pool -= spent;
                return spent;
            }
        
        private IEnumerator WaitToShowCombatDamage()
            {
                yield return StartCoroutine(GameManager.Instance.ResolveCombatWithAnimations());

                foreach (var visual in GameManager.Instance.activeCardVisuals)
                {
                    if (visual.swordIcon != null)
                        visual.swordIcon.SetActive(false);

                    visual.UpdateVisual();
                }

                AdvancePhase();


                damageCoroutine = null;
            }

        private Player.ManaPool GetPotentialManaPool(Player ai)
            {
                Player.ManaPool pool = new Player.ManaPool();
                pool.White = ai.ColoredMana.White;
                pool.Blue = ai.ColoredMana.Blue;
                pool.Black = ai.ColoredMana.Black;
                pool.Red = ai.ColoredMana.Red;
                pool.Green = ai.ColoredMana.Green;
                pool.Colorless = ai.ColoredMana.Colorless;

                foreach (var card in ai.Battlefield)
                {
                    if (card is LandCard land && !land.isTapped)
                    {
                        var colors = CardDatabase.GetCardData(land.cardName).color;
                        string color = (colors != null && colors.Count > 0) ? colors[0] : "Colorless";
                        switch (color)
                        {
                            case "White": pool.White++; break;
                            case "Blue": pool.Blue++; break;
                            case "Black": pool.Black++; break;
                            case "Red": pool.Red++; break;
                            case "Green": pool.Green++; break;
                            default: pool.Colorless++; break;
                        }
                    }
                    else if (!card.isTapped && (card is CreatureCard c && c.activatedAbilities.Contains(ActivatedAbility.TapForMana)))
                    {
                        pool.Colorless++;
                    }
                    else if (!card.isTapped && (card is ArtifactCard a && a.activatedAbilities.Contains(ActivatedAbility.TapForMana)))
                    {
                        pool.Colorless++;
                    }
                    else if (!card.isTapped && (card is ArtifactCard a2 && a2.activatedAbilities.Contains(ActivatedAbility.TapAndSacrificeForMana)))
                    {
                        pool.Colorless++;
                    }
                }

                return pool;
            }

        private bool TapLandForColor(Player ai, string color)
            {
                foreach (var card in ai.Battlefield)
                {
                    if (card is LandCard land && !land.isTapped)
                    {
                        var colors = CardDatabase.GetCardData(land.cardName).color;
                        string landColor = (colors != null && colors.Count > 0) ? colors[0] : "Colorless";
                        if (landColor == color)
                        {
                            GameManager.Instance.TapLandForMana(land, ai);
                            GameManager.Instance.FindCardVisual(land)?.UpdateVisual();
                            return true;
                        }
                    }
                }
                return false;
            }

        private bool TapAnyManaSource(Player ai)
            {
                foreach (var card in ai.Battlefield)
                {
                    if (card is LandCard land && !land.isTapped)
                    {
                        GameManager.Instance.TapLandForMana(land, ai);
                        GameManager.Instance.FindCardVisual(land)?.UpdateVisual();
                        return true;
                    }
                }

                foreach (var card in ai.Battlefield)
                {
                    if (card is CreatureCard creature && !creature.isTapped && creature.activatedAbilities.Contains(ActivatedAbility.TapForMana))
                    {
                        creature.isTapped = true;
                        ai.ColoredMana.Colorless += 1;
                        GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();
                        return true;
                    }
                    if (card is ArtifactCard artifact && !artifact.isTapped && artifact.activatedAbilities.Contains(ActivatedAbility.TapForMana))
                    {
                        artifact.isTapped = true;
                        ai.ColoredMana.Colorless += 1;
                        GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                        return true;
                    }
                    if (card is ArtifactCard artifact2 && !artifact2.isTapped && artifact2.activatedAbilities.Contains(ActivatedAbility.TapAndSacrificeForMana))
                    {
                        artifact2.isTapped = true;
                        ai.ColoredMana.Colorless += 1;
                        GameManager.Instance.SendToGraveyard(artifact2, ai);
                        GameManager.Instance.FindCardVisual(artifact2)?.UpdateVisual();
                        return true;
                    }
                }

                return false;
            }

        private bool EnsureManaForCost(Player ai, Dictionary<string, int> cost)
            {
                var potential = GetPotentialManaPool(ai);
                if (!potential.CanPay(cost))
                    return false;

                int needWhite = cost.ContainsKey("White") ? cost["White"] : 0;
                while (ai.ColoredMana.White < needWhite)
                {
                    if (!TapLandForColor(ai, "White")) return false;
                }

                int needBlue = cost.ContainsKey("Blue") ? cost["Blue"] : 0;
                while (ai.ColoredMana.Blue < needBlue)
                {
                    if (!TapLandForColor(ai, "Blue")) return false;
                }

                int needBlack = cost.ContainsKey("Black") ? cost["Black"] : 0;
                while (ai.ColoredMana.Black < needBlack)
                {
                    if (!TapLandForColor(ai, "Black")) return false;
                }

                int needRed = cost.ContainsKey("Red") ? cost["Red"] : 0;
                while (ai.ColoredMana.Red < needRed)
                {
                    if (!TapLandForColor(ai, "Red")) return false;
                }

                int needGreen = cost.ContainsKey("Green") ? cost["Green"] : 0;
                while (ai.ColoredMana.Green < needGreen)
                {
                    if (!TapLandForColor(ai, "Green")) return false;
                }

                int totalCost = cost.Values.Sum();
                while (ai.ColoredMana.Total() < totalCost)
                {
                    if (!TapAnyManaSource(ai))
                        break;
                }

                return ai.ColoredMana.CanPay(cost);
            }
        
        public void SelectAllEligibleAttackers()
            {
                GameManager.Instance.selectedAttackers.Clear();
                bool anyDeclared = false;

                foreach (var card in GameManager.Instance.humanPlayer.Battlefield)
                {
                    if (card is CreatureCard creature &&
                        !creature.isTapped &&
                        (!creature.hasSummoningSickness || creature.keywordAbilities.Contains(KeywordAbility.Haste)) &&
                        !creature.keywordAbilities.Contains(KeywordAbility.Defender))
                    {
                        GameManager.Instance.selectedAttackers.Add(creature);
                        anyDeclared = true;

                        // Tap the creature unless it has Vigilance
                        if (!creature.keywordAbilities.Contains(KeywordAbility.Vigilance))
                            creature.isTapped = true;

                        var visual = GameManager.Instance.FindCardVisual(creature);
                        if (visual != null)
                        {
                            if (visual.swordIcon != null)
                                visual.swordIcon.SetActive(true);
                            visual.UpdateVisual();
                        }
                    }
                }

                if (anyDeclared)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.declareAttack); // Or use .attack if that's your clip name
                }
            }

        public void ClearAllSelectedAttackers()
            {
                foreach (var creature in GameManager.Instance.selectedAttackers)
                {
                    creature.isTapped = false;
                }

                // Now clear the list BEFORE updating visuals
                var toClear = GameManager.Instance.selectedAttackers.ToList();
                GameManager.Instance.selectedAttackers.Clear();

                foreach (var creature in toClear)
                {
                    var visual = GameManager.Instance.FindCardVisual(creature);
                    if (visual != null)
                    {
                        if (visual.swordIcon != null)
                            visual.swordIcon.SetActive(false);

                        visual.UpdateVisual();
                    }
                }
            }
}