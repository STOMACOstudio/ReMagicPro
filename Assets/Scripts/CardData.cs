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

    // Only used for creatures
    public int power;
    public int toughness;
    public bool entersTapped = false;

    // Passive abilities like Haste, Defender
    public List<KeywordAbility> keywordAbilities = new List<KeywordAbility>();

    // Art
    public Sprite artwork;

    // Future: active effects like "on enter" or "on death"
    public List<CardAbility> abilities = new List<CardAbility>();
}