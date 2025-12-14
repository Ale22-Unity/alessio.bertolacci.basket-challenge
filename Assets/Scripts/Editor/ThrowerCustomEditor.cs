#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Thrower))]
public class ThrowerCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Thrower thrower = (Thrower)target;
        if (GUILayout.Button("DrawSimulatedThrow"))
        {
            thrower.SimulateThrow();
        }
    }
}
#endif