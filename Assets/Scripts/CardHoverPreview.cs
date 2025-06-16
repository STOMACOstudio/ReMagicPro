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
            currentPreview.transform.localScale = Vector3.one * 3f;

            RectTransform rt = currentPreview.GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = Vector2.zero;

            var visual = currentPreview.GetComponent<CardVisual>();
            CardData cardData = CardDatabase.GetCardData(card.cardName);

            visual.isInGraveyard = false;
            visual.transform.localScale = Vector3.one * 3f;

            visual.Setup(card, null, cardData);

            visual.transform.rotation = Quaternion.identity;
            if (visual.tapIcon != null)
                visual.tapIcon.SetActive(false);
        }

    public void HidePreview()
        {
            if (currentPreview != null)
            {
                Destroy(currentPreview);
            }
        }
}