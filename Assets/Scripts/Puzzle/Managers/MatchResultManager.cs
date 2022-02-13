using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MatchResultManager : Manager
{
    private PowerUpManager _powerUpManager;
    private BoardManager _boardManager;

    public override void Init()
    {
        _powerUpManager = _managerProvider.Get<PowerUpManager>();
        _boardManager = _managerProvider.Get<BoardManager>();
        
        _dependencies.Add(_powerUpManager);
        _dependencies.Add(_boardManager);
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
}