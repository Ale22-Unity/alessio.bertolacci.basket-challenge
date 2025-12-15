using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPosition : MonoBehaviour
{
    [field:SerializeField] public Transform CleanTarget {  get; private set; }
    [field:SerializeField] public Transform BBTarget {  get; private set; }
    [field: SerializeField] public float MaxThrowSpeed { get; private set; }
    [field: SerializeField] public float MinThrowSpeed { get; private set; }
    [field: SerializeField] public float DeltaH { get; private set; } = 2;
    public SweetSpotInfo PerfetThrow {  get; private set; }
    public SweetSpotInfo BBThrow { get; private set; }

    [SerializeField] private Thrower _currentThrower;

    public bool TrySetThrowerToPosition(Thrower thrower)
    {
        if(_currentThrower != null) return false;
        _currentThrower = thrower;
        thrower.transform.position = transform.position;
        Vector3 dir = BBTarget.position - thrower.transform.position;
        dir = new Vector3(dir.x, 0, dir.z);
        Quaternion newRot = Quaternion.LookRotation(dir);
        thrower.transform.rotation = newRot;
        return true;
    }
    public void RemoveThrowerFromPosition()
    {
        _currentThrower = null;
    }

    public void SetSweetSpotData(SweetSpotInfo perfetThrow, SweetSpotInfo bbThrow)
    {
        PerfetThrow = perfetThrow;
        BBThrow = bbThrow;
    }
}

public readonly struct SweetSpotInfo
{
    public readonly float Velocity;
    public readonly Vector3 Direction;

    public SweetSpotInfo(float velocity, Vector3 direction)
    {
        Velocity = velocity;
        Direction = direction;
    }
}


