using System.Collections.Generic;
using UnityEngine;

public class PropellarPowerUp : PowerUp
{
    public override List<Square> GetTriggerZone()
    {
        List<Square> triggerZone = new List<Square>();
        var board = _boardManager.Board;
        if(_boardManager.IsInBoardLimits(SquarePosition.x - 1, SquarePosition.y) && !board[SquarePosition.x - 1][SquarePosition.y].Locked) 
            triggerZone.Add(_boardManager.Board[SquarePosition.x - 1][SquarePosition.y]);
        
        if(_boardManager.IsInBoardLimits(SquarePosition.x + 1, SquarePosition.y) && !board[SquarePosition.x + 1][SquarePosition.y].Locked) 
            triggerZone.Add(_boardManager.Board[SquarePosition.x + 1][SquarePosition.y]);
        
        if(_boardManager.IsInBoardLimits(SquarePosition.x, SquarePosition.y - 1) && !board[SquarePosition.x][SquarePosition.y - 1].Locked) 
            triggerZone.Add(_boardManager.Board[SquarePosition.x][SquarePosition.y - 1]);
        
        if(_boardManager.IsInBoardLimits(SquarePosition.x, SquarePosition.y + 1) && !board[SquarePosition.x][SquarePosition.y + 1].Locked) 
            triggerZone.Add(_boardManager.Board[SquarePosition.x][SquarePosition.y + 1]);
        
        return triggerZone;
    }
}