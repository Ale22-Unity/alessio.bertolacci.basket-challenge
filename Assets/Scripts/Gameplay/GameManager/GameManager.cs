using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameManagerStateFactory _stateFactory;
    [SerializeField] private List<PlayerData> _playerData = new List<PlayerData>();
    [SerializeField] private List<ScoreEntry> _scores = new List<ScoreEntry>();
    [SerializeField] private ThrowPosition[] _throwPositions;
    [field: SerializeField] public TimerData ReadyUpTimer { get; private set; }
    [field: SerializeField] public TimerData MatchTimer { get; private set; }

    public bool GameStarted => _stateFactory.GetCurrentState() == GameManagerStates.Match;

    private void Start()
    {
        _stateFactory.InitializeStateManager(this, GameManagerStates.ReadyUp, StateManagerUpdate.Frame);
    }

    public void AddPlayer(IControlThrower player)
    {
        if(_playerData.Any(p => p.Player == player)){ return; }
        _playerData.Add(new PlayerData(player));
        AssignThrowerToRandomPos(player);
    }


    public void AssignThrowerToRandomPos(IControlThrower thrower)
    {
        bool assigned = false;
        while (!assigned)
        {
            int selectedPos = Random.Range(0, _throwPositions.Length); 
            assigned = thrower.TryAssignThrowerToPos(_throwPositions[selectedPos]);
        }
    }

    public int AddScore(ScoreCategory category, IControlThrower player)
    {
        PlayerData data = _playerData.FirstOrDefault((p) => p.Player == player);
        if(data == null) { return 0; }
        ScoreEntry selectedScore = _scores.FirstOrDefault(e => e.Category == category);
        data.AddScore(selectedScore.ScoreAmount);
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<ScoreAddedEvent>(
                new ScoreAddedEvent(selectedScore.ScoreAmount, data.Score, player.IsOwner));
        }
        return selectedScore.ScoreAmount;
    }

    public async UniTask WaitForAllThrows()
    {
        List<UniTask> throws = new List<UniTask>();
        foreach(PlayerData player in _playerData)
        {
            if(player.Player.IsPlayerThrowing(out UniTask ballThrow))
            {
                throws.Add(ballThrow);
            }
        }
        await UniTask.WhenAll(throws);
    }

    public void EndGame()
    {
        Debug.Log("end game");
        if(GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<GameEndedEvent>(new GameEndedEvent());
        }
    }
}

[System.Serializable]
public struct ScoreEntry
{
    [field:SerializeField] public ScoreCategory Category {  get; private set; }
    [field: SerializeField] public int ScoreAmount { get; private set; }
}
