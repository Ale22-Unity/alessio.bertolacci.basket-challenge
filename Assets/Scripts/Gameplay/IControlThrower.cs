using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControlThrower
{
    public FireBallModule FireballModule { get; }
    public bool TryAssignThrowerToPos(ThrowPosition position);
    public void ScoredPoints(ScoreCategory score);
    public bool IsPlayerThrowing(out UniTask throwTask);
    public void SetCameraToThrowTarget();
    public bool IsOwner {  get; }
}
