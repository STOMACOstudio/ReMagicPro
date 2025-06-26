using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    public string cardName;
    public string rarity;
    public int manaCost;
    public CardType cardType;
    public Sprite artwork;

    public List<string> color = new List<string>();
    public string PrimaryColor => color.Count > 0 ? color[0] : "None";

    // Only used for creatures
    public string tokenToCreate;
    public string requiredTargetColor = null;

    public string rulesText;

    public bool entersTapped = false;
    public bool isToken = false;
    public bool destroyTargetIfTypeMatches = false;
    
    public int numberOfTokensMin = 0;
    public int numberOfTokensMax = 0;   
    public int power;
    public int toughness;
    public int tapLifeLossAmount;
    public int damageToCreature = 0;

    public KeywordAbility abilityToGain = KeywordAbility.None;
    public List<string> subtypes = new List<string>(); // e.g., "Human", "Warrior"

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
    public bool swapGraveyardAndLibrary = false;
    public bool requiresTarget = false;
    public SorceryCard.TargetType requiredTargetType = SorceryCard.TargetType.None;
    public int damageToTarget = 0;

    public KeywordAbility keywordAbilityForTarget = KeywordAbility.None;

    public SorceryCard.PermanentTypeToDestroy typeOfPermanentToDestroyAll = SorceryCard.PermanentTypeToDestroy.None;

    // Passive abilities like Haste, Defender
    public List<KeywordAbility> keywordAbilities = new List<KeywordAbility>();

    // Activated abilities like tap for mana
    public List<ActivatedAbility> activatedAbilities = new List<ActivatedAbility>();

    // Future: active effects like "on enter" or "on death"
    public List<CardAbility> abilities = new List<CardAbility>();
}