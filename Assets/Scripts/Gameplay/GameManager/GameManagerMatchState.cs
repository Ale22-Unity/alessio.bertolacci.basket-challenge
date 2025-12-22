using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerMatchState : BaseState<GameManagerStates, GameManager>
{
    private bool _tickingSet = false;

    public GameManagerMatchState(GameManagerStates key, GameManager ctx) : base(key, ctx)
    {
    }

    public override void EnterState()
    {
        Debug.Log("Game Started!");
        _ctx.MatchTimer.ResetTimer();
        if(GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<MatchStartedEvent>(new MatchStartedEvent(_ctx.MatchTimer));
        }
    }

    public override void ExitState()
    {
        Debug.Log("Game Ended!");
    }

    public override void FixedUpdate()
    {

    }

    public override void FrameUpdate()
    {
        _ctx.MatchTimer.UpdateTimer(Time.deltaTime);
        _ctx.BBBonusManager.UpdateBBBonus();
        if(_ctx.MatchTimer.InvertedTimer < _ctx.LastSeconds && !_tickingSet)
        {
            _tickingSet = true;
            _ctx.SetTickingTimer(true);
        }
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
