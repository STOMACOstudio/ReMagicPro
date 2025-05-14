using UnityEngine;

public class CardHoverPreview : MonoBehaviour
{
    public static CardHoverPreview Instance { get; private set; }

    [SerializeField] private GameObject cardVisualPrefab;
    [SerializeField] private Transform previewSlot;

    private GameObject currentPreview;

    void Awake()
    {
        Instance = this;
    }

    public void ShowCard(Card card)
    {
        HidePreview();

        currentPreview = Instantiate(cardVisualPrefab, previewSlot);

        // Scale the entire object, including its children
        currentPreview.transform.localScale = Vector3.one * 3f; // 2x size (or adjust to taste)

        // Reset its anchored position to zero so it appears in the center of the slot
        RectTransform rt = currentPreview.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
        }

        var visual = currentPreview.GetComponent<CardVisual>();
        CardData cardData = CardDatabase.GetCardData(card.cardName);

        // Ensure it's NOT treated like a graveyard card
        visual.isInGraveyard = false;
        visual.transform.localScale = Vector3.one * 3f; // scale AFTER flag is cleared

        visual.Setup(card, null, cardData);
    }

    public void HidePreview()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }
    }
}