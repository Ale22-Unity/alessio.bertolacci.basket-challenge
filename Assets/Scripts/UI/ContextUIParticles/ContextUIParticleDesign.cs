using UnityEngine;

[CreateAssetMenu(fileName = "ContextUIParticleDesign_New", menuName = "Scriptable Objects/ContextUIParticleDesign")]
public class ContextUIParticleDesign : ScriptableObject
{
    [field:SerializeField] public AnimationCurve ParticleAnimationX {  get; private set; }
    [field: SerializeField] public AnimationCurve ParticleAnimationY { get; private set; }
    [field: SerializeField] public AnimationCurve SpeedCurve { get; private set; }
    [field:SerializeField] public AnimationCurve SizeAnimation { get; private set; }
    [field:SerializeField] public float AnimationSpeed { get; private set; }
    [field:SerializeField] public float ParticleDelay { get; private set; }
    [field: SerializeField] public Vector2 SpawnDelta { get; private set; }
}
