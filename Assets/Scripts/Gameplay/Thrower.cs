using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrower : MonoBehaviour
{
    [SerializeField] private float deltaH = 5;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _throwPos;
    [SerializeField] private Ball _ball;
    [Space]
    [SerializeField] private int _simulatedBounces = 3;
    [SerializeField] private float _simulatedTimeStep = 0.05f;
    [SerializeField] private float _maxSimulatedSteps = 1000;
    [SerializeField] private LayerMask _simulatedCollisionMask;
    [SerializeField] private float _simulatedElasticlty = 0.7f;

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
        CalculatePathWithFixedHeight(_throwPos, _target, deltaH, out Vector3 dir, out float v0, out float time);
        Debug.Log($"Direction is {dir}");
        _ball.Throw(dir, v0, time);
    }

    public void SimulateThrow()
    {
        CalculatePathWithFixedHeight(_throwPos, _target, deltaH, out Vector3 dir, out float v0, out float time);
        Vector3 stepPos = _throwPos.position;
        for(int i = 0; i < _maxSimulatedSteps; i++)
        {
            Vector3 stepVelocity = dir + Physics.gravity * _simulatedTimeStep;
            Vector3 deltaPos = (dir * _simulatedTimeStep) + (0.5f * Physics.gravity * Mathf.Pow(_simulatedTimeStep, 2));
            bool bounce = Physics.SphereCast(stepPos, _ball.Radius, dir, out RaycastHit hit, deltaPos.magnitude, _simulatedCollisionMask);
            if (bounce)
            {
                Vector3 ballPosAtImpact = stepPos + (stepVelocity.normalized * hit.distance);
                Debug.DrawLine(stepPos, ballPosAtImpact, Color.blue, 10);
                stepPos = ballPosAtImpact;
                float timeOfImpact = _simulatedTimeStep * (hit.distance / deltaPos.magnitude);
                Vector3 velocityAtImpact = stepVelocity + Physics.gravity * timeOfImpact;
                Vector3 projectedDir = Vector3.Project(velocityAtImpact, hit.normal);
                Vector3 bounceSpeed = (velocityAtImpact - projectedDir) - (_simulatedElasticlty * projectedDir);
                dir = bounceSpeed;
                Vector3 bouncedPos = stepPos + (bounceSpeed * (_simulatedTimeStep - timeOfImpact));
                Debug.DrawLine(stepPos, bouncedPos, Color.green, 10);
                stepPos = bouncedPos;
            }
            else
            {
                Debug.DrawLine(stepPos, stepPos + deltaPos, Color.red, 10);
                stepPos += deltaPos;
                dir = stepVelocity;
            }
        }
    }
}
