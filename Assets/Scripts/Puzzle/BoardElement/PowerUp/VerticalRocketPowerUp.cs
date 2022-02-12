using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class VerticalRocketPowerUp : PowerUp
{
    [SerializeField] private RectTransform Top; 
    [SerializeField] private RectTransform Bottom;

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
        
        Vector3 cachedPosTop = Top.transform.position;
        Vector3 cachedPosBottom = Bottom.transform.position;
        
        _boardManager.Board.At(SquarePosition).BoardElement = null;
        
        var sequence = DOTween.Sequence();
        sequence
            .Join(Top.DOMove(topSquare.CenterPosition, 0.5f))
            .Join(Bottom.DOMove(bottomSquare.CenterPosition, 0.5f))
            .OnUpdate(() =>
            {
                var topCurrent = _boardManager.GetBoardPosition(Top.transform.position);
                var botCurrent = _boardManager.GetBoardPosition(Bottom.transform.position);
                
                if (topCurrent != topStartPos &&  triggerZone.Contains(_boardManager.Board.At(topCurrent)))
                {
                    Debug.LogWarning("Changed top");
                    _signalBus.Fire(new TriggerSignal(new List<Square>{_boardManager.Board.At(topCurrent)}, TriggerType.Special));
                    topStartPos = topCurrent;
                }        
                
                if (botCurrent != botStartPos && triggerZone.Contains(_boardManager.Board.At(botCurrent)))
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
                
                Top.position = cachedPosTop;
                Bottom.position = cachedPosBottom;
                
                _signalBus.Fire<MatchEndSignal>();
            });
    }
}