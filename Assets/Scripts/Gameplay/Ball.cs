using Cysharp.Threading.Tasks;
using UnityEngine;

public class Ball : MonoBehaviour, IThrowable
{
    [SerializeField] private SphereCollider _collider;
    public float Radius => _collider.radius;
    private Vector3 _targetPos;

    private void Awake()
    {
        _targetPos = transform.position;
    }

    private void Update()
    {
        transform.position = _targetPos;
    }

    public void ResetThrowable(Transform resetPos)
    {
        transform.position = resetPos.position;
        _targetPos = transform.position;
    }

    public async UniTask SimulateThrow(ThrowStep[] steps, IControlThrower thrower)
    {
        bool bbHit = false;
        bool ringHit = false;

        foreach(ThrowStep step in steps)
        {
            _targetPos = step.TargetPos;
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
            }
            await UniTask.Delay(step.Ms);
        }
    }

    private void AddScore(bool bbHit, bool ringHit, IControlThrower player)
    {
        if(player == null) { return; }
        if (bbHit) { player.ScoredPoints(ScoreCategory.BB); return; }
        if (ringHit) { player.ScoredPoints(ScoreCategory.Normal); return; }
        player.ScoredPoints(ScoreCategory.Clean);
    }
}
