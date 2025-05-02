using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<Card> Deck = new List<Card>();
    public List<Card> Hand = new List<Card>();
    public List<Card> Battlefield = new List<Card>();
    public List<Card> Graveyard = new List<Card>();
    public bool hasPlayedLandThisTurn = false;
    public int ManaPool = 0;
    public int Life = 20;

    public void PlayCard(Card card)
        {
            if (!Hand.Contains(card)) return;

            Hand.Remove(card);
            Battlefield.Add(card);
            Debug.Log($"{card.cardName} is entering the battlefield.");
            card.OnEnterPlay(this);
        }

    public void SendToGraveyard(Card card)
        {
            Battlefield.Remove(card);
            Debug.Log($"{card.cardName} is being sent to the graveyard.");
            card.OnLeavePlay(this);
            Graveyard.Add(card);
        }
    public void DiscardRandomCard(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                if (Hand.Count == 0)
                {
                    Debug.Log("Player has no cards left to discard.");
                    return;
                }

                int index = Random.Range(0, Hand.Count);
                Card discarded = Hand[index];
                Hand.RemoveAt(index);

                Debug.Log($"{discarded.cardName} was randomly discarded.");
                GameManager.Instance.SendToGraveyard(discarded, this);
            }

            if (GameManager.Instance.enemyHandText != null && this == GameManager.Instance.aiPlayer)
                GameManager.Instance.enemyHandText.text = "Hand: " + Hand.Count;
        }
}