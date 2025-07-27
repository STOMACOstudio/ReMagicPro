using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraCard : EnchantmentCard
{
    public SorceryCard.TargetType requiredTargetType = SorceryCard.TargetType.Creature;
    public Card attachedTo;
    public bool targetMustBeControlledCreature = false;
    public bool gainControlOfCreature = false;

    public override void OnEnterPlay(Player owner)
    {
        base.OnEnterPlay(owner);
        if (attachedTo is CreatureCard creature)
        {
            creature.AddAuraBuff(buffPower, buffToughness);
            if (keywordBuff != KeywordAbility.None)
                creature.AddAuraKeyword(keywordBuff);
            GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();

            if (gainControlOfCreature)
            {
                Player controller = GameManager.Instance.GetControllerOfCard(creature);
                if (controller != owner)
                    GameManager.Instance.ChangeController(creature, owner);
            }

            // If toughness falls to zero or less, creature (and this aura)
            // should immediately die.
            GameManager.Instance.CheckDeaths(GameManager.Instance.humanPlayer);
            GameManager.Instance.CheckDeaths(GameManager.Instance.aiPlayer);
        }
    }

    public override void OnLeavePlay(Player owner)
    {
        if (attachedTo is CreatureCard creature)
        {
            creature.RemoveAuraBuff(buffPower, buffToughness);
            if (keywordBuff != KeywordAbility.None)
                creature.RemoveAuraKeyword(keywordBuff);
            GameManager.Instance.FindCardVisual(creature)?.UpdateVisual();

            if (gainControlOfCreature && GameManager.Instance.GetControllerOfCard(creature) == owner)
            {
                GameManager.Instance.ChangeController(creature, creature.owner);
            }
        }
        attachedTo = null;
        base.OnLeavePlay(owner);
    }
}
