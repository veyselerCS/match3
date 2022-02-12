using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HorizontalRocketPowerUp : PowerUp
{
    [SerializeField] private RectTransform Right;
    [SerializeField] private RectTransform Left;
    [SerializeField] private RectTransform Full;
    
    public override void Activate()
    {
        var board = _boardManager.Board;
        var rightSquare = board[SquarePosition.x][_boardManager.MaxRight];
        var leftSquare = board[SquarePosition.x][0];

        var triggerZone = GetTriggerZone();
        foreach (var square in triggerZone)
        {
            square.Locked = true;
        }

        Vector2Int topStartPos = SquarePosition;
        Vector2Int botStartPos = SquarePosition;
        
        Full.gameObject.SetActive(false);
        Right.gameObject.SetActive(true);
        Left.gameObject.SetActive(true);
        
        var sequence = DOTween.Sequence();
        sequence
            .Join(Right.DOMove(rightSquare.CenterPosition.IncX(100), 0.5f))
            .Join(Left.DOMove(leftSquare.CenterPosition.IncX(-100), 0.5f))
            .OnUpdate(() =>
            {
                var rightCurrent = _boardManager.GetBoardPosition(Right.transform.position);
                var leftCurrent = _boardManager.GetBoardPosition(Left.transform.position);
                
                if (_boardManager.IsInBoardLimits(rightCurrent) && rightCurrent != topStartPos && triggerZone.Contains(_boardManager.Board.At(rightCurrent)))
                {
                    Debug.LogWarning("Changed right");
                    _signalBus.Fire(new TriggerSignal(new List<Square>{_boardManager.Board.At(rightCurrent)}, TriggerType.Special));
                    topStartPos = rightCurrent;
                }        
                
                if (_boardManager.IsInBoardLimits(leftCurrent) && leftCurrent != botStartPos && triggerZone.Contains(_boardManager.Board.At(leftCurrent)))
                {
                    Debug.LogWarning("Changed left");
                    _signalBus.Fire(new TriggerSignal(new List<Square>{_boardManager.Board.At(leftCurrent)}, TriggerType.Special));
                    botStartPos = leftCurrent;
                }
            })
            .OnComplete(() =>
            {
                foreach (var square in triggerZone)
                {
                    square.Locked = false;
                }
                
                BackToPool();
                ResetComponents();
                
                _boardManager.Board.At(SquarePosition).BoardElement = null;
                _signalBus.Fire<MatchEndSignal>();
            });
    }
    
    private void ResetComponents()
    {
        var position = Full.transform.position;
        Right.transform.position = position;
        Left.transform.position = position;
        
        Full.gameObject.SetActive(true);
        Right.gameObject.SetActive(false);
        Left.gameObject.SetActive(false);
    }
    
    public override List<Square> GetTriggerZone()
    {
        return _boardManager.Board[SquarePosition.x].FindAll((square) =>
        {
            return !square.Locked;
        } );
    }
}