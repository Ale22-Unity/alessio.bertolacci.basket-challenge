
public class ReadyUpTimerEvent
{
    public readonly TimerEventCategory Category;
    public readonly int Value;

    public ReadyUpTimerEvent(TimerEventCategory category, int value)
    {
        Category = category;
        Value = value;
    }
}

public enum TimerEventCategory
{
    Started = 0,
    Update = 1,
    Ended = 2
}
