using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameManager manager = (GameManager)target;
        if (GUILayout.Button("AssignThrowerToRandomPos"))
        {
            manager.AssignPlayerToPosition();
        }
    }
}
