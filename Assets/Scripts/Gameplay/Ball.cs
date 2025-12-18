using Cysharp.Threading.Tasks;
using UnityEngine;

public class Ball : MonoBehaviour, IThrowable
{
    [SerializeField] private SphereCollider _collider;
    public float Radius => _collider.radius;

    public void ResetThrowable(Transform resetPos)
    {
        transform.position = resetPos.position;
    }

    public async UniTask<bool> SimulateThrow(ThrowStep[] steps, IControlThrower thrower)
    {
        bool bbHit = false;
        bool ringHit = false;
        bool scored = false;

        foreach(ThrowStep step in steps)
        {
            transform.position = step.TargetPos;
            if (step.HitCategory == HitCategory.Backboard)
            {
                bbHit = true;
            }
            else if (step.HitCategory == HitCategory.Ring)
            {
                ringHit = true;
            }
            if (step.Scored)
            {
                AddScore(bbHit, ringHit, thrower);
                scored = true;
            }
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, gameObject.GetCancellationTokenOnDestroy());
        }
        return scored;
    }

    private void AddScore(bool bbHit, bool ringHit, IControlThrower player)
    {
        if(player == null) { return; }
        if (bbHit) { player.ScoredPoints(ScoreCategory.BB); return; }
        if (ringHit) { player.ScoredPoints(ScoreCategory.Normal); return; }
        player.ScoredPoints(ScoreCategory.Clean);
    }
}
