using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour, IControlThrower
{
    [SerializeField] private Thrower _thrower;

    private void Awake()
    {
        if(GameClient.Client != null)
        {
            GameClient.Client.EventBus.Subscribe<SwipeEndedEvent>(On);
        }
    }

    private void OnDestroy()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Unsubscribe<SwipeEndedEvent>(On);
        }
    }

    private void On(SwipeEndedEvent e)
    {
        _thrower.ThrowFromInput(e.SwipeInput.ThrowStrenghtPerc).Forget();
    }

    public void OnAssignedToThrowPos(ThrowPosition position, float errorMarginPerc)
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<PlayerAssignedAtPositionEvent>(new PlayerAssignedAtPositionEvent(position, errorMarginPerc));
        }
    }
}
