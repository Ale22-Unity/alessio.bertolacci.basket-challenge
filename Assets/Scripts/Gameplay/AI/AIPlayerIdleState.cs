using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerIdleState : BaseState<AIPlayerStates, AIControl>
{
    public AIPlayerIdleState(AIPlayerStates key, AIControl ctx) : base(key, ctx)
    {
    }

    public override void EnterState()
    {

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
        if (_ctx.CanThrow)
        {
            return AIPlayerStates.Throwing;
        }
        return AIPlayerStates.Idle;
    }

    public override void OnDestroy()
    {

    }
}
