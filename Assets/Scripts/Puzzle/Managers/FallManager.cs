using System;
using System.Collections.Generic;
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
        _signalBus.Subscribe<SpawnEndSignal>(CheckSquaresForFall);
    }

    [SerializeField] private float BaseSpeed = 200f;
    [SerializeField] private float SpeedGap = 10f;
    [Button("Check fall")] [SerializeField]
    public void CheckSquaresForFall()
    {
        var sequence = DOTween.Sequence();
        for (int i = 0; i < _boardManager.BoardWidth; i++)
        {
            var available = GetFirstAvailableSquareInColumn(i);
            if(available == null) continue;
            
            var nonEmptySquares = GetNonEmptySquaresInColumn(available.Coordinates.x, i);
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
            }
        }

        sequence.OnComplete(() =>
        {
            _signalBus.Fire<FallEndSignal>();
        });
    }

    private int GetNumberOfDropsToFall(int col)
    {
        var board = _boardManager.Board;
        int count = 0;
        for (int i = _boardManager.BoardHeight; i < board.Count; i++)
        {
            if (_boardManager.Board[i][col].BoardElement != null)
            {
                count++;
            }
        }

        return count;
    }
    
    private Square GetFirstAvailableSquareInColumn(int col)
    {
        for (int i = 0; i < _boardManager.BoardHeight; i++)
        {
            if (AvailableForFall(i, col))
            {
                return _boardManager.Board[i][col];
            }
        }

        return null;
    }
    
    
    private List<Square> GetNonEmptySquaresInColumn(int start, int col)
    {
        List<Square> result = new List<Square>();
        for (int i = start; i < _boardManager.Board.Count; i++)
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