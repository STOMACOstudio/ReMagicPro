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
        if (!isInBattlefield)
        {
            lineRenderer.enabled = false;
            return; // don't do line logic at all if dead
        }

        if (linkedCard.isTapped)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            transform.rotation = Quaternion.identity;
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
            keywordText.text = "";
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
            statsText.text = $"{creature.power}/{creature.toughness}";
        }
        else
        {
            costText.text = "";
            statsText.text = "";
        }
    }

    public void OnClick()
    {
        if (!isInBattlefield && GameManager.Instance.humanPlayer.Hand.Contains(linkedCard) == false)
        {
            Debug.Log("Cannot play or interact with cards in the graveyard.");
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
                !clickedCreature.keywordAbilities.Contains(KeywordAbility.CantBlock))
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