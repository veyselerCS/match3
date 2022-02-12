using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TNTPowerUp : PowerUp
{
    public GameObject Particle;
    public GameObject TNTImage;
    
    private void ResetComponent()
    {
        TNTImage.SetActive(true);
        Particle.SetActive(false);
    }

    public override void Activate()
    {
        var triggerZone = GetTriggerZone();
        foreach (var square in triggerZone)
        {
            square.Locked = true;
        }
        
        Particle.gameObject.SetActive(true);
        TNTImage.gameObject.SetActive(false);
        DOVirtual.DelayedCall(0.4f, () =>
        {
            foreach (var square in triggerZone)
            {
                square.Locked = false;
            }
            BackToPool();
            ResetComponent();
            _boardManager.Board.At(SquarePosition).BoardElement = null;
            _signalBus.Fire(new TriggerSignal(triggerZone, TriggerType.Special));
            _signalBus.Fire<MatchEndSignal>();
        });
    }

    public override List<Square> GetTriggerZone()
    {
        var bottomLeft = new Vector2Int(Math.Max(SquarePosition.x - 2, 0), Math.Max(SquarePosition.y - 2, 0));
        var TopRight = new Vector2Int(Math.Min(SquarePosition.x + 2, _boardManager.MaxTop), Math.Min(SquarePosition.y + 2, _boardManager.MaxRight));
        List<Square> triggerZone = new List<Square>();
        for (int i = bottomLeft.x; i <= TopRight.x; i++)
        {
            for (int k = bottomLeft.y; k <= TopRight.y; k++)
            {
                var square = _boardManager.Board[i][k];
                if(!square.Locked)
                    triggerZone.Add(_boardManager.Board[i][k]);
            }
        }

        triggerZone.Remove(_boardManager.Board.At(SquarePosition));
        return triggerZone;
    }
}