using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAudio : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudioSource _audioSource;

    public void OnPointerClick(PointerEventData eventData)
    {
        _audioSource.Play();
    }
}
