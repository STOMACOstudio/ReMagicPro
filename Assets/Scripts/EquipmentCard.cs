using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCard : ArtifactCard
{
    public CreatureCard equippedTo;

    public int EquipCost => manaToPayToActivate;

    public void Equip(CreatureCard creature)
    {
        if (equippedTo == creature)
            return;

        Unequip();
        equippedTo = creature;
        if (equippedTo != null)
        {
            equippedTo.AddAuraBuff(buffPower, buffToughness);
            if (keywordBuff != KeywordAbility.None)
                equippedTo.AddAuraKeyword(keywordBuff);
            GameManager.Instance.FindCardVisual(equippedTo)?.UpdateVisual();
        }
    }

    public void Unequip()
    {
        if (equippedTo != null)
        {
            equippedTo.RemoveAuraBuff(buffPower, buffToughness);
            if (keywordBuff != KeywordAbility.None)
                equippedTo.RemoveAuraKeyword(keywordBuff);
            GameManager.Instance.FindCardVisual(equippedTo)?.UpdateVisual();
            equippedTo = null;
        }
    }

    public override void OnLeavePlay(Player owner)
    {
        Unequip();
        base.OnLeavePlay(owner);
    }
}
