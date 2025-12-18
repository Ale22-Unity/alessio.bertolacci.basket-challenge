public class SetFireballStatusEvent
{
    public readonly float BarPercentage;
    public readonly bool Active;

    public SetFireballStatusEvent(float barPercentage, bool active)
    {
        BarPercentage = barPercentage;
        Active = active;
    }
}
