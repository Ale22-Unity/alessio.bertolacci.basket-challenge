using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<PlayerData> _playerData = new List<PlayerData>();
    [SerializeField] private List<ScoreEntry> _scores = new List<ScoreEntry>();
    [SerializeField] private ThrowPosition[] _throwPositions;
    [SerializeField] private bool _gameStarted = true;

    public bool GameStarted => _gameStarted;

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
        if(data == null) { return; }
        ScoreEntry selectedScore = _scores.FirstOrDefault(e => e.Category == category);
        data.AddScore(selectedScore.ScoreAmount);
        return selectedScore.ScoreAmount;
    }
}

[System.Serializable]
public struct ScoreEntry
{
    [field:SerializeField] public ScoreCategory Category {  get; private set; }
    [field: SerializeField] public int ScoreAmount { get; private set; }
}
