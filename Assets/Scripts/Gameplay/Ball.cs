using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IThrowable
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private SphereCollider _collider;
    public float Radius => _collider.radius;

    public void ResetThrowable(Transform resetPos)
    {
        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;
        transform.position = resetPos.position;
    }

    public void Throw(Vector3 direction, float v0, float time)
    {
        _rb.isKinematic = false;
        _rb.velocity = direction;
    }
}
