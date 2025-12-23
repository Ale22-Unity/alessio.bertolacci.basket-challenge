using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallModule : MonoBehaviour
{
    [SerializeField] private int _consecutiveThrowsAmount = 5;
    [SerializeField] private float _fireballDuration = 8f;
    [SerializeField] private AudioSource _fireballOutSource;
    private int _currentThrows = 0;
    private bool _onFire;
    private bool _isOwner;
    IEnumerator _fireballRoutine;
    public bool OnFire => _onFire;

    public void Setup(bool isOwner)
    {
        _isOwner = isOwner;
    }

    public void AddToFireBall()
    {
        if(_fireballRoutine == null)
        {
            _currentThrows++;
            float perc = (float)_currentThrows / _consecutiveThrowsAmount;
            perc = Mathf.Clamp(perc, 0f, 1f);
            if (GameClient.Client != null && _isOwner)
            {
                GameClient.Client.EventBus.Fire<SetFireballStatusEvent>(new SetFireballStatusEvent(perc, false));
            }
        }
        if (_currentThrows >= _consecutiveThrowsAmount && _fireballRoutine == null) 
        { 
            _onFire = true;
            _fireballRoutine = Fireball();
            StartCoroutine(_fireballRoutine);
        }
    }

    public void ResetFireBall()
    {
        if (_fireballRoutine != null)
        {
            StopCoroutine(_fireballRoutine);
            _fireballRoutine = null;
            _fireballOutSource.Play();
        }
        _onFire = false;
        _currentThrows = 0;
        if (GameClient.Client != null && _isOwner)
        {
            GameClient.Client.EventBus.Fire<SetFireballStatusEvent>(new SetFireballStatusEvent(0, false));
        }
    }

    private IEnumerator Fireball()
    {
        float _currentDuration = 0;
        while(_currentDuration < _fireballDuration)
        {
            yield return new WaitForEndOfFrame();
            _currentDuration += Time.deltaTime;
            float perc = 1f - (_currentDuration / _fireballDuration);
            perc = Mathf.Clamp(perc, 0f, 1f);
            if (GameClient.Client != null && _isOwner)
            {
                GameClient.Client.EventBus.Fire<SetFireballStatusEvent>(new SetFireballStatusEvent(perc, true));
            }
        }
        ResetFireBall();
        _fireballRoutine = null;
    }

}
