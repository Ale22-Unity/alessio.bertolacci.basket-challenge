using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimations : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private string _startingAnimBool = "";
    private void Awake()
    {
        if (!string.IsNullOrEmpty(_startingAnimBool))
        {
            _anim.SetBool(_startingAnimBool, true );
        }
    }
}
