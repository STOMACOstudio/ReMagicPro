using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCard : Card
{
    public int power;
    public int toughness; // <<< CURRENT TOUGHNESS (changing)
    public int baseToughness; // <<< ORIGINAL TOUGHNESS (never changes)
    public int damageTaken = 0;
    public int tapLifeLossAmount;
    public bool hasSummoningSickness = true;

    public CreatureCard blockingThisAttacker;  // If this is a blocker
    public CreatureCard blockedByThisBlocker;  // If this is an attacker

    public List<KeywordAbility> keywordAbilities = new List<KeywordAbility>();

    public override void Play(Player player)
    {
        if (player.ManaPool >= manaCost)
        {
            player.ManaPool -= manaCost;
            base.Play(player);
            Debug.Log(cardName + " summoned to battlefield.");
        }
        else
        {
            Debug.Log("Not enough mana to summon " + cardName);
        }
    }
}