using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FavouriteCardManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Canvas canvas; // Canvas used for dragging

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip selectSound;
    [SerializeField] private AudioClip attachSound;
    [SerializeField] private AudioClip removeSound;

    [Header("Hover Animation")]
    [SerializeField] private float bounceScale = 1.2f;
    [SerializeField] private float bounceDuration = 0.1f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    // Track the star's size in world space to keep it constant regardless
    // of resolution or parent scaling.
    private Vector3 startWorldScale;
    private Vector3 currentWorldScale;
    private Transform startParent;
    private bool dragging;
    private CardVisual currentFavourite;
    private Coroutine hoverRoutine;
    private DeckEditorManager deckEditorManager;
    private static readonly HashSet<string> BasicLandNames = new HashSet<string>
    {
        "Plains", "Island", "Swamp", "Mountain", "Forest"
    };

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        startParent = rectTransform.parent;
        startPosition = rectTransform.localPosition;
        // Cache the initial world-space scale so it can be restored later.
        startWorldScale = rectTransform.lossyScale;
        currentWorldScale = startWorldScale;
        deckEditorManager = FindObjectOfType<DeckEditorManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        if (audioSource != null && selectSound != null)
            audioSource.PlayOneShot(selectSound);
        rectTransform.SetParent(canvas.transform, true);
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;
        CardVisual target = null;
        if (eventData.pointerEnter != null)
            target = eventData.pointerEnter.GetComponentInParent<CardVisual>();

        if (target == null)
        {
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var res in results)
            {
                target = res.gameObject.GetComponentInParent<CardVisual>();
                if (target != null)
                    break;
            }
        }
        CardData data = null;
        if (target != null)
            data = CardDatabase.GetCardData(target.linkedCard.cardName);

        if (data != null && data.cardType == CardType.Land && BasicLandNames.Contains(data.cardName))
            target = null;

        if (target != null)
        {
            currentFavourite = target;
            rectTransform.SetParent(target.transform, true);
            rectTransform.localPosition = Vector3.zero;
            ApplyWorldScale(currentWorldScale);
            if (deckEditorManager != null)
                deckEditorManager.SetFavouriteCard(data);
            if (audioSource != null && attachSound != null)
                audioSource.PlayOneShot(attachSound);
        }
        else
        {
            ReturnToStart();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!dragging && currentFavourite != null)
            ReturnToStart();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverRoutine != null)
            StopCoroutine(hoverRoutine);
        hoverRoutine = StartCoroutine(BounceAnimation());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverRoutine != null)
        {
            StopCoroutine(hoverRoutine);
            hoverRoutine = null;
        }
        currentWorldScale = startWorldScale;
        ApplyWorldScale(currentWorldScale);
    }

    private System.Collections.IEnumerator BounceAnimation()
    {
        Vector3 big = startWorldScale * bounceScale;
        Vector3 small = startWorldScale * 0.9f;
        float t = 0f;
        while (t < bounceDuration)
        {
            currentWorldScale = Vector3.Lerp(startWorldScale, big, t / bounceDuration);
            ApplyWorldScale(currentWorldScale);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        currentWorldScale = big;
        ApplyWorldScale(currentWorldScale);
        t = 0f;
        while (t < bounceDuration)
        {
            currentWorldScale = Vector3.Lerp(big, small, t / bounceDuration);
            ApplyWorldScale(currentWorldScale);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        currentWorldScale = small;
        ApplyWorldScale(currentWorldScale);
        t = 0f;
        while (t < bounceDuration)
        {
            currentWorldScale = Vector3.Lerp(small, startWorldScale, t / bounceDuration);
            ApplyWorldScale(currentWorldScale);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        currentWorldScale = startWorldScale;
        ApplyWorldScale(currentWorldScale);
        hoverRoutine = null;
    }

    // Reset the favourite star back to its start position
    public void ReturnToStart()
    {
        rectTransform.SetParent(startParent, true);
        rectTransform.localPosition = startPosition;
        currentWorldScale = startWorldScale;
        ApplyWorldScale(currentWorldScale);
        if (currentFavourite != null && deckEditorManager != null)
            deckEditorManager.ClearFavourite();
        currentFavourite = null;
        if (audioSource != null && removeSound != null)
            audioSource.PlayOneShot(removeSound);
    }

    // Programmatically attach the favourite star to a card visual
    public void AttachToCard(CardVisual target)
    {
        if (target == null)
            return;

        rectTransform.SetParent(target.transform, true);
        rectTransform.localPosition = Vector3.zero;
        ApplyWorldScale(currentWorldScale);
        currentFavourite = target;

        if (deckEditorManager != null)
        {
            CardData data = CardDatabase.GetCardData(target.linkedCard.cardName);
            deckEditorManager.SetFavouriteCard(data);
        }
    }

    private void LateUpdate()
    {
        // Ensure the star maintains its intended world scale even if the
        // parent hierarchy or screen resolution changes.
        ApplyWorldScale(currentWorldScale);
    }

    private void ApplyWorldScale(Vector3 worldScale)
    {
        Vector3 parentScale = rectTransform.parent != null ? rectTransform.parent.lossyScale : Vector3.one;
        rectTransform.localScale = new Vector3(
            worldScale.x / parentScale.x,
            worldScale.y / parentScale.y,
            worldScale.z / parentScale.z);
    }
}
