using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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
    [Space]
    [SerializeField] private ContextUIParticleDesign _moneyAnimDesign;
    [SerializeField] private Sprite _moneyIcon;
    [SerializeField] private int _amount;
    [Space]
    [SerializeField] private PlayerRewardUI _ownerElement;
    [SerializeField] private PlayerRewardUI _remoteElement;
    [Space]
    [SerializeField] private RectTransform _moneyPanel;
    [SerializeField] private UIBasePanelAnimations _moneyPanelAnimations;
    [SerializeField] private UIBasePanelAnimations _buttonsAnimations;
    private PlayerRewardUI _winner;
    public IAnimatedPanel PanelAnimations => _panelAnimations;
    public MenuID MenuID => menuId;

    public void Setup(BaseMenuData data)
    {
        _moneyPanelAnimations.SetOpenImmediately();
        _buttonsAnimations.SetClosedImmediately();
        if (data is RewardMenuUIData rewardMenuData)
        {
            _mainMenuBt.onClick.RemoveAllListeners();
            _mainMenuBt.onClick.AddListener(() => rewardMenuData.BackToMainMenu().Forget());
            _playAgainBt.onClick.RemoveAllListeners();
            _playAgainBt.onClick.AddListener(() => rewardMenuData.RestartGame().Forget());
            _quitBt.onClick.RemoveAllListeners();
            _quitBt.onClick.AddListener(() => rewardMenuData.QuitGame());
            SetupPlayerInfo(rewardMenuData.Results).Forget();
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public async UniTask PlayRewardAnimation()
    {
        if(GameClient.Client != null)
        {
            ContextUIParticleData data = new ContextUIParticleData(
                new ParticlesContextPosition((Vector2)_moneyPanel.position),
                new ParticlesContextPosition((Vector2)_winner.RectTransform.position),
                _moneyAnimDesign,
                _moneyIcon,
                _amount
                );
            UniTaskCompletionSource<bool> anim = new UniTaskCompletionSource<bool>();
            PlayContextParticlesEvent particleEvent = new PlayContextParticlesEvent(new List<ContextUIParticleData> { data }, anim);
            GameClient.Client.EventBus.Fire<PlayContextParticlesEvent>(particleEvent);
            await anim.Task;
        }
    }

    private async UniTask SetupPlayerInfo(PlayerResult[] results)
    {
        _ownerElement.SetWinner(results);
        _remoteElement.SetWinner(results);
        _winner = results[0].IsOwner ? _ownerElement : _remoteElement;
        await PlayRewardAnimation();
        await _moneyPanelAnimations.Close();
        await _buttonsAnimations.Open();
    }

}
