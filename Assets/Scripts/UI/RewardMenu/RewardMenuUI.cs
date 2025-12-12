using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIBasePanelAnimations))]
public class RewardMenuUI : MonoBehaviour, IMenuUI
{
    [SerializeField] private MenuID menuId;
    [SerializeField] private UIBasePanelAnimations _panelAnimations;
    [SerializeField] private Button _mainMenuBt;
    [SerializeField] private Button _playAgainBt;
    [SerializeField] private Button _quitBt;
    public IAnimatedPanel PanelAnimations => _panelAnimations;
    public MenuID MenuID => menuId;

    public void Setup(BaseMenuData data)
    {
        if (data is RewardMenuUIData rewardMenuData)
        {
            _mainMenuBt.onClick.RemoveAllListeners();
            _mainMenuBt.onClick.AddListener(() => rewardMenuData.BackToMainMenu().Forget());
            _playAgainBt.onClick.RemoveAllListeners();
            _playAgainBt.onClick.AddListener(() => rewardMenuData.RestartGame().Forget());
            _quitBt.onClick.RemoveAllListeners();
            _quitBt.onClick.AddListener(() => rewardMenuData.QuitGame());
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

}
