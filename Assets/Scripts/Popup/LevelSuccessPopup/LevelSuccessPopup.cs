using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSuccessPopup : BasePopup<LevelSuccessPopup.Data>
{
    [SerializeField] private Button FullScreenButton;

    private DataManager _dataManager;
    private PopupManager _popupManager;
    private PuzzleLoadManager _puzzleLoadManager;
    
    public override void Init()
    {
        _dataManager = ManagerProvider.Instance.Get<DataManager>();
        _popupManager = ManagerProvider.Instance.Get<PopupManager>();
        _puzzleLoadManager = ManagerProvider.Instance.Get<PuzzleLoadManager>();
    }
    
    public override void Show()
    {
        if (_dataManager.UserData.MaxLevel == _puzzleLoadManager.LevelToLoad.LevelNo)
        {
            _dataManager.UserData.MaxLevel++;
            _dataManager.UserData.MaxLevelShown = 0;
            _dataManager.UserData.Save();
        }
        
        FullScreenButton.onClick.AddListener(() =>
        {
            _popupManager.Hide(PopupData.Name);
            LoadingManager.Instance.ShowLoadingImage();
            SceneManager.UnloadSceneAsync("PuzzleScene").completed += (a) =>
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