using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckEditorCardButton : MonoBehaviour
{
    public CardData Data { get; private set; }
    private DeckEditorManager manager;

    public int Count { get; private set; } = 1;
    private TMP_Text countLabel;

    public void Initialize(CardData data, DeckEditorManager mgr, int count = 1)
    {
        Data = data;
        manager = mgr;
        Count = count;

        var btn = GetComponent<Button>();
        if (btn != null && manager != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }

        SetupLabel();
        UpdateLabel();
    }

    private void SetupLabel()
    {
        if (countLabel != null)
            return;

        GameObject labelObj = new GameObject("CountLabel");
        labelObj.transform.SetParent(transform, false);
        countLabel = labelObj.AddComponent<TextMeshProUGUI>();
        countLabel.alignment = TextAlignmentOptions.TopRight;
        countLabel.raycastTarget = false;
        RectTransform rt = countLabel.rectTransform;
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-10, -10);
    }

    public void Increment()
    {
        Count++;
        UpdateLabel();
    }

    public void Decrement()
    {
        Count--;
        UpdateLabel();
    }

    public void UpdateLabel()
    {
        if (countLabel != null)
            countLabel.text = Count > 1 ? Count.ToString() : string.Empty;
    }

    private void OnClick()
    {
        manager?.OnCardClicked(this);
    }
}
