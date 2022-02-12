using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class SpawnManager : Manager
{
    [Inject] private SignalBus _signalBus;
    private BoardManager _boardManager;
    private DropFactory _dropFactory;
    private SceneComponentService _sceneComponentService;
    
    public override void Init()
    {
        _boardManager = _managerProvider.Get<BoardManager>();
        _dropFactory = _managerProvider.Get<DropFactory>();
        _sceneComponentService = _managerProvider.Get<SceneComponentService>();
        
        _dependencies.Add(_boardManager);
        _dependencies.Add(_dropFactory);
        _dependencies.Add(_sceneComponentService);
    }

    public override void Begin()
    {
        _signalBus.Subscribe<MatchEndSignal>(OnMatchEnd);
        SetReady();
    }

    private bool spawnQueued;
    private bool spawnLock;
    private void OnMatchEnd()
    {
        if (spawnLock)
        {
            spawnQueued = true;
            return;
        }
        
        spawnLock = true;
        var board = _boardManager.Board;
        for (int i = 0; i < 9; i++)
        {
            var neededCount = GetNeededDropCountForColumn(i);
            var createdCount = GetCreatedDropCountForColumn(i);
            for (int k = createdCount; k < neededCount; k++)
            {
                var square = board[_boardManager.BoardHeight + k][i];
                Drop drop = _dropFactory.GetDropByDropType((DropType)UnityEngine.Random.Range(0, 4));
                drop.transform.position = square.CenterPosition;
                drop.gameObject.SetActive(true);
                square.BoardElement = drop;
            }
        }

        spawnLock = false;

        if (spawnQueued)
        {
            spawnQueued = false;
            //OnMatchEnd();
            return;
        }
        
        _signalBus.Fire<SpawnEndSignal>();
    }

    private int GetNeededDropCountForColumn(int col)
    {
        var board = _boardManager.Board;
        int count = 0;
        for (int i = _boardManager.BoardHeight - 1; i >= 0; i--)
        {
            var boardElement = board[i][col].BoardElement;
            if (boardElement != null && boardElement.IsStable) break;
            if (boardElement == null) count++;
        }

        return count;
    }  
    
    private int GetCreatedDropCountForColumn(int col)
    {
        var board = _boardManager.Board;
        int count = 0;
        for (int i = _boardManager.Board.Count - 1; i >= _boardManager.BoardHeight; i--)
        {
            var boardElement = board[i][col].BoardElement;
            if (boardElement != null) count++;
        }

        return count;
    }
}