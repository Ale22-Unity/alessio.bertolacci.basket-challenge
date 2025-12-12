using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;

public static class Extensions
{
    public static async UniTask WaitForAnimationCompletion(this MonoBehaviour behaviour, string animName, Animator anim, bool isTag = false, int layer = 0)
    {
        if(behaviour == null) { return; }
        UniTaskCompletionSource<bool> completionSource = new UniTaskCompletionSource<bool>();
        behaviour.StartCoroutine(WaitForAnimation(completionSource, animName, anim, isTag, layer));
        await completionSource.Task;
    }
    private static IEnumerator WaitForAnimation(UniTaskCompletionSource<bool> animationComplete, string animStateName, Animator animator, bool isTag, int layer)
    {
        yield return new WaitForAnimStart(animator, animStateName, isTag, layer);
        yield return new WaitForAnimEnd(animator, animStateName, isTag, layer);
        animationComplete.TrySetResult(true);
    }
}
