using UnityEngine;

public class DataManager : Manager
{
    [SerializeField] private int MaxLevel = 5;

    private AddressableManager _addressableManager;
    public override void Init()
    {
        _addressableManager = _managerProvider.Get<AddressableManager>();
        
        _dependencies.Add(_addressableManager);
    }

    public override async void Begin()
    {
        for (int i = 1; i <= MaxLevel; i++)
        {
            var loadHandle = _addressableManager.LoadAsset<LevelRawData>(i.ToString());
            await loadHandle.Task;
            LevelRawData levelText = loadHandle.Result;
            Debug.LogWarning(levelText.GridWidth);
        }
        SetReady();
    }
}