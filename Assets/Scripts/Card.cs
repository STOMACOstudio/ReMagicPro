using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string cardName;
    public string rarity;
    public string color;
    public int manaCost;
    public bool isToken = false;
    public bool isTapped = false;
    public bool entersTapped = false;

    public int plagueAmount;
    public int manaToGain;
    public int lifeToGain;
    public int manaToPayToActivate;
    public int cardsToDraw;
    public int damageToCreature;

    public string tokenToCreate;

    public Sprite artwork;

    public Player owner;

    public List<CardAbility> abilities = new List<CardAbility>();
    public List<ActivatedAbility> activatedAbilities = new List<ActivatedAbility>();
    public List<KeywordAbility> keywordAbilities = new List<KeywordAbility>();

    public virtual void Play(Player player)
        {
            if (entersTapped)
            {
                isTapped = true;
                Debug.Log($"{cardName} enters tapped.");
            }

            player.PlayCard(this);
        }

    public virtual void OnEnterPlay(Player owner)
        {
            foreach (var ability in abilities)
            {
                if (ability.timing != TriggerTiming.OnEnter)
                    continue;

                if (ability.requiresTarget)
                {
                    GameManager.Instance.BeginOptionalTargetSelectionAfterEntry(this, owner, ability);
                }
                else
                {
                    ability.effect?.Invoke(owner, null);
                }
            }
        }

    public virtual void OnLeavePlay(Player owner)
        {
            foreach (var ability in abilities)
            {
                if (ability.timing == TriggerTiming.OnDeath)
                    ability.effect?.Invoke(owner, null);
            }
        }

    public virtual string GetCardText()
        {
            List<string> lines = new List<string>();

            // Keyword abilities — only for creatures
            if (this is CreatureCard creature)
                {
                foreach (var keyword in creature.keywordAbilities)
                {
                    if (keyword == KeywordAbility.CantBlock ||
                        keyword == KeywordAbility.CanOnlyBlockFlying ||
                        keyword == KeywordAbility.CantBlockWithoutForest ||
                        keyword.ToString().StartsWith("ProtectionFrom")) //
                        continue;

                    lines.Add(keyword.ToString());
                }

                if (creature.keywordAbilities.Contains(KeywordAbility.CanOnlyBlockFlying))
                    lines.Add("This creature can only block creatures with flying.");
                if (creature.keywordAbilities.Contains(KeywordAbility.CantBlock))
                    lines.Add("This creature can't block.");
                if (creature.keywordAbilities.Contains(KeywordAbility.CantBlockWithoutForest))
                    lines.Add("This creature can't block if you don't control a forest.");
                if (entersTapped)
                    lines.Add("This creature enters the battlefield tapped.");
                if (creature.keywordAbilities.Contains(KeywordAbility.ProtectionFromWhite))
                    lines.Add("Protection from White");
                if (creature.keywordAbilities.Contains(KeywordAbility.ProtectionFromBlue))
                    lines.Add("Protection from Blue");
                if (creature.keywordAbilities.Contains(KeywordAbility.ProtectionFromBlack))
                    lines.Add("Protection from Black");
                if (creature.keywordAbilities.Contains(KeywordAbility.ProtectionFromRed))
                    lines.Add("Protection from Red");
                if (creature.keywordAbilities.Contains(KeywordAbility.ProtectionFromGreen))
                    lines.Add("Protection from Green");

                // Activated abilities
                if (creature.activatedAbilities != null)
                    {
                        Debug.Log($"{creature.cardName} has {creature.activatedAbilities.Count} activated abilities.");

                        foreach (var activated in creature.activatedAbilities)
                        {
                            Debug.Log($"Activated ability: {activated}");

                            switch (activated)
                            {
                                case ActivatedAbility.TapForMana:
                                    lines.Add("Tap: Add 1 mana.");
                                    break;
                                case ActivatedAbility.TapToLoseLife:
                                    lines.Add($"Tap: Your opponent loses {creature.tapLifeLossAmount} life.");
                                    break;
                                case ActivatedAbility.TapToCreateToken:
                                    lines.Add($"{creature.manaToPayToActivate}TAP: Create a {tokenToCreate} token.");
                                    break;
                                case ActivatedAbility.PayToGainAbility:
                                    lines.Add($"{creature.manaToPayToActivate}: Gains {creature.abilityToGain} until end of turn.");
                                    break;
                            }
                        }
                    }
                }

            if (this is ArtifactCard artifact)
            {
                if (entersTapped)
                    lines.Add("This card enters the battlefield tapped.");

                if (activatedAbilities != null)
                {
                    foreach (var activated in activatedAbilities)
                    {
                        switch (activated)
                        {
                            case ActivatedAbility.TapForMana:
                                lines.Add("Tap: Add 1 mana.");
                                break;
                            case ActivatedAbility.TapToGainLife:
                                lines.Add("Tap: Gain 1 life.");
                                //lines.Add($"Tap: Gain {plagueAmount} life.");
                                break;
                            case ActivatedAbility.TapAndSacrificeForMana:
                                lines.Add("Tap, sacrifice: Add 1 mana.");
                                break;
                            case ActivatedAbility.TapToPlague:
                                lines.Add($"Tap: Each player loses {plagueAmount} life.");
                                break;
                            case ActivatedAbility.SacrificeForMana:
                                lines.Add($"{manaToPayToActivate}TAP, sacrifice: Add {manaToGain}.");
                                break;
                            case ActivatedAbility.SacrificeForLife:
                                lines.Add($"{manaToPayToActivate}TAP, sacrifice: Gain {lifeToGain} life.");
                                break;
                            case ActivatedAbility.SacrificeToDrawCards:
                                lines.Add($"{manaToPayToActivate}TAP, sacrifice: Draw {cardsToDraw} card(s).");
                                break;
                            case ActivatedAbility.DealDamageToCreature:
                                    lines.Add($"{manaToPayToActivate}TAP, sacrifice: Deal {damageToCreature} damage to target creature.");
                                    break;
                        }
                    }
                }
            }
            // Triggered abilities — shared across all cards
            foreach (var ability in abilities)
            {
                if (ability.timing == TriggerTiming.OnEnter)
                    lines.Add("When this creature enters, " + ability.description);
                else if (ability.timing == TriggerTiming.OnDeath)
                    lines.Add("When this creature dies, " + ability.description);
                else if (ability.timing == TriggerTiming.OnUpkeep)
                    lines.Add("At the beginning of your upkeep, " + ability.description);
            }

            if (keywordAbilities != null)
            {
                if (keywordAbilities.Contains(KeywordAbility.AllPermanentsEnterTapped))
                    lines.Add("All permanents enter the battlefield tapped.");
            }

            return string.Join("\n", lines);
        }
}