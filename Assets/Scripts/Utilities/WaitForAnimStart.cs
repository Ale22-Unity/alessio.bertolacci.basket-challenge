using UnityEngine;

public class WaitForAnimStart : CustomYieldInstruction
{
    private readonly Animator _animator;
    private readonly string _stateName;
    private readonly bool _isTag;
    private readonly int _layer;
    private readonly float _operationTimeout = 5;
    private readonly float _startTime;

    public override bool keepWaiting => !IsAnimationStarted();

    public WaitForAnimStart(Animator animator, string stateName, bool isTag = false, int layer = 0)
    {
        _animator = animator;
        _stateName = stateName;
        _isTag = isTag;
        _layer = layer;
        _startTime = Time.time;
    }

    private bool IsAnimationStarted()
    {
        if (string.IsNullOrEmpty(_stateName)) return true;
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(_layer);
        bool stopWaiting = _isTag ? info.IsTag(_stateName) : info.IsName(_stateName);
        if(stopWaiting) return true;
        else if (Time.time - _startTime > _operationTimeout)
        {
            Debug.LogWarning($"Wait for animation start {_stateName} triggered timeout");
            return true;
        }
        return false;
    }
}