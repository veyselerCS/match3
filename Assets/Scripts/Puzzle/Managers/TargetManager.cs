using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using Zenject;

public class TargetManager : Manager
{
    [Inject] private SignalBus _signalBus;

    [SerializeField] private List<TargetComponent> Targets;
    [SerializeField] private SpriteAtlas SpriteAtlas;
    [SerializeField] private TextMeshProUGUI RemainingMoveText;

    private BoardManager _boardManager;
    private SceneComponentService _sceneComponentService;
    private PopupManager _popupManager;
    private PuzzleLoadManager _puzzleLoadManager;
    private InputManager _inputManager;

    private Dictionary<ObstacleType, TargetComponent> ObstacleTypeToTargetDict =
        new Dictionary<ObstacleType, TargetComponent>();

    private int _targetCount = 0;
    private string AtlasBaseKey = "Puzzle_Obstacle_";
    private int _moveCount;
    private int _tweenCount = 0;
    public override void Init()
    {
        _boardManager = _managerProvider.Get<BoardManager>();
        _sceneComponentService = _managerProvider.Get<SceneComponentService>();
        _popupManager = _managerProvider.Get<PopupManager>();
        _puzzleLoadManager = _managerProvider.Get<PuzzleLoadManager>();
        _inputManager = _managerProvider.Get<InputManager>();

        _dependencies.Add(_boardManager);
        _dependencies.Add(_sceneComponentService);
        _dependencies.Add(_popupManager);
        _dependencies.Add(_puzzleLoadManager);
    }

    public override void Begin()
    {
        Refresh();
        _signalBus.Subscribe<SuccessfulMoveSignal>(OnSuccessfulMoveSignal);
        SetReady();
    }

    public void Refresh()
    {
        ObstacleTypeToTargetDict.Clear();
        _targetCount = 0;
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

        _moveCount = _puzzleLoadManager.LevelToLoad.MoveCount;
        RemainingMoveText.text = _moveCount.ToString();
    }
    
    public void OnSuccessfulMoveSignal()
    {
        _moveCount--;
        RemainingMoveText.text = _moveCount.ToString();

        if (_moveCount == 0)
        {
            _inputManager.enabled = false;
        }

        if (!HasActiveTween())
        {
            if(!CheckSuccess())
                CheckFail();
        }
    }

    public bool IsOutOfMoves()
    {
        return _moveCount == 0;
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
        _tweenCount++;
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
                if(!CheckSuccess())
                    CheckFail();
                boardElement.BackToPool();
                _tweenCount--;
            });
    }

    public bool CheckSuccess()
    {
        foreach (var targetComponent in ObstacleTypeToTargetDict.Values)
        {
            if(targetComponent.RemainingAmount != 0) return false;
        }

        _inputManager.enabled = false;
        _popupManager.Show(new LevelSuccessPopup.Data());
        return true;
    }    
    
    public bool CheckFail()
    {
        if (!IsOutOfMoves() || !_boardManager.IsFullyUnlocked) return false;
        
        foreach (var targetComponent in ObstacleTypeToTargetDict.Values)
        {
            if (targetComponent.RemainingAmount > 0)
            {
                _inputManager.enabled = false;
                _popupManager.Show(new LevelFailPopup.Data());
                return true;
            }
        }
        return false;
    }

    public bool HasActiveTween()
    {
        return _tweenCount != 0;
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