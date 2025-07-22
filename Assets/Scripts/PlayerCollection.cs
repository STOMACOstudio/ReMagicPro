using System.Collections.Generic;
using UnityEngine;

public static class PlayerCollection
{
    public static List<CardData> OwnedCards = new List<CardData>();

    /// <summary>
    /// Adds a random card from the CardDatabase to the player's collection.
    /// </summary>
    public static void AddRandomCard()
    {
        var allCards = new List<CardData>(CardDatabase.GetAllCards());
        if (allCards.Count == 0)
            return;

        int index = Random.Range(0, allCards.Count);
        OwnedCards.Add(allCards[index]);
    }
}
