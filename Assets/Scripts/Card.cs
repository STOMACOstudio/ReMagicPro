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
    public Sprite artwork;

    public List<CardAbility> abilities = new List<CardAbility>();
    public List<ActivatedAbility> activatedAbilities = new List<ActivatedAbility>();

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
                if (ability.timing == TriggerTiming.OnEnter)
                    ability.effect?.Invoke(owner);
            }
        }

    public virtual void OnLeavePlay(Player owner)
        {
            foreach (var ability in abilities)
            {
                if (ability.timing == TriggerTiming.OnDeath)
                    ability.effect?.Invoke(owner);
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
                    // Skip verbose keywords — we’ll handle them separately
                    if (keyword == KeywordAbility.CantBlock || keyword == KeywordAbility.CanOnlyBlockFlying || keyword == KeywordAbility.CantBlockWithoutForest)
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

            return string.Join("\n", lines);
        }
}