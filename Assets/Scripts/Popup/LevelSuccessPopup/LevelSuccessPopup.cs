using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSuccessPopup : BasePopup<LevelSuccessPopup.Data>
{
    [SerializeField] private Button FullScreenButton;

    private DataManager _dataManager;
    public override void Init()
    {
        _dataManager = ManagerProvider.Instance.Get<DataManager>();
    }
    
    public override void Show()
    {
        _dataManager.UserData.MaxLevel++;
        _dataManager.UserData.MaxLevelShown = 0;
        _dataManager.UserData.SetDataDirty();
        
        FullScreenButton.onClick.AddListener(() =>
        {
            
            LoadingManager.Instance.ShowLoadingImage();
            SceneManager.LoadSceneAsync("MainScene").completed += (a) =>
            {
                DOVirtual.DelayedCall(2f, () =>
                {
                    LoadingManager.Instance.HideLoadingImage();
                });
            };
        });
    }
    public class Data : BasePopupData
    {
        public Data()
        {
            Name = nameof(LevelSuccessPopup);
        }
    }
}