using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SorceryCard : Card
{
    public bool requiresTarget = false;
    public int lifeToLoseForOpponent = 0;
    public int lifeLossForBothPlayers = 0;
    public int cardsToDiscardorDraw = 0;
    public int damageToEachCreatureAndPlayer = 0;
    public bool eachPlayerGainLifeEqualToLands = false;
    public bool exileAllCreaturesFromGraveyards = false;
    public int numberOfTokensMin = 0;
    public int numberOfTokensMax = 0;
    public Card chosenTarget = null;
    public int damageToTarget = 0;
    public bool destroyTargetIfTypeMatches = false;
    public string requiredTargetColor = null;
    public Player chosenPlayerTarget = null;

    public TargetType requiredTargetType = TargetType.None;
    public PermanentTypeToDestroy typeOfPermanentToDestroyAll = PermanentTypeToDestroy.None;
    
    public enum TargetType
    {
        None,
        Creature,
        Land,
        Artifact,
        Player,
        CreatureOrPlayer
    }

    public enum PermanentTypeToDestroy
        {
            None,
            Land,
            Creature,
            Artifact,
            // Add more as needed later (Artifacts, Enchantments, etc.)
        }

    public virtual void ResolveEffect(Player caster)
        {
            bool didSomething = false;

            if (!string.IsNullOrEmpty(tokenToCreate) && numberOfTokensMax > 0)
            {
                int amount = (numberOfTokensMin == numberOfTokensMax)
                    ? numberOfTokensMin
                    : Random.Range(numberOfTokensMin, numberOfTokensMax + 1);
                for (int i = 0; i < amount; i++)
                {
                    Card token = CardFactory.Create(tokenToCreate);
                    if (token != null)
                    {
                        GameManager.Instance.SummonToken(token, caster);
                    }
                }

                Debug.Log($"Spawned {amount} {tokenToCreate} tokens.");
            }

            if (lifeToGain > 0)
            {
                caster.Life += lifeToGain;
                Debug.Log($"{caster} gains {lifeToGain} life.");

                GameManager.Instance.ShowFloatingHeal(
                    lifeToGain,
                    caster == GameManager.Instance.humanPlayer
                        ? GameManager.Instance.playerLifeContainer
                        : GameManager.Instance.enemyLifeContainer
                );

                didSomething = true;
            }
            if (lifeToLoseForOpponent > 0)
            {
                Player opponent = GameManager.Instance.GetOpponentOf(caster);
                opponent.Life -= lifeToLoseForOpponent;
                Debug.Log($"{opponent} loses {lifeToLoseForOpponent} life.");

                GameObject targetUI = (opponent == GameManager.Instance.humanPlayer)
                    ? GameManager.Instance.playerLifeContainer
                    : GameManager.Instance.enemyLifeContainer;

                GameManager.Instance.ShowFloatingDamage(lifeToLoseForOpponent, targetUI);
                GameManager.Instance.CheckForGameEnd();
                didSomething = true;
            }
            if (lifeLossForBothPlayers > 0)
            {
                GameManager.Instance.humanPlayer.Life -= lifeLossForBothPlayers;
                GameManager.Instance.aiPlayer.Life -= lifeLossForBothPlayers;
                Debug.Log($"Each player loses {lifeLossForBothPlayers} life.");

                GameManager.Instance.ShowFloatingDamage(lifeLossForBothPlayers, GameManager.Instance.playerLifeContainer);
                GameManager.Instance.ShowFloatingDamage(lifeLossForBothPlayers, GameManager.Instance.enemyLifeContainer);
                GameManager.Instance.CheckForGameEnd();
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

                    GameManager.Instance.ShowFloatingHeal(humanLands, GameManager.Instance.playerLifeContainer);
                    GameManager.Instance.ShowFloatingHeal(aiLands, GameManager.Instance.enemyLifeContainer);

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

                        GameObject targetUI = (player == GameManager.Instance.humanPlayer)
                            ? GameManager.Instance.playerLifeContainer
                            : GameManager.Instance.enemyLifeContainer;

                        GameManager.Instance.ShowFloatingDamage(damageToEachCreatureAndPlayer, targetUI);

                        // Damage to each creature
                        foreach (var creature in player.Battlefield.OfType<CreatureCard>())
                        {
                            KeywordAbility protectionKeyword = GetProtectionKeyword(this.color);

                            if (creature.keywordAbilities.Contains(protectionKeyword))
                            {
                                continue;
                            }

                            creature.toughness -= damageToEachCreatureAndPlayer;
                        }
                    }

                    GameManager.Instance.CheckDeaths(GameManager.Instance.humanPlayer);
                    GameManager.Instance.CheckDeaths(GameManager.Instance.aiPlayer);
                    GameManager.Instance.CheckForGameEnd();
                    didSomething = true;
                }
                GameManager.Instance.UpdateUI();
        }

        public virtual void ResolveEffect(Player caster, Card target)
            {
                if (target != null)
                {
                    Debug.Log($"{cardName} is resolving on target {target.cardName}.");

                    if (damageToTarget > 0 && target is CreatureCard creature)
                    {
                        KeywordAbility protection = GetProtectionKeyword(color);
                        if (creature.keywordAbilities.Contains(protection))
                        {
                            Debug.Log($"{creature.cardName} is protected from {color}, takes no damage.");
                        }
                        else
                        {
                            creature.toughness -= damageToTarget;
                            GameManager.Instance.CheckDeaths(GameManager.Instance.humanPlayer);
                            GameManager.Instance.CheckDeaths(GameManager.Instance.aiPlayer);
                        }

                        GameManager.Instance.UpdateUI();
                        ResolveEffect(caster); // Add this line
                        return;
                    }

                    if (destroyTargetIfTypeMatches)
                    {
                        bool typeMatches =
                            (requiredTargetType == TargetType.Creature && target is CreatureCard) ||
                            (requiredTargetType == TargetType.Land && target is LandCard) ||
                            (requiredTargetType == TargetType.Artifact && target is ArtifactCard);

                        bool colorMatches = true;

                        if (!string.IsNullOrEmpty(requiredTargetColor))
                        {
                            CardData data = CardDatabase.GetCardData(target.cardName);
                            colorMatches = data != null && data.color == requiredTargetColor;
                        }

                        if (typeMatches && colorMatches)
                        {
                            GameManager.Instance.SendToGraveyard(target, GameManager.Instance.GetOwnerOfCard(target));
                            Debug.Log($"{cardName} destroyed {target.cardName}.");

                            ResolveEffect(caster); // Add this line
                            return;
                        }
                        else
                        {
                            Debug.LogWarning($"{cardName} failed to destroy {target.cardName}: type match = {typeMatches}, color match = {colorMatches}");
                        }
                    }
                }

                if (!destroyTargetIfTypeMatches && damageToTarget <= 0)
                {
                    Debug.LogWarning($"{cardName} resolved on {target.cardName}, but did nothing.");
                }

                GameManager.Instance.UpdateUI();
                ResolveEffect(caster);
            }

        public KeywordAbility GetProtectionKeyword(string color)
            {
                return color switch
                {
                    "White" => KeywordAbility.ProtectionFromWhite,
                    "Blue" => KeywordAbility.ProtectionFromBlue,
                    "Black" => KeywordAbility.ProtectionFromBlack,
                    "Red" => KeywordAbility.ProtectionFromRed,
                    "Green" => KeywordAbility.ProtectionFromGreen,
                    _ => KeywordAbility.None
                };
            }
        
        public virtual void ResolveEffectOnPlayer(Player caster, Player targetPlayer)
        {
            if (requiredTargetType == TargetType.Player || requiredTargetType == TargetType.CreatureOrPlayer)
            {
                if (damageToTarget > 0)
                {
                    targetPlayer.Life -= damageToTarget;
                    Debug.Log($"{cardName} deals {damageToTarget} damage to {targetPlayer}.");

                    GameObject targetUI = (targetPlayer == GameManager.Instance.humanPlayer)
                        ? GameManager.Instance.playerLifeContainer
                        : GameManager.Instance.enemyLifeContainer;

                    GameManager.Instance.CheckForGameEnd();
                    GameManager.Instance.ShowFloatingDamage(damageToTarget, targetUI);
                }
            }

            GameManager.Instance.UpdateUI();
        }
}