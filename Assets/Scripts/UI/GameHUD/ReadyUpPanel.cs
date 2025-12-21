using TMPro;
using UnityEngine;

public class ReadyUpPanel : MonoBehaviour
{
    [SerializeField] private AudioClip _endedTimerClip;
    [SerializeField] private AudioClip _normalTimerClip;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private string _animName = "TimerAnim";
    [SerializeField] private Animator _anim;
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
        _anim.Play(_animName, 0, 0);
        _timerText.text = (e.Value + 1).ToString();
        _audio.clip = e.Category == TimerEventCategory.Ended ? _endedTimerClip : _normalTimerClip;
        _audio.Play();
        if (e.Category == TimerEventCategory.Started) 
        { 
            _root.SetActive(true); 
        }
        else if(e.Category == TimerEventCategory.Ended) 
        { 
            _root.SetActive(false); 
        }
        
    }

}
