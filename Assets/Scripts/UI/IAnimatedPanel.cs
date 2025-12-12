using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IAnimatedPanel
{
    public void SetOpenImmediately();
    public void SetClosedImmediately();
    public UniTask Open();
    public UniTask Close();

}
