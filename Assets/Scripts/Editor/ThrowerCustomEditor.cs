#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Thrower))]
public class ThrowerCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Thrower thrower = (Thrower)target;
        if (GUILayout.Button("StartDynamicSimulation"))
        {
            thrower.StartDynamicSimulation().Forget();
        }
        if (GUILayout.Button("StopDynamicSimulation"))
        {
            thrower.StopDynamicSimulation();
        }
        if (GUILayout.Button("PlayOutThrow"))
        {
            thrower.PlayOutThrow().Forget();
        }
    }
}
#endif