using UnityEngine;

public class WaitForAnimEnd : CustomYieldInstruction
{
    private readonly Animator _animator;
    private readonly string _stateName;
    private readonly bool _isTag;
    private readonly int _layer;
    public override bool keepWaiting => !IsAnimationEnded();

    public WaitForAnimEnd(Animator animator, string stateName, bool isTag = false, int layer = 0)
    {
        _animator = animator;
        _isTag = isTag;
        _stateName = stateName;
        _layer = layer;
    }

    private bool IsAnimationEnded()
    {
        if(string.IsNullOrEmpty(_stateName)) return true;
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(_layer);
        bool isCorrect = _isTag ? info.IsTag(_stateName) : info.IsName(_stateName);
        if (isCorrect)
        {
            if(info.loop == false && info.normalizedTime >= 1)
            {
                return true;
            }
            return false;
        }
        return true;
    }
}
