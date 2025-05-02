using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SorceryCard : Card
{
    public int lifeToGain = 0;
    public int lifeToLoseForOpponent = 0;
    public int lifeLossForBothPlayers = 0;
    public int cardsToDraw = 0;
    public int cardsToDiscard = 0;

    public virtual void ResolveEffect(Player caster)
        {
            bool didSomething = false;

            if (lifeToGain > 0)
                {
                    caster.Life += lifeToGain;
                    Debug.Log($"{caster} gains {lifeToGain} life.");
                    didSomething = true;
                }

            if (lifeToLoseForOpponent > 0)
                {
                    Player opponent = GameManager.Instance.GetOpponentOf(caster);
                    opponent.Life -= lifeToLoseForOpponent;
                    Debug.Log($"{opponent} loses {lifeToLoseForOpponent} life.");
                    didSomething = true;
                }

            if (lifeLossForBothPlayers > 0)
                {
                    GameManager.Instance.humanPlayer.Life -= lifeLossForBothPlayers;
                    GameManager.Instance.aiPlayer.Life -= lifeLossForBothPlayers;
                    Debug.Log($"Each player loses {lifeLossForBothPlayers} life.");
                    didSomething = true;
                }
            if (cardsToDraw > 0)
                {
                    for (int i = 0; i < cardsToDraw; i++)
                        GameManager.Instance.DrawCard(caster);

                    Debug.Log($"{caster} draws {cardsToDraw} card(s).");
                    didSomething = true;
                }
            if (cardsToDiscard > 0)
                {
                    Player opponent = GameManager.Instance.GetOpponentOf(caster);

                    for (int i = 0; i < cardsToDiscard; i++)
                    {
                        if (opponent.Hand.Count > 0)
                        {
                            Card toDiscard = opponent.Hand[Random.Range(0, opponent.Hand.Count)];
                            opponent.Hand.Remove(toDiscard);
                            GameManager.Instance.SendToGraveyard(toDiscard, opponent);
                            Debug.Log($"{opponent} discarded {toDiscard.cardName}");
                        }
                        else
                        {
                            Debug.Log($"{opponent} has no cards to discard.");
                        }
                    }
                    didSomething = true;
                }

            if (!didSomething)
            {
                Debug.LogWarning("ResolveEffect() called, but no effect was defined.");
            }

            GameManager.Instance.UpdateUI();
        }
}