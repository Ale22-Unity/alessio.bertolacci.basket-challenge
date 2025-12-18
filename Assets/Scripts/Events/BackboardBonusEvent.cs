using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackboardBonusEvent : MonoBehaviour
{
    public readonly bool Started = false;
    public readonly int ScoreBonus = 0;

    public BackboardBonusEvent(bool started, int scoreBonus)
    {
        Started = started;
        ScoreBonus = scoreBonus;
    }
}
