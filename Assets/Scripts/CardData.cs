using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    public string cardName;
    public string rarity; // e.g. "Common", "Uncommon", "Rare"
    public string color;
    public int manaCost;
    public CardType cardType;
    public Sprite artwork;

    // Only used for creatures
    public int power;
    public int toughness;
    public int tapLifeLossAmount;
    public bool entersTapped = false;
    public bool isToken = false;
    public string tokenToCreate;

    //noncreature artifacts
    public enum ArtifactEffect
        {
            None,
            TapForMana,
            TapToGainLife,
            TapAndSacrificeForMana,
        }

    public ArtifactEffect artifactEffect = ArtifactEffect.None;
    public int plagueAmount;
    public int cardsToDraw;
    public int manaToPayToActivate;

    //For sorceries
    public int lifeToGain;
    public int lifeToLoseForOpponent;
    public int lifeLossForBothPlayers;
    public int cardsToDiscardorDraw;
    public int damageToEachCreatureAndPlayer;
    public int manaToGain;
    public bool eachPlayerGainLifeEqualToLands;
    public bool exileAllCreaturesFromGraveyards = false;

    public SorceryCard.PermanentTypeToDestroy typeOfPermanentToDestroyAll = SorceryCard.PermanentTypeToDestroy.None;

    // Passive abilities like Haste, Defender
    public List<KeywordAbility> keywordAbilities = new List<KeywordAbility>();

    // Activated abilities like tap for mana
    public List<ActivatedAbility> activatedAbilities = new List<ActivatedAbility>();

    // Future: active effects like "on enter" or "on death"
    public List<CardAbility> abilities = new List<CardAbility>();
}