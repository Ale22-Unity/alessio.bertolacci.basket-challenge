using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour, IControlThrower
{
    [SerializeField] private FireBallModule _fireballModule;
    [SerializeField] private Thrower _thrower;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Transform _cameraTargetThrow;
    [SerializeField] private float _errorMarginPerc = 0.1f;
    [SerializeField] private float _cameraMoveSpeed = 2f;
    [SerializeField] private float _cameraRotSpeed = 100f;
    private bool _waitingThrow = false;
    private UniTask _throw;
    private bool _validSwipe = false;

    public bool IsOwner => true;
    public FireBallModule FireballModule => _fireballModule;

    private void Start()
    {
        if (_gameManager != null)
        {
            _fireballModule.Setup(IsOwner);
            _gameManager.AddPlayer(this);
        }
    }

    private void Awake()
    {
        if(GameClient.Client != null)
        {
            GameClient.Client.EventBus.Subscribe<SwipeStartedEvent>(On);
            GameClient.Client.EventBus.Subscribe<SwipeEndedEvent>(On);
        }
    }

    private void OnDestroy()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Unsubscribe<SwipeStartedEvent>(On);
            GameClient.Client.EventBus.Unsubscribe<SwipeEndedEvent>(On);
        }
    }

    private void On(SwipeStartedEvent e)
    {
        _validSwipe = false;
        if (_waitingThrow) { return; }
        if (!_gameManager.GameStarted) { return; }
        _validSwipe = true;
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<ValidSwipeStartedEvent>(new ValidSwipeStartedEvent(e.SwipeInput));
        }
    }

    private void On(SwipeEndedEvent e)
    {
        _throw = ThrowBall(e.SwipeInput.ThrowStrenghtPerc);
        _throw.Preserve();
    }

    private async UniTask ThrowBall(float perc)
    {
        if (!_validSwipe) { return; }
        _waitingThrow = true;
        await _thrower.ThrowFromInput(perc, _errorMarginPerc);
        _gameManager.AssignThrowerToRandomPos(this);
        _thrower.ResetBall();
        _waitingThrow = false;
    }

    public void SetCameraToThrowTarget()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.GameCamera.SetTarget(_cameraTargetThrow, _thrower.BallCameraTarget, _cameraRotSpeed, _cameraMoveSpeed);
        }
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
            int addedPoints = _gameManager.AddScore(score, this, _thrower.BallCameraTarget.position);
        }
    }

    public bool IsPlayerThrowing(out UniTask throwTask)
    {
        throwTask = _throw;
        return _waitingThrow;
    }
}
