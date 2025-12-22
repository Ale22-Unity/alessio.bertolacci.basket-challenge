using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClient : MonoBehaviour
{
    public static GameClient Client;

    [SerializeField] private string GameSceneName = "GameScene";
    private bool _gameSceneLoaded = false;
    [SerializeField] private string MainMenuSceneName = "MainMenu";
    private bool _mainMenuSceneLoaded = false;
    [SerializeField] private string RewardSceneName = "RewardScene";
    private bool _rewardSceneLoaded = false;
    [SerializeField] private Transform uiRoot;
    [SerializeField] private MainMenuUI mainMenuPrefab;
    [SerializeField] private GameHUD gameHUDPrefab;
    [SerializeField] private RewardMenuUI rewardMenuPrefab;

    private List<IMenuUI> activeMenuItems = new List<IMenuUI>();

    public EventBus EventBus {  get; private set; }
    [field:SerializeField] public GameCamera GameCamera { get; private set; }

    private void Awake()
    {
        if(Client != null)
        {
            Destroy(gameObject);
            if(GameCamera != null) { Destroy(GameCamera.gameObject); }
            return;
        }
        Client = this;
        EventBus = new EventBus();
        EventBus.Subscribe<GameEndedEvent>(On);
        CreateMainMenu().Forget();
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<GameEndedEvent>(On);
    }

    private void QuitGame()
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }

    private async UniTask StartGame()
    {
        if (_rewardSceneLoaded)
        {
            await SceneManager.UnloadSceneAsync(RewardSceneName);
            await CloseMenu(MenuID.Reward);
            _rewardSceneLoaded = false;
        }
        if (_mainMenuSceneLoaded)
        {
            await SceneManager.UnloadSceneAsync(MainMenuSceneName);
            await CloseMenu(MenuID.MainMenu);
            _mainMenuSceneLoaded = false;
        }
        GameHUD gameHUDInstance = Instantiate(gameHUDPrefab, uiRoot);
        gameHUDInstance.Setup(new GameHUDData());
        activeMenuItems.Add(gameHUDInstance);
        await gameHUDInstance.PanelAnimations.Open();
        await SceneManager.LoadSceneAsync(GameSceneName, LoadSceneMode.Additive);
        _gameSceneLoaded = true;
    }
    private void On(GameEndedEvent e)
    {
        OnGameEnded(e).Forget();
    }

    private async UniTask OnGameEnded(GameEndedEvent e)
    {
        if (_gameSceneLoaded)
        {
            await CloseMenu(MenuID.HUD);
            await SceneManager.UnloadSceneAsync(GameSceneName);
            _gameSceneLoaded = false;
        }
        await SceneManager.LoadSceneAsync(RewardSceneName, LoadSceneMode.Additive);
        EventBus.Fire<SetupRewardScreenCharactersEvent>(new SetupRewardScreenCharactersEvent(e.MatchResults));
        RewardMenuUI rewardMenuInstance = Instantiate(rewardMenuPrefab, uiRoot);
        rewardMenuInstance.Setup(new RewardMenuUIData(StartGame, BackToMainMenu, QuitGame, e.MatchResults));
        activeMenuItems.Add(rewardMenuInstance);
        await rewardMenuInstance.PanelAnimations.Open();
        _rewardSceneLoaded = true;
    }

    private async UniTask CreateMainMenu()
    {
        MainMenuUI mainMenuInstance = Instantiate(mainMenuPrefab, uiRoot);
        mainMenuInstance.Setup(new MainMenuUIData(StartGame, QuitGame));
        activeMenuItems.Add(mainMenuInstance);
        await SceneManager.LoadSceneAsync(MainMenuSceneName, LoadSceneMode.Additive);
        await mainMenuInstance.PanelAnimations.Open();
        _mainMenuSceneLoaded = true;
    }

    private async UniTask BackToMainMenu()
    {
        if (_gameSceneLoaded)
        {
            await CloseMenu(MenuID.HUD);
            await SceneManager.UnloadSceneAsync(GameSceneName);
            _gameSceneLoaded = false;
        }
        if (_rewardSceneLoaded)
        {
            await CloseMenu(MenuID.Reward);
            await SceneManager.UnloadSceneAsync(RewardSceneName);
            _rewardSceneLoaded = false;
        }
        await CreateMainMenu();
    }

    private async UniTask CloseMenu(MenuID ID)
    {
        List<IMenuUI> tempList = new List<IMenuUI>(activeMenuItems);
        foreach(IMenuUI menu in tempList)
        {
            if(menu.MenuID == ID)
            {
                await menu.PanelAnimations.Close();
                if (activeMenuItems.Contains(menu))
                {
                    activeMenuItems.Remove(menu);
                    menu.Destroy();
                }
                return;
            }
        }
        return;
    }
}
