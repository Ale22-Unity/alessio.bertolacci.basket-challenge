
public class PlayerAssignedAtPositionEvent
{
    public readonly ThrowPosition Position;
    public readonly float ErrorMarginPerc;

    public PlayerAssignedAtPositionEvent(ThrowPosition pos, float errorMarginPerc)
    {
        Position = pos;
        ErrorMarginPerc = errorMarginPerc;
    }
}
