using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerControl : MonoBehaviour, IControlThrower
{
    [SerializeField] private Thrower _thrower;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Transform _cameraTargetThrow;
    [SerializeField] private float _errorMarginPerc = 0.1f;
    [SerializeField] private float _cameraMoveSpeed = 2f;
    [SerializeField] private float _cameraRotSpeed = 100f;
    private bool _waitingThrow = false;
    private UniTask _throw;

    public bool IsOwner => true;

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
        _throw = ThrowBall(e.SwipeInput.ThrowStrenghtPerc);
        _throw.Preserve();
    }

    private async UniTask ThrowBall(float perc)
    {
        if (_waitingThrow) { return; }
        if (!_gameManager.GameStarted) { Debug.Log("Game not started"); return; }
        _waitingThrow = true;
        if (GameClient.Client != null)
        {
            GameClient.Client.GameCamera.SetTarget(_cameraTargetThrow, _thrower.BallCameraTarget, _cameraRotSpeed, _cameraMoveSpeed);
        }
        await _thrower.ThrowFromInput(perc, _errorMarginPerc);
        _gameManager.AssignThrowerToRandomPos(this);
        _thrower.ResetBall();
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
            int addedPoints = _gameManager.AddScore(score, this, _thrower.FireBallActive);
        }
    }

    public bool IsPlayerThrowing(out UniTask throwTask)
    {
        throwTask = _throw;
        return _waitingThrow;
    }
}
