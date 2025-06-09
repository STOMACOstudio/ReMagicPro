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

    public TurnPhase lastPhaseBeforeStack;
    public bool waitingToResumeAI = false;

    private Coroutine damageCoroutine;

    void Start()
        {
            Instance = this;

            nextPhaseButton.onClick.AddListener(NextPhaseButton);
            confirmAttackersButton.onClick.AddListener(ConfirmAttackers);
            confirmBlockersButton.onClick.AddListener(ConfirmBlockers);

            confirmAttackersButton.gameObject.SetActive(false);
            confirmBlockersButton.gameObject.SetActive(false);

            BeginTurn(PlayerType.Human);
        }

    void Update()
            {
                if (currentPlayer == PlayerType.AI && !waitingForPlayerInput && !GameManager.Instance.isStackBusy)
                {
                    RunCurrentPhase(); // this alone is enough
                }

                // Update next phase button dynamically
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
                                ability.effect.Invoke(player, null);
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
                        
                        // Tap all untapped lands
                        foreach (var card in ai.Battlefield)
                        {
                            if (card is LandCard land && !land.isTapped)
                            {
                                GameManager.Instance.TapLandForMana(land, ai);
                                Debug.Log($"AI taps {land.cardName} for 1 mana.");
                                GameManager.Instance.FindCardVisual(land)?.UpdateVisual();
                            }
                        }

                        // Handle all artifact abilities that produce mana
                        foreach (var card in ai.Battlefield.ToList())
                        {
                            if (card is ArtifactCard artifact && !artifact.isTapped)
                            {
                                // Tap: Add 1 mana
                                if (artifact.activatedAbilities.Contains(ActivatedAbility.TapForMana))
                                {
                                    artifact.isTapped = true;
                                    ai.ColoredMana.Colorless += 1;
                                    Debug.Log($"AI taps {artifact.cardName} for 1 colorless mana.");
                                    GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                }

                                // Tap and Sacrifice: Add 1 mana
                                else if (artifact.activatedAbilities.Contains(ActivatedAbility.TapAndSacrificeForMana))
                                {
                                    artifact.isTapped = true;
                                    ai.ColoredMana.Colorless += 1;
                                    GameManager.Instance.SendToGraveyard(artifact, ai);
                                    GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                    Debug.Log($"AI taps and sacrifices {artifact.cardName} for 1 colorless mana.");
                                }

                                // Pay mana, Tap and Sacrifice: Add N mana
                                else if (artifact.activatedAbilities.Contains(ActivatedAbility.SacrificeForMana))
                                {
                                    int cost = artifact.manaToPayToActivate;

                                    if (ai.ColoredMana.Total() >= cost)
                                    {
                                        int remaining = cost;

                                        // Spend colorless first
                                        remaining -= SpendFromPool(ref ai.ColoredMana.Colorless, remaining);

                                        // Then WUBRG
                                        remaining -= SpendFromPool(ref ai.ColoredMana.White, remaining);
                                        remaining -= SpendFromPool(ref ai.ColoredMana.Blue, remaining);
                                        remaining -= SpendFromPool(ref ai.ColoredMana.Black, remaining);
                                        remaining -= SpendFromPool(ref ai.ColoredMana.Red, remaining);
                                        remaining -= SpendFromPool(ref ai.ColoredMana.Green, remaining);

                                        if (remaining > 0)
                                        {
                                            Debug.Log($"AI can't activate {artifact.cardName}: not enough mana to pay {cost}.");
                                            return;
                                        }

                                        artifact.isTapped = true;
                                        ai.ColoredMana.Colorless += artifact.manaToGain;
                                        GameManager.Instance.SendToGraveyard(artifact, ai);
                                        GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                        Debug.Log($"AI pays {cost}, taps and sacrifices {artifact.cardName} to gain {artifact.manaToGain} colorless mana.");
                                    }
                                    else
                                    {
                                        Debug.Log($"AI can't activate {artifact.cardName}: not enough total mana.");
                                    }
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
                                
                                if (card is CreatureCard creature)
                                {
                                    var cost = GameManager.Instance.GetManaCostBreakdown(creature.manaCost, creature.color);
                                    if (ai.ColoredMana.CanPay(cost))
                                    {
                                        ai.ColoredMana.Pay(cost);
                                        ai.Hand.Remove(card);
                                        ai.Battlefield.Add(card);
                                        card.OnEnterPlay(ai);

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
                                                            (ability.requiredTargetType == SorceryCard.TargetType.Creature && c is CreatureCard) ||
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
                                    if (ai.ColoredMana.CanPay(cost))
                                    {
                                        if (sorcery.requiredTargetType == SorceryCard.TargetType.Creature &&
                                            sorcery.destroyTargetIfTypeMatches)
                                        {
                                            Player opponent = GameManager.Instance.GetOpponentOf(ai);

                                            // Pick enemy creature with the highest mana cost
                                            var target = opponent.Battlefield
                                                .OfType<CreatureCard>()
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
                                    if (ai.ColoredMana.CanPay(cost))
                                    {
                                        ai.ColoredMana.Pay(cost);
                                        ai.Hand.Remove(card);
                                        ai.Battlefield.Add(card);
                                        card.OnEnterPlay(ai);

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

                                    if (ai.ColoredMana.Total() >= cost)
                                    {
                                        int remaining = cost;

                                        // Spend colorless first
                                        remaining -= SpendFromPool(ref ai.ColoredMana.Colorless, remaining);

                                        // Spend from WUBRG
                                        remaining -= SpendFromPool(ref ai.ColoredMana.White, remaining);
                                        remaining -= SpendFromPool(ref ai.ColoredMana.Blue, remaining);
                                        remaining -= SpendFromPool(ref ai.ColoredMana.Black, remaining);
                                        remaining -= SpendFromPool(ref ai.ColoredMana.Red, remaining);
                                        remaining -= SpendFromPool(ref ai.ColoredMana.Green, remaining);

                                        if (remaining > 0)
                                        {
                                            Debug.Log($"AI can't create token — not enough mana to pay {cost}.");
                                            continue;
                                        }

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
                                string color = creature.color;

                                bool canActivate = false;

                                if (!string.IsNullOrEmpty(color) && color != "Artifact")
                                {
                                    int colorAvailable = color switch
                                    {
                                        "White" => ai.ColoredMana.White,
                                        "Blue" => ai.ColoredMana.Blue,
                                        "Black" => ai.ColoredMana.Black,
                                        "Red" => ai.ColoredMana.Red,
                                        "Green" => ai.ColoredMana.Green,
                                        _ => 0
                                    };

                                    int genericNeeded = cost - 1;
                                    int totalGeneric = ai.ColoredMana.Total() - colorAvailable;

                                    canActivate = (colorAvailable >= 1) && (totalGeneric >= genericNeeded);
                                }
                                else // Colorless/generic ability
                                {
                                    canActivate = ai.ColoredMana.Total() >= cost;
                                }

                                if (canActivate)
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
                                    ai.Life += 1;
                                    Debug.Log($"AI taps {artifact.cardName} to gain 1 life.");
                                    GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                    GameManager.Instance.UpdateUI();
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
                                else if (artifact.activatedAbilities.Contains(ActivatedAbility.SacrificeForLife) &&
                                        ai.ColoredMana.Total() >= artifact.manaToPayToActivate)
                                {
                                    int remaining = artifact.manaToPayToActivate;

                                    remaining -= SpendFromPool(ref ai.ColoredMana.Colorless, remaining);
                                    remaining -= SpendFromPool(ref ai.ColoredMana.White, remaining);
                                    remaining -= SpendFromPool(ref ai.ColoredMana.Blue, remaining);
                                    remaining -= SpendFromPool(ref ai.ColoredMana.Black, remaining);
                                    remaining -= SpendFromPool(ref ai.ColoredMana.Red, remaining);
                                    remaining -= SpendFromPool(ref ai.ColoredMana.Green, remaining);

                                    if (remaining > 0)
                                    {
                                        Debug.Log($"AI can't activate {artifact.cardName} — not enough mana.");
                                        return;
                                    }

                                    ai.Life += artifact.lifeToGain;
                                    artifact.isTapped = true;
                                    GameManager.Instance.SendToGraveyard(artifact, ai);
                                    Debug.Log($"AI sacrifices {artifact.cardName} to gain {artifact.lifeToGain} life.");
                                    GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                    GameManager.Instance.UpdateUI();
                                }
                                else if (artifact.activatedAbilities.Contains(ActivatedAbility.SacrificeToDrawCards) &&
                                        ai.ColoredMana.Total() >= artifact.manaToPayToActivate)
                                {
                                    int remaining = artifact.manaToPayToActivate;

                                    remaining -= SpendFromPool(ref ai.ColoredMana.Colorless, remaining);
                                    remaining -= SpendFromPool(ref ai.ColoredMana.White, remaining);
                                    remaining -= SpendFromPool(ref ai.ColoredMana.Blue, remaining);
                                    remaining -= SpendFromPool(ref ai.ColoredMana.Black, remaining);
                                    remaining -= SpendFromPool(ref ai.ColoredMana.Red, remaining);
                                    remaining -= SpendFromPool(ref ai.ColoredMana.Green, remaining);

                                    if (remaining > 0)
                                    {
                                        Debug.Log($"AI can't activate {artifact.cardName} — not enough mana.");
                                        return;
                                    }

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
                        waitingForPlayerInput = true;
                        confirmAttackersButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("→ AI chooses attackers.");
                        GameManager.Instance.currentAttackers.Clear();

                        foreach (var card in GameManager.Instance.aiPlayer.Battlefield)
                        {
                            if (card is CreatureCard creature &&
                                !creature.isTapped &&
                                !creature.hasSummoningSickness &&
                                !creature.keywordAbilities.Contains(KeywordAbility.Defender))
                            {
                                if (!creature.keywordAbilities.Contains(KeywordAbility.Vigilance)) {
                                    creature.isTapped = true;
                                }

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
                    if (currentPlayer == PlayerType.Human)
                    {
                        Debug.Log("→ AI is assigning blockers as defender.");

                        Player ai = GameManager.Instance.aiPlayer;
                        Player human = GameManager.Instance.humanPlayer;
                        var attackers = GameManager.Instance.currentAttackers;
                        var availableBlockers = new List<CreatureCard>();

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

                        // Assign blockers to attackers
                        foreach (var attacker in attackers)
                        {
                            foreach (var blocker in availableBlockers)
                            {
                                // FLYING rule: can only block flying if blocker has Flying or Reach
                                if (attacker.keywordAbilities.Contains(KeywordAbility.Flying) &&
                                    !blocker.keywordAbilities.Contains(KeywordAbility.Flying) &&
                                    !blocker.keywordAbilities.Contains(KeywordAbility.Reach))
                                {
                                    continue; // this blocker can't block this attacker
                                }

                                if (blocker.keywordAbilities.Contains(KeywordAbility.CanOnlyBlockFlying) &&
                                    !attacker.keywordAbilities.Contains(KeywordAbility.Flying))
                                {
                                    continue; // this blocker can't block non-flying creatures
                                }
                                
                                // LANDWALK rule: attacker is unblockable if AI controls matching land
                                if (IsLandwalkPreventingBlock(attacker, ai))
                                {
                                    Debug.Log($"AI can't block {attacker.cardName} due to landwalk.");
                                    continue;
                                }

                                // PROTECTION: attacker cannot be blocked by this blocker if it has protection from blocker's color
                                if (attacker.keywordAbilities.Contains(GetProtectionKeyword(blocker.color)))
                                {
                                    Debug.Log($"{attacker.cardName} has protection from {blocker.color}, so AI cannot assign {blocker.cardName} to block it.");
                                    continue;
                                }

                                GameManager.Instance.blockingAssignments[attacker] = blocker;
                                blocker.blockingThisAttacker = attacker;
                                attacker.blockedByThisBlocker = blocker;

                                Debug.Log($"AI blocks {attacker.cardName} with {blocker.cardName}");

                                availableBlockers.Remove(blocker);
                                break;
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
                        StopCoroutine(damageCoroutine);

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
                        if (card is CreatureCard creature &&
                            creature.activatedAbilities != null &&
                            creature.activatedAbilities.Contains(ActivatedAbility.PayToGainAbility))
                        {
                            if (creature.keywordAbilities.Contains(creature.abilityToGain))
                            {
                                creature.keywordAbilities.Remove(creature.abilityToGain);
                                var visual = GameManager.Instance.FindCardVisual(card);
                                if (visual != null)
                                    visual.UpdateVisual();
                                Debug.Log($"{creature.cardName} loses {creature.abilityToGain} at end of turn.");
                            }
                        }
                    }

                    // Only after cleanup, begin next turn
                    BeginTurn(currentPlayer == PlayerType.Human ? PlayerType.AI : PlayerType.Human);
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
                var (playerDamage, aiDamage) = GameManager.Instance.ResolveCombat();

                foreach (var visual in GameManager.Instance.activeCardVisuals)
                {
                    if (visual.swordIcon != null)
                        visual.swordIcon.SetActive(false);

                    visual.UpdateVisual();
                }

                if (playerDamage > 0)
                {
                    GameManager.Instance.ShowFloatingDamage(playerDamage, GameManager.Instance.playerLifeContainer);
                }

                if (aiDamage > 0)
                {
                    GameManager.Instance.ShowFloatingDamage(aiDamage, GameManager.Instance.enemyLifeContainer);
                }

                if (playerDamage > 0 || aiDamage > 0)
                    yield return new WaitForSeconds(1f);

                AdvancePhase();

                damageCoroutine = null;
            }
}