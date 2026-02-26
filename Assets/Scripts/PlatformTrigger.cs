using UnityEngine;
using System.Collections.Generic;

public class PlatformTrigger : MonoBehaviour
{
    public string playerTag = "Player";
    public Material activeMaterial;
    
    [Header("Subtitles")]
    public SubtitleManager subtitleManager;
    // This creates the same list structure you have in PlayerMovement
    public List<SubtitleManager.SubtitleLine> interactionLines; 

    private Material originalMaterial;
    private Renderer rend;
    private AudioSource audioSource;
    private int playerCount = 0;
    private bool hasTriggered = false; // Prevents re-triggering the text repeatedly

    void Awake()
    {
        rend = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        originalMaterial = rend.material;
        if (subtitleManager == null) subtitleManager = Object.FindFirstObjectByType<SubtitleManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerCount++;

        if (playerCount == 1)
        {
            if (activeMaterial != null) rend.material = activeMaterial;
            if (audioSource != null) audioSource.Play();

            if (subtitleManager != null && !hasTriggered)
            {
                subtitleManager.DisplaySequence(interactionLines);
                hasTriggered = true; // Only play the sequence once
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerCount--;

        if (playerCount <= 0)
        {
            playerCount = 0;
            rend.material = originalMaterial;
            
            // --- NEW: STOP SUBTITLES ON EXIT ---
            if (subtitleManager != null)
            {
                subtitleManager.StopSequence(); 
            }
            
            hasTriggered = false; // Allows the sequence to play again if they re-enter
        }
    }
}