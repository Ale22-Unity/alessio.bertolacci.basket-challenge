
public class MatchStartedEvent 
{
    public readonly TimerData MatchTimer;

    public MatchStartedEvent(TimerData matchTimer)
    {
        MatchTimer = matchTimer;
    }
}
