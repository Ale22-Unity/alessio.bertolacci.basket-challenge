using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndedEvent
{
    public readonly PlayerResult[] MatchResults;

    public GameEndedEvent(PlayerResult[] matchResults)
    {
        MatchResults = matchResults;
    }
}

public readonly struct PlayerResult
{
    public readonly bool Winner;
    public readonly int Score;
    public readonly bool IsOwner;

    public PlayerResult(int score, bool isOwner, bool winner)
    {
        Score = score;
        IsOwner = isOwner;
        Winner = winner;
    }
}
