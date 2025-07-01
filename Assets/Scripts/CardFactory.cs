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
                creature.basePower = data.power;
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
                sorcery.drawIfOpponentCantDiscard = data.drawIfOpponentCantDiscard;
                sorcery.eachPlayerGainLifeEqualToLands = data.eachPlayerGainLifeEqualToLands;
                sorcery.typeOfPermanentToDestroyAll = data.typeOfPermanentToDestroyAll;
                sorcery.exileAllCreaturesFromGraveyards = data.exileAllCreaturesFromGraveyards;
                sorcery.damageToEachCreatureAndPlayer = data.damageToEachCreatureAndPlayer;
                sorcery.manaToGainMin = data.manaToGainMin;
                sorcery.manaToGainMax = data.manaToGainMax;
                sorcery.swapGraveyardAndLibrary = data.swapGraveyardAndLibrary;
                sorcery.tokenToCreate = data.tokenToCreate;
                sorcery.numberOfTokensMin = data.numberOfTokensMin;
                sorcery.numberOfTokensMax = data.numberOfTokensMax;
                sorcery.requiresTarget = data.requiresTarget;
                sorcery.requiredTargetType = data.requiredTargetType;
                sorcery.damageToTarget = data.damageToTarget;
                sorcery.destroyTargetIfTypeMatches = data.destroyTargetIfTypeMatches;
                sorcery.destroyAllWithSameName = data.destroyAllWithSameName;
                sorcery.keywordToGrant = data.keywordToGrant;
                sorcery.requiredTargetColor = data.requiredTargetColor;
                sorcery.excludeArtifactCreatures = data.excludeArtifactCreatures;
                sorcery.buffPower = data.powerBuff;
                sorcery.buffToughness = data.toughnessBuff;
                newCard = sorcery;
                break;
            case CardType.Enchantment:
                EnchantmentCard enchantment = new EnchantmentCard();
                enchantment.buffPower = data.powerBuff;
                enchantment.buffToughness = data.toughnessBuff;
                enchantment.requiresTarget = data.requiresTarget;
                enchantment.requiredTargetType = data.requiredTargetType;
                newCard = enchantment;
                break;
            case CardType.Artifact:
                ArtifactCard artifact = new ArtifactCard();
                artifact.entersTapped = data.entersTapped;
                artifact.plagueAmount = data.plagueAmount;
                artifact.manaToGain = data.manaToGain;
                artifact.lifeToGain = data.lifeToGain;
                artifact.cardsToDraw = data.cardsToDraw;
                artifact.tokenToCreate = data.tokenToCreate;
                artifact.manaToPayToActivate = data.manaToPayToActivate;
                artifact.activatedAbilities = new List<ActivatedAbility>(data.activatedAbilities);
                artifact.keywordAbilities = data.keywordAbilities != null
                    ? new List<KeywordAbility>(data.keywordAbilities)
                    : new List<KeywordAbility>();
                artifact.damageToCreature = data.damageToCreature;
                artifact.buffPower = data.powerBuff;
                artifact.buffToughness = data.toughnessBuff;
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
        newCard.color = data.color != null ? new List<string>(data.color) : new List<string>();
        newCard.artwork = data.artwork;
        newCard.entersTapped = data.entersTapped;
        newCard.abilities = new List<CardAbility>(data.abilities);
        newCard.rulesText = data.rulesText;
        newCard.isToken = data.isToken;

        Debug.Log($"{newCard.cardName} created with {newCard.abilities.Count} abilities.");

        return newCard;
    }
}