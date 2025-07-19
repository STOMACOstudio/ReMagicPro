using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;

public class DeckGenerator : MonoBehaviour
{
    public List<CardData> GeneratedDeck { get; private set; } = new List<CardData>();
    private System.Random rng = new System.Random();

    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardVisualPrefab;
    public TextMeshProUGUI rerollText;
    private int rerollsRemaining = 3;

    void Start()
        {
            Debug.Log("START DeckGenerator");
            Generate();
            Debug.Log("DECK GENERATED");
            DeckHolder.SelectedDeck = GeneratedDeck;
            ShowCardsInDeckBuilder();
            UpdateRerollText();
            Debug.Log("CARDS SHOWN");
        }

    [ContextMenu("Generate Deck")]
    public void Generate()
        {
            rng = new System.Random();
            string colorPref = PlayerPrefs.GetString("PlayerColor", "Red");
            string[] chosenColors = colorPref.Split(',').Select(c => c.Trim()).ToArray();

            // Fallback to single Red if empty
            if (chosenColors.Length == 0) chosenColors = new[] { "Red" };

            GeneratedDeck = new List<CardData>();

            // 1. Add cards by rarity for each color
            foreach (string color in chosenColors)
            {
                AddCardsByRarity(color, "Rare", 2 / chosenColors.Length);
                AddCardsByRarity(color, "Uncommon", 8 / chosenColors.Length);
                AddCardsByRarity(color, "Common", 14 / chosenColors.Length);
            }

            // 2. Count how many cards of each color were actually added (excluding artifacts)
            Dictionary<string, int> colorCounts = chosenColors.ToDictionary(c => c, c => 0);
            foreach (var card in GeneratedDeck)
            {
                foreach (string c in card.color)
                {
                    if (colorCounts.ContainsKey(c))
                        colorCounts[c]++;
                }
            }

            // 3. Add 16 lands proportionally
            int totalColoredCards = colorCounts.Values.Sum();
            int landsToAdd = 16;
            Dictionary<string, int> landsPerColor = new Dictionary<string, int>();

            int totalAdded = 0;
            foreach (string color in chosenColors)
            {
                int count = colorCounts[color];
                int share = Mathf.RoundToInt((count / (float)totalColoredCards) * landsToAdd);
                landsPerColor[color] = share;
                totalAdded += share;
            }

            // Adjust if over 16 due to rounding
            while (totalAdded > 16)
            {
                foreach (string color in chosenColors)
                {
                    if (landsPerColor[color] > 0 && totalAdded > 16)
                    {
                        landsPerColor[color]--;
                        totalAdded--;
                    }
                }
            }

            // Fill up to exactly 16 if needed
            while (totalAdded < 16)
            {
                foreach (string color in chosenColors)
                {
                    landsPerColor[color]++;
                    totalAdded++;
                    if (totalAdded == 16) break;
                }
            }

            // Now add the lands
            foreach (var kvp in landsPerColor)
            {
                var land = CardDatabase.GetCardData(BasicLandNameForColor(kvp.Key));
                for (int i = 0; i < kvp.Value; i++)
                    GeneratedDeck.Add(land);
            }

            // Print result
            Debug.Log("Generated Deck:");
            foreach (var card in GeneratedDeck)
                Debug.Log(card.cardName);
        }

    private void AddCardsByRarity(string color, string rarity, int count)
        {
            string[] chosenColors = PlayerPrefs.GetString("PlayerColor", "Red")
                                        .Split(',')
                                        .Select(c => c.Trim())
                                        .ToArray();

            var chosenColorSet = new HashSet<string>(chosenColors);

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
            }
        }

    private string BasicLandNameForColor(string color)
        {
            return color switch
            {
                "White" => "Plains",
                "Blue" => "Island",
                "Black" => "Swamp",
                "Red" => "Mountain",
                "Green" => "Forest",
                _ => "Plains"
            };
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
            List<string> colors = original.color;

            // pool of cards matching rarity and sharing at least one color
            var pool = CardDatabase.GetAllCards()
                .Where(c => c.rarity == rarity &&
                            c.cardType != CardType.Land &&
                            (c.color.Intersect(colors).Any() || c.color.Contains("Artifact")))
                .ToList();

            if (pool.Count == 0)
                pool = CardDatabase.GetAllCards().Where(c => c.rarity == rarity).ToList();

            if (pool.Count == 0)
                return;

            GeneratedDeck[index] = pool[rng.Next(pool.Count)];
            rerollsRemaining--;
            ShowCardsInDeckBuilder();
            UpdateRerollText();
        }

}