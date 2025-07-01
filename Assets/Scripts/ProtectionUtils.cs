using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProtectionUtils
{
    public static KeywordAbility GetProtectionKeyword(string color)
    {
        return color switch
        {
            "White" => KeywordAbility.ProtectionFromWhite,
            "Blue" => KeywordAbility.ProtectionFromBlue,
            "Black" => KeywordAbility.ProtectionFromBlack,
            "Red" => KeywordAbility.ProtectionFromRed,
            "Green" => KeywordAbility.ProtectionFromGreen,
            _ => KeywordAbility.None
        };
    }
}
