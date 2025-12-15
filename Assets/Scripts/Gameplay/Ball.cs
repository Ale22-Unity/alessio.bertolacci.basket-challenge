using Cysharp.Threading.Tasks;
using UnityEngine;

public class Ball : MonoBehaviour, IThrowable
{
    [SerializeField] private SphereCollider _collider;
    public float Radius => _collider.radius;

    public void ResetThrowable(Transform resetPos)
    {
        transform.position = resetPos.position;
    }

    public async UniTask SimulateThrow(ThrowStep[] steps)
    {
        foreach(ThrowStep step in steps)
        {
            await UniTask.Delay(step.Ms);
            transform.position = step.TargetPos;
        }
    }
}
