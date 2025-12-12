using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(UIBasePanelAnimations))]
public class GameHUD : MonoBehaviour, IMenuUI
{
    [SerializeField] private MenuID menuId;
    [SerializeField] private UIBasePanelAnimations _panelAnimations;
    [SerializeField] private Button _fireGameEndedEvent;
    public IAnimatedPanel PanelAnimations => _panelAnimations;
    public MenuID MenuID => menuId;

    public void Setup(BaseMenuData data)
    {
        if (data is GameHUDData HUDData)
        {
            _fireGameEndedEvent.onClick.RemoveAllListeners();
            _fireGameEndedEvent.onClick.AddListener(FireGameEndedEvent);
        }
    }

    private void FireGameEndedEvent()
    {
        if(GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<GameEndedEvent>(new GameEndedEvent());
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
