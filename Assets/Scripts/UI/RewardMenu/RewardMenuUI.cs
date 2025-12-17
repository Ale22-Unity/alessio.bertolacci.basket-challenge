using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private string _scoredString = "you scored {0} points";
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
            PlayerResult ownerResult = GetOwnerResult(rewardMenuData.Results);
            _scoreText.text = string.Format(_scoredString, ownerResult.Score);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private PlayerResult GetOwnerResult(PlayerResult[] results)
    {
        foreach (PlayerResult result in results)
        {
            if (result.IsOwner) { return result; }
        }
        return results[0];
    }
}
