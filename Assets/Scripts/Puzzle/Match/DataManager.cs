using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DataManager : Manager
{
    [Inject] private SignalBus _signalBus;
    [SerializeField] private int MaxLevel = 5;

    private AddressableManager _addressableManager;

    public List<LevelData> LevelData = new List<LevelData>();

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
            LevelRawData levelRawData = loadHandle.Result;
            LevelData.Add(HanleRawData(levelRawData));
        }

        SetReady();
        _signalBus.Fire<DataReadySignal>();
    }

    private LevelData HanleRawData(LevelRawData rawData)
    {
        var levelData = new LevelData(rawData.LevelNo, rawData.GridWidth, rawData.GridHeight, rawData.MoveCount);
        var boardElements = rawData.Squares.Split(',');
        var counter = 0;
        for (int i = 0; i < rawData.GridHeight; i++)
        {
            for (int k = 0; k < rawData.GridWidth; k++)
            {
                HandleBoardElement(levelData, Int32.Parse(boardElements[counter]) % 100, new Vector2Int(i, k));
                counter++;
            }
        }

        return levelData;
    }

    private void HandleBoardElement(LevelData levelData, int boardElementId, Vector2Int pos)
    {
        if (boardElementId < 4)
        {
            levelData.Drops[(DropType)boardElementId].Add(pos);
        }
        else if (boardElementId < 8)
        {
            levelData.Obstacles[(ObstacleType)(boardElementId - Enum.GetValues(typeof(DropType)).Length)].Add(pos);
        }
        else
        {
            Debug.LogWarning("id is not supported");
        }
    }
}