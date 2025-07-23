using UnityEngine;
using UnityEngine.UI;

public class DeckEditorCardButton : MonoBehaviour
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
        manager?.OnCardClicked(Data, gameObject);
    }
}
