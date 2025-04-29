using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string cardName;
    public int manaCost;
    public bool isTapped = false;

    public virtual void Play(Player player)
    {
        // By default, move to battlefield
        player.Battlefield.Add(this);
        player.Hand.Remove(this);
    }
}