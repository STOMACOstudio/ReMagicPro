public enum TriggerTiming
{
    OnEnter,
    OnDeath,
    OnUpkeep,
}

public class CardAbility
{
    public TriggerTiming timing;
    public System.Action<Player> effect;
    public string description;
}