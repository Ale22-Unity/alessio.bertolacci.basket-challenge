using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRewardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private GameObject _winnerObject;
    [SerializeField] private RectTransform _rectTransform;
    [field: SerializeField] public bool IsOwner { get; private set; } = false;
    public RectTransform RectTransform => _rectTransform;

    public void SetWinner(PlayerResult[] results)
    {
        foreach (PlayerResult result in results)
        {
            if(result.IsOwner == IsOwner)
            {
                _winnerObject.SetActive(result.Winner);
                _scoreText.text = result.Score.ToString();
                return;
            }
        }
    }

}
