using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [Header("Type Filter Buttons")]
    [SerializeField] private Button creatureFilterButton;
    [SerializeField] private Button sorceryFilterButton;
    [SerializeField] private Button enchantmentFilterButton;
    [Header("Filter Colors")]
    [SerializeField] private Color activeFilterColor = Color.yellow;

    [Header("Deck Count Display")]
    [SerializeField] private TMP_Text deckCardNumberText;
    [SerializeField] private Color deckValidColor = Color.black;
    [SerializeField] private Color deckInvalidColor = Color.red;

    private List<CardData> deck = new List<CardData>();
    private List<CardData> collection = new List<CardData>();

    // Card that has been marked as favourite in the editor
    public CardData FavouriteCard { get; private set; }

    // Map color names to their associated filter buttons for easy updates
    private readonly Dictionary<string, Button> filterButtons = new Dictionary<string, Button>();
    // Original button colors so we can revert when a filter is deactivated
    private readonly Dictionary<string, Color> originalButtonColors = new Dictionary<string, Color>();

    // Currently selected color filters. "Colorless" represents the colorless/artifact button
    private HashSet<string> activeFilters = new HashSet<string>();

    void Start()
    {
        if (DeckHolder.SelectedDeck != null)
            deck = new List<CardData>(DeckHolder.SelectedDeck);
        ShowDeck();

        if (!string.IsNullOrEmpty(DeckHolder.FavouriteCardName))
        {
            FavouriteCard = CardDatabase.GetCardData(DeckHolder.FavouriteCardName);
            AttachFavouriteStar();
        }

        LoadRemovedList();

        SetupFilterButton(whiteFilterButton, "White");
        SetupFilterButton(blueFilterButton, "Blue");
        SetupFilterButton(blackFilterButton, "Black");
        SetupFilterButton(redFilterButton, "Red");
        SetupFilterButton(greenFilterButton, "Green");
        SetupFilterButton(colorlessFilterButton, "Colorless");
        SetupFilterButton(creatureFilterButton, "Creature");
        SetupFilterButton(sorceryFilterButton, "Sorcery");
        SetupFilterButton(enchantmentFilterButton, "Enchantment");

        UpdateFilterButtonVisuals();

        UpdateDeckCardNumber();
    }

    private void ClearContainer(Transform container)
    {
        // Destroy is delayed until the end of the frame, so detach the object
        // first to ensure the child list is immediately empty. This avoids
        // stale children interfering with newly instantiated entries.
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
    }

    private void LoadRemovedList()
    {
        ClearContainer(removedListContainer);

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
            else if (filter == "Creature" || filter == "Sorcery" || filter == "Enchantment")
            {
                if (data.cardType.ToString() != filter)
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
        ClearContainer(removedListContainer);

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

        UpdateFilterButtonVisuals();

        RefreshCollectionDisplay();
    }

    private void SetupFilterButton(Button button, string color)
    {
        if (button == null)
            return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => ToggleColorFilter(color));

        filterButtons[color] = button;
        if (button.image != null)
            originalButtonColors[color] = button.image.color;
    }

    // Update button visuals to reflect which filters are currently active
    private void UpdateFilterButtonVisuals()
    {
        foreach (var kvp in filterButtons)
        {
            var btn = kvp.Value;
            if (btn == null)
                continue;

            Image img = btn.image;
            if (img == null)
                continue;

            if (activeFilters.Contains(kvp.Key))
                img.color = activeFilterColor;
            else if (originalButtonColors.TryGetValue(kvp.Key, out var orig))
                img.color = orig;
        }
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
#if UNITY_EDITOR
                : UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefab/CardPrefab.prefab");
#else
                : Resources.Load<GameObject>("Prefab/CardPrefab");
#endif
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

        var handler = go.AddComponent<DeckEditorCardButton>();
        handler.Initialize(data, this);
    }

    public void OnCardClicked(CardData data, GameObject visual)
    {
        if (!deck.Remove(data))
            return;

        // If the removed card was favourited, reset the favourite star
        if (FavouriteCard != null && FavouriteCard.cardName == data.cardName)
        {
            FavouriteCardManager star = FindObjectOfType<FavouriteCardManager>();
            if (star != null)
                star.ReturnToStart();
        }

        collection.Add(data);
        PlayerCollection.OwnedCards.Add(data);
        Destroy(visual);

        RefreshCollectionDisplay();
        UpdateDeckCardNumber();
    }

    public void OnCollectionEntryClicked(CardData data, GameObject entry)
    {
        // Try to remove the card from the local collection list. In some edge
        // cases the reference might not exist (for example after changing
        // filters), but we still want to allow adding the card back to the deck
        // even if the removal fails.
        collection.Remove(data);

        PlayerCollection.OwnedCards.Remove(data);
        Destroy(entry);

        GameObject prefab = cardPrefab;
        if (prefab == null)
        {
            prefab = CardHoverPreview.Instance != null
                ? CardHoverPreview.Instance.CardVisualPrefab
#if UNITY_EDITOR
                : UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefab/CardPrefab.prefab");
#else
                : Resources.Load<GameObject>("Prefab/CardPrefab");
#endif
        }

        deck.Add(data);
        SpawnCardVisual(prefab, data);
        RefreshCollectionDisplay();
        UpdateDeckCardNumber();
    }

    private void UpdateRemovedButtons()
    {
        for (int i = 0; i < removedListContainer.childCount; i++)
        {
            var entry = removedListContainer.GetChild(i);
            var handler = entry.gameObject.AddComponent<DeckEditorCollectionButton>();
            handler.Initialize(collection[i], this);
        }
    }

    private void UpdateDeckCardNumber()
    {
        if (deckCardNumberText == null)
            return;

        deckCardNumberText.text = $"{deck.Count}/40";
        deckCardNumberText.color = deck.Count < 40 ? deckInvalidColor : deckValidColor;
    }

    public void SetFavouriteCard(CardData data)
    {
        FavouriteCard = data;
    }

    public void ClearFavourite()
    {
        FavouriteCard = null;
    }

    public bool IsDeckComplete => deck.Count >= 40;

    public void ConfirmDeck()
    {
        DeckHolder.SelectedDeck = new List<CardData>(deck);
        DeckHolder.FavouriteCardName = FavouriteCard != null ? FavouriteCard.cardName : null;
        collection.Clear();
    }

    private void AttachFavouriteStar()
    {
        FavouriteCardManager star = FindObjectOfType<FavouriteCardManager>();
        if (star == null || FavouriteCard == null)
            return;

        foreach (Transform child in cardContainer)
        {
            CardVisual visual = child.GetComponent<CardVisual>();
            if (visual != null && visual.linkedCard != null && visual.linkedCard.cardName == FavouriteCard.cardName)
            {
                star.AttachToCard(visual);
                break;
            }
        }
    }
}
