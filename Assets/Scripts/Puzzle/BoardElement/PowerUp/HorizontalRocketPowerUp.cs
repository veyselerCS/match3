using System.Collections.Generic;
using UnityEngine;

public class HorizontalRocketPowerUp : PowerUp
{
    [SerializeField] private GameObject ParticleSystem;
    
    public override void OnPowerUpActivated()
    {
    }

    private List<Square>  GetInvolvedSquares()
    {
       return _boardManager.Board[SquarePosition.x];
    }
}