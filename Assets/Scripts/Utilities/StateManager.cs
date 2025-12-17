using System;
using System.Collections.Generic;
using UnityEngine;

public class StateManager<Estate, T, S> : MonoBehaviour where Estate : Enum where S : BaseState<Estate, T>
{
    protected Dictionary<Estate, S> _states = new Dictionary<Estate, S>();
    protected S _currentState;
    private StateManagerUpdate _updateType;

    public Estate GetCurrentState()
    {
        if(_currentState == null) { return default; }
        return _currentState.StateKey;
    }

    public virtual void InitializeStateManager(T ctx, Estate startinState, StateManagerUpdate updateType)
    {
        _updateType = updateType;
        ChangeState(startinState);
    }

    private void ChangeState(Estate state)
    {
        if (!_states.ContainsKey(state)) { return; }
        _currentState?.ExitState();
        _currentState = _states[state];
        _currentState.EnterState();
    }

    private bool TryChangeState()
    {
        if(_currentState == null) { return false; }
        Estate newState = _currentState.GetNextState();
        if(newState.Equals(_currentState.StateKey))
        {
            return false;
        }
        ChangeState(newState);
        return true;
    }

    public void Update()
    {
        if(_updateType == StateManagerUpdate.Fixed) { return; }
        if (!TryChangeState())
        {
            _currentState?.FrameUpdate();
        }
    }

    public void FixedUpdate()
    {
        if (_updateType == StateManagerUpdate.Frame) { return; }
        if (!TryChangeState())
        {
            _currentState?.FixedUpdate();
        }
    }

    public void OnDestroy()
    {
        _currentState.OnDestroy();
    }
}

public enum StateManagerUpdate
{
    Frame = 0,
    Fixed = 1,
    FrameAndFixed = 2,
}
