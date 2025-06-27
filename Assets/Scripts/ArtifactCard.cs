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
                    owner.ColoredMana.Colorless += 1; // TEMP: All artifact mana goes to white for now
                    break;

                case ActivatedAbility.TapToGainLife:
                    GameManager.Instance.TryGainLife(owner, 1);
                    break;

                case ActivatedAbility.TapAndSacrificeForMana:
                    owner.ColoredMana.Colorless += 1; // TEMP again
                    GameManager.Instance.SendToGraveyard(this, owner);
                    break;

                case ActivatedAbility.SacrificeForLife:
                    if (owner.ColoredMana.Total() >= manaToPayToActivate)
                    {
                        int remaining = manaToPayToActivate;

                        // 1. Spend colorless first
                        int useColorless = Mathf.Min(owner.ColoredMana.Colorless, remaining);
                        owner.ColoredMana.Colorless -= useColorless;
                        remaining -= useColorless;

                        // 2. Spend colored mana in WUBRG order
                        remaining -= SpendFromPool(ref owner.ColoredMana.White, remaining);
                        remaining -= SpendFromPool(ref owner.ColoredMana.Blue, remaining);
                        remaining -= SpendFromPool(ref owner.ColoredMana.Black, remaining);
                        remaining -= SpendFromPool(ref owner.ColoredMana.Red, remaining);
                        remaining -= SpendFromPool(ref owner.ColoredMana.Green, remaining);

                        if (remaining > 0)
                        {
                            Debug.LogWarning("SacrificeForLife: Somehow not enough mana despite CanPay check.");
                            return;
                        }

                        GameManager.Instance.TryGainLife(owner, lifeToGain);
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

    private int SpendFromPool(ref int pool, int needed)
        {
            int spent = Mathf.Min(pool, needed);
            pool -= spent;
            return spent;
        }

}