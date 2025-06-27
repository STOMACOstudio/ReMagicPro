public enum TriggerTiming
{
    OnEnter,
    OnDeath,
    OnUpkeep,
    OnArtifactEnter,
    OnLandEnter,
    OnLandLeave,
    OnLifeGain,
    OnCardDraw,
    OnCreatureDiesOrDiscarded,
    OnCombatDamageToPlayer,
}

public class CardAbility
{
    public TriggerTiming timing;
    public System.Action<Player, Card> effect;
    public string description;
    public bool requiresTarget = false;
    public SorceryCard.TargetType requiredTargetType = SorceryCard.TargetType.None;
    public bool excludeArtifactCreatures = false;
}