using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollectibleCard : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string cardName = "Card Name";
    [SerializeField] private GameObject textObject;
    [SerializeField] private bool destroyOnCollect = true;

    [Header("Audio")]
    [SerializeField] private AudioClip collectSfx;

    private TextMeshPro tmpComponent;
    private bool playerInRange;
    private bool hasBeenCollected;

    private void Start()
    {
        if (textObject == null)
        {
            Debug.LogError("[CollectibleCard] Text object is not assigned.");
            return;
        }

        tmpComponent = textObject.GetComponent<TextMeshPro>();
        if (tmpComponent == null)
        {
            Debug.LogError("[CollectibleCard] Text object does not have a TextMeshPro component.");
            return;
        }

        UpdatePromptText();
        textObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || hasBeenCollected || textObject == null)
            return;

        playerInRange = true;
        textObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || textObject == null)
            return;

        playerInRange = false;
        textObject.SetActive(false);
    }

    private void Update()
    {
        if (!hasBeenCollected && playerInRange && Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            CollectCard();
            return;
        }

        if (textObject != null && textObject.activeSelf && Camera.main != null)
        {
            textObject.transform.LookAt(
                textObject.transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
        }
    }

    private void UpdatePromptText()
    {
        if (tmpComponent == null)
            return;

        string displayName = string.IsNullOrWhiteSpace(cardName) ? "Card" : cardName;
        tmpComponent.text = "Press Q to collect\n" + "<color=yellow>" + displayName + "</color>";
    }

    private void CollectCard()
    {
        hasBeenCollected = true;
        playerInRange = false;

        CardData data = CardDatabase.GetCardData(cardName);
        if (data == null)
        {
            hasBeenCollected = false;
            UpdatePromptText();
            return;
        }

        PlayerCollection.OwnedCards.Add(data);
        Debug.Log($"[CollectibleCard] Added '{data.cardName}' to player collection.");

        TryPlayCollectSfx();

        if (textObject != null)
            textObject.SetActive(false);

        if (destroyOnCollect)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    private void TryPlayCollectSfx()
    {
        if (collectSfx == null)
            return;

        EnsureAudioListenerExists();

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound(collectSfx);
        else
            AudioSource.PlayClipAtPoint(collectSfx, transform.position);
    }

    private void EnsureAudioListenerExists()
    {
        if (FindFirstObjectByType<AudioListener>() != null)
            return;

        if (Camera.main != null)
        {
            if (Camera.main.GetComponent<AudioListener>() == null)
                Camera.main.gameObject.AddComponent<AudioListener>();

            return;
        }

        GameObject listenerObject = new GameObject("AutoAudioListener");
        listenerObject.AddComponent<AudioListener>();
    }
}
