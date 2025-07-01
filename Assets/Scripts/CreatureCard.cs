using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCard : Card
{
    public int power;
    public int toughness; // <<< CURRENT TOUGHNESS (changing)
    public int basePower; // ORIGINAL POWER (never changes)
    public int baseToughness; // ORIGINAL TOUGHNESS (never changes)
    public int plusOneCounters = 0;
    public int minusOneCounters = 0;
    public int tempPowerBonus = 0;
    public int tempToughnessBonus = 0;
    public int enchantPowerBonus = 0;
    public int enchantToughnessBonus = 0;
    public List<EnchantmentCard> attachedEnchantments = new List<EnchantmentCard>();

    public void RecalculateStats()
    {
        power = basePower + plusOneCounters - minusOneCounters + tempPowerBonus + enchantPowerBonus;
        toughness = baseToughness + plusOneCounters - minusOneCounters + tempToughnessBonus + enchantToughnessBonus;
    }

    public void AddPlusOneCounter()
    {
        plusOneCounters++;
        RecalculateStats();
    }

    public void AddMinusOneCounter()
    {
        minusOneCounters++;
        RecalculateStats();
    }

    public void AddTemporaryBuff(int powerAmount, int toughnessAmount)
    {
        tempPowerBonus += powerAmount;
        tempToughnessBonus += toughnessAmount;
        RecalculateStats();
    }

    public void ResetTemporaryBuff()
    {
        if (tempPowerBonus != 0 || tempToughnessBonus != 0)
        {
            tempPowerBonus = 0;
            tempToughnessBonus = 0;
            RecalculateStats();
        }
    }

    public void AttachEnchantment(EnchantmentCard aura)
    {
        if (!attachedEnchantments.Contains(aura))
        {
            attachedEnchantments.Add(aura);
            enchantPowerBonus += aura.buffPower;
            enchantToughnessBonus += aura.buffToughness;
            RecalculateStats();
        }
    }

    public void DetachEnchantment(EnchantmentCard aura)
    {
        if (attachedEnchantments.Remove(aura))
        {
            enchantPowerBonus -= aura.buffPower;
            enchantToughnessBonus -= aura.buffToughness;
            RecalculateStats();
        }
    }
    public int damageTaken = 0;
    public int tapLifeLossAmount;
    public bool hasSummoningSickness = true;
    public KeywordAbility abilityToGain = KeywordAbility.Flying;
    public List<KeywordAbility> temporaryKeywordAbilities = new List<KeywordAbility>();
    public CreatureCard blockingThisAttacker;  // If this is a blocker
    public List<CreatureCard> blockedByThisBlocker = new List<CreatureCard>();  // If this is an attacker

    public override void Play(Player player)
    {
        var cost = GameManager.Instance.GetManaCostBreakdown(manaCost, color);
        int reduction = GameManager.Instance.GetCreatureCostReduction(player);
        CardData data = CardDatabase.GetCardData(cardName);
        if (data != null && data.subtypes.Contains("Beast"))
            reduction += GameManager.Instance.GetBeastCreatureCostReduction(player);
        if (reduction > 0 && cost.ContainsKey("Colorless"))
            cost["Colorless"] = Mathf.Max(0, cost["Colorless"] - reduction);
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

    public override void OnLeavePlay(Player owner)
    {
        foreach (var aura in new List<EnchantmentCard>(attachedEnchantments))
        {
            GameManager.Instance.SendToGraveyard(aura, aura.owner);
        }
        attachedEnchantments.Clear();

        base.OnLeavePlay(owner);
        plusOneCounters = 0;
        minusOneCounters = 0;
        ResetTemporaryBuff();
        RecalculateStats();
    }
}
