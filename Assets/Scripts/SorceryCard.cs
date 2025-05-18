using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SorceryCard : Card
{
    //public int lifeToGain = 0;
    public int lifeToLoseForOpponent = 0;
    public int lifeLossForBothPlayers = 0;
    public int cardsToDiscardorDraw = 0;
    public int damageToEachCreatureAndPlayer = 0;
    public bool eachPlayerGainLifeEqualToLands = false;
    public bool exileAllCreaturesFromGraveyards = false;

    public enum PermanentTypeToDestroy
        {
            None,
            Land,
            Creature,
            Artifact,
            // Add more as needed later (Artifacts, Enchantments, etc.)
        }

public PermanentTypeToDestroy typeOfPermanentToDestroyAll = PermanentTypeToDestroy.None;

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
            if (cardsToDiscardorDraw > 0)
                {
                    Player opponent = GameManager.Instance.GetOpponentOf(caster);
                    bool opponentDiscarded = false;

                    for (int i = 0; i < cardsToDiscardorDraw; i++)
                    {
                        if (opponent.Hand.Count > 0)
                        {
                            Card toDiscard = opponent.Hand[Random.Range(0, opponent.Hand.Count)];
                            opponent.Hand.Remove(toDiscard);
                            GameManager.Instance.SendToGraveyard(toDiscard, opponent);
                            Debug.Log($"{opponent} discarded {toDiscard.cardName}");
                            opponentDiscarded = true;
                        }
                        else
                        {
                            Debug.Log($"{opponent} has no cards to discard.");
                        }
                    }

                    if (!opponentDiscarded)
                    {
                        GameManager.Instance.DrawCard(caster);
                        Debug.Log($"{caster} draws a card because opponent had nothing to discard.");
                    }

                    didSomething = true;
                }
            if (eachPlayerGainLifeEqualToLands)
                {
                    Player human = GameManager.Instance.humanPlayer;
                    Player ai = GameManager.Instance.aiPlayer;

                    int humanLands = human.Battlefield.Count(card => card is LandCard);
                    int aiLands = ai.Battlefield.Count(card => card is LandCard);

                    human.Life += humanLands;
                    ai.Life += aiLands;

                    Debug.Log($"Each player gains life equal to their own lands. Human: +{humanLands}, AI: +{aiLands}");
                    didSomething = true;
                }
                if (typeOfPermanentToDestroyAll != PermanentTypeToDestroy.None)
                {
                    List<(Card card, Player owner)> destroyedCards = new List<(Card, Player)>();

                    foreach (var player in new[] { GameManager.Instance.humanPlayer, GameManager.Instance.aiPlayer })
                    {
                        var targets = player.Battlefield
                            .Where(card =>
                            {
                                if (typeOfPermanentToDestroyAll == PermanentTypeToDestroy.Land && card is LandCard)
                                    return true;

                                if (typeOfPermanentToDestroyAll == PermanentTypeToDestroy.Creature && card is CreatureCard)
                                    return true;

                                if (typeOfPermanentToDestroyAll == PermanentTypeToDestroy.Artifact)
                                {
                                    if (card is ArtifactCard)
                                        return true;

                                    if (card is CreatureCard)
                                    {
                                        var data = CardDatabase.GetCardData(card.cardName);
                                        if (data != null && data.color == "Artifact")
                                            return true;
                                    }
                                }

                                return false;
                            })
                            .ToList();

                        foreach (var card in targets)
                        {
                            destroyedCards.Add((card, player));
                        }
                    }

                    foreach (var (card, owner) in destroyedCards)
                    {
                        GameManager.Instance.SendToGraveyard(card, owner);
                    }

                    Debug.Log($"Destroyed all {typeOfPermanentToDestroyAll}s: {string.Join(", ", destroyedCards.Select(c => c.card.cardName))}");
                    didSomething = true;
                }
            if (exileAllCreaturesFromGraveyards)
                {
                    List<Card> exiledCards = new List<Card>();

                    foreach (var player in new[] { GameManager.Instance.humanPlayer, GameManager.Instance.aiPlayer })
                    {
                        var toRemove = player.Graveyard
                            .Where(c => c is CreatureCard)
                            .ToList();

                        foreach (var card in toRemove)
                        {
                            player.Graveyard.Remove(card);

                            CardVisual visual = GameManager.Instance.FindCardVisual(card);
                            if (visual != null)
                            {
                                GameManager.Instance.activeCardVisuals.Remove(visual);
                                GameObject.Destroy(visual.gameObject);
                            }

                            exiledCards.Add(card);
                        }
                    }

                    Debug.Log($"Exiled creatures from graveyards: {string.Join(", ", exiledCards.Select(c => c.cardName))}");
                    didSomething = true;
                }
            if (damageToEachCreatureAndPlayer > 0)
                {
                    foreach (var player in new[] { GameManager.Instance.humanPlayer, GameManager.Instance.aiPlayer })
                    {
                        // Damage to player
                        player.Life -= damageToEachCreatureAndPlayer;

                        // Damage to each creature
                        foreach (var creature in player.Battlefield.OfType<CreatureCard>())
                        {
                            creature.toughness -= damageToEachCreatureAndPlayer;
                            Debug.Log($"{creature.cardName} takes {damageToEachCreatureAndPlayer} damage.");
                        }
                    }

                    GameManager.Instance.CheckDeaths(GameManager.Instance.humanPlayer);
                    GameManager.Instance.CheckDeaths(GameManager.Instance.aiPlayer);

                    Debug.Log($"Fire Spirals deals {damageToEachCreatureAndPlayer} damage to all creatures and players.");
                    didSomething = true;
                }

            if (!didSomething)
            {
                Debug.LogWarning("ResolveEffect() called, but no effect was defined.");
            }

            GameManager.Instance.UpdateUI();
        }
}