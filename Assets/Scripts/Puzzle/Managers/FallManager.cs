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
                var firstAvailableSquareForFall = GetFirstAvailableSquareInColumn(i);
                //no empty square in this column so no fall
                if(firstAvailableSquareForFall == null) continue;
                
                var firstSquareToFall = GetFirstNonEmptySquareInColumn(firstAvailableSquareForFall.Coordinates.x, i);
                //all squares are empty above
                if(firstSquareToFall == null) continue;

                int fallCount = 0;
                for (int k = firstSquareToFall.Coordinates.x; k < 9; k++)
                {
                    var square = board[k][i];
                    var boardElement = square.BoardElement;
                    if (boardElement != null)
                    {
                        var squareToFall = board[firstAvailableSquareForFall.Coordinates.x + fallCount][i];
                        squareToFall.BoardElement = boardElement;
                        square.BoardElement = null;
                        boardElement.transform.DOKill();
                        
                        boardElement.transform.DOMove(squareToFall.CenterPosition, 0.15f + 0.1f * fallCount).SetEase(Ease.InOutQuad).OnComplete(
                            () =>
                            {
                                _signalBus.Fire<BoardElementFallSignal>();
                            }).SetLink(boardElement.gameObject);

                        fall = true;
                        fallCount++;
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

    private Square GetFirstAvailableSquareInColumn(int col)
    {
        for (int i = 0; i < 9; i++)
        {
            if (AvailableForFall(i, col))
            {
                return _boardManager.Board[i][col];
            }
        }

        return null;
    }

    private Square GetFirstNonEmptySquareInColumn(int start, int col)
    {
        for (int i = start; i < 9; i++)
        {
            if (_boardManager.Board[i][col].BoardElement != null)
            {
                return _boardManager.Board[i][col];
            }
        }

        return null;
    }
    
    private bool AvailableForFall(int x, int y)
    {
        var board = _boardManager.Board;
        if (x < 0) return false;

        return board[x][y].BoardElement == null;
    }
}