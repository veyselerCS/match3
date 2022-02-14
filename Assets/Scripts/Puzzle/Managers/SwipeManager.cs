using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class SwipeManager : Manager
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
        _signalBus.Subscribe<SwipeStartSignal>(OnSwipeStarted);
        _signalBus.Subscribe<SwipeFailSignal>(OnSwipeFail);
        SetReady();
    }

    private void OnSwipeStarted(SwipeStartSignal data)
    {
        var board = _boardManager.Board;

        var swipeStartElement = board[data.From.x][data.From.y];
        var swipeEndElement = board[data.To.x][data.To.y];
       
        Swap(swipeStartElement, swipeEndElement, () =>
        {
            _signalBus.Fire(new SwipeEndSignal(swipeStartElement.Coordinates, swipeEndElement.Coordinates));
        });
    }    
    
    private void OnSwipeFail(SwipeFailSignal data)
    {
        var board = _boardManager.Board;

        var swipeStartElement = board[data.From.x][data.From.y];
        var swipeEndElement = board[data.To.x][data.To.y];

        Swap(swipeStartElement, swipeEndElement);
    }

    private void Swap(Square swipeStartElement, Square swipeEndElement, Action onSwipeEnd = null)
    {
        var board = _boardManager.Board;

        var sequence = DOTween.Sequence();

        sequence
            .Join(swipeStartElement.BoardElement.transform.DOMove(swipeEndElement.CenterPosition, 0.2f))
            .Join(swipeEndElement.BoardElement.transform.DOMove(swipeStartElement.CenterPosition, 0.2f))
            .OnComplete(() =>
            {
                //swap square position data
                (board[swipeStartElement.Coordinates.x][swipeStartElement.Coordinates.y].BoardElement.SquarePosition, board[swipeEndElement.Coordinates.x][swipeEndElement.Coordinates.y].BoardElement.SquarePosition) = 
                    (board[swipeEndElement.Coordinates.x][swipeEndElement.Coordinates.y].BoardElement.SquarePosition, board[swipeStartElement.Coordinates.x][swipeStartElement.Coordinates.y].BoardElement.SquarePosition);
                
                //swap board element data
                (board[swipeStartElement.Coordinates.x][swipeStartElement.Coordinates.y].BoardElement, board[swipeEndElement.Coordinates.x][swipeEndElement.Coordinates.y].BoardElement) =
                    (board[swipeEndElement.Coordinates.x][swipeEndElement.Coordinates.y].BoardElement, board[swipeStartElement.Coordinates.x][swipeStartElement.Coordinates.y].BoardElement);
                
                onSwipeEnd?.Invoke();
            });
    }
}