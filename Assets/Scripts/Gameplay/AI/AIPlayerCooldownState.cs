using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerCooldownState : BaseState<AIPlayerStates, AIControl>
{
    private float _currentCooldown;
    private float _targetCooldown;

    public AIPlayerCooldownState(AIPlayerStates key, AIControl ctx) : base(key, ctx)
    {
    }

    public override void EnterState()
    {
        _targetCooldown = Random.Range(_ctx.MinCooldown, _ctx.MaxCooldown);
        _currentCooldown = 0;
        Debug.Log($"Started cooldown of {_targetCooldown} sec");
    }

    public override void ExitState()
    {

    }

    public override void FixedUpdate()
    {

    }

    public override void FrameUpdate()
    {
        _currentCooldown += Time.deltaTime;
    }

    public override AIPlayerStates GetNextState()
    {
        if (!_ctx.CanThrow) { return AIPlayerStates.Idle; }
        if (_currentCooldown >= _targetCooldown) { return AIPlayerStates.Throwing; }
        return AIPlayerStates.Cooldown;
    }

    public override void OnDestroy()
    {

    }
}
