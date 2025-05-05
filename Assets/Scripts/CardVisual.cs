using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    public Card linkedCard; // which card it represents
    public GameManager gameManager; // which manager it talks to
    public CreatureCard blockingThisAttacker; // If blocking, who am I blocking
    public CreatureCard blockedByThisBlocker; // If attacker, who blocks me
    public LineRenderer lineRenderer;
    public Image artImage;

    public TMP_Text titleText;
    public TMP_Text sicknessText;
    public TMP_Text costText;
    public TMP_Text statsText;
    public TMP_Text keywordText;

    public bool isInBattlefield = false;

    void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

    public void UpdateVisual()
        {
            if (linkedCard.isTapped)
            {
                transform.rotation = Quaternion.Euler(0, 0, -90);
            }
            else
            {
                transform.rotation = Quaternion.identity;
            }

            if (!isInBattlefield)
            {
                lineRenderer.enabled = false;
                return; // don't do line logic at all if dead
            }

            if (linkedCard is CreatureCard creature)
            {
                // Show summoning sickness if creature is on the battlefield
                if (isInBattlefield && creature.hasSummoningSickness)
                {
                    sicknessText.text = "(@)";
                }
                else
                {
                    sicknessText.text = "";
                }

                statsText.text = $"{creature.power}/{creature.toughness}";

                List<string> keywords = new List<string>();

                if (creature.keywordAbilities.Contains(KeywordAbility.Haste))
                    keywords.Add("Haste");
                if (creature.keywordAbilities.Contains(KeywordAbility.Defender))
                    keywords.Add("Defender");
                if (creature.keywordAbilities.Contains(KeywordAbility.CantBlock))
                    keywords.Add("This creature can't block.");
                if (creature.keywordAbilities.Contains(KeywordAbility.Vigilance))
                    keywords.Add("Vigilance.");
                if (creature.keywordAbilities.Contains(KeywordAbility.Flying))
                    keywords.Add("Flying.");
                if (creature.entersTapped)
                    keywords.Add("This creature enters the battlefield tapped.");

                // Show keyword + triggered ability text together
                keywordText.text = linkedCard.GetCardText();
            }
            else
            {
                sicknessText.text = "";
                statsText.text = "";
                keywordText.text = linkedCard.GetCardText();
            }

            if (linkedCard is CreatureCard c && isInBattlefield)
            {
                if (c.blockingThisAttacker != null)
                {
                    lineRenderer.enabled = true;
                    lineRenderer.SetPosition(0, transform.position);
                    var attackerVisual = GameManager.Instance.FindCardVisual(c.blockingThisAttacker);
                    if (attackerVisual != null)
                        lineRenderer.SetPosition(1, attackerVisual.transform.position);
                }
                else
                {
                    lineRenderer.enabled = false;
                }
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }

    public void Setup(Card card, GameManager manager)
        {
            linkedCard = card;
            gameManager = manager;
            titleText.text = card.cardName;
            lineRenderer = GetComponent<LineRenderer>();
            artImage.sprite = linkedCard.artwork;

            sicknessText.text = ""; // Clear at start

            if (linkedCard is CreatureCard creature)
            {
                costText.text = creature.manaCost.ToString();
                int displayPower = creature.power;
                int displayToughness = creature.toughness;
                statsText.text = $"{displayPower}/{displayToughness}";
                keywordText.text = linkedCard.GetCardText();
            }
            else if (linkedCard is SorceryCard sorcery)
            {
                costText.text = sorcery.manaCost.ToString();
                statsText.text = "";
                sicknessText.text = "";

                string rules = "";

                if (sorcery.lifeToGain > 0)
                    rules += $"Gain {sorcery.lifeToGain} life.\n";
                if (sorcery.lifeToLoseForOpponent > 0)
                    rules += $"Opponent loses {sorcery.lifeToLoseForOpponent} life.\n";
                if (sorcery.lifeLossForBothPlayers > 0)
                    rules += $"Each player loses {sorcery.lifeLossForBothPlayers} life.\n";
                if (sorcery.cardsToDraw > 0)
                    rules += $"Draw {sorcery.cardsToDraw} card(s).\n";
                if (sorcery.cardsToDiscardorDraw > 0)
                    rules += $"Opponent discards {sorcery.cardsToDiscardorDraw} card(s) at random. If can't, you draw a card.\n";
                if (sorcery.eachPlayerGainLifeEqualToLands)
                    rules += $"Each player gains life equal to the number of lands they control.\n";
                if (sorcery.typeOfPermanentToDestroyAll != SorceryCard.PermanentTypeToDestroy.None)
                    {
                        string typeStr = "permanents";
                        switch (sorcery.typeOfPermanentToDestroyAll)
                        {
                            case SorceryCard.PermanentTypeToDestroy.Land:
                                typeStr = "lands";
                                break;
                            case SorceryCard.PermanentTypeToDestroy.Creature:
                                typeStr = "creatures";
                                break;
                            // Add more types if needed
                        }
                        rules += $"Destroy all {typeStr}.\n";
                    }
                if (sorcery.exileAllCreaturesFromGraveyards)
                    rules += "Exile all creature cards from all graveyards.\n";
                if (sorcery.damageToEachCreatureAndPlayer > 0)
                    rules += $"Deal {sorcery.damageToEachCreatureAndPlayer} damage to each creature and each player.\n";

                keywordText.text = rules.Trim();
            }
            else if (linkedCard is ArtifactCard artifact)
            {
                costText.text = artifact.manaCost.ToString();
                statsText.text = "";
                keywordText.text = artifact.GetCardText();
            }
            else
            {
                // Default fallback for other card types
                costText.text = "";
                statsText.text = "";
                keywordText.text = "";
            }
        }

    public void OnClick()
        {
            if (!isInBattlefield && GameManager.Instance.humanPlayer.Hand.Contains(linkedCard) == false)
            {
                Debug.Log("Cannot play or interact with cards in the graveyard.");
                return;
            }

            // TAP-FOR-MANA ability during Main Phase
                /*if (linkedCard is CreatureCard creatureForTap &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(creatureForTap) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2) &&
                    creatureForTap.activatedAbilities != null &&
                    creatureForTap.activatedAbilities.Contains(ActivatedAbility.TapForMana) &&
                    !creatureForTap.isTapped &&
                    (!creatureForTap.hasSummoningSickness || creatureForTap.keywordAbilities.Contains(KeywordAbility.Haste)))
                {
                    GameManager.Instance.TapCardForMana(creatureForTap);
                    UpdateVisual();
                    return;
                }*/
                if (linkedCard.activatedAbilities != null &&
                    linkedCard.activatedAbilities.Contains(ActivatedAbility.TapForMana) &&
                    !linkedCard.isTapped &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2))
                {
                    linkedCard.isTapped = true;
                    GameManager.Instance.humanPlayer.ManaPool++;
                    GameManager.Instance.UpdateUI();
                    UpdateVisual();
                    return;
                }
                
            // TAP-TO-CREATE-TOKEN generic ability
                if (linkedCard.activatedAbilities.Contains(ActivatedAbility.TapToCreateToken) &&
                    linkedCard is CreatureCard tokenSpawner &&
                    !linkedCard.isTapped &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2) &&
                    (!tokenSpawner.hasSummoningSickness || tokenSpawner.keywordAbilities.Contains(KeywordAbility.Haste)))
                {
                    int cost = linkedCard.manaToPayToActivate;
                    if (GameManager.Instance.humanPlayer.ManaPool >= cost)
                    {
                        GameManager.Instance.humanPlayer.ManaPool -= cost;
                        linkedCard.isTapped = true;

                        string tokenName = linkedCard.tokenToCreate;
                        Card token = CardFactory.Create(tokenName);
                        if (token != null)
                        {
                            GameManager.Instance.SummonToken(token, GameManager.Instance.humanPlayer);
                            Debug.Log($"{linkedCard.cardName} created a {tokenName} token.");
                        }
                        else
                        {
                            Debug.LogError($"Failed to create token: {tokenName}");
                        }

                        GameManager.Instance.UpdateUI();
                        UpdateVisual();
                    }
                    else
                    {
                        Debug.Log("Not enough mana to activate token creation.");
                    }

                    return;
                }
            // TAP-TO-LOSE-LIFE ability during Main Phase
                if (linkedCard is CreatureCard creatureForDrain &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(creatureForDrain) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2) &&
                    creatureForDrain.activatedAbilities != null &&
                    creatureForDrain.activatedAbilities.Contains(ActivatedAbility.TapToLoseLife) &&
                    !creatureForDrain.isTapped &&
                    (!creatureForDrain.hasSummoningSickness || creatureForDrain.keywordAbilities.Contains(KeywordAbility.Haste)))
                {
                    GameManager.Instance.TapToLoseLife(creatureForDrain);
                    UpdateVisual();
                    return;
                }

            // TAP-TO-PLAGUE ability during Main Phase
                if (linkedCard.activatedAbilities.Contains(ActivatedAbility.TapToPlague) &&
                !linkedCard.isTapped &&
                GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
                TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2))
            {
                linkedCard.isTapped = true;
                GameManager.Instance.humanPlayer.Life -= linkedCard.plagueAmount;
                GameManager.Instance.aiPlayer.Life -= linkedCard.plagueAmount;
                GameManager.Instance.UpdateUI();
                UpdateVisual();
                Debug.Log($"{linkedCard.cardName} tapped: Both players lose {linkedCard.plagueAmount} life.");
                return;
            }
            
            // TAP-AND-SACRIFICE-FOR-MANA or SACRIFICE-FOR-MANA during Main Phase
                if (linkedCard.activatedAbilities != null &&
                    (linkedCard.activatedAbilities.Contains(ActivatedAbility.TapAndSacrificeForMana) ||
                    linkedCard.activatedAbilities.Contains(ActivatedAbility.SacrificeForMana)) &&
                    !linkedCard.isTapped &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2))
                {
                    ArtifactCard artifact = linkedCard as ArtifactCard;

                    // Check for SacrificeForMana with cost
                    if (linkedCard.activatedAbilities.Contains(ActivatedAbility.SacrificeForMana))
                    {
                        if (GameManager.Instance.humanPlayer.ManaPool >= artifact.manaToPayToActivate)
                        {
                            GameManager.Instance.humanPlayer.ManaPool -= artifact.manaToPayToActivate;
                            GameManager.Instance.humanPlayer.ManaPool += artifact.manaToGain;
                            linkedCard.isTapped = true;
                            GameManager.Instance.SendToGraveyard(linkedCard, GameManager.Instance.humanPlayer);
                            GameManager.Instance.UpdateUI();
                            UpdateVisual();
                            Debug.Log($"{linkedCard.cardName} activated: +{artifact.manaToGain} mana.");
                        }
                        else
                        {
                            Debug.Log("Not enough mana for ability.");
                        }
                    }
                    else // TapAndSacrificeForMana (no cost, gain 1 mana)
                    {
                        linkedCard.isTapped = true;
                        GameManager.Instance.humanPlayer.ManaPool++;
                        GameManager.Instance.SendToGraveyard(linkedCard, GameManager.Instance.humanPlayer);
                        GameManager.Instance.UpdateUI();
                        UpdateVisual();
                        Debug.Log($"{linkedCard.cardName} sacrificed: +1 mana.");
                    }

                    return;
                }

            
            // TAP-TO-GAIN-LIFE ability during Main Phase
                if (linkedCard.activatedAbilities != null &&
                    linkedCard.activatedAbilities.Contains(ActivatedAbility.TapToGainLife) &&
                    !linkedCard.isTapped &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2))
                {
                    linkedCard.isTapped = true;
                    GameManager.Instance.humanPlayer.Life += 1;
                    GameManager.Instance.UpdateUI();
                    UpdateVisual();
                    return;
                }

            // SACRIFICE-FOR-LIFE during Main Phase
                if (linkedCard.activatedAbilities != null &&
                    linkedCard.activatedAbilities.Contains(ActivatedAbility.SacrificeForLife) &&
                    !linkedCard.isTapped &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2))
                {
                    ArtifactCard artifact = linkedCard as ArtifactCard;

                    if (GameManager.Instance.humanPlayer.ManaPool >= artifact.manaToPayToActivate)
                    {
                        GameManager.Instance.humanPlayer.ManaPool -= artifact.manaToPayToActivate;
                        GameManager.Instance.humanPlayer.Life += artifact.lifeToGain;
                        linkedCard.isTapped = true;
                        GameManager.Instance.SendToGraveyard(linkedCard, GameManager.Instance.humanPlayer);
                        GameManager.Instance.UpdateUI();
                        UpdateVisual();
                        Debug.Log($"{linkedCard.cardName} activated: Gain {artifact.lifeToGain} life.");
                    }
                    else
                    {
                        Debug.Log("Not enough mana for ability.");
                    }

                    return;
                }
            
            // SACRIFICE-TO-DRAW-CARDS during Main Phase
                if (linkedCard.activatedAbilities != null &&
                    linkedCard.activatedAbilities.Contains(ActivatedAbility.SacrificeToDrawCards) &&
                    !linkedCard.isTapped &&
                    GameManager.Instance.humanPlayer.Battlefield.Contains(linkedCard) &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human &&
                    (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main1 || TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.Main2))
                {
                    ArtifactCard artifact = linkedCard as ArtifactCard;

                    if (GameManager.Instance.humanPlayer.ManaPool >= artifact.manaToPayToActivate)
                    {
                        GameManager.Instance.humanPlayer.ManaPool -= artifact.manaToPayToActivate;

                        for (int i = 0; i < artifact.cardsToDraw; i++)
                        {
                            GameManager.Instance.DrawCard(GameManager.Instance.humanPlayer);
                        }
                        
                        linkedCard.isTapped = true;
                        GameManager.Instance.SendToGraveyard(linkedCard, GameManager.Instance.humanPlayer);
                        GameManager.Instance.UpdateUI();
                        UpdateVisual();
                        Debug.Log($"{linkedCard.cardName} activated: Draw {artifact.cardsToDraw} cards.");
                    }
                    else
                    {
                        Debug.Log("Not enough mana for ability.");
                    }

                    return;
                }

            // Blocking phase
                if (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.ChooseBlockers &&
                    TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.AI)

                if (linkedCard is CreatureCard clickedCreature)
                {
                    Player you = GameManager.Instance.humanPlayer;
                    Player enemy = GameManager.Instance.aiPlayer;

                    // If you click an attacker first
                    if (enemy.Battlefield.Contains(clickedCreature) &&
                        GameManager.Instance.currentAttackers.Contains(clickedCreature))
                    {
                        GameManager.Instance.selectedAttackerForBlocking = clickedCreature;
                        Debug.Log($"Selected attacker to block: {clickedCreature.cardName}");
                        return;
                    }

                    // If you click your own untapped creature next
                    if (you.Battlefield.Contains(clickedCreature) &&
                        !clickedCreature.isTapped &&
                        !clickedCreature.keywordAbilities.Contains(KeywordAbility.CantBlock) &&
                        !(
                            clickedCreature.keywordAbilities.Contains(KeywordAbility.CantBlockWithoutForest) &&
                            !you.Battlefield.Exists(card => card is LandCard land && land.cardName.ToLower().Contains("forest"))
                        ))
                    {
                        var attacker = GameManager.Instance.selectedAttackerForBlocking;

                        if (clickedCreature.blockingThisAttacker != null)
                        {
                            // Already blocking → remove block
                            Debug.Log($"{clickedCreature.cardName} stops blocking.");
                            clickedCreature.blockingThisAttacker.blockedByThisBlocker = null;
                            clickedCreature.blockingThisAttacker = null;
                            GameManager.Instance.UpdateUI();
                            return;
                        }

                        if (attacker == null)
                        {
                            Debug.Log("Click an attacking enemy creature first to block.");
                            return;
                        }

                        // Remove previous block from this attacker, if any
                        if (attacker.blockedByThisBlocker != null)
                        {
                            CreatureCard oldBlocker = attacker.blockedByThisBlocker;
                            oldBlocker.blockingThisAttacker = null;
                            attacker.blockedByThisBlocker = null;

                            // Force line to disappear on old blocker
                            var oldVisual = GameManager.Instance.FindCardVisual(oldBlocker);
                            if (oldVisual != null)
                                oldVisual.UpdateVisual();
                        }

                        // Check if this blocker is allowed to block the attacker (Flying rule)
                        if (attacker.keywordAbilities.Contains(KeywordAbility.Flying) &&
                            !clickedCreature.keywordAbilities.Contains(KeywordAbility.Flying) &&
                            !clickedCreature.keywordAbilities.Contains(KeywordAbility.Reach))
                        {
                            Debug.Log($"{clickedCreature.cardName} can't block {attacker.cardName} because it lacks Flying or Reach.");
                            return;
                        }

                        // Check if blocker is restricted to flying-only
                        if (clickedCreature.keywordAbilities.Contains(KeywordAbility.CanOnlyBlockFlying) &&
                            !attacker.keywordAbilities.Contains(KeywordAbility.Flying))
                        {
                            Debug.Log($"{clickedCreature.cardName} can only block flying creatures.");
                            return;
                        }

                        // Assign the block
                        clickedCreature.blockingThisAttacker = attacker;
                        attacker.blockedByThisBlocker = clickedCreature;
                        GameManager.Instance.selectedAttackerForBlocking = null;

                        Debug.Log($"{clickedCreature.cardName} is blocking {attacker.cardName}");
                        GameManager.Instance.UpdateUI();
                        return;
                    }
                }

            // Declare attacker
                if (linkedCard is CreatureCard creature)
                {
                    if (TurnSystem.Instance.currentPhase == TurnSystem.TurnPhase.ChooseAttackers &&
                        TurnSystem.Instance.currentPlayer == TurnSystem.PlayerType.Human)
                    {
                        if (GameManager.Instance.selectedAttackers.Contains(creature))
                        {
                            // Removing from combat
                            GameManager.Instance.selectedAttackers.Remove(creature);
                            if (!creature.keywordAbilities.Contains(KeywordAbility.Vigilance))
                                creature.isTapped = false;

                            Debug.Log($"{creature.cardName} removed from attackers.");
                        }
                        else if (!creature.hasSummoningSickness && !creature.keywordAbilities.Contains(KeywordAbility.Defender))
                        {
                            // Adding to combat
                            GameManager.Instance.selectedAttackers.Add(creature);
                            if (!creature.keywordAbilities.Contains(KeywordAbility.Vigilance))
                                creature.isTapped = true;

                            Debug.Log($"{creature.cardName} declared as attacker.");
                        }

                        UpdateVisual();
                        return;
                    }
                }

            // Land usage
                if (isInBattlefield && linkedCard is LandCard land)
                {
                    GameManager.Instance.TapLandForMana(land, GameManager.Instance.humanPlayer);
                    UpdateVisual();
                    return;
                }

            // Playing card from hand
                if (!isInBattlefield)
                    {
                        if (TurnSystem.Instance.currentPlayer != TurnSystem.PlayerType.Human ||
                            (TurnSystem.Instance.currentPhase != TurnSystem.TurnPhase.Main1 &&
                            TurnSystem.Instance.currentPhase != TurnSystem.TurnPhase.Main2))
                        {
                            Debug.Log("You can only play cards during your own Main Phase.");
                            return;
                        }

                        GameManager.Instance.PlayCard(GameManager.Instance.humanPlayer, this);
                        UpdateVisual();
                        return;
                    }
        } 
}