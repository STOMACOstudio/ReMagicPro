using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentCard : Card
{
    // Enchantments may track counters for various effects.
    // "minusOneCounters" are used by some cards as generic counters.
    public int minusOneCounters = 0;

    public void AddMinusOneCounter()
    {
        minusOneCounters++;
    }
}
