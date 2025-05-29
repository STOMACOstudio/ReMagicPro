public enum TriggerTiming
{
    OnEnter,
    OnDeath,
    OnUpkeep,
}

public class CardAbility
{
    public TriggerTiming timing;
    public System.Action<Player, Card> effect;
    public string description;
    public bool requiresTarget = false;
    public SorceryCard.TargetType requiredTargetType = SorceryCard.TargetType.None;
}