using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentCard : Card
{
    // Enchantments may track counters for various effects.
    // "minusOneCounters" are used by some cards as generic counters.
    public int minusOneCounters = 0;

    // Tracks temporary aura-style buffs applied to creatures by enchantments
    // like Brotherhood so they can be properly removed when game state changes.
    public Dictionary<CreatureCard, int> brotherhoodBuffs = new Dictionary<CreatureCard, int>();

    public void AddMinusOneCounter()
    {
        minusOneCounters++;
    }
}
