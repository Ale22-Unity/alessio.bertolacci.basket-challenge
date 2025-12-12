using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MainMenuUIData : BaseMenuData
{
    public readonly Func<UniTask> StartGame;
    public readonly Action QuitGame;

    public MainMenuUIData(Func<UniTask> startGame, Action quitGame)
    {
        StartGame = startGame;
        QuitGame = quitGame;
    }
}
