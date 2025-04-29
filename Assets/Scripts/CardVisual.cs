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

    public TMP_Text titleText;
    public TMP_Text sicknessText;
    public TMP_Text costText;
    public TMP_Text statsText;

    public bool isInBattlefield = false;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void UpdateVisual()
    {
        if (linkedCard.isTapped)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
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
                sicknessText.text = "(Summoning Sickness)";
            }
            else
            {
                sicknessText.text = "";
            }

            statsText.text = $"{creature.power}/{creature.toughness}";
        }
        else
        {
            sicknessText.text = "";
            statsText.text = "";
        }

        // Handle LineRenderer for blockers
        if (linkedCard is CreatureCard c)
        {
            if (c.blockingThisAttacker != null)
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, gameManager.FindCardVisual(c.blockingThisAttacker).transform.position);
            }
            else if (c.blockedByThisBlocker != null)
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, gameManager.FindCardVisual(c.blockedByThisBlocker).transform.position);
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
        if (gameManager.isBlockingPhase)
        {
            Debug.Log("Clicked card: " + linkedCard.cardName);
            gameManager.TryAssignBlocker(this);
            return;
        }

        if (isInBattlefield)
        {
            // Already in play: tap/untap if it's a Land!
            if (linkedCard is LandCard)
            {
                gameManager.OnLandClicked(this);
            }
        }
        else
        {
            // Still in hand: normal play
            gameManager.OnCardClicked(this);
        }
    }
}