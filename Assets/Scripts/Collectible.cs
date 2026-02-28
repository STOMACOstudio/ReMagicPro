using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class Collectible : MonoBehaviour
{
    [Header("Settings")]
    public string itemName = "Item";
    public string starterColor = "Red";
    public GameObject textObject; // Drag your 3D Text object here
    public bool destroyOnCollect = true;

    [Header("Post-Collect Subtitle")]
    [SerializeField] private SubtitleManager subtitleManager;
    [SerializeField] private List<SubtitleManager.SubtitleLine> postDeckGeneratedSubtitles = new List<SubtitleManager.SubtitleLine>();

    [Header("Audio")]
    [SerializeField] private AudioClip collectSfx;

    private TextMeshPro tmpComponent;
    private bool playerInRange;
    private bool hasBeenCollected;

    void Start()
    {
        // Get the actual text component and format it
        tmpComponent = textObject.GetComponent<TextMeshPro>();
        tmpComponent.text = "Press Q to collect\n" + "<color=yellow>" + itemName + "</color>";

        // Ensure it's hidden at the start
        textObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            textObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            textObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!hasBeenCollected && playerInRange && Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            CollectReward();
            return;
        }

        // Bonus: Make the text always face the player so it's readable
        if (textObject.activeSelf && Camera.main != null)
        {
            textObject.transform.LookAt(
                textObject.transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
        }
    }

    private void CollectReward()
    {
        hasBeenCollected = true;
        playerInRange = false;

        TryPlayCollectSfx();

        StartCoroutine(GenerateDeckRoutine());
    }

    private void TryPlayCollectSfx()
    {
        if (collectSfx == null)
            return;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound(collectSfx);
        else
            AudioSource.PlayClipAtPoint(collectSfx, transform.position);
    }

    private IEnumerator GenerateDeckRoutine()
    {
        if (textObject != null)
        {
            textObject.SetActive(true);
            if (tmpComponent != null)
                tmpComponent.text = "Generating starting deck...";
        }

        // Allow one frame for the message to render before doing heavy work.
        yield return null;

        PlayerPrefs.SetString("PlayerColors", starterColor);
        DeckHolder.SelectedDeck = DeckDatabase.BuildPlayerStarterDeck(starterColor);
        PlayerPrefs.Save();

        int deckCount = DeckHolder.SelectedDeck != null ? DeckHolder.SelectedDeck.Count : 0;
        DeckHolder.IsStarterDeckRewardCollected = deckCount > 0;

        if (deckCount > 0)
            Debug.Log($"[Collectible] Starter deck generated for color '{starterColor}' with {deckCount} cards.");
        else
            Debug.LogError($"[Collectible] Failed to generate starter deck for color '{starterColor}'.");

        TryPlayPostDeckGeneratedSubtitles();

        if (textObject != null)
            textObject.SetActive(false);

        if (destroyOnCollect)
            Destroy(gameObject);
    }

    private void TryPlayPostDeckGeneratedSubtitles()
    {
        if (postDeckGeneratedSubtitles == null || postDeckGeneratedSubtitles.Count == 0)
            return;

        if (subtitleManager == null)
            subtitleManager = FindFirstObjectByType<SubtitleManager>();

        if (subtitleManager == null)
        {
            Debug.LogWarning("[Collectible] Post-deck subtitles are configured but no SubtitleManager was found.");
            return;
        }

        subtitleManager.DisplaySequence(postDeckGeneratedSubtitles);
    }
}
