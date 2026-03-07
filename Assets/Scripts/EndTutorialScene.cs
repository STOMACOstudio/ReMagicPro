using System.Collections.Generic;
using UnityEngine;

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

    private PlayerMovement cachedPlayerMovement;
    private CharacterController cachedCharacterController;
    private bool hasTriggered;
    private float targetY;

    void Awake()
    {
        if (subtitleManager == null)
            subtitleManager = Object.FindFirstObjectByType<SubtitleManager>();
    }

    void Update()
    {
        if (!hasTriggered)
            return;

        Vector3 currentPosition = transform.position;
        float newY = Mathf.MoveTowards(currentPosition.y, targetY, liftSpeed * Time.deltaTime);
        transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);
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

        if (subtitleManager != null && endingLines != null && endingLines.Count > 0)
            subtitleManager.DisplaySequence(endingLines);
    }
}
