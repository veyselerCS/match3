using DG.Tweening;
using UnityEngine.SceneManagement;
using Zenject;

public class PuzzleLoadManager : Manager
{
    [Inject] private SignalBus _signalBus;
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
        _signalBus.Subscribe<HideLoadFinishSignal>(OnHideFinish);
        SetReady();
    }

    public void LoadLevel(int levelId)
    {
        LevelToLoad = _dataManager.LevelData[levelId - 1];
        
        _dataManager.UserData.MaxLevelShown = 1;
        _dataManager.UserData.Save();
        
        LoadingManager.Instance.ShowLoadingImage();

        SceneManager.LoadSceneAsync("PuzzleScene", LoadSceneMode.Additive).completed += (asyncOperation) =>
        {
            DOVirtual.DelayedCall(2f, () => { LoadingManager.Instance.HideLoadingImage(); });
        };
    }

    private void OnHideFinish()
    {
        if (SceneManager.GetSceneAt(SceneManager.sceneCount - 1)  == gameObject.scene)
        {
            _popupManager.Show(new LevelEnterPopup.Data(_dataManager.LevelData));
        }
    }
}