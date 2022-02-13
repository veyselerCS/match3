using System;
using UnityEngine;

public abstract class BasePopup : MonoBehaviour, IDisposable
{
    public abstract BasePopupData GetPopupData();
    public void Dispose()
    {
    }
}

public class BasePopup<TPopupData> : BasePopup where TPopupData : BasePopupData
{
    public TPopupData PopupData;

    public override BasePopupData GetPopupData()
    {
        return PopupData;
    }
}