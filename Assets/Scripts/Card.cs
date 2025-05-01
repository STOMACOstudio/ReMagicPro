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

    public virtual void Play(Player player)
    {
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
                lines.Add(keyword.ToString());
        }

        // Triggered abilities — shared across all cards
        foreach (var ability in abilities)
        {
            if (ability.timing == TriggerTiming.OnEnter)
                lines.Add("When this creature enters, " + ability.description);
            else if (ability.timing == TriggerTiming.OnDeath)
                lines.Add("When this creature dies, " + ability.description);

        }

        return string.Join("\n", lines);
    }
}