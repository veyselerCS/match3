using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class FallManager : Manager
{
    [Inject] private SignalBus _signalBus;
    
    private BoardManager _boardManager;
    
    public override void Init()
    {
        _boardManager = _managerProvider.Get<BoardManager>();
        _dependencies.Add(_boardManager);
    }

    public override void Begin()
    {
        _signalBus.Subscribe<SpawnEndSignal>(CheckSquaresForFall);
        SetReady();
    }

    [SerializeField] private float BaseSpeed = 200f;
    [SerializeField] private float SpeedGap = 10f;
    [Button("Check fall")]
    public void CheckSquaresForFall()
    {
        var sequence = DOTween.Sequence();
        bool fall = false;
        int row;
        for (int i = 0; i < _boardManager.BoardWidth; i++)
        {
            row = 0;
            while (row < _boardManager.Board.Count)
            {
                var stableSquare = GetFirstStableInColumn(row, i);
                int topLimit = stableSquare == null ? _boardManager.Board.Count : stableSquare.Coordinates.x;
            
                var available = GetFirstAvailableSquareInColumn(row, i);
                if (available == null)
                {
                    row = topLimit + 1;
                    continue;
                }
            
                var nonEmptySquares = GetNonEmptySquaresInColumn(available.Coordinates.x, i, topLimit);
                for (int k = 0; k < nonEmptySquares.Count; k++)
                {
                    var boardElement = nonEmptySquares[k].BoardElement;
                    var speed = BaseSpeed - SpeedGap * k;
                    var distance = nonEmptySquares[k].CenterPosition - available.CenterPosition;
                    var duration = distance.y / speed;
                    sequence.Join(boardElement.transform.DOMove(available.CenterPosition, duration)); 
                    available.BoardElement = boardElement;
                    available = available.Up;
                    nonEmptySquares[k].BoardElement = null;
                    fall = true;
                }

                row = topLimit + 1;
            }
        }

        if (fall)
        {
            sequence.OnComplete(() =>
            {
                _signalBus.Fire<FallEndSignal>();
            });
        }
    }

    public void check()
    {
        var sequence = DOTween.Sequence();
        bool fall = false;
    }

    private Square GetFirstStableInColumn(int startRow, int col)
    {
        for (int i = startRow; i < _boardManager.BoardHeight; i++)
        {
            var square = _boardManager.Board[i][col];
            if (square.BoardElement != null && square.BoardElement.IsStable)
            {
                return _boardManager.Board[i][col];
            }
        }

        return null;
    }
    
    private Square GetFirstAvailableSquareInColumn(int startRow, int col)
    {
        for (int i = startRow; i < _boardManager.BoardHeight; i++)
        {
            if (AvailableForFall(i, col))
            {
                return _boardManager.Board[i][col];
            }
        }

        return null;
    }
    
    
    private List<Square> GetNonEmptySquaresInColumn(int start, int col, int topLimit)
    {
        List<Square> result = new List<Square>();
        for (int i = start; i < topLimit; i++)
        {
            if (_boardManager.Board[i][col].BoardElement != null)
            {
                result.Add( _boardManager.Board[i][col]);
            }
        }

        return result;
    }
    
    private bool AvailableForFall(int x, int y)
    {
        var board = _boardManager.Board;
        if (x < 0) return false;

        return board[x][y].BoardElement == null;
    }
}