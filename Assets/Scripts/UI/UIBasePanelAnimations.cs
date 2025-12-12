using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIBasePanelAnimations : MonoBehaviour, IAnimatedPanel
{
    [SerializeField] private Animator _panelAnimator;
    [SerializeField] private string _openAnimBool = "open";
    [SerializeField] private string _openAnimStateName = "Open";
    [SerializeField] private string _openingAnimStateName = "Opening";
    [SerializeField] private string _closeAnimStateName = "Close";
    [SerializeField] private string _closingAnimStateName = "Closing";

    public void SetOpenImmediately()
    {
        _panelAnimator.Play(_openAnimStateName);
    }

    public void SetClosedImmediately()
    {
        _panelAnimator.Play(_closeAnimStateName);
    }

    public async UniTask Open()
    {
        if (_panelAnimator.GetCurrentAnimatorStateInfo(0).IsName(_openAnimStateName))
        {
            return;
        }
        _panelAnimator.SetBool(_openAnimBool, true);
        await this.WaitForAnimationCompletion(_openingAnimStateName, _panelAnimator);
    }

    public async UniTask Close()
    {
        if (_panelAnimator.GetCurrentAnimatorStateInfo(0).IsName(_closeAnimStateName))
        {
            return;
        }
        _panelAnimator.SetBool(_openAnimBool, false);
        await this.WaitForAnimationCompletion(_closingAnimStateName, _panelAnimator);
    }
}
