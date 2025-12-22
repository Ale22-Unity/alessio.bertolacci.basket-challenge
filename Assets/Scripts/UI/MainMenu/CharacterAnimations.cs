using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimations : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private string _startingAnimName = "";
    [Space]
    [SerializeField] private string _winAnimName = "";
    [SerializeField] private string _loseAnimName = "";
    [SerializeField] private string _idleAnimName = "";
    [SerializeField] private string _shootAnimName = "";
    [Space]
    [SerializeField] private string _shootTriggerName = "";
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _throwClip;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(_startingAnimName))
        {
            _anim.Play(_startingAnimName);
        }
    }

    public void SetWinAnimation(bool winner)
    {
        if (!string.IsNullOrEmpty(_winAnimName) && !string.IsNullOrEmpty(_loseAnimName))
        {
            string animToPlay = winner ? _winAnimName : _startingAnimName;
            _anim.Play(animToPlay);
        }
    }

    public void SetIdleAnimation()
    {
        if (!string.IsNullOrEmpty(_idleAnimName))
        {
            _anim.Play(_idleAnimName);
        }
    }

    public async UniTask WaitShootBallAnimation()
    {
        _anim.SetTrigger(_shootTriggerName);
        await this.WaitForAnimationCompletion(_shootAnimName, _anim);
        _audioSource.clip = _throwClip;
        _audioSource.Play();
    }
}
