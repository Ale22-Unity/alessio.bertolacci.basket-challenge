using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerMatchState : BaseState<GameManagerStates, GameManager>
{
    public GameManagerMatchState(GameManagerStates key, GameManager ctx) : base(key, ctx)
    {
    }

    public override void EnterState()
    {
        _ctx.MatchTimer.ResetTimer();
    }

    public override void ExitState()
    {

    }

    public override void FixedUpdate()
    {

    }

    public override void FrameUpdate()
    {
        _ctx.MatchTimer.UpdateTimer(Time.deltaTime);
    }

    public override GameManagerStates GetNextState()
    {
        if (_ctx.MatchTimer.TimerEnded()) { return GameManagerStates.WaitMatchEnd; }
        return GameManagerStates.Match;
    }

    public override void OnDestroy()
    {

    }
}
