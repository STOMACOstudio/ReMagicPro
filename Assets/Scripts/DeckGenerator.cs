using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DeckGenerator : MonoBehaviour
{
    public List<CardData> GeneratedDeck { get; private set; } = new List<CardData>();

    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardVisualPrefab;

    void Start()
        {
            Debug.Log("START DeckGenerator");
            Generate();
            Debug.Log("DECK GENERATED");
            DeckHolder.SelectedDeck = GeneratedDeck;
            ShowCardsInDeckBuilder();
            Debug.Log("CARDS SHOWN");
            //SceneManager.LoadScene("GameScene");
            // Uncomment this if you've added the UI later:
            // DisplayDeck();
        }

    [ContextMenu("Generate Deck")]
    public void Generate()
        {
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
                if (colorCounts.ContainsKey(card.color))
                    colorCounts[card.color]++;
            }

            // 3. Add 16 lands proportionally
            int totalColoredCards = colorCounts.Values.Sum();
            int landsToAdd = 16;
            foreach (string color in chosenColors)
            {
                int count = colorCounts[color];
                int landsForThisColor = Mathf.RoundToInt((count / (float)totalColoredCards) * landsToAdd);

                var basicLand = CardDatabase.GetCardData(BasicLandNameForColor(color));
                for (int i = 0; i < landsForThisColor; i++)
                    GeneratedDeck.Add(basicLand);
            }

            // Print result
            Debug.Log("Generated Deck:");
            foreach (var card in GeneratedDeck)
                Debug.Log(card.cardName);
        }

    private void AddCardsByRarity(string color, string rarity, int count)
        {
            var pool = CardDatabase.GetAllCards()
            .Where(card =>
                card.rarity == rarity &&
                (
                    card.color == color ||
                    card.cardType == CardType.Artifact
                )
            )
            .ToList();

            Dictionary<string, int> copies = GeneratedDeck
                .GroupBy(c => c.cardName)
                .ToDictionary(g => g.Key, g => g.Count());

            System.Random rng = new System.Random();
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

}