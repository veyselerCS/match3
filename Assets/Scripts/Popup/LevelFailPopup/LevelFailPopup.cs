using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelFailPopup : BasePopup<LevelFailPopup.Data>
{
    [SerializeField] private Button RetryButton;
    [SerializeField] private Button ExitButton;

    private LoadingManager _loadingManager;
    private PopupManager _popupManager;
    private BoardManager _boardManager;
    
    public override void Init()
    {
        _popupManager = ManagerProvider.Instance.Get<PopupManager>();
        _boardManager = ManagerProvider.Instance.Get<BoardManager>();
    }
    
    public override void Show()
    {
        RetryButton.onClick.AddListener(() =>
        {
            _boardManager.ResetBoard();
            _popupManager.Hide("LevelFailPopup");
        });
        
        ExitButton.onClick.AddListener(() =>
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
            Name = nameof(LevelFailPopup);
        }
    }
}