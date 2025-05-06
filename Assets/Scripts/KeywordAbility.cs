public enum KeywordAbility
{
    None,
    Haste,
    Defender,
    CantBlock,
    Vigilance,
    Flying,
    Reach,
    CanOnlyBlockFlying,
    Lifelink,
    CantBlockWithoutForest,
    Plainswalk,
    Islandwalk,
    Swampwalk,
    Mountainwalk,
    Forestwalk
    // Add more like Flying, FirstStrike later
}

public enum ActivatedAbility
{
    TapForMana,
    TapToLoseLife,
    TapAndSacrificeForMana,
    SacrificeForMana,
    SacrificeForLife,
    TapToGainLife,
    TapToPlague,
    SacrificeToDrawCards,
    TapToCreateToken,
    PayToGainAbility,
    // etc.
}