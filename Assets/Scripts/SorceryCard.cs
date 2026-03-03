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
    public bool drawIfOpponentCantDiscard = true;
    public int damageToEachCreatureAndPlayer = 0;
    public int creaturesToSacrificeEachPlayerMin = 0;
    public int creaturesToSacrificeEachPlayerMax = 0;
    public int manaToGainMin = 0;
    public int manaToGainMax = 0;
    public bool eachPlayerGainLifeEqualToLands = false;
    public bool exileAllCreaturesFromGraveyards = false;
    public bool swapGraveyardAndLibrary = false;
    public bool revealUntilCreature = false;
    public bool revealUntilLand = false;
    public bool searchRandomBasicLandToBattlefieldTapped = false;
    public bool returnRandomCreatureFromGraveyard = false;
    public bool returnRandomCheapCreatureToBattlefield = false;
    public bool returnTargetCreatureToOwnerHand = false;
    public int maxManaCostForReturn = 0;
    public int numberOfTokensMin = 0;
    public int numberOfTokensMax = 0;
    public int cardsToDrawMin = 0;
    public int cardsToDrawMax = 0;
    public Card chosenTarget = null;
    public int damageToTarget = 0;
    public int damageToTargetMin = 0;
    public int damageToTargetMax = 0;
    public bool destroyTargetIfTypeMatches = false;
    public bool destroyAllWithSameName = false;
    public KeywordAbility keywordToGrant = KeywordAbility.None;
    public string requiredTargetColor = null;
    public string excludedTargetColor = null;
    public bool canTargetArtifactCreatures = false;
    public bool excludeArtifactCreatures = false;
    public bool requireNonTokenTarget = false;
    public Player chosenPlayerTarget = null;
    public bool addXPlusOneCounters = false;
    public bool addXMinusOneCounters = false;
    public int controlledCreaturesPowerBuff = 0;
    public int controlledCreaturesToughnessBuff = 0;

    public TargetType requiredTargetType = TargetType.None;
    public PermanentTypeToDestroy typeOfPermanentToDestroyAll = PermanentTypeToDestroy.None;
    
    public enum TargetType
    {
        None,
        Creature,
        TappedCreature,
        Land,
        Artifact,
        Enchantment,
        Player,
        CreatureOrPlayer
    }

    public enum PermanentTypeToDestroy
        {
            None,
            Land,
            Creature,
            Artifact,
            Enchantment,
            // Add more as needed later (Artifacts, Enchantments, etc.)
        }

    public bool IsValidArtifactTarget(Card target)
    {
        return target is ArtifactCard ||
               (canTargetArtifactCreatures && target is CreatureCard creature && creature.color.Contains("Artifact"));
    }

    public virtual void ResolveEffect(Player caster)
        {
            if (revealUntilCreature)
            {
                GameManager.Instance.pendingStackEffects++;
                GameManager.Instance.StartCoroutine(GameManager.Instance.RevealUntilCreature(caster));
            }

            if (revealUntilLand)
            {
                GameManager.Instance.pendingStackEffects++;
                GameManager.Instance.StartCoroutine(GameManager.Instance.RevealUntilLand(caster));
            }

            if (searchRandomBasicLandToBattlefieldTapped)
            {
                GameManager.Instance.SearchLibraryForRandomBasicLandToBattlefieldTapped(caster);
            }

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

            if (returnRandomCreatureFromGraveyard)
            {
                GameManager.Instance.ReturnRandomCreatureFromGraveyard(caster);
            }

            if (returnRandomCheapCreatureToBattlefield)
            {
                GameManager.Instance.ReturnRandomCreatureFromGraveyardToBattlefield(caster, maxManaCostForReturn);
            }

            if (lifeToGain > 0)
            {
                GameManager.Instance.TryGainLife(caster, lifeToGain);
                Debug.Log($"{caster} gains {lifeToGain} life.");
            }
            if (manaToGainMax > 0)
            {
                int amount = (manaToGainMin == manaToGainMax)
                    ? manaToGainMin
                    : Random.Range(manaToGainMin, manaToGainMax + 1);

                switch (PrimaryColor)
                {
                    case "White": caster.ColoredMana.White += amount; break;
                    case "Blue": caster.ColoredMana.Blue += amount; break;
                    case "Black": caster.ColoredMana.Black += amount; break;
                    case "Red": caster.ColoredMana.Red += amount; break;
                    case "Green": caster.ColoredMana.Green += amount; break;
                    default: caster.ColoredMana.Colorless += amount; break;
                }

                Debug.Log($"{caster} gains {amount} {PrimaryColor} mana.");
                GameManager.Instance.UpdateUI();
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
            }
            if (lifeLossForBothPlayers > 0)
            {
                GameManager.Instance.humanPlayer.Life -= lifeLossForBothPlayers;
                GameManager.Instance.aiPlayer.Life -= lifeLossForBothPlayers;
                Debug.Log($"Each player loses {lifeLossForBothPlayers} life.");

                GameManager.Instance.ShowFloatingDamage(lifeLossForBothPlayers, GameManager.Instance.playerLifeContainer);
                GameManager.Instance.ShowFloatingDamage(lifeLossForBothPlayers, GameManager.Instance.enemyLifeContainer);
                GameManager.Instance.CheckForGameEnd();
            }
            if (cardsToDrawMax > 0)
            {
                int amount = (cardsToDrawMin == cardsToDrawMax)
                    ? cardsToDrawMin
                    : Random.Range(cardsToDrawMin, cardsToDrawMax + 1);
                GameManager.Instance.DrawCards(caster, amount);
                Debug.Log($"{caster} draws {amount} card(s).");
            }
            else if (cardsToDraw > 0)
            {
                GameManager.Instance.DrawCards(caster, cardsToDraw);
                Debug.Log($"{caster} draws {cardsToDraw} card(s).");
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
                            GameManager.Instance.SendToGraveyard(toDiscard, opponent);
                            Debug.Log($"{opponent} discarded {toDiscard.cardName}");
                            opponentDiscarded = true;
                        }
                        else
                        {
                            Debug.Log($"{opponent} has no cards to discard.");
                        }
                }

                if (!opponentDiscarded && drawIfOpponentCantDiscard)
                {
                    GameManager.Instance.DrawCard(caster);
                    Debug.Log($"{caster} draws a card because opponent had nothing to discard.");
                }

                }

            if (controlledCreaturesPowerBuff != 0 || controlledCreaturesToughnessBuff != 0)
            {
                foreach (CreatureCard creature in caster.Battlefield.OfType<CreatureCard>())
                {
                    creature.AddTemporaryBuff(controlledCreaturesPowerBuff, controlledCreaturesToughnessBuff);
                    var visual = GameManager.Instance.FindCardVisual(creature);
                    if (visual != null)
                        visual.UpdateVisual();
                }

                Debug.Log($"{cardName} gives your creatures +{controlledCreaturesPowerBuff}/+{controlledCreaturesToughnessBuff} until end of turn.");
            }

            if (creaturesToSacrificeEachPlayerMax > 0)
            {
                List<(Card card, Player owner)> sacrifices = new List<(Card, Player)>();
                foreach (var player in new[] { GameManager.Instance.humanPlayer, GameManager.Instance.aiPlayer })
                {
                    int amount = (creaturesToSacrificeEachPlayerMin == creaturesToSacrificeEachPlayerMax)
                        ? creaturesToSacrificeEachPlayerMin
                        : Random.Range(creaturesToSacrificeEachPlayerMin, creaturesToSacrificeEachPlayerMax + 1);

                    var creatures = player.Battlefield.OfType<CreatureCard>().ToList();
                    if (amount > creatures.Count)
                        amount = creatures.Count;

                    for (int i = 0; i < amount; i++)
                    {
                        var chosen = creatures[Random.Range(0, creatures.Count)];
                        creatures.Remove(chosen);
                        sacrifices.Add((chosen, player));
                    }
                }

                foreach (var (card, owner) in sacrifices)
                {
                    GameManager.Instance.SendToGraveyard(card, owner);
                }

            }
            if (eachPlayerGainLifeEqualToLands)
                {
                    Player human = GameManager.Instance.humanPlayer;
                    Player ai = GameManager.Instance.aiPlayer;

                    int humanLands = human.Battlefield.Count(card => card is LandCard);
                    int aiLands = ai.Battlefield.Count(card => card is LandCard);

                    GameManager.Instance.TryGainLife(human, humanLands);
                    GameManager.Instance.TryGainLife(ai, aiLands);

                    Debug.Log($"Each player gains life equal to their own lands. Human: +{humanLands}, AI: +{aiLands}");
                }
            if (typeOfPermanentToDestroyAll != PermanentTypeToDestroy.None)
                {
                    List<(Card card, Player owner)> destroyedCards = new List<(Card, Player)>();

                    foreach (var player in new[] { GameManager.Instance.humanPlayer, GameManager.Instance.aiPlayer })
                    {
                        var targets = player.Battlefield
                            .Where(card =>
                            {
                                if (card.keywordAbilities.Contains(KeywordAbility.Indestructible))
                                    return false;

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
                                        if (data != null && data.color.Contains("Artifact"))
                                            return true;
                                    }
                                }

                                if (typeOfPermanentToDestroyAll == PermanentTypeToDestroy.Enchantment && card is EnchantmentCard)
                                    return true;

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
                            KeywordAbility protection = ProtectionUtils.GetProtectionKeyword(this.PrimaryColor);

                            if (creature.keywordAbilities.Contains(protection))
                            {
                                continue;
                            }

                            creature.TakeDamage(damageToEachCreatureAndPlayer);
                        }
                    }

                    GameManager.Instance.CheckDeaths(GameManager.Instance.humanPlayer);
                    GameManager.Instance.CheckDeaths(GameManager.Instance.aiPlayer);
                    GameManager.Instance.CheckForGameEnd();
                }
            if (swapGraveyardAndLibrary)
                {
                    foreach (var player in new[] { GameManager.Instance.humanPlayer, GameManager.Instance.aiPlayer })
                    {
                        List<Card> oldDeck = new List<Card>(player.Deck);
                        player.Deck = new List<Card>(player.Graveyard);
                        player.Graveyard = oldDeck;

                        for (int i = 0; i < player.Deck.Count; i++)
                        {
                            Card temp = player.Deck[i];
                            int rand = Random.Range(i, player.Deck.Count);
                            player.Deck[i] = player.Deck[rand];
                            player.Deck[rand] = temp;
                        }

                        GameManager.Instance.RefreshGraveyardVisuals(player);
                    }

                    Debug.Log("Graveyards and libraries swapped and shuffled.");
                }
                GameManager.Instance.UpdateUI();
        }

        public virtual void ResolveEffect(Player caster, Card target)
        {
            int dmg = 0; // Declare outside so it's visible throughout the method

            if (target != null)
            {
                dmg = damageToTargetMax > 0
                    ? (damageToTargetMin == damageToTargetMax
                        ? damageToTargetMin
                        : Random.Range(damageToTargetMin, damageToTargetMax + 1))
                    : damageToTarget;

                if (dmg > 0 && target is CreatureCard creature)
                {
                    KeywordAbility protection = ProtectionUtils.GetProtectionKeyword(PrimaryColor);
                    if (creature.keywordAbilities.Contains(protection))
                    {
                        Debug.Log($"{creature.cardName} is protected from {color}, takes no damage.");
                    }
                    else
                    {
                        creature.TakeDamage(dmg);
                        GameManager.Instance.CheckDeaths(GameManager.Instance.humanPlayer);
                        GameManager.Instance.CheckDeaths(GameManager.Instance.aiPlayer);
                    }

                    GameManager.Instance.UpdateUI();
                    ResolveEffect(caster);
                    return;
                }

                if (destroyAllWithSameName && target is CreatureCard)
                {
                    string name = target.cardName;
                    List<(Card card, Player owner)> toDestroy = new List<(Card, Player)>();
                    foreach (var player in new[] { GameManager.Instance.humanPlayer, GameManager.Instance.aiPlayer })
                    {
                        foreach (var card in player.Battlefield.OfType<CreatureCard>()
                            .Where(c => c.cardName == name && !c.keywordAbilities.Contains(KeywordAbility.Indestructible))
                            .ToList())
                        {
                            toDestroy.Add((card, player));
                        }
                    }
                    foreach (var (card, owner) in toDestroy)
                    {
                        GameManager.Instance.SendToGraveyard(card, owner);
                    }

                    Debug.Log($"{cardName} destroyed {toDestroy.Count} copies of {name}.");

                    ResolveEffect(caster);
                    return;
                }

                if (returnTargetCreatureToOwnerHand && target is CreatureCard)
                {
                    Player owner = GameManager.Instance.GetOwnerOfCard(target);
                    if (owner != null && owner.Battlefield.Remove(target))
                    {
                        owner.Hand.Add(target);

                        CardVisual targetVisual = GameManager.Instance.FindCardVisual(target);
                        if (targetVisual != null)
                        {
                            GameManager.Instance.activeCardVisuals.Remove(targetVisual);
                            Object.Destroy(targetVisual.gameObject);
                        }

                        if (owner == GameManager.Instance.humanPlayer)
                        {
                            GameObject obj = Object.Instantiate(GameManager.Instance.cardPrefab, GameManager.Instance.playerHandArea);
                            CardVisual visual = obj.GetComponent<CardVisual>();
                            CardData sourceData = CardDatabase.GetCardData(target.cardName);
                            visual.Setup(target, GameManager.Instance, sourceData);
                            GameManager.Instance.activeCardVisuals.Add(visual);
                        }

                        Debug.Log($"{cardName} returned {target.cardName} to its owner's hand.");
                        ResolveEffect(caster);
                        return;
                    }
                }

                if (destroyTargetIfTypeMatches)
                {
                    bool typeMatches =
                        (requiredTargetType == TargetType.Creature && target is CreatureCard targetCreature &&
                            !(excludeArtifactCreatures && targetCreature.color.Contains("Artifact"))) ||
                        (requiredTargetType == TargetType.Land && target is LandCard) ||
                        (requiredTargetType == TargetType.Artifact && IsValidArtifactTarget(target)) ||
                        (requiredTargetType == TargetType.Enchantment && target is EnchantmentCard);

                    bool colorMatches = true;

                    if (!string.IsNullOrEmpty(requiredTargetColor))
                    {
                        CardData data = CardDatabase.GetCardData(target.cardName);
                        colorMatches = data != null && data.color.Contains(requiredTargetColor);
                    }

                    if (!string.IsNullOrEmpty(excludedTargetColor))
                    {
                        CardData data = CardDatabase.GetCardData(target.cardName);
                        colorMatches = colorMatches && (data == null || !data.color.Contains(excludedTargetColor));
                    }

                    if (typeMatches && colorMatches)
                    {
                        if (target.keywordAbilities.Contains(KeywordAbility.Indestructible))
                        {
                            Debug.Log($"{cardName} failed to destroy {target.cardName}: indestructible.");
                        }
                        else
                        {
                            GameManager.Instance.SendToGraveyard(target, GameManager.Instance.GetOwnerOfCard(target));
                            Debug.Log($"{cardName} destroyed {target.cardName}.");
                        }

                        ResolveEffect(caster);
                        return;
                    }
                    else
                    {
                        Debug.LogWarning($"{cardName} failed to destroy {target.cardName}: type match = {typeMatches}, color match = {colorMatches}");
                    }
                }
            }

            if (keywordToGrant != KeywordAbility.None && target is CreatureCard keywordCreature)
            {
                if (!keywordCreature.keywordAbilities.Contains(keywordToGrant))
                    keywordCreature.keywordAbilities.Add(keywordToGrant);

                if (!keywordCreature.temporaryKeywordAbilities.Contains(keywordToGrant))
                    keywordCreature.temporaryKeywordAbilities.Add(keywordToGrant);

                if (keywordToGrant == KeywordAbility.Haste)
                    keywordCreature.hasSummoningSickness = false;

                var visual = GameManager.Instance.FindCardVisual(keywordCreature);
                if (visual != null)
                    visual.UpdateVisual();

                Debug.Log($"{keywordCreature.cardName} gains {keywordToGrant} until end of turn.");
            }

            if ((buffPower != 0 || buffToughness != 0) && target is CreatureCard buffCreature)
            {
                buffCreature.AddTemporaryBuff(buffPower, buffToughness);
                var visual = GameManager.Instance.FindCardVisual(buffCreature);
                if (visual != null)
                    visual.UpdateVisual();

                Debug.Log($"{buffCreature.cardName} gets +{buffPower}/+{buffToughness} until end of turn.");
            }

            if (addXPlusOneCounters && target is CreatureCard plusTarget && xValue > 0)
            {
                for (int i = 0; i < xValue; i++)
                    plusTarget.AddPlusOneCounter();

                var visual = GameManager.Instance.FindCardVisual(plusTarget);
                if (visual != null)
                    visual.UpdateVisual();

                Debug.Log($"{plusTarget.cardName} receives {xValue} +1/+1 counters.");
            }

            if (addXMinusOneCounters && target is CreatureCard minusTarget && xValue > 0)
            {
                for (int i = 0; i < xValue; i++)
                    minusTarget.AddMinusOneCounter();

                var visual = GameManager.Instance.FindCardVisual(minusTarget);
                if (visual != null)
                    visual.UpdateVisual();

                GameManager.Instance.CheckDeaths(GameManager.Instance.humanPlayer);
                GameManager.Instance.CheckDeaths(GameManager.Instance.aiPlayer);

                Debug.Log($"{minusTarget.cardName} receives {xValue} -1/-1 counters.");
            }
            else if (!destroyTargetIfTypeMatches && dmg <= 0 && keywordToGrant == KeywordAbility.None)
            {
                Debug.LogWarning($"{cardName} resolved on {target?.cardName ?? "null"}, but did nothing.");
            }

            GameManager.Instance.UpdateUI();
            ResolveEffect(caster);
        }


        
        public virtual void ResolveEffectOnPlayer(Player caster, Player targetPlayer)
        {
            if (requiredTargetType == TargetType.Player || requiredTargetType == TargetType.CreatureOrPlayer)
            {
                int dmg = damageToTargetMax > 0
                    ? (damageToTargetMin == damageToTargetMax
                        ? damageToTargetMin
                        : Random.Range(damageToTargetMin, damageToTargetMax + 1))
                    : damageToTarget;

                if (dmg > 0)
                {
                    targetPlayer.Life -= dmg;
                    Debug.Log($"{cardName} deals {dmg} damage to {targetPlayer}.");

                    GameObject targetUI = (targetPlayer == GameManager.Instance.humanPlayer)
                        ? GameManager.Instance.playerLifeContainer
                        : GameManager.Instance.enemyLifeContainer;

                    GameManager.Instance.CheckForGameEnd();
                    GameManager.Instance.ShowFloatingDamage(dmg, targetUI);
                }
            }

            GameManager.Instance.UpdateUI();
            ResolveEffect(caster);
        }
}
