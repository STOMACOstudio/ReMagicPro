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
    public Material crippledMaterial;
    public float lockTimeRequirement = 15f;

    [Header("Crippled State")]
    [Tooltip("Optional particle system to stop and disable after the battle is won.")]
    public ParticleSystem manaParticleSystem;
    
    [Header("Audio")]
    public AudioClip lockSound;

    [Header("Subtitles")]
    public SubtitleManager subtitleManager;
    public List<SubtitleManager.SubtitleLine> interactionLines; 
    public List<SubtitleManager.SubtitleLine> postLockLines; 
    public List<SubtitleManager.SubtitleLine> beginnerBattleLines; 
    public List<SubtitleManager.SubtitleLine> postBeginnerBattleWinLines;
    public List<SubtitleManager.SubtitleLine> wrongPlatformLines;

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
    private Coroutine beginnerBattleCoroutine;
    private bool isThisPlatformLocked = false;
    private bool hasPlayedPostBeginnerBattleWinSubtitles = false;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();

        if (rend == null)
        {
            Debug.LogError($"[{nameof(PlatformTrigger)}] Renderer is missing on {gameObject.name}.", this);
            enabled = false;
            return;
        }

        originalMaterial = rend.material;

        if (subtitleManager == null)
            subtitleManager = Object.FindFirstObjectByType<SubtitleManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (isAnyPlatformLocked)
        {
            playerCount++;
            if (playerCount == 1)
            {
                TryStartBeginnerBattle();
            }
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

        string playerColor = PlayerPrefs.GetString("PlayerColors", string.Empty);
        string deckKey = ResolveBeginnerDeckKeyForPlatform();
        if (string.IsNullOrEmpty(deckKey))
            return;

        string expectedDeckKey = ResolveOppositeBeginnerDeckKeyForPlayerColor(playerColor);
        if (string.IsNullOrEmpty(expectedDeckKey))
            return;

        if (!string.Equals(deckKey, expectedDeckKey, System.StringComparison.Ordinal))
        {
            if (subtitleManager != null && wrongPlatformLines != null && wrongPlatformLines.Count > 0)
            {
                subtitleManager.DisplaySequence(wrongPlatformLines);
            }
            return;
        }

        if (beginnerBattleCoroutine != null)
            StopCoroutine(beginnerBattleCoroutine);

        beginnerBattleCoroutine = StartCoroutine(BeginnerBattleAfterSubtitles(deckKey));
    }

    private IEnumerator BeginnerBattleAfterSubtitles(string deckKey)
    {
        if (subtitleManager != null && beginnerBattleLines != null && beginnerBattleLines.Count > 0)
        {
            yield return subtitleManager.DisplaySequenceAndWait(beginnerBattleLines);
            subtitleManager.StopSequence();
        }

        beginnerBattleCoroutine = null;

        if (playerCount <= 0)
            yield break;

        if (!DeckHolder.IsStarterDeckRewardCollected)
            yield break;

        BattleData.CurrentZoneId = null;
        BattleData.CurrentDeckKey = deckKey;
        BattleData.ReturnSceneName = SceneManager.GetActiveScene().name;
        BattleData.IsBattleOpenedAdditively = true;
        BattleData.TriggeringPlatform = this;
        BattleData.PauseReturnScene();

        SceneManager.LoadScene(BattleSceneName, LoadSceneMode.Additive);
        Scene battleScene = SceneManager.GetSceneByName(BattleSceneName);
        if (battleScene.IsValid() && battleScene.isLoaded)
        {
            EventSystemUtility.EnableOnlyForScene(battleScene);
            EventSystemUtility.EnsureSingleAudioListener(battleScene);
            SceneManager.SetActiveScene(battleScene);
        }
    }

    public void ApplyCrippledState()
    {
        if (crippledMaterial != null && rend != null)
            rend.material = crippledMaterial;

        if (manaParticleSystem != null)
        {
            manaParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            manaParticleSystem.gameObject.SetActive(false);
        }
    }

    public void PlayPostBeginnerBattleWinSubtitlesIfNeeded()
    {
        if (hasPlayedPostBeginnerBattleWinSubtitles)
            return;

        if (postBeginnerBattleWinLines == null || postBeginnerBattleWinLines.Count == 0)
            return;

        if (subtitleManager == null)
            subtitleManager = Object.FindFirstObjectByType<SubtitleManager>();

        if (subtitleManager == null)
            return;

        subtitleManager.DisplaySequence(postBeginnerBattleWinLines);
        hasPlayedPostBeginnerBattleWinSubtitles = true;
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

    private string ResolveOppositeBeginnerDeckKeyForPlayerColor(string playerColor)
    {
        switch (playerColor.ToLowerInvariant())
        {
            case "white": return "Deck_Graveyard"; // Opponent black
            case "blue": return "Deck_Camp"; // Opponent red
            case "black": return "Deck_Thicket"; // Opponent green
            case "red": return "Deck_Village"; // Opponent white
            case "green": return "Deck_Shore"; // Opponent blue
            default:
                Debug.LogWarning($"[PlatformTrigger] Unknown player color '{playerColor}'.");
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
        if (subtitleManager != null && postLockLines != null && postLockLines.Count > 0)
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

        playerCount--;
        if (playerCount <= 0)
        {
            playerCount = 0;

            if (isAnyPlatformLocked)
            {
                if (beginnerBattleCoroutine != null)
                {
                    StopCoroutine(beginnerBattleCoroutine);
                    beginnerBattleCoroutine = null;
                }

                // Keep post-lock subtitles playing on the selected mana platform,
                // but still stop every other subtitle sequence when leaving.
                bool shouldKeepPlayingSubtitles = isThisPlatformLocked;
                if (subtitleManager != null && !shouldKeepPlayingSubtitles)
                {
                    subtitleManager.StopSequence();
                }
                return;
            }

            if (isThisPlatformLocked) return;

            if (lockCoroutine != null) StopCoroutine(lockCoroutine);
            rend.material = originalMaterial;

            if (subtitleManager != null)
            {
                subtitleManager.StopSequence();
            }
        }
    }
}
