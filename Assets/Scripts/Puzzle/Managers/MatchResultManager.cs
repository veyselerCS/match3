using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class MatchResultManager : Manager
{
    [Inject] private SignalBus _signalBus;
    
    private PowerUpManager _powerUpManager;
    private BoardManager _boardManager;
    private TriggerManager _triggerManager;

    public override void Init()
    {
        _powerUpManager = _managerProvider.Get<PowerUpManager>();
        _boardManager = _managerProvider.Get<BoardManager>();
        _triggerManager = _managerProvider.Get<TriggerManager>();
        
        _dependencies.Add(_powerUpManager);
        _dependencies.Add(_boardManager);
        _dependencies.Add(_triggerManager);
    }

    public override void Begin()
    {
        SetReady();
    }

    public void ApplyResult(Sequence matchSequence, Vector2Int mergePosition, List<Vector2Int> involvedPositions,
        MatchResultType matchResultType)
    {
        switch (matchResultType)
        {
            case MatchResultType.DropPop:
                HandleDropCase(involvedPositions);
                break;
            case MatchResultType.VerticalRocket:
                _powerUpManager.CreatePowerUp(matchSequence, mergePosition, involvedPositions, PowerUpType.VerticalRocket);
                break;
            case MatchResultType.HorizontalRocket:
                _powerUpManager.CreatePowerUp(matchSequence, mergePosition, involvedPositions, PowerUpType.HorizontalRocket);
                break;
            case MatchResultType.Propeller:
                _powerUpManager.CreatePowerUp(matchSequence, mergePosition, involvedPositions, PowerUpType.Propeller);
                break;
            case MatchResultType.TNT:
                _powerUpManager.CreatePowerUp(matchSequence, mergePosition, involvedPositions, PowerUpType.TNT);
                break;
        }

        TriggerNearSquares(involvedPositions);
    }

    private void HandleDropCase(List<Vector2Int> involvedPositions)
    {
        var board = _boardManager.Board;
        
        foreach (var coordinate in involvedPositions)
        {
            var square = board[coordinate.x][coordinate.y];
            square.Unlock();
            square.BoardElement.BackToPool();
            square.BoardElement = null;
        }
    }

    private void TriggerNearSquares(List<Vector2Int> involvedPositions)
    {
        List<Square> triggerZone = new List<Square>();
        var board = _boardManager.Board;
        foreach (var position in involvedPositions)
        {
            var square = board.At(position);
            var nearSquares = square.GetNearSquares();
            foreach (var nearSquare in nearSquares)
            {
                if (nearSquare != null && !involvedPositions.Contains(nearSquare.Coordinates))
                {
                    triggerZone.Add(nearSquare);
                }
            }
        }
        
        _signalBus.Fire(new TriggerSignal(triggerZone, TriggerType.NearMatch));
    }
}