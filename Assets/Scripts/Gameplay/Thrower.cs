using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class Thrower : MonoBehaviour
{
    [SerializeField] private float _deltaH = 5;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _throwPos;
    [SerializeField] private Ball _ball;
    [Space]
    [SerializeField] private int _maxSimulatedSteps = 1000;
    [SerializeField] private LayerMask _simulatedCollisionMask;
    [SerializeField] private float _simulatedElasticlty = 0.7f;
    [SerializeField] private float _simulatedFriction = 0.1f;
    [SerializeField] private float _basketSpeedReduction = 0.35f;
    [Space]
    [SerializeField] private string _backboardTag = "BackBoard";
    [SerializeField] private string _ringTag = "Ring";
    [SerializeField] private LayerMask _basketLayer;
    [Space]
    [SerializeField] private ThrowPosition _assignedPos;
    [SerializeField] private float _maxSpeed = 8;
    [SerializeField] private float _minSpeed = 6;
    [Space]
    private IControlThrower _controller;
    private bool _dynamicSimulation;
    private bool _activeSimulation;

    public Transform BallCameraTarget => _ball.transform;

    private void Awake()
    {
        _controller = gameObject.GetComponent<IControlThrower>();
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Subscribe<ThrowBallTestEvent>(On);
            GameClient.Client.EventBus.Subscribe<ResetBallTestEvent>(On);
        }
        _ball.ResetThrowable(_throwPos);
    }

    private void OnDestroy()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Unsubscribe<ThrowBallTestEvent>(On);
            GameClient.Client.EventBus.Unsubscribe<ResetBallTestEvent>(On);
        }
    }

    float QuadraticEquation(float a, float b, float c, float sign)
    {
        return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
    }

    public void CalculatePathWithFixedHeight(Transform fromTransform, Transform targetTransform, float deltaH, out Vector3 fireDirection, out float v0)
    {
        Vector3 direction = targetTransform.transform.position - fromTransform.position;
        Vector3 groundDir = new Vector3(direction.x, 0, direction.z);

        Vector3 targetPos = new Vector3(groundDir.magnitude, direction.y, 0);
        groundDir = groundDir.normalized;

        float targetHeight = targetTransform.position.y + deltaH - fromTransform.position.y;

        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float b = Mathf.Sqrt(2 * g * targetHeight);
        float a = (-0.5f * g);
        float c = -yt;

        float tplus = QuadraticEquation(a, b, c, 1);
        float tmin = QuadraticEquation(a, b, c, -1);

        float time = tplus > tmin ? tplus : tmin;

        float angle = Mathf.Atan(b * time / xt);


        v0 = b / Mathf.Sin(angle);
        fireDirection = (groundDir * (Mathf.Cos(angle) * v0)) + (Vector3.up * (Mathf.Sin(angle) * v0));
    }

    private void On(ResetBallTestEvent e)
    {
        _ball.ResetThrowable(_throwPos);
    }

    private void On(ThrowBallTestEvent e)
    {
        CalculatePathWithFixedHeight(_throwPos, _target, _deltaH, out Vector3 dir, out float v0);
        _ball.SimulateThrow(SimulateThrow(dir, v0, 0.01f), _controller).Forget();
    }

    public ThrowStep[] SimulateThrow(Vector3 startDirection, float v0, float duration)
    {
        Vector3 dir = startDirection.normalized * v0;
        ThrowStep[] steps = new ThrowStep[_maxSimulatedSteps];
        Debug.Log($"Throw speed is {v0}");
        Vector3 stepPos = _throwPos.position;
        int i = 0;
        while(i < _maxSimulatedSteps)
        {
            float _accountedTimeStep = Time.fixedDeltaTime;
            bool bounced = false;
            while (_accountedTimeStep > 0 && i < _maxSimulatedSteps)
            {
                Vector3 stepVelocity = dir + Physics.gravity * _accountedTimeStep;
                Vector3 deltaPos = (dir * _accountedTimeStep) + (0.5f * Physics.gravity * Mathf.Pow(_accountedTimeStep, 2));
                bool bounce = Physics.SphereCast(stepPos, _ball.Radius, dir, out RaycastHit hit, deltaPos.magnitude, _simulatedCollisionMask, QueryTriggerInteraction.Ignore);
                if (bounce)
                {
                    float colliderElasticity = hit.collider.material != null ? hit.collider.material.bounciness : 1f;
                    bounced = true;
                    Vector3 ballPosAtImpact = stepPos + (stepVelocity.normalized * hit.distance);
                    Debug.DrawLine(stepPos, ballPosAtImpact, Color.blue, duration);
                    bool scored = CheckForBasket(stepPos, ballPosAtImpact);
                    stepPos = ballPosAtImpact;
                    float timeOfImpact = _accountedTimeStep * (hit.distance / deltaPos.magnitude);
                    _accountedTimeStep -= timeOfImpact;
                    Vector3 velocityAtImpact = stepVelocity + Physics.gravity * timeOfImpact;
                    Vector3 projectedDir = Vector3.Project(velocityAtImpact, hit.normal);
                    Vector3 bounceSpeed = ((1f - _simulatedFriction) * (velocityAtImpact - projectedDir)) + (colliderElasticity * _simulatedElasticlty * -projectedDir);
                    dir = scored ? (bounceSpeed * _basketSpeedReduction) : bounceSpeed;
                    steps[i] = new ThrowStep(ballPosAtImpact, scored, GetHitCategory(hit.collider.gameObject));
                    i++;
                }
                else
                {
                    Debug.DrawLine(stepPos, stepPos + deltaPos, bounced? Color.green : Color.red, duration);
                    bool scored = CheckForBasket(stepPos, stepPos + deltaPos);
                    stepPos += deltaPos;
                    dir = scored ? (stepVelocity * _basketSpeedReduction) : stepVelocity;
                    steps[i] = new ThrowStep(stepPos, scored, HitCategory.None);
                    _accountedTimeStep = 0;
                    i++;
                }
            }
        }
        return steps;
    }

    public async UniTask PlayOutThrow()
    {
        CalculatePathWithFixedHeight(_throwPos, _target, _deltaH, out Vector3 dir, out float v0);
        await _ball.SimulateThrow(SimulateThrow(dir, v0, 0.01f), _controller);
    }

    private bool CheckForBasket(Vector3 startPos, Vector3 endPos)
    {
        if (Physics.Linecast(startPos, endPos, _basketLayer, QueryTriggerInteraction.Collide))
        {
            Debug.Log("Scored basket!");
            return true;
        }
        return false;
    }

    private HitCategory GetHitCategory(GameObject collision)
    {
        if (collision.CompareTag(_backboardTag))
        {
            Debug.Log("Backboard hit!");
            return HitCategory.Backboard;
        }
        if (collision.CompareTag(_ringTag))
        {
            Debug.Log("Ring hit!");
            return HitCategory.Ring;
        }
        else
        {
            return HitCategory.Default;
        }
    }

    public async UniTask StartDynamicSimulation()
    {
        _dynamicSimulation = true;
        while (_dynamicSimulation)
        {
            CalculatePathWithFixedHeight(_throwPos, _target, _deltaH, out Vector3 dir, out float v0);
            SimulateThrow(dir, v0, 0.2f);
            await Task.Delay(200);
        }
    }
    
    public void StopDynamicSimulation()
    {
        _dynamicSimulation = false;
    }
    
    public bool TryAssignThrowerToPosition(ThrowPosition pos)
    {
        ThrowPosition oldPos = _assignedPos;
        if (pos.TrySetThrowerToPosition(this)) 
        { 
            _assignedPos = pos;
            if(_controller != null)
            {
                CalculatePathWithFixedHeight(_throwPos, _assignedPos.CleanTarget, _assignedPos.DeltaH, out Vector3 pDir, out float perfectV);
                CalculatePathWithFixedHeight(_throwPos, _assignedPos.BBTarget, _assignedPos.DeltaH, out Vector3 bbDir, out float bbV);
                _assignedPos.SetSweetSpotData(new SweetSpotInfo(perfectV, pDir), new SweetSpotInfo(bbV, bbDir));
                _controller.TryAssignThrowerToPos(_assignedPos);
            }
            if (oldPos != null) { oldPos.RemoveThrowerFromPosition(); }
            return true; 
        }
        return false;
    }

    public async UniTask ThrowFromInput(float throwPerc, float errorMargin)
    {
        if (_activeSimulation) { return; }
        _activeSimulation = true;
        float deltaSpeed = _assignedPos.MaxThrowSpeed - _assignedPos.MinThrowSpeed;
        float speedError = deltaSpeed * errorMargin * 0.5f;
        float throwSpeed = _assignedPos.MinThrowSpeed + (deltaSpeed * throwPerc);
        Vector3 dir = _assignedPos.PerfetThrow.Direction;
        if(Utils.BetweenValuesCheck(_assignedPos.PerfetThrow.Velocity - speedError, _assignedPos.PerfetThrow.Velocity + speedError, throwSpeed))
        {
            throwSpeed = _assignedPos.PerfetThrow.Velocity;
        }
        else if (Utils.BetweenValuesCheck(_assignedPos.BBThrow.Velocity - speedError, _assignedPos.BBThrow.Velocity + speedError, throwSpeed))
        {
            throwSpeed = _assignedPos.BBThrow.Velocity;
            dir = _assignedPos.BBThrow.Direction;
        }
        if(throwSpeed > _assignedPos.BBThrow.Velocity + speedError)
        {
            dir = _assignedPos.BBThrow.Direction;
        }
        await _ball.SimulateThrow(SimulateThrow(dir, throwSpeed, 0.01f), _controller);
        _activeSimulation = false;
    }

    public void ResetBall()
    {
        _ball.ResetThrowable(_throwPos);
    }
}
