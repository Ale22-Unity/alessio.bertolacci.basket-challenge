using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireBallBarUI : MonoBehaviour
{
    [SerializeField] private Image _filler;
    [SerializeField] private float _fillSpeed = 1;
    private float _targetFillAmount;

    private void Awake()
    {
        _targetFillAmount = 0;
        _filler.fillAmount = _targetFillAmount;
        if(GameClient.Client != null)
        {
            GameClient.Client.EventBus.Subscribe<SetFireballStatusEvent>(On);
        }
    }

    private void OnDestroy()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Unsubscribe<SetFireballStatusEvent>(On);
        }
    }

    private void Update()
    {
        _filler.fillAmount = Mathf.Lerp(_filler.fillAmount, _targetFillAmount, Time.deltaTime * _fillSpeed);
    }

    private void On(SetFireballStatusEvent e)
    {
        _targetFillAmount = e.BarPercentage;
    }


}
