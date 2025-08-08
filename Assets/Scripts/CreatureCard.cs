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
    public int auraPowerBonus = 0;
    public int auraToughnessBonus = 0;
    public List<KeywordAbility> auraKeywordAbilities = new List<KeywordAbility>();

    public void RecalculateStats()
    {
        power = basePower + plusOneCounters - minusOneCounters + tempPowerBonus + auraPowerBonus;
        int effectiveDamage = keywordAbilities.Contains(KeywordAbility.Indestructible) ? 0 : damageTaken;
        toughness = baseToughness + plusOneCounters - minusOneCounters + tempToughnessBonus + auraToughnessBonus - effectiveDamage;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        damageTaken += amount;
        RecalculateStats();
    }

    public void ResetDamage()
    {
        if (damageTaken != 0)
        {
            damageTaken = 0;
            RecalculateStats();
        }
    }

    public void Kill()
    {
        if (!keywordAbilities.Contains(KeywordAbility.Indestructible))
        {
            damageTaken = baseToughness + plusOneCounters - minusOneCounters + tempToughnessBonus + auraToughnessBonus;
            RecalculateStats();
        }
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

    public void AddAuraBuff(int powerAmount, int toughnessAmount)
    {
        auraPowerBonus += powerAmount;
        auraToughnessBonus += toughnessAmount;
        RecalculateStats();
    }

    public void AddAuraKeyword(KeywordAbility ability)
    {
        if (!keywordAbilities.Contains(ability))
            keywordAbilities.Add(ability);
        if (!auraKeywordAbilities.Contains(ability))
            auraKeywordAbilities.Add(ability);
    }

    public void RemoveAuraBuff(int powerAmount, int toughnessAmount)
    {
        auraPowerBonus -= powerAmount;
        auraToughnessBonus -= toughnessAmount;
        RecalculateStats();
    }

    public void RemoveAuraKeyword(KeywordAbility ability)
    {
        if (auraKeywordAbilities.Contains(ability))
            auraKeywordAbilities.Remove(ability);
        if (keywordAbilities.Contains(ability))
            keywordAbilities.Remove(ability);
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

    public override void OnEnterPlay(Player owner)
    {
        foreach (var ability in abilities)
        {
            if (ability.timing != TriggerTiming.OnEnter)
                continue;

            if (ability.requiresTarget)
            {
                if (owner == GameManager.Instance.humanPlayer)
                    GameManager.Instance.BeginOptionalTargetSelectionAfterEntry(this, owner, ability);
                // AI-targeted ETBs handled separately in GameManager
            }
            else
            {
                GameManager.Instance.StartCoroutine(
                    GameManager.Instance.ResolveTriggeredAbilityOnStack(ability, owner, this, this));
            }
        }
    }

    public override void OnLeavePlay(Player owner)
    {
        base.OnLeavePlay(owner);
        // Clear combat state when this creature leaves the battlefield so
        // it won't retain old attack/block icons if it returns later.
        blockingThisAttacker = null;
        blockedByThisBlocker.Clear();
        plusOneCounters = 0;
        minusOneCounters = 0;
        ResetTemporaryBuff();
        ResetDamage();
        RecalculateStats();
    }
}
