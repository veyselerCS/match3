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

    private Tween _moveTween;
    
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

            _boardManager.Board.At(SquarePosition).BoardElement = null;
            GoToTarget();
            _signalBus.Fire<CheckFallSignal>();
        });
    }

    private void GoToTarget()
    {
        _moveTween.Kill();
        
        var targetSquare = _targetManager.GetRandomTarget();
        if(targetSquare == null) BackToPool();

        _moveTween = _rectTransform.DOMove(targetSquare.CenterPosition, 1f).SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                if (targetSquare.Locked)
                {
                    GoToTarget();
                }
            })
            .OnComplete(() =>
            {
                _signalBus.Fire(new TriggerSignal(new List<Square>(){targetSquare}, TriggerType.Special));
                _signalBus.Fire<CheckFallSignal>();
                ResetComponent();
                BackToPool();
            });
    }
    
    public void ResetComponent()
    {
        _playRotation = false;
        transform.rotation = Quaternion.Euler(Vector3.zero);
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