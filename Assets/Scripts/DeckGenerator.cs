using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class DeckGenerator : MonoBehaviour
{
    private const int TotalDeckSize = 40;
    private const int NonLandCardCount = 23;
    private const int TotalLandCount = TotalDeckSize - NonLandCardCount;
    private const int DesiredBasicLandCopies = 17;

    private static readonly Dictionary<string, string> ColorToBasicLand = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "White", "Plains" },
        { "Blue", "Island" },
        { "Black", "Swamp" },
        { "Red", "Mountain" },
        { "Green", "Forest" }
    };

    public List<CardData> GeneratedDeck { get; private set; } = new List<CardData>();
    private System.Random rng = new System.Random();

    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardVisualPrefab;
    public TextMeshProUGUI rerollText;
    [SerializeField] private AudioClip rerollSound;
    private AudioSource audioSource;
    private int rerollsRemaining = 3;

    void Start()
        {
            Debug.Log("START DeckGenerator");
            audioSource = gameObject.AddComponent<AudioSource>();
            Generate();
            Debug.Log("DECK GENERATED");
            EnsureBasicLandsAvailable();
            DeckHolder.SelectedDeck = GeneratedDeck;
            ShowCardsInDeckBuilder();
            UpdateRerollText();
            Debug.Log("CARDS SHOWN");
        }

    [ContextMenu("Generate Deck")]
    public void Generate()
        {
            rng = new System.Random();
            string colorPref = PlayerPrefs.GetString("PlayerColors", "Red");
            if (string.IsNullOrEmpty(colorPref))
                colorPref = "Red";

            string[] chosenColors = colorPref.Split(',');

            var sanitizedColors = chosenColors
                .Select(c => c.Trim())
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .ToList();

            if (sanitizedColors.Count == 0)
                sanitizedColors.Add("Red");

            GeneratedDeck = new List<CardData>();

            int rareCount = 2;
            int uncommonCount = 7;
            int commonCount = Mathf.Max(0, NonLandCardCount - rareCount - uncommonCount);

            DistributeCardsByRarity(sanitizedColors, "Rare", rareCount);
            DistributeCardsByRarity(sanitizedColors, "Uncommon", uncommonCount);
            DistributeCardsByRarity(sanitizedColors, "Common", commonCount);

            AddBasicLandsToDeck(sanitizedColors);

            // Print result
            Debug.Log("Generated Deck:");
            foreach (var card in GeneratedDeck)
                Debug.Log(card.cardName);
        }

    private void DistributeCardsByRarity(IList<string> colors, string rarity, int totalCount)
        {
            if (colors == null || colors.Count == 0 || totalCount <= 0)
                return;

            int baseCount = totalCount / colors.Count;
            int remainder = totalCount % colors.Count;
            int remainingToAssign = totalCount;

            foreach (string color in colors)
            {
                int countForColor = baseCount;
                if (remainder > 0)
                {
                    countForColor++;
                    remainder--;
                }

                if (countForColor <= 0)
                    continue;

                int added = AddCardsByRarity(color, rarity, countForColor);
                remainingToAssign -= added;
            }

            int safety = 0;
            while (remainingToAssign > 0 && safety < 1000)
            {
                bool progress = false;
                foreach (string color in colors)
                {
                    if (remainingToAssign <= 0)
                        break;

                    int added = AddCardsByRarity(color, rarity, 1);
                    if (added > 0)
                    {
                        remainingToAssign -= added;
                        progress = true;
                    }
                }

                if (!progress)
                    break;

                safety++;
            }
        }

    private int AddCardsByRarity(string color, string rarity, int count)
        {
            string chosenColorsPref = PlayerPrefs.GetString("PlayerColors", "Red");
            if (string.IsNullOrEmpty(chosenColorsPref))
                chosenColorsPref = "Red";

            var chosenColorSet = new HashSet<string>(chosenColorsPref.Split(','));

            var pool = CardDatabase.GetAllCards()
                .Where(card =>
                    card.rarity == rarity &&
                    card.cardType != CardType.Land &&
                    (
                        (card.color.Contains(color) && card.color.All(c => chosenColorSet.Contains(c))) ||
                        card.cardType == CardType.Artifact ||
                        card.color.Contains("Artifact")
                    )
                )
                .ToList();

            Dictionary<string, int> copies = GeneratedDeck
                .GroupBy(c => c.cardName)
                .ToDictionary(g => g.Key, g => g.Count());

            int attempts = 0;
            int maxAttempts = 500;
            int added = 0;

            while (count > 0 && attempts < maxAttempts)
            {
                if (pool.Count == 0) break;

                CardData candidate = pool[rng.Next(pool.Count)];

                if (!copies.ContainsKey(candidate.cardName)) copies[candidate.cardName] = 0;
                if (copies[candidate.cardName] >= 4)
                {
                    pool.Remove(candidate); // avoid retrying same maxed-out card
                    attempts++;
                    continue;
                }

                GeneratedDeck.Add(candidate);
                copies[candidate.cardName]++;
                count--;
                attempts++;
                added++;
            }

            return added;
        }

    private void AddBasicLandsToDeck(IList<string> chosenColors)
    {
        if (chosenColors == null)
            return;

        var primaryColors = chosenColors
            .Where(color => ColorToBasicLand.ContainsKey(color))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (primaryColors.Count == 0)
            primaryColors.Add("Red");

        int baseCount = TotalLandCount / primaryColors.Count;
        int remainder = TotalLandCount % primaryColors.Count;

        foreach (string color in primaryColors)
        {
            int copies = baseCount;
            if (remainder > 0)
            {
                copies++;
                remainder--;
            }

            if (copies <= 0)
                continue;

            string landName = ColorToBasicLand[color];
            CardData landData = CardDatabase.GetCardData(landName);
            if (landData == null)
                continue;

            for (int i = 0; i < copies; i++)
                GeneratedDeck.Add(landData);
        }
    }

    public void ShowCardsInDeckBuilder()
        {
            foreach (Transform child in cardContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var cardData in GeneratedDeck)
            {
                Card card = CardFactory.Create(cardData.cardName);
                GameObject go = Instantiate(cardVisualPrefab, cardContainer);

                go.transform.localScale = Vector3.one * 1.5f;

                CardVisual visual = go.GetComponent<CardVisual>();
                CardData sourceData = CardDatabase.GetCardData(card.cardName);
                visual.Setup(card, null, sourceData);
            }
        }

    private void UpdateRerollText()
        {
            if (rerollText == null)
                return;

            if (rerollsRemaining > 0)
                rerollText.text = $"You can reroll a card {rerollsRemaining} times";
            else
                rerollText.text = "Out of rerolls";
        }

    public void RerollCard(int index)
        {
            if (rerollsRemaining <= 0 || index < 0 || index >= GeneratedDeck.Count)
                return;

            CardData original = GeneratedDeck[index];
            string rarity = original.rarity;

            string chosenColorsPref = PlayerPrefs.GetString("PlayerColors", "Red");
            if (string.IsNullOrEmpty(chosenColorsPref))
                chosenColorsPref = "Red";
            var chosenColorSet = new HashSet<string>(chosenColorsPref.Split(','));

            // pool of cards matching rarity and allowed colors
            var pool = CardDatabase.GetAllCards()
                .Where(c => c.rarity == rarity &&
                            c.cardType != CardType.Land &&
                            ((c.color.All(chosenColorSet.Contains) && c.color.Count > 0) ||
                             c.cardType == CardType.Artifact ||
                             c.color.Contains("Artifact")))
                .Where(c => c.cardName != original.cardName)
                .ToList();

            if (pool.Count == 0)
                return;

            GeneratedDeck[index] = pool[rng.Next(pool.Count)];
            rerollsRemaining--;
            if (rerollSound != null && audioSource != null)
                audioSource.PlayOneShot(rerollSound);
            ShowCardsInDeckBuilder();
            UpdateRerollText();
        }

    private void EnsureBasicLandsAvailable()
        {
            string[] basicLands = { "Plains", "Island", "Swamp", "Mountain", "Forest" };

            foreach (string landName in basicLands)
            {
                CardData landData = CardDatabase.GetCardData(landName);
                if (landData == null)
                    continue;

                int existing = PlayerCollection.OwnedCards.Count(card => card.cardName == landName);
                for (int i = existing; i < DesiredBasicLandCopies; i++)
                    PlayerCollection.OwnedCards.Add(landData);
            }
        }
}
