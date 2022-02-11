using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class MatchManager : SingletonManager<MatchManager>
{
    [Inject] private SignalBus _signalBus;

    private BoardManager _boardManager;
    private MatchResultManager _matchResultManager;
    private PatternService _patternService;
    private Sequence _matchSequence = null;
    private Vector2Int _matchSourcePosition;
    private HashSet<Vector2Int> _matched = new HashSet<Vector2Int>();
    
    private List<Vector2Int> _swipedSquareCoordinates = new List<Vector2Int>();
    private void Start()
    {
        _boardManager = BoardManager.Instance;
        _matchResultManager = MatchResultManager.Instance;
        _patternService = PatternService.Instance;

        _signalBus.Subscribe<SwipeEndSignal>(OnSwipeEndSignal);
        _signalBus.Subscribe<FallEndSignal>(OnFallEndSignal);
    }

    private void OnSwipeEndSignal(SwipeEndSignal data)
    {
        _matchSequence = DOTween.Sequence();

        _swipedSquareCoordinates.Clear();
        _swipedSquareCoordinates.Add(data.From);
        _swipedSquareCoordinates.Add(data.To);

        CheckFullBoard();
    }

    private void OnFallEndSignal()
    {
        CheckFullBoard();
    }

    [Button("Check match")]
    public void CheckFullBoard()
    {
        var match = CheckMatchNoSignal();
        _matchSequence.OnComplete(() =>
        {
            if(match)
                _signalBus.Fire<MatchEndSignal>();
        });
    }

    public bool CheckMatchNoSignal()
    {
        var board = _boardManager.Board;
        _matched.Clear();
        _matchSequence = DOTween.Sequence();

        var patternShapes = _patternService.PatternShapes;
        bool match = false;
        foreach (var patternShape in patternShapes)
        {
            
            for (int i = 0; i < _boardManager.BoardHeight - patternShape.PatternHeight; i++)
            {
                for (int k = 0; k < _boardManager.BoardWidth - patternShape.PatternWidth; k++)
                {
                    var firstNonZero = patternShape.NonZeros[0];
                    var firstNonZeroSquare = board[i + firstNonZero.x][k + firstNonZero.y];

                    if (firstNonZeroSquare.BoardElement != null && firstNonZeroSquare.BoardElement is Drop firstDrop && !_matched.Contains(firstNonZeroSquare.Coordinates))
                    {
                        var dropType = firstDrop.DropType;
                        var patternFound = true;
                        for (int j = 1; j < patternShape.NonZeros.Count; j++)
                        {
                            var nonZero = patternShape.NonZeros[j];
                            var nonZeroSquare = board[i + nonZero.x][k + nonZero.y];
                            if (nonZeroSquare.BoardElement == null ||
                                !(nonZeroSquare.BoardElement is Drop drop) ||
                                dropType != drop.DropType ||
                                _matched.Contains(nonZeroSquare.Coordinates))
                            {
                                patternFound = false;
                                break;
                            }
                        }
                        
                        //pattern match found
                        var mergePosition = firstNonZeroSquare.Coordinates;
                        List<Vector2Int> involvedPositions = new List<Vector2Int>();
                        for (int j = 0; patternFound && j < patternShape.NonZeros.Count; j++)
                        {
                            var nonZero = patternShape.NonZeros[j];
                            var nonZeroSquare = board[i + nonZero.x][k + nonZero.y];
                            if (_swipedSquareCoordinates.Contains(nonZeroSquare.Coordinates))
                            {
                                mergePosition = nonZeroSquare.Coordinates;
                            }
                            involvedPositions.Add(nonZeroSquare.Coordinates);
                            _matched.Add(nonZeroSquare.Coordinates);
                        }

                        if (patternFound)
                        {
                            _matchResultManager.ApplyResult(_matchSequence, mergePosition, involvedPositions, patternShape.MatchResultType);
                            match = true;
                        }
                    }
                }
            }
        }

        return match;
    }
}

public enum MatchResultType
{
    DropPop = 0,
    VerticalRocket = 1,
    HorizontalRocket = 2,
    Propeller = 3,
    TNT = 4
}