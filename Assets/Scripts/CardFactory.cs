using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardFactory
{
    public static Card Create(string cardName)
    {
        CardData data = CardDatabase.GetCardData(cardName);
        if (data == null)
        {
            Debug.LogError("Failed to create card: " + cardName);
            return null;
        }

        Card newCard;

        switch (data.cardType)
        {
            case CardType.Land:
                newCard = new LandCard();
                break;

            case CardType.Creature:
                CreatureCard creature = new CreatureCard();
                creature.power = data.power;
                creature.toughness = data.toughness;
                creature.baseToughness = data.toughness;
                creature.keywordAbilities = new List<KeywordAbility>(data.keywordAbilities);
                newCard = creature;
                newCard.abilities = new List<CardAbility>(data.abilities);
                Debug.Log($"{newCard.cardName} created with {newCard.abilities.Count} abilities.");
                break;

            default:
                Debug.LogWarning("Unsupported card type: " + data.cardType);
                newCard = new Card(); // fallback
                break;
        }

        newCard.cardName = data.cardName;
        newCard.manaCost = data.manaCost;
        newCard.artwork = data.artwork;
        newCard.entersTapped = data.entersTapped;

        newCard.abilities = new List<CardAbility>(data.abilities);

        return newCard;
    }
}