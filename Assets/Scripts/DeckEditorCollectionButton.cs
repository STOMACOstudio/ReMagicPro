using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeckEditorCollectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardData Data { get; private set; }
    private DeckEditorManager manager;

    public void Initialize(CardData data, DeckEditorManager mgr)
    {
        Data = data;
        manager = mgr;
        var btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        manager?.OnCollectionEntryClicked(Data, gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Data == null || CardHoverPreview.Instance == null)
            return;

        Card card = CardFactory.Create(Data.cardName);
        if (card != null)
            CardHoverPreview.Instance.ShowCard(card);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CardHoverPreview.Instance != null)
            CardHoverPreview.Instance.HidePreview();
    }
}
