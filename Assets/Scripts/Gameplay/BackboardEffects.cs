using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackboardEffects : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private string _hitAnimClip;
    [SerializeField] private AudioSource _audioSource;

    public void BackboardHit()
    {
        _anim.Play(_hitAnimClip, 0, 0);
        _audioSource.Play();
    }

}
