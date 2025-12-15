using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Thrower _thrower;
    [SerializeField] private ThrowPosition[] _throwPositions;

    private void Start()
    {
        AssignThrowerToRandomPos();
    }

    public void AssignThrowerToRandomPos()
    {
        bool assigned = false;
        while (!assigned)
        {
            int selectedPos = Random.Range(0, _throwPositions.Length); 
            assigned = _thrower.TryAssignThrowerToPosition(_throwPositions[selectedPos]);
        }
    }
}
