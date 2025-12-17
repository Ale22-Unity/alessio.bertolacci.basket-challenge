using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] private bool forOwner;
    [SerializeField] Image _timerBG;
    [SerializeField] private TMP_Text _scoreText;
    private TimerData _timerData;
    private void Awake()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Subscribe<MatchStartedEvent>(On);
            GameClient.Client.EventBus.Subscribe<ScoreAddedEvent>(On);
        }
    }

    private void OnDestroy()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Unsubscribe<MatchStartedEvent>(On);
            GameClient.Client.EventBus.Unsubscribe<ScoreAddedEvent>(On);
        }
    }

    private void On(MatchStartedEvent e)
    {
        _timerData = e.MatchTimer; 
    }

    private void Update()
    {
        UpdateTimerGraphics();
    }

    private void On(ScoreAddedEvent e)
    {
        if(forOwner != e.IsOwner) { return; }
        _scoreText.text = e.TotPoints.ToString();
    }

    private void UpdateTimerGraphics()
    {
        if (_timerData == null) { return; }
        float perc = 1 - _timerData.TimerPerc;
        _timerBG.fillAmount = perc;
    }
}
