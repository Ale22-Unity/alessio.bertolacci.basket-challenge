using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IThrowable
{
    public void ResetThrowable(Transform position);
    public UniTask SimulateThrow(ThrowStep[] steps, IControlThrower thrower);
}
