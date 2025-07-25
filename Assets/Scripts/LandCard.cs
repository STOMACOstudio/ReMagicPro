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
        if (!isTapped && GameManager.Instance.GetOwnerOfCard(this) == player)
        {
            GameManager.Instance.TapLandForMana(this, player);
        }
    }
}