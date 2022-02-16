using DG.Tweening;
using UnityEngine.SceneManagement;

public class PuzzleLoadManager : Manager
{
    private DataManager _dataManager;
    private PopupManager _popupManager;
    public LevelData LevelToLoad;

    public override void Init()
    {
        _dataManager = _managerProvider.Get<DataManager>();
        _popupManager = _managerProvider.Get<PopupManager>();
        
        _dependencies.Add(_dataManager);
        _dependencies.Add(_popupManager);
    }

    public override void Begin()
    {
        _popupManager.Show(new LevelEnterPopup.Data(_dataManager.LevelData));
        SetReady();
    }

    public void LoadLevel(int levelId)
    {
        LevelToLoad = _dataManager.LevelData[levelId - 1];
        
        LoadingManager.Instance.ShowLoadingImage();
        SceneManager.LoadSceneAsync("PuzzleScene", LoadSceneMode.Additive).completed += (a) =>
        {
            DOVirtual.DelayedCall(2f, () =>
            {
                _popupManager.Hide("LevelEnterPopup");
                LoadingManager.Instance.HideLoadingImage();
            });
        };
    }
}