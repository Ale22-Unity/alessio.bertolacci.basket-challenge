using System;
using System.Collections.Generic;

public class EventBus
{
    private readonly Dictionary<Type, List<object>> _eventList = new();

    public void Subscribe<T>(Action<T> action)
    {
        if (!_eventList.TryGetValue(typeof(T), out List<object> actionList))
        {
            actionList = new List<object>();
            _eventList[typeof(T)] = actionList;
        }
        actionList.Add(action);
    }

    public void Unsubscribe<T>(Action<T> action)
    {
        if (_eventList.TryGetValue(typeof(T), out List<object> actionList))
        {
            if (!actionList.Contains(action)) { return; }
            actionList.Remove(action);

            if (actionList.Count == 0)
                _eventList.Remove(typeof(T));
        }
    }

    public void Fire<T>(T actionData)
    {
        if (_eventList.TryGetValue(typeof(T), out List<object> actionList))
        {
            List<object> tempList = new List<object>(actionList);

            foreach (object obj in tempList)
            {
                if (obj is not Action<T> action) { continue; }
                action?.Invoke(actionData);
            }
        }
    }
}
