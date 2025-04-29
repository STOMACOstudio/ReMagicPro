using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandCard : Card
{
    public bool manaUsedThisTurn = false;

    public override void Play(Player player)
    {
        base.Play(player);
        Debug.Log(cardName + " played as a land.");
    }

    public void TapForMana(Player player)
    {
        if (!isTapped)
        {
            isTapped = true;
            player.ManaPool += 1;
            Debug.Log(cardName + " tapped for 1 mana.");
        }
    }
}