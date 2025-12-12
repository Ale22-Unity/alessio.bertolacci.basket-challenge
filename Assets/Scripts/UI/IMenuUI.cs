using UnityEngine;

public interface IMenuUI
{
    public void Destroy();
    public IAnimatedPanel PanelAnimations { get; }
    public MenuID MenuID { get; }
    public void Setup(BaseMenuData data);
}
