using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IThrowable
{
    public void ResetThrowable(Transform position);
    public UniTask<bool> SimulateThrow(ThrowStep[] steps, IControlThrower thrower);
}
