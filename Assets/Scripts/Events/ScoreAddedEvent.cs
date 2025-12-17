public class ScoreAddedEvent
{
    public readonly int AddedPoints;
    public readonly int TotPoints;
    public readonly bool IsOwner;

    public ScoreAddedEvent(int addedPoints, int totPoints, bool isOwner)
    {
        AddedPoints = addedPoints;
        TotPoints = totPoints;
        IsOwner = isOwner;
    }
}
