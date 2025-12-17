using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReadyUpPanel : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _timerText;

    private void Awake()
    {
        if(GameClient.Client != null)
        {
            GameClient.Client.EventBus.Subscribe<ReadyUpTimerEvent>(On);
        }
    }

    private void OnDestroy()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Unsubscribe<ReadyUpTimerEvent>(On);
        }
    }

    private void On(ReadyUpTimerEvent e)
    {
        if(e.Category == TimerEventCategory.Started) { _root.SetActive(true); }
        else if(e.Category == TimerEventCategory.Ended) { _root.SetActive(false); }

        _timerText.text = e.Value.ToString();
    }

}
