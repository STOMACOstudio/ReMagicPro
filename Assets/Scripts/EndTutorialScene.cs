using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTutorialScene : MonoBehaviour
{
    [Header("Trigger")]
    public string playerTag = "Player";

    [Header("Lift")]
    public float liftSpeed = 2f;
    public float liftHeight = 10f;

    [Header("Subtitles")]
    public SubtitleManager subtitleManager;
    public List<SubtitleManager.SubtitleLine> endingLines;

    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName = "FarmScene";
    [SerializeField] private bool waitForSubtitlesBeforeLoad = true;

    private PlayerMovement cachedPlayerMovement;
    private CharacterController cachedCharacterController;
    private bool hasTriggered;
    private bool hasReachedLiftTarget;
    private bool hasCompletedEndingSequence;
    private bool hasLoadedNextScene;
    private float targetY;

    void Awake()
    {
        if (subtitleManager == null)
            subtitleManager = Object.FindFirstObjectByType<SubtitleManager>();
    }

    void Update()
    {
        if (!hasTriggered || hasLoadedNextScene)
            return;

        if (!hasReachedLiftTarget)
        {
            Vector3 currentPosition = transform.position;
            float newY = Mathf.MoveTowards(currentPosition.y, targetY, liftSpeed * Time.deltaTime);
            transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);
            hasReachedLiftTarget = Mathf.Approximately(newY, targetY);
        }

        TryLoadNextScene();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag(playerTag))
            return;

        hasTriggered = true;
        targetY = transform.position.y + liftHeight;

        cachedPlayerMovement = other.GetComponent<PlayerMovement>();
        if (cachedPlayerMovement != null)
            cachedPlayerMovement.enabled = false;

        cachedCharacterController = other.GetComponent<CharacterController>();
        if (cachedCharacterController != null)
            cachedCharacterController.enabled = false;

        StartCoroutine(PlayEndingSequence());
    }

    private IEnumerator PlayEndingSequence()
    {
        if (subtitleManager != null && endingLines != null && endingLines.Count > 0)
        {
            if (waitForSubtitlesBeforeLoad)
                yield return subtitleManager.DisplaySequenceAndWait(endingLines);
            else
                subtitleManager.DisplaySequence(endingLines);
        }

        hasCompletedEndingSequence = true;
        TryLoadNextScene();
    }

    private void TryLoadNextScene()
    {
        if (hasLoadedNextScene || !hasReachedLiftTarget)
            return;

        if (waitForSubtitlesBeforeLoad && !hasCompletedEndingSequence)
            return;

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError($"[{nameof(EndTutorialScene)}] nextSceneName is empty; cannot transition.", this);
            return;
        }

        hasLoadedNextScene = true;
        SceneManager.LoadScene(nextSceneName);
    }
}
