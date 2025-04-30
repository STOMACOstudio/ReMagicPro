using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnSystem : MonoBehaviour
{
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
        nextPhaseButton.onClick.AddListener(NextPhaseButton);
        confirmAttackersButton.onClick.AddListener(ConfirmAttackers);
        confirmBlockersButton.onClick.AddListener(ConfirmBlockers);

        confirmAttackersButton.gameObject.SetActive(false);
        confirmBlockersButton.gameObject.SetActive(false);

        BeginTurn(PlayerType.Human);
    }

    void Update()
    {
        if (currentPlayer == PlayerType.AI && !waitingForPlayerInput)
        {
            RunCurrentPhase();
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
        AdvancePhase(); // <-- advance immediately
    }
}

public void ConfirmBlockers()
{
    if (waitingForPlayerInput)
    {
        Debug.Log("[UI] Player confirmed blockers");
        waitingForPlayerInput = false;
        confirmBlockersButton.gameObject.SetActive(false);
        AdvancePhase(); // <-- advance immediately
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
        currentPhase++;
        RunCurrentPhase();
    }

    void RunCurrentPhase()
    {
        Debug.Log($"[Phase] {currentPlayer} - {currentPhase}");

        string label = $"{currentPlayer} - {currentPhase}";
        Debug.Log($"[Phase] {label}");
        if (phaseText != null)
            phaseText.text = label;

        switch (currentPhase)
        {
            case TurnPhase.Untap:
                Debug.Log("→ Untapping all permanents.");
                AdvancePhase();
                break;

            case TurnPhase.Upkeep:
                Debug.Log("→ Upkeep phase (no triggers yet).");
                AdvancePhase();
                break;

            case TurnPhase.Draw:
                Debug.Log("→ Drawing a card.");
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
                    AdvancePhase();
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
            Debug.Log("→ Skipping attacker choice (AI turn).");
            AdvancePhase(); // skip to ChooseBlockers
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
                AdvancePhase(); // already confirmed, move on
            }
        }
        else
        {
            Debug.Log("→ Skipping attacker confirmation (AI turn).");
            AdvancePhase();
        }
        break;

        case TurnPhase.ChooseBlockers:
            if (currentPlayer == PlayerType.AI)
            {
                Debug.Log("→ Player chooses blockers.");
                waitingForPlayerInput = true;
                confirmBlockersButton.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("→ Skipping blocker choice (Player turn).");
                AdvancePhase(); // skip to Damage
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
                    AdvancePhase(); // already confirmed
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
                AdvancePhase();
                break;

            case TurnPhase.EndTurn:
                Debug.Log("→ Ending turn.");
                BeginTurn(currentPlayer == PlayerType.Human ? PlayerType.AI : PlayerType.Human);
                break;
        }
    }
}