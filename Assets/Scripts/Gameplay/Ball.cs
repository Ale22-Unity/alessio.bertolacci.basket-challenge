using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour, IThrowable
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _impact;
    [SerializeField] private AudioClip _bbImpact;
    [SerializeField] private AudioClip _ringImpact;
    [Space]
    [SerializeField] private GameObject _ballOnFireEffect;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Material _onFireMaterial;
    [SerializeField] private Material _normalMaterial;
    [SerializeField] private SphereCollider _collider;
    [Space]
    [SerializeField] private string _backboardTag = "BackBoard";
    [SerializeField] private string _ringTag = "Ring";
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
            EvaluateCollision(step.HitObject, out bool bbHitStep, out bool ringHitStep);
            if (bbHitStep)
            {
                bbHit = true;
            }
            else if (ringHitStep)
            {
                ringHit = true;
            }
            if (step.Scored)
            {
                AddScore(bbHit, ringHit, thrower);
                step.BasketObject.PlayBasketEffects(!bbHit && !ringHit);
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

    private void EvaluateCollision(GameObject collision, out bool bbHit, out bool ringHit)
    {
        bbHit = false;
        ringHit = false;
        if (collision == null) { return; }
        if (collision.CompareTag(_backboardTag))
        {
            Debug.Log("Backboard hit!");
            _audioSource.clip = _bbImpact;
            _audioSource.Play();
            bbHit = true;
        }
        else if (collision.CompareTag(_ringTag))
        {
            Debug.Log("Ring hit!");
            _audioSource.clip = _ringImpact;
            _audioSource.Play();
            ringHit = true;
        }
        else
        {
            _audioSource.clip = _impact;
            _audioSource.Play();
        }
    }
}
