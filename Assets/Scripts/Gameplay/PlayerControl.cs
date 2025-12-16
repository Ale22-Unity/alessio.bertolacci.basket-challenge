using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour, IControlThrower
{
    [SerializeField] private Thrower _thrower;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Transform _cameraTarget;
    private bool _waitingThrow = false;

    private void Start()
    {
        if (_gameManager != null)
        {
            _gameManager.AssignThrowerToRandomPos(_thrower);
        }
    }

    private void Awake()
    {
        if(GameClient.Client != null)
        {
            GameClient.Client.EventBus.Subscribe<SwipeEndedEvent>(On);
        }
    }

    private void OnDestroy()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Unsubscribe<SwipeEndedEvent>(On);
        }
    }

    private void On(SwipeEndedEvent e)
    {
        ThrowBall(e.SwipeInput.ThrowStrenghtPerc).Forget();
    }

    private async UniTask ThrowBall(float perc)
    {
        if (_waitingThrow) { return; }
        _waitingThrow = true;
        if (!_gameManager.GameStarted) { return; }
        await _thrower.ThrowFromInput(perc);
        _gameManager.AssignThrowerToRandomPos(_thrower);
        _waitingThrow = false;
    }

    public void OnAssignedToThrowPos(ThrowPosition position, float errorMarginPerc)
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.GameCamera.SetTarget(_cameraTarget, position.CleanTarget);
            GameClient.Client.EventBus.Fire<PlayerAssignedAtPositionEvent>(new PlayerAssignedAtPositionEvent(position, errorMarginPerc));
        }
    }
}
