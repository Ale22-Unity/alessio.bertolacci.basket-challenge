using UnityEngine;

public class ParticlesContextPosition
{
    private readonly ParticlesContextSpace _space;
    private readonly Vector2 _uiPos;
    private readonly Vector3 _worldPos;

    public ParticlesContextPosition(Vector2 uiPos)
    {
        _space = ParticlesContextSpace.UISpace;
        _uiPos = uiPos;
        _worldPos = default;
    }

    public ParticlesContextPosition(Vector3 worldPos)
    {
        _space = ParticlesContextSpace.World;
        _uiPos = default;
        _worldPos = worldPos;
    }

    public Vector2 ToScreenPos(Camera camera, Canvas canvas, RectTransform particlesParent)
    {
        switch (_space)
        {
            case ParticlesContextSpace.UISpace:
                return particlesParent.InverseTransformPoint(_uiPos);

            case ParticlesContextSpace.World:
                Vector3 screenPos = camera.WorldToScreenPoint(_worldPos);
                return particlesParent.InverseTransformPoint(screenPos);

            default:
                return Vector2.zero;
        }
    }
}

public enum ParticlesContextSpace
{
    UISpace,
    World
}
