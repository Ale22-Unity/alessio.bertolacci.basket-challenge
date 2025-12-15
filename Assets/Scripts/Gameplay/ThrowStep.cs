using UnityEngine;

public readonly struct ThrowStep
{
    public readonly int Ms;
    public readonly Vector3 TargetPos;
    public readonly bool Scored;
    public readonly HitCategory HitCategory;

    public ThrowStep(int ms, Vector3 targetPos, bool scored, HitCategory hitCategory)
    {
        Ms = ms;
        TargetPos = targetPos;
        Scored = scored;
        HitCategory = hitCategory;
    }
}

public enum HitCategory
{
    None = 0,
    Backboard = 1,
    Ring = 2,
    Default = 3
}
