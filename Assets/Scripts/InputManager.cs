using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private float _maxSwipeDurationSec = 3f;
    [SerializeField] private float _screenMutiplier = 0.7f;
    private SwipeInput _currentSwipeInput;
    private Vector2 _currentTouchPos;

    private void Update()
    {
        if(_currentSwipeInput != null)
        {
            if(Time.time - _currentSwipeInput.StartTime > _maxSwipeDurationSec)
            {
                EndSwipeAction(_currentTouchPos);
            }
        }
    }

    public void UpdateTouchPosition(InputAction.CallbackContext ctx)
    {
        _currentTouchPos = ctx.ReadValue<Vector2>();
        if(_currentSwipeInput != null)
        {
            _currentSwipeInput.SetCurrentValues(_currentTouchPos);
        }
    }

    public void SetSwipeActive(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            CreateSwipeAction(_currentTouchPos);
        }
        if (ctx.phase == InputActionPhase.Canceled)
        {
            EndSwipeAction(_currentTouchPos);
        }
    }

    private void CreateSwipeAction(Vector2 startPos)
    {
        _currentSwipeInput = new SwipeInput();
        _currentSwipeInput.SetStartValues(startPos, Time.time, Screen.height, _screenMutiplier);
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<SwipeStartedEvent>(new SwipeStartedEvent(_currentSwipeInput));
        }
    }

    private void EndSwipeAction(Vector2 endPos)
    {
        if(_currentSwipeInput == null) {return; }
        _currentSwipeInput.SetEndValues(endPos);
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<SwipeEndedEvent>(new SwipeEndedEvent(_currentSwipeInput));
        }
        _currentSwipeInput = null;
    }
}


public class SwipeInput
{
    public float ThrowStrenghtPerc { get; private set; }
    public float StartTime { get; private set; }
    public Vector3 StartPosition { get; private set; }
    public Vector3 EndPosition { get; private set; }
    public Vector3 CurrentPosition { get; private set; }
    private float _maxDeltaY;
    private float _lastDeltaY;

    public void SetStartValues(Vector3 position, float startTime, int screenHeight, float screenPerc)
    {
        _maxDeltaY = screenHeight * screenPerc;
        StartTime = startTime;
        StartPosition = position;
        CurrentPosition = position;
        EndPosition = position;
        ThrowStrenghtPerc = 0;
    }

    public void SetEndValues(Vector3 position)
    {
        EndPosition = position;
        CurrentPosition = position;
        CalculateThrowStrenght();
    }

    public void SetCurrentValues(Vector3 currentPos)
    {
        CurrentPosition = currentPos;
        CalculateThrowStrenght();
    }

    private void CalculateThrowStrenght()
    {
        float deltaY = Mathf.Abs(StartPosition.y - CurrentPosition.y);
        _lastDeltaY = deltaY < _lastDeltaY ? _lastDeltaY : deltaY;
        _lastDeltaY = Mathf.Clamp(_lastDeltaY, 0, _maxDeltaY);
        ThrowStrenghtPerc = _lastDeltaY / _maxDeltaY;
    }
}
