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
    [SerializeField] private float _errorMarginPerc = 0.1f;
    private bool _waitingThrow = false;

    private void Start()
    {
        if (_gameManager != null)
        {
            _gameManager.AddPlayer(this);
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
        await _thrower.ThrowFromInput(perc, _errorMarginPerc);
        _gameManager.AssignThrowerToRandomPos(this);
        _waitingThrow = false;
    }

    public bool TryAssignThrowerToPos(ThrowPosition position)
    {
        bool assign = _thrower.TryAssignThrowerToPosition(position);
        if (!assign) { return false; }
        if (GameClient.Client != null)
        {
            GameClient.Client.GameCamera.SetTarget(_cameraTarget, position.CleanTarget);
            GameClient.Client.EventBus.Fire<PlayerAssignedAtPositionEvent>(new PlayerAssignedAtPositionEvent(position, _errorMarginPerc));
        }
        return true;
    }

    public void ScoredPoints(ScoreCategory score)
    {
        if(_gameManager != null)
        {
            int addedPoints = _gameManager.AddScore(score, this);
        }
    }
}
