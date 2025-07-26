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
                sorcery.returnRandomCreatureFromGraveyard = data.returnRandomCreatureFromGraveyard;
                sorcery.returnRandomCheapCreatureToBattlefield = data.returnRandomCheapCreatureToBattlefield;
                sorcery.maxManaCostForReturn = data.maxManaCostForReturn;
                sorcery.tokenToCreate = data.tokenToCreate;
                sorcery.numberOfTokensMin = data.numberOfTokensMin;
                sorcery.numberOfTokensMax = data.numberOfTokensMax;
                sorcery.cardsToDrawMin = data.cardsToDrawMin;
                sorcery.cardsToDrawMax = data.cardsToDrawMax;
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
                sorcery.addXPlusOneCounters = data.addXPlusOneCounters;
                sorcery.addXMinusOneCounters = data.addXMinusOneCounters;
                newCard = sorcery;
                break;
            case CardType.Artifact:
                ArtifactCard artifact;
                if (data.subtypes != null && data.subtypes.Contains("Equipment"))
                {
                    EquipmentCard equip = new EquipmentCard();
                    artifact = equip;
                }
                else
                {
                    artifact = new ArtifactCard();
                }

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
            case CardType.Enchantment:
                EnchantmentCard enchantment;
                if (data.subtypes != null && data.subtypes.Contains("Aura"))
                    enchantment = new AuraCard();
                else
                    enchantment = new EnchantmentCard();
                enchantment.entersTapped = data.entersTapped;
                enchantment.plagueAmount = data.plagueAmount;
                enchantment.manaToGain = data.manaToGain;
                enchantment.lifeToGain = data.lifeToGain;
                enchantment.cardsToDraw = data.cardsToDraw;
                enchantment.tokenToCreate = data.tokenToCreate;
                enchantment.manaToPayToActivate = data.manaToPayToActivate;
                enchantment.activatedAbilities = new List<ActivatedAbility>(data.activatedAbilities);
                enchantment.keywordAbilities = data.keywordAbilities != null
                    ? new List<KeywordAbility>(data.keywordAbilities)
                    : new List<KeywordAbility>();
                enchantment.damageToCreature = data.damageToCreature;
                enchantment.buffPower = data.powerBuff;
                enchantment.buffToughness = data.toughnessBuff;
                enchantment.keywordBuff = data.keywordBuff;
                if (enchantment is AuraCard aura)
                    aura.requiredTargetType =
                        data.requiredTargetType == SorceryCard.TargetType.None
                            ? SorceryCard.TargetType.Creature
                            : data.requiredTargetType;
                if (enchantment is AuraCard aura2)
                    aura2.targetMustBeControlledCreature = data.targetMustBeControlledCreature;
                newCard = enchantment;
                break;

            default:
                Debug.LogWarning("Unsupported card type: " + data.cardType);
                newCard = new Card(); // fallback
                break;
        }

        // Shared assignment
        newCard.cardName = data.cardName;
        newCard.manaCost = data.manaCost;
        newCard.hasXCost = data.hasXCost;
        newCard.color = data.color != null ? new List<string>(data.color) : new List<string>();
        newCard.subtypes = data.subtypes != null ? new List<string>(data.subtypes) : new List<string>();
        newCard.artwork = data.artwork;
        newCard.entersTapped = data.entersTapped;
        newCard.abilities = new List<CardAbility>(data.abilities);
        newCard.rulesText = data.rulesText;
        newCard.flavorText = data.flavorText;
        newCard.isToken = data.isToken;
        newCard.keywordBuff = data.keywordBuff;

        Debug.Log($"{newCard.cardName} created with {newCard.abilities.Count} abilities.");

        return newCard;
    }}