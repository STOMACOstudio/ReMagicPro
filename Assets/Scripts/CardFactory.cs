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

        Card newCard = null;

        switch (data.cardType)
        {
            case CardType.Land:
                newCard = new LandCard();
                newCard.entersTapped = data.entersTapped;
                break;

            case CardType.Creature:
                CreatureCard creature = new CreatureCard();
                creature.power = data.power;
                creature.toughness = data.toughness;
                creature.baseToughness = data.toughness;
                creature.entersTapped = data.entersTapped;
                creature.tapLifeLossAmount = data.tapLifeLossAmount;
                creature.manaToPayToActivate = data.manaToPayToActivate;
                creature.tokenToCreate = data.tokenToCreate;
                creature.abilityToGain = data.abilityToGain;
                creature.keywordAbilities = data.keywordAbilities != null
                    ? new List<KeywordAbility>(data.keywordAbilities)
                    : new List<KeywordAbility>();
                creature.activatedAbilities = data.activatedAbilities != null
                    ? new List<ActivatedAbility>(data.activatedAbilities)
                    : new List<ActivatedAbility>();
                newCard = creature;
                break;
            case CardType.Sorcery:
                SorceryCard sorcery = new SorceryCard();
                sorcery.lifeToGain = data.lifeToGain;
                sorcery.lifeToLoseForOpponent = data.lifeToLoseForOpponent;
                sorcery.lifeLossForBothPlayers = data.lifeLossForBothPlayers;
                sorcery.cardsToDraw = data.cardsToDraw;
                sorcery.cardsToDiscardorDraw = data.cardsToDiscardorDraw;
                sorcery.eachPlayerGainLifeEqualToLands = data.eachPlayerGainLifeEqualToLands;
                sorcery.typeOfPermanentToDestroyAll = data.typeOfPermanentToDestroyAll;
                sorcery.exileAllCreaturesFromGraveyards = data.exileAllCreaturesFromGraveyards;
                sorcery.damageToEachCreatureAndPlayer = data.damageToEachCreatureAndPlayer;
                newCard = sorcery;
                break;
            case CardType.Artifact:
                ArtifactCard artifact = new ArtifactCard();
                artifact.entersTapped = data.entersTapped;
                artifact.plagueAmount = data.plagueAmount;
                artifact.manaToGain = data.manaToGain;
                artifact.lifeToGain = data.lifeToGain;
                artifact.cardsToDraw = data.cardsToDraw;
                artifact.manaToPayToActivate = data.manaToPayToActivate;
                artifact.activatedAbilities = new List<ActivatedAbility>(data.activatedAbilities);
                newCard = artifact;
                break;

            default:
                Debug.LogWarning("Unsupported card type: " + data.cardType);
                newCard = new Card(); // fallback
                break;
        }

        // Shared assignment
        newCard.cardName = data.cardName;
        newCard.manaCost = data.manaCost;
        newCard.artwork = data.artwork;
        newCard.entersTapped = data.entersTapped;
        newCard.abilities = new List<CardAbility>(data.abilities);
        newCard.isToken = data.isToken;

        Debug.Log($"{newCard.cardName} created with {newCard.abilities.Count} abilities.");

        return newCard;
    }
}