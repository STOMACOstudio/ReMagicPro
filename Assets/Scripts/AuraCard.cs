using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraCard : EnchantmentCard
{
    public SorceryCard.TargetType requiredTargetType = SorceryCard.TargetType.Creature;
    public Card attachedTo;

    public override void OnEnterPlay(Player owner)
    {
        base.OnEnterPlay(owner);
        if (attachedTo is CreatureCard creature)
        {
            creature.AddAuraBuff(buffPower, buffToughness);
        }
    }

    public override void OnLeavePlay(Player owner)
    {
        if (attachedTo is CreatureCard creature)
        {
            creature.RemoveAuraBuff(buffPower, buffToughness);
        }
        attachedTo = null;
        base.OnLeavePlay(owner);
    }
}
