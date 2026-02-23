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
    private readonly List<string> basicLandNames = new List<string>
    {
        "Plains",
        "Island",
        "Swamp",
        "Mountain",
        "Forest"
    };

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
                if (filter == "Sorcery")
                {
                    if (data.cardType != CardType.Sorcery && data.cardType != CardType.Instant)
                        return false;
                }
                else if (data.cardType.ToString() != filter)
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

        IEnumerable<CardData> filtered = PlayerCollection.OwnedCards.FindAll(card => !CardData.IsBasicLand(card));
        if (activeFilters.Count > 0)
            filtered = PlayerCollection.OwnedCards.FindAll(card => !CardData.IsBasicLand(card) && CardMatchesFilters(card));

        collection = new List<CardData>(filtered);

        foreach (string basicLandName in basicLandNames)
        {
            CardData basicLand = CardDatabase.GetCardData(basicLandName);
            if (basicLand == null)
                continue;

            if (!CardMatchesFilters(basicLand))
                continue;

            collection.Add(basicLand);
        }

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

        GameObject prefab = ResolveCardPrefab();
        if (prefab == null)
        {
            Debug.LogError("DeckEditorManager could not load a card prefab.");
            return;
        }

        var groupedCards = new Dictionary<string, (CardData data, int count)>();
        var order = new List<string>();

        foreach (var data in deck)
        {
            if (groupedCards.TryGetValue(data.cardName, out var entry))
                groupedCards[data.cardName] = (entry.data, entry.count + 1);
            else
            {
                groupedCards[data.cardName] = (data, 1);
                order.Add(data.cardName);
            }
        }

        foreach (var name in order)
        {
            var entry = groupedCards[name];
            SpawnCardVisual(prefab, entry.data, entry.count);
        }
    }

    private void SpawnCardVisual(GameObject prefab, CardData data, int count)
    {
        if (prefab == null || data == null)
            return;

        Card card = CardFactory.Create(data.cardName);
        if (card == null)
        {
            Debug.LogError($"Failed to spawn deck editor card for '{data.cardName}'.");
            return;
        }

        GameObject go = Instantiate(prefab, cardContainer);
        go.transform.localScale = Vector3.one * 1.5f;
        CardVisual visual = go.GetComponent<CardVisual>();
        if (visual == null)
        {
            Debug.LogError("Card prefab is missing CardVisual component.");
            Destroy(go);
            return;
        }

        CardData sourceData = CardDatabase.GetCardData(card.cardName);
        visual.Setup(card, null, sourceData);

        var handler = go.AddComponent<DeckEditorCardButton>();
        handler.Initialize(data, this, count);
    }

    public void OnCardClicked(CardData data, GameObject visual)
    {
        var handler = visual.GetComponent<DeckEditorCardButton>();
        if (handler != null)
            OnCardClicked(handler);
    }

    public void OnCardClicked(DeckEditorCardButton button)
    {
        CardData data = button.Data;

        if (!deck.Remove(data))
            return;

        collection.Add(data);
        PlayerCollection.OwnedCards.Add(data);

        bool isFavourite = FavouriteCard != null && FavouriteCard.cardName == data.cardName;

        button.Decrement();
        if (button.Count <= 0)
        {
            if (isFavourite)
            {
                FavouriteCardManager star = UnityEngine.Object.FindFirstObjectByType<FavouriteCardManager>();
                if (star != null)
                    star.ReturnToStart();
            }
            Destroy(button.gameObject);
        }

        RefreshCollectionDisplay();
        UpdateDeckCardNumber();
    }

    public void OnCollectionEntryClicked(CardData data, GameObject entry)
    {
        if (data == null)
            return;

        bool isBasicLand = CardData.IsBasicLand(data);

        // Try to remove the card from the local collection list. In some edge
        // cases the reference might not exist (for example after changing
        // filters), but we still want to allow adding the card back to the deck
        // even if the removal fails.
        if (!isBasicLand)
            collection.Remove(data);

        if (!isBasicLand)
        {
            PlayerCollection.OwnedCards.Remove(data);
            Destroy(entry);
        }

        GameObject prefab = ResolveCardPrefab();
        if (prefab == null)
        {
            Debug.LogError("DeckEditorManager could not load a card prefab.");
            return;
        }

        deck.Add(data);

        DeckEditorCardButton existing = null;
        foreach (Transform child in cardContainer)
        {
            var handler = child.GetComponent<DeckEditorCardButton>();
            if (handler != null && handler.Data.cardName == data.cardName)
            {
                existing = handler;
                break;
            }
        }

        if (existing != null)
        {
            existing.Increment();
        }
        else
        {
            SpawnCardVisual(prefab, data, 1);
        }

        if (!isBasicLand)
            RefreshCollectionDisplay();

        UpdateDeckCardNumber();
    }

    private GameObject ResolveCardPrefab()
    {
        if (cardPrefab != null)
            return cardPrefab;

        if (CardHoverPreview.Instance != null && CardHoverPreview.Instance.CardVisualPrefab != null)
            return CardHoverPreview.Instance.CardVisualPrefab;

#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefab/CardPrefab.prefab");
#else
        return Resources.Load<GameObject>("Prefab/CardPrefab");
#endif
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
        FavouriteCardManager star = UnityEngine.Object.FindFirstObjectByType<FavouriteCardManager>();
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
