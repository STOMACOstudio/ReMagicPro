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
    public bool entersTapped = false;
    public bool isToken = false;

    //For sorceries
    public int lifeToGain; // Optional, default to 0
    public int lifeToLoseForOpponent;
    public int lifeLossForBothPlayers;
    public int cardsToDraw;
    public int cardsToDiscardorDraw;
    public bool eachPlayerGainLifeEqualToLands;
    public int damageToEachCreatureAndPlayer;
    public bool exileAllCreaturesFromGraveyards = false;

    public SorceryCard.PermanentTypeToDestroy typeOfPermanentToDestroyAll = SorceryCard.PermanentTypeToDestroy.None;

    // Passive abilities like Haste, Defender
    public List<KeywordAbility> keywordAbilities = new List<KeywordAbility>();

    // Activated abilities like tap for mana
    public List<ActivatedAbility> activatedAbilities = new List<ActivatedAbility>();

    // Future: active effects like "on enter" or "on death"
    public List<CardAbility> abilities = new List<CardAbility>();
}