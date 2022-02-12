using System.Collections.Generic;
using UnityEngine;

public class VerticalRocketPowerUp : PowerUp
{
    public override List<Square> GetTriggerZone()
    {
        List<Square> triggerZone = new List<Square>();
        
        for (int i = 0; i < _boardManager.BoardHeight; i++)
        {
            triggerZone.Add(_boardManager.Board[i][SquarePosition.y]);
        }

        return triggerZone;
    }
}