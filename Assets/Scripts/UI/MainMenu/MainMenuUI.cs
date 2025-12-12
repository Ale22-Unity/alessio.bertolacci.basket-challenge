using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(UIBasePanelAnimations))]
public class MainMenuUI : MonoBehaviour, IMenuUI
{
    [SerializeField] private MenuID menuId;
    [SerializeField] private UIBasePanelAnimations _panelAnimations;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _quitGameButton;
    public IAnimatedPanel PanelAnimations => _panelAnimations;
    public MenuID MenuID => menuId;

    public void Setup(BaseMenuData data)
    {
        if(data is MainMenuUIData mainMenuData)
        {
            _startGameButton.onClick.RemoveAllListeners();
            _startGameButton.onClick.AddListener(() => mainMenuData.StartGame().Forget());
            _quitGameButton.onClick.RemoveAllListeners();
            _quitGameButton.onClick.AddListener(() => mainMenuData.QuitGame());
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
