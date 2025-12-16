using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ThrowPosition[] _throwPositions;
    [SerializeField] private bool _gameStarted = true;

    public bool GameStarted => _gameStarted;

    public void AssignThrowerToRandomPos(Thrower thrower)
    {
        bool assigned = false;
        while (!assigned)
        {
            int selectedPos = Random.Range(0, _throwPositions.Length); 
            assigned = thrower.TryAssignThrowerToPosition(_throwPositions[selectedPos]);
        }
    }
}
