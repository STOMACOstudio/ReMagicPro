using UnityEngine;
using UnityEngine.EventSystems;

public class FavouriteCardManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private Canvas canvas; // Canvas used for dragging

    private RectTransform rectTransform;
    private Vector3 startPosition;
    private Transform startParent;
    private bool dragging;
    private CardVisual currentFavourite;
    private DeckEditorManager deckEditorManager;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        startParent = rectTransform.parent;
        startPosition = rectTransform.localPosition;
        deckEditorManager = FindObjectOfType<DeckEditorManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        rectTransform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        var target = eventData.pointerEnter == null ? null : eventData.pointerEnter.GetComponentInParent<CardVisual>();
        if (target != null)
        {
            currentFavourite = target;
            rectTransform.SetParent(target.transform, true);
            rectTransform.localPosition = Vector3.zero;
            if (deckEditorManager != null)
            {
                CardData data = CardDatabase.GetCardData(target.linkedCard.cardName);
                deckEditorManager.SetFavouriteCard(data);
            }
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
