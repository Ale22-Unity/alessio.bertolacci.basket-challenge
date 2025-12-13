using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClient : MonoBehaviour
{
    public static GameClient Client;

    [SerializeField] private string GameSceneName = "GameScene";
    [SerializeField] private Transform uiRoot;
    [SerializeField] private MainMenuUI mainMenuPrefab;
    [SerializeField] private GameHUD gameHUDPrefab;
    [SerializeField] private RewardMenuUI rewardMenuPrefab;

    private List<IMenuUI> activeMenuItems = new List<IMenuUI>();

    public EventBus EventBus {  get; private set; }
    [field:SerializeField] public Camera GameCamera { get; private set; }

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
        await CloseMenu(MenuID.Reward);
        await CloseMenu(MenuID.MainMenu);
        await SceneManager.LoadSceneAsync(GameSceneName, LoadSceneMode.Additive);
        GameHUD gameHUDInstance = Instantiate(gameHUDPrefab, uiRoot);
        gameHUDInstance.Setup(new GameHUDData());
        activeMenuItems.Add(gameHUDInstance);
        await gameHUDInstance.PanelAnimations.Open();
    }
    private void On(GameEndedEvent e)
    {
        OnGameEnded(e).Forget();
    }

    private async UniTask OnGameEnded(GameEndedEvent e)
    {
        await CloseMenu(MenuID.HUD);
        await SceneManager.UnloadSceneAsync(GameSceneName);
        RewardMenuUI rewardMenuInstance = Instantiate(rewardMenuPrefab, uiRoot);
        rewardMenuInstance.Setup(new RewardMenuUIData(StartGame, BackToMainMenu, QuitGame));
        activeMenuItems.Add(rewardMenuInstance);
        await rewardMenuInstance.PanelAnimations.Open();
    }

    private async UniTask CreateMainMenu()
    {
        MainMenuUI mainMenuInstance = Instantiate(mainMenuPrefab, uiRoot);
        mainMenuInstance.Setup(new MainMenuUIData(StartGame, QuitGame));
        activeMenuItems.Add(mainMenuInstance);
        await mainMenuInstance.PanelAnimations.Open();
    }

    private async UniTask BackToMainMenu()
    {
        await CloseMenu(MenuID.HUD);
        await CloseMenu(MenuID.Reward);
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
