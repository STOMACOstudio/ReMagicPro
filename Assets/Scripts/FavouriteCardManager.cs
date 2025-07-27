using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FavouriteCardManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private Canvas canvas; // Canvas used for dragging

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private Transform startParent;
    private bool dragging;
    private CardVisual currentFavourite;
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
        startParent = rectTransform.parent;
        startPosition = rectTransform.localPosition;
        deckEditorManager = FindObjectOfType<DeckEditorManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
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
            if (deckEditorManager != null)
                deckEditorManager.SetFavouriteCard(data);
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

    private void ReturnToStart()
    {
        rectTransform.SetParent(startParent, true);
        rectTransform.localPosition = startPosition;
        if (currentFavourite != null && deckEditorManager != null)
            deckEditorManager.ClearFavourite();
        currentFavourite = null;
    }
}
