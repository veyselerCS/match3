using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PropellarPowerUp : PowerUp
{
    [SerializeField] private RectTransform _rectTransform;
    
    public override void Activate()
    {
        Activated = true;
        _powerUpManager.PowerUpCount--;
        
        var triggerZone = GetTriggerZone();
        triggerZone.Add(_boardManager.Board.At(SquarePosition));
        foreach (var square in triggerZone)
        {
            square.Lock();
        }
        
        _signalBus.Fire(new TriggerSignal(triggerZone, TriggerType.Special));
        DOVirtual.DelayedCall(0.2f, () =>
        {
            foreach (var square in triggerZone)
            {
                square.Unlock();
            }
            BackToPool();
            _boardManager.Board.At(SquarePosition).BoardElement = null;
            _signalBus.Fire<MatchEndSignal>();
        });
    }

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

    private bool playAnim;
    [Button("Play Anim")]
    private void PlayAnim()
    {
        playAnim = !playAnim;
    }

    private void Update()
    {
        if (playAnim)
        {
            transform.rotation = Quaternion.Euler(0,0, transform.rotation.eulerAngles.z + 6);
        }
    }
}