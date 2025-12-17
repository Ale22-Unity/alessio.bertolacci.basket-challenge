using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerStateFactory : StateManager<GameManagerStates, GameManager, BaseState<GameManagerStates, GameManager>>
{
    public override void InitializeStateManager(GameManager ctx, GameManagerStates startinState, StateManagerUpdate updateType)
    {
        _states.Add(GameManagerStates.ReadyUp, new GameManagerReadyUpState(GameManagerStates.ReadyUp, ctx));
        _states.Add(GameManagerStates.Match, new GameManagerMatchState(GameManagerStates.Match, ctx));
        _states.Add(GameManagerStates.WaitMatchEnd, new GameManagerWaitMatchEndState(GameManagerStates.WaitMatchEnd, ctx));
        base.InitializeStateManager(ctx, startinState, updateType);
    }
}


public enum GameManagerStates
{
    ReadyUp = 0,
    Match = 1,
    WaitMatchEnd = 2
}