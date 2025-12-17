using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(UIBasePanelAnimations))]
public class GameHUD : MonoBehaviour, IMenuUI
{
    [SerializeField] private MenuID menuId;
    [SerializeField] private UIBasePanelAnimations _panelAnimations;

    public IAnimatedPanel PanelAnimations => _panelAnimations;
    public MenuID MenuID => menuId;

    public void Setup(BaseMenuData data)
    {
        if (data is GameHUDData HUDData)
        {
        }
    }


    public void Destroy()
    {
        Destroy(gameObject);
    }
}
