using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackboardBonusManager : MonoBehaviour
{
    [SerializeField] private BackboardBonusData[] _possibleBonuses = new BackboardBonusData[3];
    [SerializeField] private float MaxCooldown;
    [SerializeField] private float MinCooldown;
    private float _currentCooldown = 0;
    private float _currentTimer = 0;
    public bool Active { get; private set; } = false;
    public BackboardBonusData ActiveBBBonusData { get; private set; }

    private void Awake()
    {
        SetBBBonusInactive();
    }

    public void UpdateBBBonus()
    {
        UpdateCooldow();
        UpdateBBBonusTimer();
    }

    private BackboardBonusData GetWeightedBackboardBonus()
    {
        int totWeights = 0;
        foreach (BackboardBonusData bbBonus in _possibleBonuses)
        {
            totWeights += bbBonus.Weight;
        }
        int selectedBonus = Random.Range(0, totWeights);
        int prev = 0;
        foreach (BackboardBonusData bbBonus in _possibleBonuses)
        {
            if (Utils.BetweenValuesCheck(prev, prev + bbBonus.Weight, selectedBonus))
            {
                return bbBonus;
            }
            prev += bbBonus.Weight;
        }
        return _possibleBonuses[0];
    }

    private void SetActiveBBBonus(BackboardBonusData bbBonus)
    {
        Active = true;
        ActiveBBBonusData = bbBonus;
        _currentTimer = bbBonus.Duration;
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<BackboardBonusEvent>(new BackboardBonusEvent(true, bbBonus.ScoreBonus));
        }
    }

    private void SetBBBonusInactive()
    {
        Active = false;
        _currentCooldown = Random.Range(MinCooldown, MaxCooldown);
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<BackboardBonusEvent>(new BackboardBonusEvent(false, 0));
        }
    }

    private void UpdateCooldow()
    {
        if(_currentCooldown > 0 && !Active) { _currentCooldown -= Time.deltaTime; }
        else if (!Active) 
        {
            SetActiveBBBonus(GetWeightedBackboardBonus());
        }
    }

    private void UpdateBBBonusTimer()
    {
        if(_currentTimer > 0 && Active)
        {
            _currentTimer -= Time.deltaTime;
        }
        else if (Active)
        {
            SetBBBonusInactive();
        }
    }
}
