public enum TriggerTiming
{
    OnEnter,
    OnDeath
}

public class CardAbility
{
    public TriggerTiming timing;
    public System.Action<Player> effect;
    public string description;
}