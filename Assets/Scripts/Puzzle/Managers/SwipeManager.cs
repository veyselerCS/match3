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
        SetReady();
    }

    private void OnSwipeStarted(SwipeStartSignal data)
    {
        var board = _boardManager.Board;

        var swipeStartElement = board[data.From.x][data.From.y];
        var swipeEndElement = board[data.To.x][data.To.y];
        
        var sequence = DOTween.Sequence();

        sequence
            .Join(swipeStartElement.BoardElement.transform.DOMove(swipeEndElement.CenterPosition, 0.5f))
            .Join(swipeEndElement.BoardElement.transform.DOMove(swipeStartElement.CenterPosition, 0.5f))
            .OnComplete(() =>
            {
                (board[data.From.x][data.From.y].BoardElement.SquarePosition, board[data.To.x][data.To.y].BoardElement.SquarePosition) = (board[data.To.x][data.To.y].BoardElement.SquarePosition, board[data.From.x][data.From.y].BoardElement.SquarePosition);
                (board[data.From.x][data.From.y].BoardElement, board[data.To.x][data.To.y].BoardElement) = (board[data.To.x][data.To.y].BoardElement, board[data.From.x][data.From.y].BoardElement);
                
                _signalBus.Fire(new SwipeEndSignal(data.From, data.To));
            });
    }
}