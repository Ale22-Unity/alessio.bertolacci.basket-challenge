using System;
using TMPro;
using UnityEngine;

public class BackboardBonusUI : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _scoreBonusText;

    private void Awake()
    {
        _root.SetActive(false);
        if(GameClient.Client != null)
        {
            GameClient.Client.EventBus.Subscribe<BackboardBonusEvent>(On);
        }
    }

    private void OnDestroy()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Unsubscribe<BackboardBonusEvent>(On);
        }
    }

    private void On(BackboardBonusEvent e)
    {
        _root.SetActive(e.Started);
        _scoreBonusText.text = e.ScoreBonus.ToString();
    }
}
