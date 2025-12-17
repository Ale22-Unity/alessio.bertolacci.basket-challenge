using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerReadyUpState : BaseState<GameManagerStates, GameManager>
{

    public GameManagerReadyUpState(GameManagerStates key, GameManager ctx) : base(key, ctx)
    {
    }

    public override void EnterState()
    {
        _ctx.ReadyUpTimer.ResetTimer();
    }

    public override void ExitState()
    {
        
    }

    public override void FixedUpdate()
    {

    }

    public override void FrameUpdate()
    {
        _ctx.ReadyUpTimer.UpdateTimer(Time.deltaTime);
    }

    public override GameManagerStates GetNextState()
    {
        if(_ctx.ReadyUpTimer.TimerEnded()) { return GameManagerStates.Match; }
        return GameManagerStates.ReadyUp;
    }

    public override void OnDestroy()
    {
       
    }
}
