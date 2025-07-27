using System.Collections.Generic;
using UnityEngine;

public static class PlayerCollection
{
    public static List<CardData> OwnedCards = new List<CardData>();

    /// <summary>
    /// Adds a random card from the CardDatabase to the player's collection.
    /// </summary>
    public static CardData AddRandomCard()
    {
        var allCards = new List<CardData>(CardDatabase.GetAllCards());
        if (allCards.Count == 0)
            return null;

        // Filter out token cards and basic lands from the pool of potential rewards
        var filtered = allCards.FindAll(card =>
            !card.isToken &&
            !(card.cardType == CardType.Land &&
              (card.cardName == "Plains" ||
               card.cardName == "Island" ||
               card.cardName == "Swamp" ||
               card.cardName == "Mountain" ||
               card.cardName == "Forest")));

        if (filtered.Count == 0)
            return null;

        int index = Random.Range(0, filtered.Count);
        var card = filtered[index];
        OwnedCards.Add(card);
        return card;
    }
}
