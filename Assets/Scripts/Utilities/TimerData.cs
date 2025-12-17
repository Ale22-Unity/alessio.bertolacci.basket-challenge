using UnityEngine;

[System.Serializable]
public class TimerData
{
    [field: SerializeField] public float TimerDurationS { get; private set; } = 3;
    [SerializeField] private float _currentTimer = 0;

    public float CurrentTimer => _currentTimer;

    public void UpdateTimer(float deltaTime)
    {
        _currentTimer += deltaTime;
    }

    public void ResetTimer()
    {
        _currentTimer = 0;
    }

    public bool TimerEnded()
    {
        return _currentTimer >= TimerDurationS;
    }
}
