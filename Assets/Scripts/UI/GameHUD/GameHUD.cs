using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(UIBasePanelAnimations))]
public class GameHUD : MonoBehaviour, IMenuUI
{
    [SerializeField] private MenuID menuId;
    [SerializeField] private UIBasePanelAnimations _panelAnimations;
    [SerializeField] private Button _fireGameEndedEvent;
    [SerializeField] private Button _throwBallBt;
    [SerializeField] private Button _resetBallBt;
    public IAnimatedPanel PanelAnimations => _panelAnimations;
    public MenuID MenuID => menuId;

    public void Setup(BaseMenuData data)
    {
        if (data is GameHUDData HUDData)
        {
            _fireGameEndedEvent.onClick.RemoveAllListeners();
            _fireGameEndedEvent.onClick.AddListener(FireGameEndedEvent);
            _throwBallBt.onClick.RemoveAllListeners();
            _throwBallBt.onClick.AddListener(TestThrowBall);
            _resetBallBt.onClick.RemoveAllListeners();
            _resetBallBt.onClick.AddListener(TestResetBall);
        }
    }

    private void FireGameEndedEvent()
    {
        if(GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<GameEndedEvent>(new GameEndedEvent());
        }
    }

    private void TestThrowBall()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<ThrowBallTestEvent>(new ThrowBallTestEvent());
        }
    }

    private void TestResetBall()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Fire<ResetBallTestEvent>(new ResetBallTestEvent());
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
