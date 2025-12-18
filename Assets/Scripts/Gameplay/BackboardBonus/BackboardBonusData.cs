using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BackboardBonusData
{
    [field: SerializeField] public int ScoreBonus { get; set; }
    [field: SerializeField] public int Weight {  get; set; }
    [field: SerializeField] public float Duration { get; set; }
}
