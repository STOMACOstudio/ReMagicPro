using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement; // At the top of your script

public class DeckGenerator : MonoBehaviour
{
    public List<CardData> GeneratedDeck { get; private set; } = new List<CardData>();

    void Start()
    {
        Generate();
        DeckHolder.SelectedDeck = GeneratedDeck;
        SceneManager.LoadScene("GameScene");
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
            .Where(card => card.rarity == rarity && (card.color == color || card.color == "None"))
            .ToList();

        Dictionary<string, int> copies = GeneratedDeck
            .GroupBy(c => c.cardName)
            .ToDictionary(g => g.Key, g => g.Count());

        System.Random rng = new System.Random();

        while (count > 0 && pool.Count > 0)
        {
            CardData candidate = pool[rng.Next(pool.Count)];

            // Slightly reduce artifact chance
            if (candidate.color == "None" && rng.NextDouble() < 0.4) continue;

            if (!copies.ContainsKey(candidate.cardName)) copies[candidate.cardName] = 0;
            if (copies[candidate.cardName] >= 4) continue;

            GeneratedDeck.Add(candidate);
            copies[candidate.cardName]++;
            count--;
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
}