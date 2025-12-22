using Cysharp.Threading.Tasks;

using UnityEngine;

public class Ball : MonoBehaviour, IThrowable
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _impact;
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
    [Space]
    [SerializeField] private FireBallModule _fireballModule;
    public float Radius => _collider.radius;

    public void Setup(bool isOwner)
    {
        _fireballModule.Setup(isOwner);
    }

    public void ResetThrowable(Transform resetPos)
    {
        _renderer.enabled = false;
        transform.position = resetPos.position;
    }

    public async UniTask<bool> SimulateThrow(ThrowStep[] steps, IControlThrower thrower)
    {
        _renderer.enabled = true;
        bool onFire = _fireballModule.OnFire;
        _ballOnFireEffect.SetActive(onFire);
        _renderer.material = onFire ? _onFireMaterial : _normalMaterial;
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
                AddScore(bbHit, ringHit, thrower, onFire);
                step.BasketObject.PlayBasketEffects(!bbHit && !ringHit);
                scored = true;
            }
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, gameObject.GetCancellationTokenOnDestroy());
        }
        if (_fireballModule != null && !scored)
        {
            _fireballModule.ResetFireBall();
        }
        return scored;
    }

    private void AddScore(bool bbHit, bool ringHit, IControlThrower player, bool onFire)
    {
        if(player == null) { return; }
        if (bbHit) 
        { 
            player.ScoredPoints(ScoreCategory.BB, onFire); 
        }
        else if (ringHit) 
        { 
            player.ScoredPoints(ScoreCategory.Normal, onFire); 
        }
        else
        {
            player.ScoredPoints(ScoreCategory.Clean, onFire);
        }
        if (_fireballModule != null)
        {
            _fireballModule.AddToFireBall();
        }

    }

    private void EvaluateCollision(GameObject collision, out bool bbHit, out bool ringHit)
    {
        bbHit = false;
        ringHit = false;
        if (collision == null) { return; }
        _audioSource.clip = _impact;
        if (collision.CompareTag(_backboardTag))
        {
            Debug.Log("Backboard hit!");
            BackboardEffects bbEffects = collision.GetComponent<BackboardEffects>();
            bbEffects.BackboardHit();
            bbHit = true;
        }
        else if (collision.CompareTag(_ringTag))
        {
            Debug.Log("Ring hit!");
            _audioSource.clip = _ringImpact;
            ringHit = true;
        }
        _audioSource.Play();
    }
}
