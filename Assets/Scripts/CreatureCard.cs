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
    public KeywordAbility abilityToGain = KeywordAbility.Flying;
    public CreatureCard blockingThisAttacker;  // If this is a blocker
    public CreatureCard blockedByThisBlocker;  // If this is an attacker

    public override void Play(Player player)
    {
        var cost = GameManager.Instance.GetManaCostBreakdown(manaCost, color);
        if (player.ColoredMana.CanPay(cost))
        {
            player.ColoredMana.Pay(cost);
            base.Play(player);
            Debug.Log(cardName + " summoned to battlefield.");
        }
        else
        {
            Debug.Log("Not enough colored mana to summon " + cardName);
        }
    }
}