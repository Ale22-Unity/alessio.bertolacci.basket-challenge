using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerWaitMatchEndState : BaseState<GameManagerStates, GameManager>
{
    public GameManagerWaitMatchEndState(GameManagerStates key, GameManager ctx) : base(key, ctx)
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

    public override GameManagerStates GetNextState()
    {
        return GameManagerStates.WaitMatchEnd;
    }

    public override void OnDestroy()
    {

    }
}
