using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PropellarPowerUp : PowerUp
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private List<GameObject> Particles;
    
    public override void Activate()
    {
        Activated = true;
        _playRotation = true;
        _powerUpManager.PowerUpCount--;
        
        foreach (var particle in Particles)
        {
            particle.SetActive(true);
        }
        
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

            ResetComponent();
            BackToPool();
            _boardManager.Board.At(SquarePosition).BoardElement = null;
            _signalBus.Fire<MatchEndSignal>();
        });
    }

    public void ResetComponent()
    {
        _playRotation = false;
        foreach (var particle in Particles)
        {
            particle.SetActive(false);
        }
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

    private bool _playRotation;
    [Button("Play Anim")]
    private void PlayAnim()
    {
        _playRotation = !_playRotation;
    }

    private void Update()
    {
        if (_playRotation)
        {
            transform.rotation = Quaternion.Euler(0,0, transform.rotation.eulerAngles.z + 6);
        }
    }
}