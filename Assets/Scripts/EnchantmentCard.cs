using System.Collections.Generic;
using UnityEngine;

public class EnchantmentCard : Card
{
    public int buffPower = 0;
    public int buffToughness = 0;
    public CreatureCard enchantedCreature;
    public bool requiresTarget = true;
    public SorceryCard.TargetType requiredTargetType = SorceryCard.TargetType.Creature;
    public bool isAura = false;

    public void Play(Player player, CreatureCard target = null)
    {
        if (isAura)
            enchantedCreature = target;

        base.Play(player);
    }

    public override void OnEnterPlay(Player owner)
    {
        base.OnEnterPlay(owner);
        if (isAura && enchantedCreature != null)
        {
            enchantedCreature.AttachEnchantment(this);
        }
    }

    public override void OnLeavePlay(Player owner)
    {
        if (isAura && enchantedCreature != null)
        {
            enchantedCreature.DetachEnchantment(this);
            enchantedCreature = null;
        }
        base.OnLeavePlay(owner);
    }
}
