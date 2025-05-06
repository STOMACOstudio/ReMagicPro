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
                RunCurrentPhase();
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
                Debug.Log($"[UI] Player clicked Next Phase");
                waitingForPlayerInput = false;
                HideAllConfirmButtons();
                AdvancePhase();
            }
        }

    public void ConfirmAttackers()
        {
            if (waitingForPlayerInput)
            {
                Debug.Log("[UI] Player confirmed attackers");
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
                Debug.Log("[UI] Player confirmed blockers");
                waitingForPlayerInput = false;
                confirmBlockersButton.gameObject.SetActive(false);
                AdvancePhase();
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
            // Empty mana at the end of each phase
            var player = currentPlayer == PlayerType.Human
                ? GameManager.Instance.humanPlayer
                : GameManager.Instance.aiPlayer;

            player.ManaPool = 0;
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
                Debug.Log("Stack is busy — AI is waiting.");
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
                            if (ability.timing == TriggerTiming.OnUpkeep)
                            {
                                Debug.Log($"[Upkeep Trigger] {card.cardName} triggers OnUpkeep.");
                                ability.effect?.Invoke(player);
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
                                    //ai.Battlefield.Add(land);

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

                        Debug.Log($"AI mana pool: {ai.ManaPool}");

                        // Handle all artifact abilities that produce mana
                        foreach (var card in ai.Battlefield.ToList())
                        {
                            if (card is ArtifactCard artifact && !artifact.isTapped)
                            {
                                // Tap: Add 1 mana
                                if (artifact.activatedAbilities.Contains(ActivatedAbility.TapForMana))
                                {
                                    artifact.isTapped = true;
                                    ai.ManaPool++;
                                    Debug.Log($"AI taps {artifact.cardName} for 1 mana.");
                                    GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                }

                                // Tap and Sacrifice: Add 1 mana
                                else if (artifact.activatedAbilities.Contains(ActivatedAbility.TapAndSacrificeForMana))
                                {
                                    artifact.isTapped = true;
                                    ai.ManaPool++;
                                    GameManager.Instance.SendToGraveyard(artifact, ai);
                                    GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                    Debug.Log($"AI taps and sacrifices {artifact.cardName} for 1 mana.");
                                }

                                // Pay mana, Tap and Sacrifice: Add N mana
                                else if (artifact.activatedAbilities.Contains(ActivatedAbility.SacrificeForMana))
                                {
                                    if (ai.ManaPool >= artifact.manaToPayToActivate)
                                    {
                                        ai.ManaPool -= artifact.manaToPayToActivate;
                                        artifact.isTapped = true;
                                        ai.ManaPool += artifact.manaToGain;
                                        GameManager.Instance.SendToGraveyard(artifact, ai);
                                        GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                        Debug.Log($"AI pays {artifact.manaToPayToActivate}, taps and sacrifices {artifact.cardName} to gain {artifact.manaToGain} mana.");
                                    }
                                    else
                                    {
                                        Debug.Log($"AI can't activate {artifact.cardName}: not enough mana ({ai.ManaPool}/{artifact.manaToPayToActivate}).");
                                    }
                                }
                            }
                        }

                        Debug.Log($"AI mana pool: {ai.ManaPool}");

                        // Play as many cards as AI can afford
                        bool playedCard = true;

                        while (playedCard && !GameManager.Instance.isStackBusy)
                        {
                            playedCard = false;

                            for (int i = 0; i < ai.Hand.Count; i++)
                            {
                                Card card = ai.Hand[i];
                                
                                if (card is CreatureCard creature && ai.ManaPool >= creature.manaCost)
                                {
                                    ai.ManaPool -= creature.manaCost;
                                    ai.Hand.Remove(card);
                                    ai.Battlefield.Add(card);
                                    card.OnEnterPlay(ai);

                                    if (card.entersTapped)
                                    {
                                        card.isTapped = true;
                                        Debug.Log($"{card.cardName} (AI) enters tapped.");
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
                                else if (card is SorceryCard sorcery && ai.ManaPool >= sorcery.manaCost)
                                {
                                    GameObject obj = GameObject.Instantiate(GameManager.Instance.cardPrefab, GameManager.Instance.stackZone);
                                    CardVisual visual = obj.GetComponent<CardVisual>();
                                    visual.Setup(sorcery, GameManager.Instance);
                                    GameManager.Instance.activeCardVisuals.Add(visual);

                                    GameManager.Instance.PlayCard(ai, visual);
                                    Debug.Log($"AI cast sorcery: {sorcery.cardName}");

                                    playedCard = true;
                                    break;
                                }
                                else if (card is ArtifactCard artifact && ai.ManaPool >= artifact.manaCost)
                                {
                                    ai.ManaPool -= artifact.manaCost;
                                    ai.Hand.Remove(card);
                                    ai.Battlefield.Add(card);
                                    card.OnEnterPlay(ai);

                                    if (card.entersTapped)
                                    {
                                        card.isTapped = true;
                                        Debug.Log($"{card.cardName} (AI) enters tapped.");
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
                                else if (creature.activatedAbilities.Contains(ActivatedAbility.TapToCreateToken))
                                {
                                    int cost = creature.manaToPayToActivate;
                                    if (ai.ManaPool >= cost)
                                    {
                                        ai.ManaPool -= cost;
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
                                }

                                //GAIN ABILITY
                                if (creature.activatedAbilities.Contains(ActivatedAbility.PayToGainAbility) &&
                                    !creature.keywordAbilities.Contains(creature.abilityToGain) &&
                                    ai.ManaPool >= creature.manaToPayToActivate)
                                {
                                    GameManager.Instance.PayToGainAbility(creature);
                                    GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();
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
                                    Debug.Log($"AI taps {artifact.cardName}: Both players lose {artifact.plagueAmount} life.");
                                    GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                    GameManager.Instance.UpdateUI();
                                }
                                else if (artifact.activatedAbilities.Contains(ActivatedAbility.SacrificeForLife) &&
                                        ai.ManaPool >= artifact.manaToPayToActivate)
                                {
                                    ai.ManaPool -= artifact.manaToPayToActivate;
                                    ai.Life += artifact.lifeToGain;
                                    artifact.isTapped = true;
                                    GameManager.Instance.SendToGraveyard(artifact, ai);
                                    Debug.Log($"AI sacrifices {artifact.cardName} to gain {artifact.lifeToGain} life.");
                                    GameManager.Instance.FindCardVisual(artifact)?.UpdateVisual();
                                    GameManager.Instance.UpdateUI();
                                }
                                else if (artifact.activatedAbilities.Contains(ActivatedAbility.SacrificeToDrawCards) &&
                                        ai.ManaPool >= artifact.manaToPayToActivate)
                                {
                                    ai.ManaPool -= artifact.manaToPayToActivate;
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
                            Debug.Log("AI cast a sorcery — waiting for resolution before advancing phase.");
                            StartCoroutine(WaitAndAdvancePhase());
                        }
                        else
                        {
                            AdvancePhase();
                        }
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
                                
                                // LANDWALK rule: attacker is unblockable if AI controls matching land
                                if (IsLandwalkPreventingBlock(attacker, ai))
                                {
                                    Debug.Log($"AI can't block {attacker.cardName} due to landwalk.");
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
                    GameManager.Instance.ResolveCombat();
                    AdvancePhase();
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
                AdvancePhase();
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

}