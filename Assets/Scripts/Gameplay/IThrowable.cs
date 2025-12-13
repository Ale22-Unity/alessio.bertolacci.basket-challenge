using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IThrowable
{
    public void ResetThrowable(Transform position);
    public void Throw(Vector3 direction, float v0, float time);
}
