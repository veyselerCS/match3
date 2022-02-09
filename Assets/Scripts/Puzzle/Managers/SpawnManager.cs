using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class SpawnManager : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    private BoardManager _boardManager;
    private DropFactory _dropFactory;
    private SceneComponentService _sceneComponentService;

    private void Start()
    {
        _boardManager = BoardManager.Instance;
        _dropFactory = DropFactory.Instance;
        _sceneComponentService = SceneComponentService.Instance;
        
        _signalBus.Subscribe<MatchEndSignal>(OnMatchEnd);
    }

    private int frame = 0;
    private bool check;
    private void OnMatchEnd()
    {
        var board = _boardManager.Board;
        frame = Time.frameCount;
        check = true;
        for (int i = 0; i < 9; i++)
        {
            var neededCount = GetNeededDropCountForColumn(i);
            for (int k = 0; k < neededCount; k++)
            {
                var square = board[_boardManager.BoardHeight + k][i];
                var dropPrefab = _dropFactory.GetDropByDropType((DropType)UnityEngine.Random.Range(0, 4));
                Drop drop = Instantiate(dropPrefab, _sceneComponentService.BoardElementParent.transform);
                drop.transform.position = square.CenterPosition;
                square.BoardElement = drop;
            }
        }
        _signalBus.Fire<SpawnEndSignal>();

    }



    private int GetNeededDropCountForColumn(int col)
    {
        var board = _boardManager.Board;
        int count = 0;
        for (int i = 0; i < 9; i++)
        {
            var boardElement = board[i][col].BoardElement;
            if (boardElement == null) count++;
        }

        return count;
    }
}