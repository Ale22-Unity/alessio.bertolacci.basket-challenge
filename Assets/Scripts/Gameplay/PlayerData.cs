using System.Collections;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public readonly IControlThrower Player;
    [field:SerializeField] public int Score { get; private set; }

    public PlayerData(IControlThrower player)
    {
        Player = player;
    }

    public void AddScore(int amount)
    {
        Score += amount;
    }

    public PlayerResult GetPlayerResult()
    {
        return new PlayerResult(Score, Player.IsOwner);
    }
}
