using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LoadingManager : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [SerializeField] private Image LoadingImage;
    
    public static LoadingManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowLoadingImage()
    {
        LoadingImage.DOFade(1, 0);
        LoadingImage.gameObject.SetActive(true);
    }    
    
    public void HideLoadingImage()
    {
        LoadingImage.DOFade(0, 0.5f).OnComplete(() =>
        {
            LoadingImage.gameObject.SetActive(false);
            _signalBus.Fire<HideLoadFinishSignal>();
        });
    }
}