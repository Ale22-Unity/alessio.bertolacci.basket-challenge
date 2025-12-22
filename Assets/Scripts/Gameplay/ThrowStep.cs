using UnityEngine;

public readonly struct ThrowStep
{
    public readonly Vector3 TargetPos;
    public readonly bool Scored;
    public readonly BasketEffects BasketObject;
    public readonly GameObject HitObject;

    public ThrowStep(Vector3 targetPos, bool scored, GameObject hitObject, BasketEffects basketObject)
    {
        TargetPos = targetPos;
        Scored = scored;
        HitObject = hitObject;
        BasketObject = basketObject;
    }
}
