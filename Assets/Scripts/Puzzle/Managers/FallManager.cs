using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class FallManager : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    
    public static FallManager Instance;

    private BoardManager _boardManager;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _boardManager = BoardManager.Instance;
        _signalBus.Subscribe<BoardElementPopSignal>(OnBoardElementPop);
        _signalBus.Subscribe<BoardElementSpawnedSignal>(OnBoardElementSpawned);
    }

    private void OnBoardElementPop(BoardElementPopSignal data)
    {
        CheckSquaresForFall();
    }
       
    private void OnBoardElementSpawned(BoardElementSpawnedSignal data)
    {
        CheckSquaresForFall();
    }
    
    [Button("Check fall")]
    private void CheckSquaresForFall()
    {
        while (true)
        {
            var board = _boardManager.Board;

            bool fall = false;
            for (int i = 0; i < 9; i++)
            {
                for (int k = 0; k < 9; k++)
                {
                    var square = board[i][k];
                    var boardElement = square.BoardElement;
                    if (boardElement != null && AvailableForFall(i - 1, k))
                    {
                        var squareToFall = board[i - 1][k];
                        boardElement.transform.DOMove(squareToFall.CenterPosition, 0.5f).SetEase(Ease.InOutQuad).OnComplete(
                            () =>
                            {
                                _signalBus.Fire<BoardElementFallSignal>();
                                squareToFall.BoardElement = boardElement;
                                square.BoardElement = null;
                            });

                        fall = true;
                    }
                }
            }

            if (fall)
            {
                continue;
            }

            break;
        }
    }

    private bool AvailableForFall(int x, int y)
    {
        var board = _boardManager.Board;

        if (x < 0) return false;

        return board[x][y].BoardElement == null;
    }
}