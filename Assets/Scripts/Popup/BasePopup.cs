using System;
using UnityEngine;

public abstract class BasePopup : MonoBehaviour, IDisposable
{
    public abstract BasePopupData GetPopupData();
    public void Dispose()
    {
    }
}

public abstract class BasePopup<TPopupData> : BasePopup where TPopupData : BasePopupData
{
    public TPopupData PopupData;

    public abstract void Show();
    public abstract void Init();
    
    public override BasePopupData GetPopupData()
    {
        return PopupData;
    }
}