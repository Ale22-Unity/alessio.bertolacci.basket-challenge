using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerReadyUpState : BaseState<GameManagerStates, GameManager>
{
    private int _timerInt = 0;
    private bool _timerEnded = false;

    public GameManagerReadyUpState(GameManagerStates key, GameManager ctx) : base(key, ctx)
    {
    }

    public override void EnterState()
    {
        _ctx.ReadyUpTimer.ResetTimer();
        _timerEnded = _ctx.ReadyUpTimer.TimerEnded();
        _timerInt = (int)_ctx.ReadyUpTimer.InvertedTimer;
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<ReadyUpTimerEvent>(new ReadyUpTimerEvent(TimerEventCategory.Started, _timerInt));
        }
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
        int tepTimerInt = (int)_ctx.ReadyUpTimer.InvertedTimer;
        if (_ctx.ReadyUpTimer.TimerEnded())
        {
            _timerEnded = true;
            if (GameClient.Client != null)
            {
                GameClient.Client.EventBus.Fire<ReadyUpTimerEvent>(new ReadyUpTimerEvent(TimerEventCategory.Ended, tepTimerInt));
            }
            return;
        }
        if(tepTimerInt != _timerInt)
        {
            _timerInt = tepTimerInt;
            if (GameClient.Client != null)
            {
                GameClient.Client.EventBus.Fire<ReadyUpTimerEvent>(new ReadyUpTimerEvent(TimerEventCategory.Update, _timerInt));
            }
        }
        
    }

    public override GameManagerStates GetNextState()
    {
        if(_timerEnded) { return GameManagerStates.Match; }
        return GameManagerStates.ReadyUp;
    }

    public override void OnDestroy()
    {
       
    }
}
    