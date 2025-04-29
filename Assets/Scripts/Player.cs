using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<Card> Deck = new List<Card>();
    public List<Card> Hand = new List<Card>();
    public List<Card> Battlefield = new List<Card>();
    public List<Card> Graveyard = new List<Card>();
    public int ManaPool = 0;
    public int Life = 20;

    public void PlayCard(Card card)
    {
        if (!Hand.Contains(card)) return;

        Hand.Remove(card);
        Battlefield.Add(card);
    }

    public void SendToGraveyard(Card card)
    {
        Battlefield.Remove(card);
        Graveyard.Add(card);
    }
}