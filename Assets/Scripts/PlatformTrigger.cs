using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlatformTrigger : MonoBehaviour
{
    private const string BattleSceneName = "GameScene";

    public string playerTag = "Player";
    public Material activeMaterial;
    public Material lockedMaterial; 
    public float lockTimeRequirement = 15f;
    
    [Header("Audio")]
    public AudioClip lockSound;

    [Header("Subtitles")]
    public SubtitleManager subtitleManager;
    public List<SubtitleManager.SubtitleLine> interactionLines; 
    public List<SubtitleManager.SubtitleLine> postLockLines; 

    [Header("Spawning")]
    [Tooltip("Assign the specific Prefab for this platform here.")]
    public GameObject rewardPrefab; 
    [Tooltip("Place an Empty GameObject in your scene where the item should appear.")]
    public Transform spawnPoint;

    private static bool isAnyPlatformLocked = false;
    private Material originalMaterial;
    private Renderer rend;
    private AudioSource audioSource;
    private int playerCount = 0;
    private Coroutine lockCoroutine;
    private bool isThisPlatformLocked = false;

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

        if (isAnyPlatformLocked)
        {
            TryStartBeginnerBattle();
            return;
        }

        playerCount++;

        if (playerCount == 1)
        {
            if (activeMaterial != null) rend.material = activeMaterial;
            if (audioSource != null) audioSource.Play();

            if (subtitleManager != null)
            {
                subtitleManager.DisplaySequence(interactionLines);
            }

            lockCoroutine = StartCoroutine(LockTimer());
        }
    }

    private void TryStartBeginnerBattle()
    {
        if (!DeckHolder.IsStarterDeckRewardCollected)
            return;

        string deckKey = ResolveBeginnerDeckKeyForPlatform();
        if (string.IsNullOrEmpty(deckKey))
            return;

        BattleData.CurrentZoneId = null;
        BattleData.CurrentDeckKey = deckKey;
        SceneManager.LoadScene(BattleSceneName);
    }

    private string ResolveBeginnerDeckKeyForPlatform()
    {
        if (rewardPrefab == null)
            return null;

        Collectible collectible = rewardPrefab.GetComponent<Collectible>();
        if (collectible == null)
            return null;

        switch (collectible.starterColor.ToLowerInvariant())
        {
            case "white": return "Deck_Village";
            case "blue": return "Deck_Shore";
            case "black": return "Deck_Graveyard";
            case "red": return "Deck_Camp";
            case "green": return "Deck_Thicket";
            default:
                Debug.LogWarning($"[PlatformTrigger] Unknown starter color '{collectible.starterColor}' on {gameObject.name}.");
                return null;
        }
    }

    IEnumerator LockTimer()
    {
        yield return new WaitForSeconds(lockTimeRequirement);
        
        isAnyPlatformLocked = true;
        isThisPlatformLocked = true;

        if (lockedMaterial != null) rend.material = lockedMaterial;
        
        if (audioSource != null && lockSound != null)
        {
            audioSource.PlayOneShot(lockSound);
        }

        // --- NEW ACTIONS ON LOCK ---
        
        // 1. Play the "Success" Subtitles
        if (subtitleManager != null && postLockLines.Count > 0)
        {
            subtitleManager.DisplaySequence(postLockLines);
        }

        // 2. Spawn the specific prefab
        if (rewardPrefab != null && spawnPoint != null)
        {
            Instantiate(rewardPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        Debug.Log(gameObject.name + " is now LOCKED and spawned an object.");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (isThisPlatformLocked) return;

        playerCount--;

        if (playerCount <= 0)
        {
            playerCount = 0;
            if (lockCoroutine != null) StopCoroutine(lockCoroutine);
            rend.material = originalMaterial;
            
            if (subtitleManager != null)
            {
                subtitleManager.StopSequence(); 
            }
        }
    }
}
