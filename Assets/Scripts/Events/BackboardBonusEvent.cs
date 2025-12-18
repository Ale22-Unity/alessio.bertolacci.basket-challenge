public class BackboardBonusEvent
{
    public readonly bool Started = false;
    public readonly int ScoreBonus = 0;

    public BackboardBonusEvent(bool started, int scoreBonus)
    {
        Started = started;
        ScoreBonus = scoreBonus;
    }
}
