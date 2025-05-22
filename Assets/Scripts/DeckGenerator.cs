using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement; // At the top of your script

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
            string chosenColor = PlayerPrefs.GetString("PlayerColor", "Red"); // fallback to Red
            GeneratedDeck = new List<CardData>();

            // 1. Add 16 basic lands
            var basicLand = CardDatabase.GetCardData(BasicLandNameForColor(chosenColor));
            for (int i = 0; i < 16; i++) GeneratedDeck.Add(basicLand);

            // 2. Add cards by rarity
            AddCardsByRarity(chosenColor, "Rare", 2);
            AddCardsByRarity(chosenColor, "Uncommon", 8);
            AddCardsByRarity(chosenColor, "Common", 14);

            // Print result
            Debug.Log("Generated Deck:");
            foreach (var card in GeneratedDeck)
            {
                Debug.Log(card.cardName);
            }
        }

    private void AddCardsByRarity(string color, string rarity, int count)
        {
            var pool = CardDatabase.GetAllCards()
                .Where(card =>
                    card.rarity == rarity &&
                    (
                        card.color == color || 
                        (card.color == "Artifact" && card.cardType == CardType.Creature)
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
                CardData sourceData = CardDatabase.GetCardData(card.cardName); // ✅ renamed here
                visual.Setup(card, null, sourceData);
            }
        }

}