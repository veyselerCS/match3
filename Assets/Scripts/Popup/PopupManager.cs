using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class PopupManager : Manager
{
    [SerializeField] private Canvas PopupCanvas;
    private AddressableManager _addressableManager;

    private List<BasePopup> _activePopups = new List<BasePopup>();

    public override void Init()
    {
        _addressableManager = _managerProvider.Get<AddressableManager>();

        _dependencies.Add(_addressableManager);
    }

    public override void Begin()
    {
        SetReady();
    }

    public async void Show<TPopupData>(TPopupData data) where TPopupData : BasePopupData
    {
        if(IsOpen(data.Name)) return;
        
        var handle = _addressableManager.LoadAsset<GameObject>(data.Name);
        await handle.Task;
        var popupGO = handle.Result;
        BasePopup<TPopupData> popup = Instantiate(popupGO, PopupCanvas.transform).GetComponent<BasePopup<TPopupData>>();
        popup.PopupData = data;
        popup.Init();
        popup.Show();
        _activePopups.Add(popup);
    }

    public bool IsOpen(string name)
    {
        for (int i = 0; i < _activePopups.Count; i++)
        {
            var popup = _activePopups[i];
            if (popup.GetPopupData().Name == name)
            {
                return true;
            }
        }

        return false;
    }
    
    public void Hide(string name)
    {
        for (int i = 0; i < _activePopups.Count; i++)
        {
            var popup = _activePopups[i];
            if (popup.GetPopupData().Name == name)
            {
                _activePopups.RemoveAt(i);
                Destroy(popup.gameObject);
            }
        }
    }
}