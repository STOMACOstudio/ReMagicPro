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
    [Header("Color Filter Buttons")]
    [SerializeField] private Button whiteFilterButton;
    [SerializeField] private Button blueFilterButton;
    [SerializeField] private Button blackFilterButton;
    [SerializeField] private Button redFilterButton;
    [SerializeField] private Button greenFilterButton;
    [SerializeField] private Button colorlessFilterButton;

    private List<CardData> deck = new List<CardData>();
    private List<CardData> collection = new List<CardData>();

    // Currently selected color filters. "Colorless" represents the colorless/artifact button
    private HashSet<string> activeFilters = new HashSet<string>();

    void Start()
    {
        if (DeckHolder.SelectedDeck != null)
            deck = new List<CardData>(DeckHolder.SelectedDeck);
        ShowDeck();

        LoadRemovedList();

        SetupFilterButton(whiteFilterButton, "White");
        SetupFilterButton(blueFilterButton, "Blue");
        SetupFilterButton(blackFilterButton, "Black");
        SetupFilterButton(redFilterButton, "Red");
        SetupFilterButton(greenFilterButton, "Green");
        SetupFilterButton(colorlessFilterButton, "Colorless");
    }

    private void LoadRemovedList()
    {
        foreach (Transform child in removedListContainer)
            Destroy(child.gameObject);

        RefreshCollectionDisplay();
    }

    private bool CardMatchesFilters(CardData data)
    {
        if (activeFilters.Count == 0)
            return true;

        if (data == null)
            return false;

        List<string> colors = data.color ?? new List<string>();

        foreach (string filter in activeFilters)
        {
            if (filter == "Colorless")
            {
                bool isColorless = colors.Count == 0 || colors.Contains("Artifact");
                if (!isColorless)
                    return false;
            }
            else if (!colors.Contains(filter))
            {
                return false;
            }
        }

        return true;
    }

    private void RefreshCollectionDisplay()
    {
        foreach (Transform child in removedListContainer)
            Destroy(child.gameObject);

        IEnumerable<CardData> filtered = PlayerCollection.OwnedCards;
        if (activeFilters.Count > 0)
            filtered = PlayerCollection.OwnedCards.FindAll(CardMatchesFilters);

        collection = new List<CardData>(filtered);

        foreach (var data in collection)
        {
            GameObject entry = Instantiate(textPrefab, removedListContainer);
            TMP_Text text = entry.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = data.cardName;
        }

        UpdateRemovedButtons();
    }

    public void ToggleColorFilter(string color)
    {
        if (activeFilters.Contains(color))
            activeFilters.Remove(color);
        else
            activeFilters.Add(color);

        RefreshCollectionDisplay();
    }

    private void SetupFilterButton(Button button, string color)
    {
        if (button == null)
            return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => ToggleColorFilter(color));
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

        RefreshCollectionDisplay();
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
        RefreshCollectionDisplay();
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
