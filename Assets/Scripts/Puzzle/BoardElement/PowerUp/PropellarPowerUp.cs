using System.Collections.Generic;
using UnityEngine;

public class PropellarPowerUp : PowerUp
{
    public override List<Square> GetTriggerZone()
    {
        List<Square> triggerZone = new List<Square>();
        
        if(_boardManager.IsInBoardLimits(SquarePosition.x - 1, SquarePosition.y)) triggerZone.Add(_boardManager.Board[SquarePosition.x - 1][SquarePosition.y]);
        if(_boardManager.IsInBoardLimits(SquarePosition.x + 1, SquarePosition.y)) triggerZone.Add(_boardManager.Board[SquarePosition.x + 1][SquarePosition.y]);
        if(_boardManager.IsInBoardLimits(SquarePosition.x , SquarePosition.y - 1)) triggerZone.Add(_boardManager.Board[SquarePosition.x][SquarePosition.y - 1]);
        if(_boardManager.IsInBoardLimits(SquarePosition.x , SquarePosition.y + 1)) triggerZone.Add(_boardManager.Board[SquarePosition.x][SquarePosition.y + 1]);

        return triggerZone;
    }
}