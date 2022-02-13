using UnityEngine.SceneManagement;

public class PuzzleLoadManager : Manager
{
    private DataManager _dataManager;
    public LevelData LevelToLoad;

    public override void Init()
    {
        _dataManager = _managerProvider.Get<DataManager>();
        
        _dependencies.Add(_dataManager);
    }

    public override void Begin()
    {
        SetReady();
        LoadLevel(0);
    }

    public void LoadLevel(int levelId)
    {
        LevelToLoad = _dataManager.LevelData[levelId];
        SceneManager.LoadScene("PuzzleScene", LoadSceneMode.Additive);
    }
}