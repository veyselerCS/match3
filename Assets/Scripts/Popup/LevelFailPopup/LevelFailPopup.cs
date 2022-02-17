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
    private InputManager _inputManager;
    private TargetManager _targetManager;
    
    public override void Init()
    {
        _popupManager = ManagerProvider.Instance.Get<PopupManager>();
        _boardManager = ManagerProvider.Instance.Get<BoardManager>();
        _inputManager = ManagerProvider.Instance.Get<InputManager>();
        _targetManager = ManagerProvider.Instance.Get<TargetManager>();
    }
    
    public override void Show()
    {
        RetryButton.onClick.AddListener(() =>
        {
            _inputManager.enabled = true;
            _boardManager.ResetBoard();
            _targetManager.Refresh();
            _popupManager.Hide("LevelFailPopup");
        });
        
        ExitButton.onClick.AddListener(() =>
        {
            _popupManager.Hide("LevelFailPopup");
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
            Name = nameof(LevelFailPopup);
        }
    }
}