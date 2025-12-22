using Cysharp.Threading.Tasks;
using UnityEngine;

public class Ball : MonoBehaviour, IThrowable
{
    [SerializeField] private GameObject _ballOnFireEffect;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Material _onFireMaterial;
    [SerializeField] private Material _normalMaterial;
    [SerializeField] private SphereCollider _collider;
    public float Radius => _collider.radius;

    public void ResetThrowable(Transform resetPos)
    {
        gameObject.SetActive(false);
        transform.position = resetPos.position;
    }

    public async UniTask<bool> SimulateThrow(ThrowStep[] steps, IControlThrower thrower)
    {
        gameObject.SetActive(true);
        _ballOnFireEffect.SetActive(thrower.FireballModule.OnFire);
        _renderer.material = thrower.FireballModule.OnFire ? _onFireMaterial : _normalMaterial;
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
        if (thrower.FireballModule != null && !scored)
        {
            thrower.FireballModule.ResetFireBall();
        }
        return scored;
    }

    private void AddScore(bool bbHit, bool ringHit, IControlThrower player)
    {
        if(player == null) { return; }
        if (bbHit) 
        { 
            player.ScoredPoints(ScoreCategory.BB); 
        }
        else if (ringHit) 
        { 
            player.ScoredPoints(ScoreCategory.Normal); 
        }
        else
        {
            player.ScoredPoints(ScoreCategory.Clean);
        }
        if (player.FireballModule != null)
        {
            player.FireballModule.AddToFireBall();
        }

    }
}
