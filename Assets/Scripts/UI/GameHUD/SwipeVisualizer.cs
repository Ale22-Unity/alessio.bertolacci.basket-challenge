using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SwipeVisualizer : MonoBehaviour
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private int _amount = 10;
    [SerializeField] private RectTransform _swipeElementPrefab;
    [SerializeField] private AnimationCurve _scaleCurve;
    private RectTransform[] _elements;
    private SwipeInput _currentSwipe;
    private RectTransform _thisRect;

    private void Awake()
    {
        _thisRect = GetComponent<RectTransform>();
        Initialize();
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Subscribe<SwipeStartedEvent>(On);
            GameClient.Client.EventBus.Subscribe<SwipeEndedEvent>(On);
        }
    }

    private void OnDestroy()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Unsubscribe<SwipeStartedEvent>(On);
            GameClient.Client.EventBus.Unsubscribe<SwipeEndedEvent>(On);
        }
    }

    private void Initialize()
    {
        _elements = new RectTransform[_amount];
        for(int i = 0; i < _amount; i++)
        {
            RectTransform element = Instantiate(_swipeElementPrefab, this.transform);
            _elements[i] = element;
            element.gameObject.SetActive(false);
            float scale = _scaleCurve.Evaluate((float)i/(_amount - 1));
            element.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void SetElementsActive(Vector2 pos)
    {
        for (int i = 0; i < _amount; i++)
        {
            _elements[i].gameObject.SetActive(true);
            _elements[i].transform.localPosition = _thisRect.InverseTransformPoint(pos);
        }
    }

    private void SetElementsInactive()
    {
        for (int i = 0; i < _amount; i++)
        {
            _elements[i].gameObject.SetActive(false);
        }
    }

    private void On(SwipeEndedEvent e)
    {
        _currentSwipe = null;
        SetElementsInactive();
    }

    private void On(SwipeStartedEvent e)
    {
        _currentSwipe = e.SwipeInput;
        Vector2 uiPos = _currentSwipe.StartPosition;
        SetElementsActive(uiPos);
    }

    private void Update()
    {
        if(_currentSwipe == null) { return; }
        Vector2 targetPos = _thisRect.InverseTransformPoint(_currentSwipe.CurrentPosition);
        for (int i = 0; i < _amount; i++)
        {
            Transform element = _elements[i].transform;
            element.localPosition = Vector2.Lerp(element.localPosition, targetPos, Time.deltaTime * _speed);
            targetPos = element.localPosition;
        }
    }
}
