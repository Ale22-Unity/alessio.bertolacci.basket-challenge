using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControl : MonoBehaviour, IControlThrower
{
    [SerializeField] private FireBallModule _fireballModule;
    [SerializeField] private AIPlayerStateManager _stateManager;
    [SerializeField] private Thrower _thrower;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private float _errorMargin = 0.1f;
    [field: SerializeField] public float MinCooldown { get; private set; } = 1f;
    [field: SerializeField] public float MaxCooldown { get; private set; } = 3f;
    public bool WaitingThrow { get; private set; } = false;
    private UniTask _throw;
    public bool IsOwner => false;
    public bool CanThrow => _gameManager.GameStarted;
    public FireBallModule FireballModule => _fireballModule;

    private void Start()
    {
        _gameManager.AddPlayer(this);
        _stateManager.InitializeStateManager(this, AIPlayerStates.Idle, StateManagerUpdate.Frame);
    }

    public bool IsPlayerThrowing(out UniTask throwTask)
    {
        throwTask = _throw;
        return WaitingThrow;
    }

    public void ScoredPoints(ScoreCategory score)
    {
        if (_gameManager != null)
        {
            _gameManager.AddScore(score, this, _thrower.BallCameraTarget.position);
        }
    }

    public bool TryAssignThrowerToPos(ThrowPosition position)
    {
        return _thrower.TryAssignThrowerToPosition(position);
    }

    public void StartThrow()
    {
        float perc = Random.Range(0f, 1f);
        _throw = ThrowBall(perc);
        _throw.Preserve();
    }

    private async UniTask ThrowBall(float perc)
    {
        if (WaitingThrow) { return; }
        if (!_gameManager.GameStarted) { return; }
        WaitingThrow = true;
        await _thrower.ThrowFromInput(perc, _errorMargin);
        _gameManager.AssignThrowerToRandomPos(this);
        _thrower.ResetBall();
        WaitingThrow = false;
    }
}
