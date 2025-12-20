
using UnityEngine;

public class ScoreAddedEvent
{
    public readonly bool OnFire;
    public readonly ScoreCategory Category;
    public readonly int AddedPoints;
    public readonly int TotPoints;
    public readonly bool IsOwner;
    public readonly Vector3 BallPosition;

    public ScoreAddedEvent(int addedPoints, int totPoints, bool isOwner, Vector3 ballPos, bool onFire, ScoreCategory category)
    {
        AddedPoints = addedPoints;
        TotPoints = totPoints;
        IsOwner = isOwner;
        BallPosition = ballPos;
        OnFire = onFire;
        Category = category;
    }
}
