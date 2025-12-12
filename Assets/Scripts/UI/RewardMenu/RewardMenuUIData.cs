using Cysharp.Threading.Tasks;
using System;

public class RewardMenuUIData : BaseMenuData
{
    public readonly Func<UniTask> RestartGame;
    public readonly Func<UniTask> BackToMainMenu;
    public readonly Action QuitGame;

    public RewardMenuUIData(Func<UniTask> restartGame, Func<UniTask> backToMainMenu, Action quitGame)
    {
        RestartGame = restartGame;
        BackToMainMenu = backToMainMenu;
        QuitGame = quitGame;
    }
}
