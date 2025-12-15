using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Thrower : MonoBehaviour
{
    [SerializeField] private float deltaH = 5;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _throwPos;
    [SerializeField] private Ball _ball;
    [Space]
    [SerializeField] private float _simulatedTimeStep = 0.05f;
    [SerializeField] private int _maxSimulatedSteps = 1000;
    [SerializeField] private LayerMask _simulatedCollisionMask;
    [SerializeField] private float _simulatedElasticlty = 0.7f;
    [Space]
    [SerializeField] private string _backboardTag = "BackBoard";
    [SerializeField] private string _ringTag = "Ring";
    [SerializeField] private LayerMask _basketLayer;

    private bool _dynamicSimulation;

    private void Awake()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Subscribe<ThrowBallTestEvent>(On);
            GameClient.Client.EventBus.Subscribe<ResetBallTestEvent>(On);
        }
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

    public void CalculatePathWithFixedHeight(Transform fromTransform, Transform targetTransform, float deltaH, out Vector3 fireDirection, out float v0, out float time)
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

        time = tplus > tmin ? tplus : tmin;

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
        _ball.SimulateThrow(SimulateThrow(0.01f)).Forget();
    }

    public ThrowStep[] SimulateThrow(float duration)
    {
        ThrowStep[] steps = new ThrowStep[_maxSimulatedSteps];
        CalculatePathWithFixedHeight(_throwPos, _target, deltaH, out Vector3 dir, out float v0, out float time);
        Vector3 stepPos = _throwPos.position;
        int i = 0;
        while(i < _maxSimulatedSteps)
        {
            float _accountedTimeStep = _simulatedTimeStep;
            bool bounced = false;
            while (_accountedTimeStep > 0 && i < _maxSimulatedSteps)
            {
                Vector3 stepVelocity = dir + Physics.gravity * _accountedTimeStep;
                Vector3 deltaPos = (dir * _accountedTimeStep) + (0.5f * Physics.gravity * Mathf.Pow(_accountedTimeStep, 2));
                bool bounce = Physics.SphereCast(stepPos, _ball.Radius, dir, out RaycastHit hit, deltaPos.magnitude, _simulatedCollisionMask, QueryTriggerInteraction.Ignore);
                if (bounce)
                {
                    bounced = true;
                    Vector3 ballPosAtImpact = stepPos + (stepVelocity.normalized * hit.distance);
                    Debug.DrawLine(stepPos, ballPosAtImpact, Color.blue, duration);
                    bool scored = CheckForBasket(stepPos, ballPosAtImpact);
                    stepPos = ballPosAtImpact;
                    float timeOfImpact = _accountedTimeStep * (hit.distance / deltaPos.magnitude);
                    _accountedTimeStep -= timeOfImpact;
                    Vector3 velocityAtImpact = stepVelocity + Physics.gravity * timeOfImpact;
                    Vector3 projectedDir = Vector3.Project(velocityAtImpact, hit.normal);
                    Vector3 bounceSpeed = (velocityAtImpact - projectedDir) - (_simulatedElasticlty * projectedDir);
                    dir = bounceSpeed;
                    steps[i] = new ThrowStep((int)(timeOfImpact * 1000), ballPosAtImpact, scored, GetHitCategory(hit.collider.gameObject));
                    i++;
                }
                else
                {
                    Debug.DrawLine(stepPos, stepPos + deltaPos, bounced? Color.green : Color.red, duration);
                    bool scored = CheckForBasket(stepPos, stepPos + deltaPos);
                    stepPos += deltaPos;
                    dir = stepVelocity;
                    steps[i] = new ThrowStep((int)(_accountedTimeStep * 1000), stepPos, scored, HitCategory.None);
                    _accountedTimeStep = 0;
                    i++;
                }
            }
        }
        return steps;
    }

    public async UniTask PlayOutThrow()
    {
        await _ball.SimulateThrow(SimulateThrow(0.01f));
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
            SimulateThrow(0.2f);
            await Task.Delay(200);
        }
    }
    
    public void StopDynamicSimulation()
    {
        _dynamicSimulation = false;
    }
    
}
