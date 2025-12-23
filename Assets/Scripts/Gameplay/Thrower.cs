using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class Thrower : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _throwPos;
    [SerializeField] private Ball _ball;
    [SerializeField] private CharacterAnimations _characterAnimations;
    [Space]
    [SerializeField] private LayerMask _simulatedCollisionMask;
    [SerializeField] private float _simulatedElasticlty = 0.7f;
    [SerializeField] private float _simulatedFriction = 0.1f;
    [SerializeField] private float _basketSpeedReduction = 0.35f;
    [SerializeField] private float _randomDirFluctuation = 0.5f;
    [Space]
    [SerializeField] private LayerMask _basketLayer;
    [SerializeField] private ThrowPosition _assignedPos;

    private IControlThrower _controller;
    private bool _dynamicSimulation;
    private bool _activeSimulation;

    public Transform BallCameraTarget => _ball.transform;

    private void Awake()
    {
        _controller = gameObject.GetComponent<IControlThrower>();
        _ball.Setup(_controller.IsOwner);
        _ball.ResetThrowable(_throwPos);
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

    public ThrowStep[] SimulateThrow(Vector3 startDirection, float v0, float duration, int maxSteps)
    {
        Vector3 dir = startDirection.normalized * v0;
        ThrowStep[] steps = new ThrowStep[maxSteps];
        Debug.Log($"Throw speed is {v0}");
        Vector3 stepPos = _throwPos.position;
        int i = 0;
        while(i < maxSteps)
        {
            float _accountedTimeStep = Time.fixedDeltaTime;
            bool bounced = false;
            while (_accountedTimeStep > 0 && i < maxSteps)
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
                    bool scored = CheckForBasket(stepPos, ballPosAtImpact, out BasketEffects basketObj);
                    stepPos = ballPosAtImpact;
                    float timeOfImpact = _accountedTimeStep * (hit.distance / deltaPos.magnitude);
                    _accountedTimeStep -= timeOfImpact;
                    Vector3 velocityAtImpact = stepVelocity + Physics.gravity * timeOfImpact;
                    Vector3 projectedDir = Vector3.Project(velocityAtImpact, hit.normal);
                    Vector3 bounceSpeed = ((1f - _simulatedFriction) * (velocityAtImpact - projectedDir)) + (colliderElasticity * _simulatedElasticlty * -projectedDir);
                    dir = scored ? (bounceSpeed * _basketSpeedReduction) : bounceSpeed;
                    steps[i] = new ThrowStep(ballPosAtImpact, scored, hit.collider.gameObject, basketObj);
                    i++;
                }
                else
                {
                    Debug.DrawLine(stepPos, stepPos + deltaPos, bounced? Color.green : Color.red, duration);
                    bool scored = CheckForBasket(stepPos, stepPos + deltaPos, out BasketEffects basketObj);
                    stepPos += deltaPos;
                    dir = scored ? (stepVelocity * _basketSpeedReduction) : stepVelocity;
                    steps[i] = new ThrowStep(stepPos, scored, null, basketObj);
                    _accountedTimeStep = 0;
                    i++;
                }
            }
        }
        return steps;
    }

    private bool CheckForBasket(Vector3 startPos, Vector3 endPos, out BasketEffects basketObject)
    {
        basketObject = null;
        if (Physics.Linecast(startPos, endPos, out RaycastHit hit, _basketLayer, QueryTriggerInteraction.Collide))
        {
            basketObject = hit.collider.gameObject.GetComponent<BasketEffects>();
            return true;
        }
        return false;
    }

    public async UniTask StartDynamicSimulation()
    {
        _dynamicSimulation = true;
        while (_assignedPos != null && _dynamicSimulation)
        {
            Vector3 dir = Vector3.zero;
            float v0 = 0;
            CalculatePathWithFixedHeight(_throwPos, _assignedPos.CleanTarget, _assignedPos.DeltaH, out dir, out v0);
            SimulateThrow(dir, v0, 0.2f, _assignedPos.SimulationSteps);
            CalculatePathWithFixedHeight(_throwPos, _assignedPos.BBTarget, _assignedPos.DeltaH, out dir, out v0);
            SimulateThrow(dir, v0, 0.2f, _assignedPos.SimulationSteps);
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
        Vector3 dir = _assignedPos.BBThrow.Direction;
        bool noFluctuation = false;
        if(Utils.BetweenValuesCheck(_assignedPos.PerfetThrow.Velocity - speedError, _assignedPos.PerfetThrow.Velocity + speedError, throwSpeed))
        {
            throwSpeed = _assignedPos.PerfetThrow.Velocity;
            noFluctuation = true;
        }
        else if (Utils.BetweenValuesCheck(_assignedPos.BBThrow.Velocity - speedError, _assignedPos.BBThrow.Velocity + speedError, throwSpeed))
        {
            throwSpeed = _assignedPos.BBThrow.Velocity;
            dir = _assignedPos.BBThrow.Direction;
            noFluctuation = true;
        }
        if(throwSpeed < _assignedPos.PerfetThrow.Velocity + speedError)
        {
            dir = _assignedPos.PerfetThrow.Direction;
        }
        if (!noFluctuation)
        {
            float fx = Random.Range(-_randomDirFluctuation, _randomDirFluctuation);
            float fy = Random.Range(-_randomDirFluctuation, _randomDirFluctuation);
            float fz = Random.Range(-_randomDirFluctuation, _randomDirFluctuation);
            dir += new Vector3(fx, fy, fz);
        }
        if (_characterAnimations != null)
        {
            await _characterAnimations.WaitShootBallAnimation();
        }
        _controller.SetCameraToThrowTarget();
        bool scored = await _ball.SimulateThrow(SimulateThrow(dir, throwSpeed, 0.01f, _assignedPos.SimulationSteps), _controller);
        _activeSimulation = false;
    }

    public void ResetBall()
    {
        _ball.ResetThrowable(_throwPos);
    }
}
