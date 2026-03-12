using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CardGameTrigger : MonoBehaviour
{
    private const string BattleSceneName = "GameScene";

    [Header("Trigger")]
    public string playerTag = "Player";

    [Header("Battle")]
    [Tooltip("Deck key from GameManager.LoadDeckByKey (for example: Deck_Shore, Deck_Camp, Deck_Boss).")]
    public string enemyDeckKey = "Deck_Starter";

    [Tooltip("Cards the player can choose from after winning this match. Only one card can be claimed.")]
    public List<string> cardRewardOptions = new List<string>();

    [Header("Subtitles")]
    public SubtitleManager subtitleManager;
    public List<SubtitleManager.SubtitleLine> preBattleLines;

    [Header("Interaction Prompt")]
    [SerializeField] private GameObject interactionTextObject;

    private bool playerInRange;
    private bool waitingForBattleToEnd;
    private bool wasBattleOpenLastFrame;
    private TextMeshPro promptText;
    private Collider playerColliderInRange;

    void Awake()
    {
        if (subtitleManager == null)
            subtitleManager = Object.FindFirstObjectByType<SubtitleManager>();

        if (interactionTextObject != null)
        {
            promptText = interactionTextObject.GetComponent<TextMeshPro>();
            if (promptText != null)
                promptText.text = "Press Q to talk";

            interactionTextObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        // The return scene is temporarily deactivated while the battle scene is loaded.
        // When this object is re-enabled, reset local state from the current global battle flag.
        bool isBattleOpen = BattleData.IsBattleOpenedAdditively;
        wasBattleOpenLastFrame = isBattleOpen;

        if (!isBattleOpen)
        {
            waitingForBattleToEnd = false;

            if (playerInRange && interactionTextObject != null)
                interactionTextObject.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInRange = true;
        playerColliderInRange = other;

        if (interactionTextObject != null && !waitingForBattleToEnd)
            interactionTextObject.SetActive(true);
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInRange = true;
        if (playerColliderInRange == null)
            playerColliderInRange = other;

        if (interactionTextObject != null && !BattleData.IsBattleOpenedAdditively && !waitingForBattleToEnd)
            interactionTextObject.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInRange = false;
        waitingForBattleToEnd = false;
        playerColliderInRange = null;

        if (interactionTextObject != null)
            interactionTextObject.SetActive(false);
    }

    void Update()
    {
        bool isBattleOpen = BattleData.IsBattleOpenedAdditively;
        if (wasBattleOpenLastFrame && !isBattleOpen)
        {
            waitingForBattleToEnd = false;

            if (playerInRange && interactionTextObject != null)
                interactionTextObject.SetActive(true);
        }

        wasBattleOpenLastFrame = isBattleOpen;

        if (playerInRange &&
            playerColliderInRange != null &&
            !isBattleOpen &&
            !waitingForBattleToEnd &&
            Keyboard.current != null &&
            Keyboard.current.qKey.wasPressedThisFrame)
        {
            waitingForBattleToEnd = true;

            if (interactionTextObject != null)
                interactionTextObject.SetActive(false);

            StartCoroutine(StartBattleSequence(playerColliderInRange));
            return;
        }

        if (interactionTextObject != null && interactionTextObject.activeSelf && Camera.main != null)
        {
            interactionTextObject.transform.LookAt(
                interactionTextObject.transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
        }
    }

    private IEnumerator StartBattleSequence(Collider playerCollider)
    {
        LockPlayerMovement(playerCollider);

        if (subtitleManager != null && preBattleLines != null && preBattleLines.Count > 0)
            yield return subtitleManager.DisplaySequenceAndWait(preBattleLines);

        StartBattle();
    }

    private void LockPlayerMovement(Collider playerCollider)
    {
        if (playerCollider == null)
            return;

        PlayerMovement movement = playerCollider.GetComponent<PlayerMovement>();
        BattleData.PausePlayerMovement(movement);
    }

    private void StartBattle()
    {
        string trimmedDeckKey = string.IsNullOrWhiteSpace(enemyDeckKey) ? "Deck_Starter" : enemyDeckKey.Trim();

        BattleData.CurrentZoneId = null;
        BattleData.CurrentDeckKey = trimmedDeckKey;
        BattleData.SetRewardCards(cardRewardOptions);
        BattleData.ReturnSceneName = SceneManager.GetActiveScene().name;
        BattleData.IsBattleOpenedAdditively = true;
        BattleData.TriggeringPlatform = null;
        BattleData.PauseReturnScene();

        // The trigger scene gets disabled immediately after this call, so persist the
        // transition state now to ensure battle-end detection is correct on return.
        wasBattleOpenLastFrame = true;

        SceneManager.LoadScene(BattleSceneName, LoadSceneMode.Additive);
        Scene battleScene = SceneManager.GetSceneByName(BattleSceneName);
        if (battleScene.IsValid() && battleScene.isLoaded)
        {
            EventSystemUtility.EnableOnlyForScene(battleScene);
            EventSystemUtility.EnsureSingleAudioListener(battleScene);
            SceneManager.SetActiveScene(battleScene);
        }
    }
}
