using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class VerticalRocketPowerUp : PowerUp
{
    [SerializeField] private RectTransform Top; 
    [SerializeField] private RectTransform Bottom;
    [SerializeField] private RectTransform Full;

    public override List<Square> GetTriggerZone()
    {
        List<Square> triggerZone = new List<Square>();
        
        for (int i = 0; i < _boardManager.BoardHeight; i++)
        {
            var square = _boardManager.Board[i][SquarePosition.y];
            if(!square.Locked)
                triggerZone.Add(_boardManager.Board[i][SquarePosition.y]);
        }

        return triggerZone;
    }

    private void ResetComponents()
    {
        var position = Full.transform.position;
        Bottom.transform.position = position;
        Top.transform.position = position;
        
        Full.gameObject.SetActive(true);
        Bottom.gameObject.SetActive(false);
        Top.gameObject.SetActive(false);
    }
    
    public override void Activate()
    {
        var board = _boardManager.Board;
        var topSquare = board[_boardManager.MaxTop][SquarePosition.y];
        var bottomSquare = board[0][SquarePosition.y];

        var triggerZone = GetTriggerZone();
        foreach (var square in triggerZone)
        {
            square.Locked = true;
        }

        Vector2Int topStartPos = SquarePosition;
        Vector2Int botStartPos = SquarePosition;
        
        _boardManager.Board.At(SquarePosition).BoardElement = null;
        
        Full.gameObject.SetActive(false);
        Top.gameObject.SetActive(true);
        Bottom.gameObject.SetActive(true);
        
        var sequence = DOTween.Sequence();
        sequence
            .Join(Top.DOMove(topSquare.CenterPosition.IncY(100), 0.5f))
            .Join(Bottom.DOMove(bottomSquare.CenterPosition.IncY(-100), 0.5f))
            .OnUpdate(() =>
            {
                var topCurrent = _boardManager.GetBoardPosition(Top.transform.position);
                var botCurrent = _boardManager.GetBoardPosition(Bottom.transform.position);
                
                if (_boardManager.IsInBoardLimits(topCurrent) && topCurrent != topStartPos &&  triggerZone.Contains(_boardManager.Board.At(topCurrent)))
                {
                    Debug.LogWarning("Changed top");
                    _signalBus.Fire(new TriggerSignal(new List<Square>{_boardManager.Board.At(topCurrent)}, TriggerType.Special));
                    topStartPos = topCurrent;
                }        
                
                if (_boardManager.IsInBoardLimits(botCurrent) && botCurrent != botStartPos && triggerZone.Contains(_boardManager.Board.At(botCurrent)))
                {
                    Debug.LogWarning("Changed bottom");
                    _signalBus.Fire(new TriggerSignal(new List<Square>{_boardManager.Board.At(botCurrent)}, TriggerType.Special));
                    botStartPos = botCurrent;
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
                
                _signalBus.Fire<MatchEndSignal>();
            });
    }
}