using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerThrowingState : BaseState<AIPlayerStates, AIControl>
{
    public AIPlayerThrowingState(AIPlayerStates key, AIControl ctx) : base(key, ctx)
    {
    }

    public override void EnterState()
    {
        Debug.Log("AI started throw");
        _ctx.StartThrow();
    }

    public override void ExitState()
    {

    }

    public override void FixedUpdate()
    {

    }

    public override void FrameUpdate()
    {

    }

    public override AIPlayerStates GetNextState()
    {
        if (_ctx.WaitingThrow) { return AIPlayerStates.Throwing; }
        if (!_ctx.CanThrow) { return AIPlayerStates.Idle;}
        return AIPlayerStates.Cooldown;
    }

    public override void OnDestroy()
    {

    }
}
