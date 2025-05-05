using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactCard : Card
{

    public void Tap()
    {
        if (isTapped) return;

        isTapped = true;

        foreach (var ability in activatedAbilities)
        {
            switch (ability)
            {
                case ActivatedAbility.TapForMana:
                    owner.ManaPool += 1;
                    break;

                case ActivatedAbility.TapToGainLife:
                    owner.Life += 1;
                    break;

                case ActivatedAbility.TapAndSacrificeForMana:
                    owner.ManaPool += 1;
                    GameManager.Instance.SendToGraveyard(this, owner);
                    break;
                case ActivatedAbility.SacrificeForLife:
                    if (owner.ManaPool >= manaToPayToActivate)
                    {
                        owner.ManaPool -= manaToPayToActivate;
                        owner.Life += lifeToGain;
                        GameManager.Instance.SendToGraveyard(this, owner);
                    }
                    else
                    {
                        Debug.Log("Not enough mana to activate SacrificeForLife.");
                    }
                    break;
            }
        }

        GameManager.Instance.UpdateUI();
    }
}
