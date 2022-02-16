using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.U2D;

public class TargetManager : Manager
{
    [SerializeField] private List<TargetComponent> Targets;
    [SerializeField] private SpriteAtlas SpriteAtlas;

    private BoardManager _boardManager;
    private SceneComponentService _sceneComponentService;

    private Dictionary<ObstacleType, TargetComponent> ObstacleTypeToTargetDict =
        new Dictionary<ObstacleType, TargetComponent>();

    private int _targetCount = 0;
    private string AtlasBaseKey = "Puzzle_Obstacle_";

    public override void Init()
    {
        _boardManager = _managerProvider.Get<BoardManager>();
        _sceneComponentService = _managerProvider.Get<SceneComponentService>();

        _dependencies.Add(_boardManager);
        _dependencies.Add(_sceneComponentService);
    }

    public override void Begin()
    {
        var board = _boardManager.Board;

        for (int i = 0; i < Targets.Count; i++)
        {
            Targets[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _boardManager.BoardHeight; i++)
        {
            for (int k = 0; k < _boardManager.BoardWidth; k++)
            {
                var square = board[i][k];
                if (square.TryGetByType(out Obstacle obstacle, null))
                {
                    if (!ObstacleTypeToTargetDict.ContainsKey(obstacle.ObstacleType))
                    {
                        var targetComponent = Targets[_targetCount];
                        targetComponent.gameObject.SetActive(true);
                        targetComponent.SetSprite(SpriteAtlas.GetSprite(AtlasBaseKey + obstacle.ObstacleType));
                        targetComponent.RemainingAmount = 1;
                        ObstacleTypeToTargetDict[obstacle.ObstacleType] = targetComponent;

                        _targetCount++;
                    }
                    else
                    {
                        ObstacleTypeToTargetDict[obstacle.ObstacleType].RemainingAmount++;
                    }
                }
            }
        }

        for (int i = 0; i < Targets.Count; i++)
        {
            Targets[i].Init();
        }

        SetReady();
    }

    public void CheckTarget(Square square)
    {
        if (square.TryGetByType(out Obstacle obstacle, null))
        {
            if(!ObstacleTypeToTargetDict.ContainsKey(obstacle.ObstacleType)) return;//for cheat
            
            var targetComponent = ObstacleTypeToTargetDict[obstacle.ObstacleType];
            TweenToTarget(square.BoardElement, targetComponent);
            square.BoardElement = null;
        }
    }

    private void TweenToTarget(BoardElement boardElement, TargetComponent targetComponent)
    {
        var boardElementRT = (RectTransform)boardElement.transform;
        var cachedSize = boardElementRT.sizeDelta;

        boardElementRT.SetParent(_sceneComponentService.TargetParent); //to render above the target canvas
        var targetImageRT = targetComponent.TargetImage.rectTransform;
        var sequence = DOTween.Sequence();
        sequence
            .Join(boardElementRT.DOMove(targetImageRT.position, 0.75f).SetEase(Ease.InOutCubic))
            .Join(boardElementRT.DOSizeDelta(targetImageRT.sizeDelta, .75f))
            .OnComplete(() =>
            {
                boardElementRT.sizeDelta = cachedSize;
                targetComponent.DecRemainingAmount();
                boardElement.BackToPool();
            });
    }

    public Square GetRandomTarget()
    {
        var board = _boardManager.Board;
        List<Square> possibleSquares = new List<Square>();
        int currentMaxScore = 0;
        for (int i = 0; i < _boardManager.BoardHeight; i++)
        {
            for (int k = 0; k < _boardManager.BoardWidth; k++)
            {
                var square = board[i][k];
                var boardElement = square.BoardElement;
                if (!square.Locked && boardElement != null)
                {
                    if (boardElement.PropellerTargetScore > currentMaxScore)
                    {
                        currentMaxScore = boardElement.PropellerTargetScore;
                        possibleSquares.Add(square);
                        possibleSquares.Clear();
                    }

                    if (boardElement.PropellerTargetScore == currentMaxScore)
                    {
                        possibleSquares.Add(square);
                    }
                }
            }
        }

        if (possibleSquares.Count == 0) return null;
        
        return possibleSquares.GetRandomElement();
    }
}