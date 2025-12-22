using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketEffects : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _perfectClip;
    [SerializeField] private AudioClip _normalClip;
    [SerializeField] private ParticleSystem _perfectParticles;
    [SerializeField] private ParticleSystem _normalParticles;

    public void PlayBasketEffects(bool perfectThrow)
    {
        _audioSource.clip = perfectThrow ? _perfectClip : _normalClip;
        _audioSource.Play();
        ParticleSystem particles = perfectThrow ? _perfectParticles : _normalParticles;
        particles.Play();
    }
}
