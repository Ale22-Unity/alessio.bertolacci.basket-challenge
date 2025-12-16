using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private float _targetError = 0.1f;
    [SerializeField] private float _moveSpeedDefault = 10;
    [SerializeField] private float _rotationSpeedDefault = 100;
    private Transform _posTarget;
    private Transform _rotTarget;
    private float _moveSpeed;
    private float _rotationSpeed;
    private bool _atPos;

    private void LateUpdate()
    {
        _atPos = UpdatePosition();
        UpdateRotation();
    }


    private bool UpdatePosition()
    {
        if(_posTarget == null) { return true; }
        transform.position = Vector3.Lerp(transform.position, _posTarget.position, Time.deltaTime * _moveSpeed);
        return Vector3.Distance(transform.position, _posTarget.position) > _targetError;
    }

    private void UpdateRotation()
    {
        if (_rotTarget == null) { return; }
        transform.LookAt(_rotTarget, Vector3.up);
    }

    //private void UpdateRotation()
    //{
    //    if (_rotTarget == null) { return; }
    //    Vector3 dir = _rotTarget.position - transform.position;
    //    Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _rotationSpeed);
    //}

    public void SetTarget(Transform posTarget, Transform rotTarget)
    {
        _posTarget = posTarget;
        _rotTarget = rotTarget;
        _moveSpeed = _moveSpeedDefault;
        _rotationSpeed = _rotationSpeedDefault;
    }

    public void SetTarget(Transform posTarget, Transform rotTarget, float rotSpeed, float moveSpeed)
    {
        _posTarget = posTarget;
        _rotTarget = rotTarget;
        _moveSpeed = moveSpeed;
        _rotationSpeed = rotSpeed;
    }
}
