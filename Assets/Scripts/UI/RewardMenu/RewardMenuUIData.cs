using Cysharp.Threading.Tasks;
using System;

public class RewardMenuUIData : BaseMenuData
{
    public readonly PlayerResult[] Results;
    public readonly Func<UniTask> RestartGame;
    public readonly Func<UniTask> BackToMainMenu;
    public readonly Action QuitGame;

    public RewardMenuUIData(Func<UniTask> restartGame, Func<UniTask> backToMainMenu, Action quitGame, PlayerResult[] results)
    {
        RestartGame = restartGame;
        BackToMainMenu = backToMainMenu;
        QuitGame = quitGame;
        Results = results;
    }
}
