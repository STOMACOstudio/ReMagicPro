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
    public ManaPool ColoredMana = new ManaPool();
    public int Life = 20;

    [System.Serializable]
    public class ManaPool
    {
        public int White = 0;
        public int Blue = 0;
        public int Black = 0;
        public int Red = 0;
        public int Green = 0;
        public int Colorless = 0;

        public void Clear()
            {
                White = Blue = Black = Red = Green = Colorless = 0;
            }

        public int Total()
            {
                return White + Blue + Black + Red + Green + Colorless;
            }

        public bool CanPay(Dictionary<string, int> cost)
            {
                int genericRequired = cost.ContainsKey("Generic") ? cost["Generic"] : 0;

                // Check specific colored mana requirements
                foreach (var kvp in cost)
                {
                    string color = kvp.Key;
                    int required = kvp.Value;

                    if (color == "Generic") continue;

                    int available = color switch
                    {
                        "White" => White,
                        "Blue" => Blue,
                        "Black" => Black,
                        "Red" => Red,
                        "Green" => Green,
                        "Colorless" => Colorless,
                        _ => 0
                    };

                    if (available < required)
                        return false;
                }

                // Now check if remaining mana can cover the generic requirement
                int leftover = White + Blue + Black + Red + Green + Colorless;

                foreach (var kvp in cost)
                {
                    if (kvp.Key == "Generic") continue;
                    leftover -= kvp.Value;
                }

                return leftover >= genericRequired;
            }

        public void Pay(Dictionary<string, int> cost)
            {
                int genericToSpend = cost.ContainsKey("Generic") ? cost["Generic"] : 0;

                // Pay specific colors first
                foreach (var kvp in cost)
                {
                    string color = kvp.Key;
                    int amount = kvp.Value;

                    if (color == "Generic") continue;

                    switch (color)
                    {
                        case "White": White -= amount; break;
                        case "Blue": Blue -= amount; break;
                        case "Black": Black -= amount; break;
                        case "Red": Red -= amount; break;
                        case "Green": Green -= amount; break;
                        case "Colorless": Colorless -= amount; break;
                    }
                }

                // Spend generic cost from any available mana
                SpendGeneric(genericToSpend);
            }

        private void SpendGeneric(int amount)
            {
                int remaining = amount;

                // Spend in fixed order
                if (remaining > 0) { int used = Mathf.Min(White, remaining); White -= used; remaining -= used; }
                if (remaining > 0) { int used = Mathf.Min(Blue, remaining); Blue -= used; remaining -= used; }
                if (remaining > 0) { int used = Mathf.Min(Black, remaining); Black -= used; remaining -= used; }
                if (remaining > 0) { int used = Mathf.Min(Red, remaining); Red -= used; remaining -= used; }
                if (remaining > 0) { int used = Mathf.Min(Green, remaining); Green -= used; remaining -= used; }
                if (remaining > 0) { int used = Mathf.Min(Colorless, remaining); Colorless -= used; remaining -= used; }

                if (remaining > 0)
                {
                    Debug.LogWarning("Tried to spend more generic mana than available. This should not happen if CanPay() was used.");
                }
            }
        }

    public void PlayCard(Card card)
        {
            if (!Hand.Contains(card)) return;

            Hand.Remove(card);

            if (card.entersTapped)
            {
                card.isTapped = true;
                Debug.Log($"{card.cardName} enters the battlefield tapped.");
            }
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

                Debug.Log($"{discarded.cardName} was randomly discarded.");
                GameManager.Instance.SendToGraveyard(discarded, this);
            }

            if (GameManager.Instance.enemyHandText != null && this == GameManager.Instance.aiPlayer)
                GameManager.Instance.enemyHandText.text = "Hand: " + Hand.Count;
        }
}