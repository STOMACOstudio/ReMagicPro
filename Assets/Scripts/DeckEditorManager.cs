using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckEditorManager : MonoBehaviour
{
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform removedListContainer;
    [SerializeField] private GameObject textPrefab;

    private List<CardData> deck = new List<CardData>();
    private List<CardData> collection = new List<CardData>();

    void Start()
    {
        if (DeckHolder.SelectedDeck != null)
            deck = new List<CardData>(DeckHolder.SelectedDeck);
        ShowDeck();

        LoadRemovedList();
    }

    private void LoadRemovedList()
    {
        foreach (Transform child in removedListContainer)
            Destroy(child.gameObject);

        collection = new List<CardData>(PlayerCollection.OwnedCards);

        foreach (var data in collection)
        {
            GameObject entry = Instantiate(textPrefab, removedListContainer);
            TMP_Text text = entry.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = data.cardName;
        }

        UpdateRemovedButtons();
    }

    private void ShowDeck()
    {
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        GameObject prefab = cardPrefab;
        if (prefab == null)
        {
            prefab = CardHoverPreview.Instance != null
                ? CardHoverPreview.Instance.CardVisualPrefab
                : Resources.Load<GameObject>("Prefab/CardPrefab");
        }

        foreach (var data in deck)
            SpawnCardVisual(prefab, data);
    }

    private void SpawnCardVisual(GameObject prefab, CardData data)
    {
        Card card = CardFactory.Create(data.cardName);
        GameObject go = Instantiate(prefab, cardContainer);
        go.transform.localScale = Vector3.one * 1.5f;
        CardVisual visual = go.GetComponent<CardVisual>();
        CardData sourceData = CardDatabase.GetCardData(card.cardName);
        visual.Setup(card, null, sourceData);

        Button btn = go.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnCardClicked(visual));
    }

    public void OnCardClicked(CardVisual visual)
    {
        int index = visual.transform.GetSiblingIndex();
        if (index < 0 || index >= deck.Count)
            return;

        CardData data = deck[index];
        deck.RemoveAt(index);
        collection.Add(data);
        PlayerCollection.OwnedCards.Add(data);
        Destroy(visual.gameObject);

        GameObject entry = Instantiate(textPrefab, removedListContainer);
        TMP_Text text = entry.GetComponentInChildren<TMP_Text>();
        if (text != null)
            text.text = data.cardName;

        UpdateRemovedButtons();
    }

    public void OnTextClicked(int index)
    {
        if (index < 0 || index >= collection.Count)
            return;

        CardData data = collection[index];
        collection.RemoveAt(index);
        PlayerCollection.OwnedCards.Remove(data);
        Destroy(removedListContainer.GetChild(index).gameObject);

        GameObject prefab = cardPrefab;
        if (prefab == null)
        {
            prefab = CardHoverPreview.Instance != null
                ? CardHoverPreview.Instance.CardVisualPrefab
                : Resources.Load<GameObject>("Prefab/CardPrefab");
        }

        deck.Add(data);
        SpawnCardVisual(prefab, data);
        UpdateRemovedButtons();
    }

    private void UpdateRemovedButtons()
    {
        for (int i = 0; i < removedListContainer.childCount; i++)
        {
            int idx = i;
            Button btn = removedListContainer.GetChild(i).GetComponent<Button>();
            if (btn == null)
                continue;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnTextClicked(idx));
        }
    }

    public void ConfirmDeck()
    {
        DeckHolder.SelectedDeck = new List<CardData>(deck);
        collection.Clear();
    }
}
