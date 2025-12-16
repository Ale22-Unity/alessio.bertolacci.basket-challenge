using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private float _targetError = 0.1f;
    [SerializeField] private float _moveSpeed = 10;
    [SerializeField] private float _rotationSpeed = 10;
    private Transform _posTarget;
    private Transform _rotTarget;
    private bool _atPos;
    private bool _atRot;

    private void LateUpdate()
    {
        _atPos = UpdatePosition();
        _atRot = UpdateRotation();
    }

    private bool UpdatePosition()
    {
        if(_posTarget == null) { return true; }
        transform.position = Vector3.Lerp(transform.position, _posTarget.position, Time.deltaTime * _moveSpeed);
        return Vector3.Distance(transform.position, _posTarget.position) > _targetError;
    }

    private bool UpdateRotation()
    {
        if (_rotTarget == null) { return true; }
        Vector3 dir = _rotTarget.position - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * _rotationSpeed);
        return Quaternion.Angle(transform.rotation, targetRot) > _targetError;
    }

    public void SetTarget(Transform posTarget, Transform rotTarget)
    {
        _posTarget = posTarget;
        _rotTarget = rotTarget;
    }
}
