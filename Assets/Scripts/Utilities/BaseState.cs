using System;
using UnityEngine;

public abstract class BaseState<EState, T> where EState : Enum
{
    public EState StateKey { get; private set; }
    protected T _ctx;

    public BaseState(EState key, T ctx)
    {
        _ctx = ctx;
        StateKey = key;
    }

    public abstract void EnterState();

    public abstract void FrameUpdate();

    public abstract void FixedUpdate();

    public abstract void ExitState();

    public abstract EState GetNextState();

    public abstract void OnDestroy();

}
