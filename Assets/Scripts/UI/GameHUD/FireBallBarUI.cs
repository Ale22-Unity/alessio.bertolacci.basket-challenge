using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireBallBarUI : MonoBehaviour
{
    [SerializeField] private Image _fireballImage;
    [SerializeField] private Image _overlayImage;
    [SerializeField] private Color _overlayColorNormal;
    [SerializeField] private Color _overlayColorFire;
    [Space]
    [SerializeField] private Image _filler;
    [SerializeField] private float _fillSpeed = 1;
    private float _targetFillAmount;
    private bool _prevValue = false;

    private void Awake()
    {
        SetOnFireGraphics(_prevValue);
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
        if (_prevValue != e.Active)
        {
            SetOnFireGraphics(e.Active);
            _prevValue = e.Active;
        }
    }

    private void SetOnFireGraphics(bool on)
    {
        _fireballImage.gameObject.SetActive(on);
        _overlayImage.color = on ? _overlayColorFire : _overlayColorNormal;
    }


}
