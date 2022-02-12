using System.Collections.Generic;
using UnityEngine;

public class HorizontalRocketPowerUp : PowerUp
{
    [SerializeField] private GameObject ParticleSystem;

    public override List<Square> GetTriggerZone()
    {
        return _boardManager.Board[SquarePosition.x].FindAll((square) =>
        {
            return !square.Locked;
        } );
    }
}