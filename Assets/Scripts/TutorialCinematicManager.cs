using System.Collections.Generic;
using UnityEngine;

public class TutorialCinematicManager : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public SubtitleManager subtitleManager;

    [Header("Entrance")]
    public bool playEntranceCinematic = true;
    public float startY = 10f;
    public float targetY = 0.6f;
    public float descentSpeed = 2f;
    public float initialLookDownAngle = -5f;
    public float movementUnlockFootstepDelay = 0.2f;

    [Header("Movement Boundary")]
    public bool enforceBoundary = true;
    public float boundaryRadius = 12.5f;
    public Transform boundaryCenter;

    [Header("Subtitles")]
    public bool playSubtitlesOnStart = true;
    public List<SubtitleManager.SubtitleLine> subtitleLines;

    private bool cinematicFinished;

    void Start()
    {
        if (playerMovement == null)
            playerMovement = Object.FindFirstObjectByType<PlayerMovement>();

        if (subtitleManager == null)
            subtitleManager = Object.FindFirstObjectByType<SubtitleManager>();

        if (playerMovement == null)
        {
            Debug.LogWarning("TutorialCinematicManager could not find PlayerMovement.");
            enabled = false;
            return;
        }

        Vector3 center = boundaryCenter != null
            ? boundaryCenter.position
            : new Vector3(playerMovement.transform.position.x, targetY, playerMovement.transform.position.z);

        if (enforceBoundary && boundaryCenter == null)
            Debug.Log("TutorialCinematicManager: boundaryCenter not assigned, using player's spawn position as boundary center.");
        playerMovement.SetBoundary(enforceBoundary, boundaryRadius, center);
        playerMovement.SetFixedY(targetY);
        playerMovement.SetLookPitch(initialLookDownAngle);

        if (playSubtitlesOnStart && subtitleManager != null && subtitleLines != null && subtitleLines.Count > 0)
            subtitleManager.DisplaySequence(subtitleLines);

        if (playEntranceCinematic)
        {
            playerMovement.SetMovementEnabled(false);
            Vector3 pos = playerMovement.transform.position;
            playerMovement.transform.position = new Vector3(pos.x, startY, pos.z);
        }
        else
        {
            Vector3 pos = playerMovement.transform.position;
            playerMovement.transform.position = new Vector3(pos.x, targetY, pos.z);
            UnlockPlayer();
        }
    }

    void Update()
    {
        if (!playEntranceCinematic || cinematicFinished || playerMovement == null)
            return;

        Transform player = playerMovement.transform;
        float newY = Mathf.MoveTowards(player.position.y, targetY, descentSpeed * Time.deltaTime);
        player.position = new Vector3(player.position.x, newY, player.position.z);

        if (Mathf.Abs(player.position.y - targetY) <= 0.001f)
            UnlockPlayer();
    }

    void UnlockPlayer()
    {
        cinematicFinished = true;
        playerMovement.transform.position = new Vector3(
            playerMovement.transform.position.x,
            targetY,
            playerMovement.transform.position.z
        );
        playerMovement.SetMovementEnabled(true);
        playerMovement.SetFootstepDelay(movementUnlockFootstepDelay);
    }
}
