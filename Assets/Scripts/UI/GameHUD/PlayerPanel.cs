using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] private string _onFireText = "On Fire!<br>";
    [SerializeField] private CategoryTextInfo[] _categories;
    [SerializeField] private bool forOwner;
    [SerializeField] Image _timerBG;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private ContextUIParticleDesign _scoredParticleDesign;
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
        if (e.IsOwner)
        {
            if (GameClient.Client != null)
            {
                string scoreText = GetScoreText(e.Category, e.AddedPoints, e.OnFire, out Color textColor);
                ContextUIParticleData particleData = new ContextUIParticleData(
                    new ParticlesContextPosition(e.BallPosition),
                    new ParticlesContextPosition((Vector2)GetComponent<RectTransform>().position),
                    _scoredParticleDesign,
                    null,
                    1,
                    scoreText,
                    textColor
                    );
                PlayContextParticlesEvent eventData = new PlayContextParticlesEvent(new List<ContextUIParticleData>() { particleData },
                    new UniTaskCompletionSource<bool>());
                GameClient.Client.EventBus.Fire<PlayContextParticlesEvent>(eventData);
            }
        }
    }

    private string GetScoreText(ScoreCategory category, int added, bool onFire, out Color textColor)
    {
        string text = string.Empty;
        if (onFire) { text += _onFireText; }
        text += GetCategoryText(category, out textColor);
        text += $"+{added}pt.";
        return text;
    }
    
    private string GetCategoryText(ScoreCategory category, out Color color)
    {
        CategoryTextInfo info = _categories.FirstOrDefault(c => c.Category == category);
        color = info.Color;
        return info.Text;
    }

    private void UpdateTimerGraphics()
    {
        if (_timerData == null) { return; }
        float perc = 1 - _timerData.TimerPerc;
        _timerBG.fillAmount = perc;
    }
}

[System.Serializable]
public struct CategoryTextInfo
{
    [field:SerializeField] public ScoreCategory Category {  get; private set; }
    [field: SerializeField] public string Text { get; private set; }
    [field: SerializeField] public Color Color { get; private set; }
}