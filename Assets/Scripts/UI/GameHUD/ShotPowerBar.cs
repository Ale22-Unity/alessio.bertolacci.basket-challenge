using System;
using UnityEngine;
using UnityEngine.UI;

public class ShotPowerBar : MonoBehaviour
{

    [SerializeField] private RectTransform _powerBar, _cleanIndicator, _bbIndicator, _powerIndicator;
    [SerializeField] private Image _powerBarFiller;

    private SwipeInput _currentSwipe;
    private float _barHeight;

    private void Awake()
    {
        _barHeight = _powerBar.rect.height;
        _powerIndicator.gameObject.SetActive(false);
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Subscribe<SwipeStartedEvent>(On);
            GameClient.Client.EventBus.Subscribe<SwipeEndedEvent>(On);
        }
    }

    private void OnDestroy()
    {
        if(GameClient.Client != null)
        {
            GameClient.Client.EventBus.Unsubscribe<SwipeStartedEvent>(On);
            GameClient.Client.EventBus.Unsubscribe<SwipeEndedEvent>(On);
        }
    }

    private void Update()
    {
        if(_currentSwipe != null)
        {
            DrawIndicator(_currentSwipe.ThrowStrenghtPerc);
        }
    }

    private void DrawPowerRequirements(float cleanPerc, float bbPerc, float marginPerc)
    {
        _cleanIndicator.transform.localPosition = new Vector3(_cleanIndicator.transform.localPosition.x, _barHeight * cleanPerc, 0);
        _bbIndicator.transform.localPosition = new Vector3(_bbIndicator.transform.localPosition.x, _barHeight * bbPerc, 0);

        _cleanIndicator.sizeDelta = new Vector2(_cleanIndicator.sizeDelta.x, _barHeight * marginPerc);
        _bbIndicator.sizeDelta = new Vector2(_bbIndicator.sizeDelta.x, _barHeight * marginPerc);

        HideIndicator();
    }

    private void DrawIndicator(float perc)
    {
        _powerIndicator.transform.localPosition = new Vector3(_powerIndicator.transform.localPosition.x, _barHeight * perc, 0);
        _powerBarFiller.fillAmount = perc;
    }

    private void ShowIndicator()
    {
        _powerIndicator.gameObject.SetActive(true);
    }

    private void HideIndicator()
    {
        _powerIndicator.gameObject.SetActive(false);
        _powerBarFiller.fillAmount = 0;
    }

    private void On(SwipeEndedEvent e)
    {
        _currentSwipe = null;
        HideIndicator();
    }

    private void On(SwipeStartedEvent e)
    {
        DrawPowerRequirements(0.5f, 0.65f, 0.1f);
        ShowIndicator();
        _currentSwipe = e.SwipeInput;
    }


}
