using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Zenject;

public class MatchManager : Manager
{
    [Inject] private SignalBus _signalBus;

    private BoardManager _boardManager;
    private MatchResultManager _matchResultManager;
    private PatternService _patternService;
    private PowerUpManager _powerUpManager;
    private Sequence _matchSequence = null;
    private Vector2Int _matchSourcePosition;
    private HashSet<Vector2Int> _previouslyMatched = new HashSet<Vector2Int>();
    
    private List<Vector2Int> _swipedSquareCoordinates = new List<Vector2Int>();

    public override void Init()
    {
        _boardManager = _managerProvider.Get<BoardManager>();
        _matchResultManager = _managerProvider.Get<MatchResultManager>();
        _powerUpManager = _managerProvider.Get<PowerUpManager>();
        _patternService = _managerProvider.Get<PatternService>();
        
        _dependencies.Add(_boardManager);
        _dependencies.Add(_matchResultManager);
        _dependencies.Add(_patternService);
        _dependencies.Add(_powerUpManager);
    }

    public override void Begin()
    {
        _signalBus.Subscribe<SwipeEndSignal>(OnSwipeEndSignal);
        _signalBus.Subscribe<FallEndSignal>(OnFallEndSignal);
        
        SetReady();
    }

    private void OnSwipeEndSignal(SwipeEndSignal data)
    {
        var board = _boardManager.Board;
        _matchSequence = DOTween.Sequence();
        
        
        _swipedSquareCoordinates.Clear();
        _swipedSquareCoordinates.Add(data.From);
        _swipedSquareCoordinates.Add(data.To);

        bool powerUpActivated = false;
        foreach (var swipedSquare in _swipedSquareCoordinates)
        {
            var boardElement = board.At(swipedSquare).BoardElement;
            
            if (boardElement != null && boardElement is PowerUp powerUp)
            {
                powerUpActivated = true;
                powerUp.Activate();
            }
        }

        if (!CheckFullBoard() && !powerUpActivated)
        {
            _signalBus.Fire(new SwipeFailSignal(data.To, data.From));
        }
    }

    private void OnFallEndSignal()
    {
        if (!CheckFullBoard() && !CheckNoPossibleMove())
        {
            Debug.LogWarning("Game over");
        }
    }

    [Button("Check match")]
    public bool CheckFullBoard()
    {
        var match = CheckMatch();
        _matchSequence.OnComplete(() =>
        {
            if(match)
                _signalBus.Fire<CheckFallSignal>();
        });
        return match;
    }

    public bool CheckMatch()
    {
        var board = _boardManager.Board;
        _previouslyMatched.Clear();
        _matchSequence = DOTween.Sequence();

        var patternShapes = _patternService.PatternShapes;
        bool swipeFound = false;
        foreach (var patternShape in patternShapes)
        {
            
            for (int i = 0; i < _boardManager.BoardHeight - patternShape.PatternHeight; i++)
            {
                for (int k = 0; k < _boardManager.BoardWidth - patternShape.PatternWidth; k++)
                {
                    var firstNonZero = patternShape.NonZeros[0];
                    var firstNonZeroSquare = board[i + firstNonZero.x][k + firstNonZero.y];
                    
                    if (firstNonZeroSquare.TryGetByType(out Drop firstDrop, null) && !_previouslyMatched.Contains(firstNonZeroSquare.Coordinates) && !firstNonZeroSquare.Locked)
                    {
                        var dropType = firstDrop.DropType;
                        var patternFound = true;
                        for (int j = 1; j < patternShape.NonZeros.Count; j++)
                        {
                            var nonZero = patternShape.NonZeros[j];
                            var nonZeroSquare = board[i + nonZero.x][k + nonZero.y];
                            if (nonZeroSquare.BoardElement == null ||
                                nonZeroSquare.Locked ||
                                !(nonZeroSquare.BoardElement is Drop drop) ||
                                dropType != drop.DropType ||
                                _previouslyMatched.Contains(nonZeroSquare.Coordinates))
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
                            nonZeroSquare.Lock();
                            _previouslyMatched.Add(nonZeroSquare.Coordinates);
                        }

                        if (patternFound)
                        {
                            _matchResultManager.ApplyResult(_matchSequence, mergePosition, involvedPositions, patternShape.MatchResultType);
                            swipeFound = true;
                        }
                    }
                }
            }
        }

        return swipeFound;
    }

    private List<Square> _possibleswipesquares = new List<Square>();
    private Square _possibleSwipeSquare = null;
    [Button("Check no swipe")] 
    //Looks ugly but optimized to return as early as possible to make minimum comparisons
    private bool CheckNoPossibleMove()
    {
        _possibleswipesquares.Clear();
        var board = _boardManager.Board;
        _possibleSwipeSquare = board[0][0];
        _matchSequence = DOTween.Sequence();

        var patternShapes = _patternService.PatternShapes;

        if (_powerUpManager.PowerUpCount > 0) return true;
        
        List<DropType> possibleMatchDropTypes = new List<DropType>();//at most 2 elements at a time or no swipe possible to create that pattern
        foreach (var patternShape in patternShapes)
        {
            for (int i = 0; i < _boardManager.BoardHeight - patternShape.PatternHeight; i++)
            {
                for (int k = 0; k < _boardManager.BoardWidth - patternShape.PatternWidth; k++)
                {
                    var firstNonZero = patternShape.NonZeros[0];
                    var firstNonZeroSquare = board[i + firstNonZero.x][k + firstNonZero.y];
                    
                    if (!firstNonZeroSquare.TryGetByType(out Drop firstDrop, null)) continue;//no drop no pattern
                    
                    possibleMatchDropTypes.Clear();
                    possibleMatchDropTypes.Add(firstDrop.DropType);
                    
                    if (!firstNonZeroSquare.Locked)
                    {
                        bool possibleSwipeFound = true;
                        bool canAcceptMoreTypes = true;
                        Square possibleSwipeSquare = firstNonZeroSquare;
                        DropType pickedType = firstDrop.DropType;
                        _possibleswipesquares.Clear();
                        _possibleswipesquares.Add(firstNonZeroSquare);
                        
                        for (int j = 1; j < patternShape.NonZeros.Count; j++)
                        {
                            var nonZero = patternShape.NonZeros[j];
                            var nonZeroSquare = board[i + nonZero.x][k + nonZero.y];

                            if (nonZeroSquare.BoardElement == null ||
                                nonZeroSquare.Locked ||
                                !(nonZeroSquare.BoardElement is Drop drop))
                            {
                                possibleSwipeFound = false;
                                break;
                            }

                            var isInPossibleTypes = possibleMatchDropTypes.Contains(drop.DropType);               

                            if (!isInPossibleTypes && possibleMatchDropTypes.Count > 1)//no way to find a match since we can't fit 2 elements by swipe at any time
                            {
                                possibleSwipeFound = false;
                                break;
                            }

                            if (!canAcceptMoreTypes && drop.DropType != pickedType)//we can't allow more than one of each type as well
                            {
                                possibleSwipeFound = false;
                                break;
                            }
                            
                            if(!isInPossibleTypes && canAcceptMoreTypes)
                            {
                                possibleMatchDropTypes.Add(drop.DropType);
                                if (j == patternShape.NonZeros.Count - 1) //last element has to be the picked element if its the new type
                                {
                                    possibleSwipeSquare = nonZeroSquare;
                                }
                            }
                            
                            if (isInPossibleTypes && possibleMatchDropTypes.Count == 2) //we have already seen two types so the new one must be the match type if it is possible
                            {
                                if (j > 2 && pickedType != drop.DropType)//after deciding picked type we can't afford to have the other type back
                                {
                                    possibleSwipeFound = false;
                                    break;
                                }
                                
                                pickedType = drop.DropType;
                                
                                if (drop.DropType == firstDrop.DropType && possibleSwipeSquare == firstNonZeroSquare) //change the drop type and the possible square
                                {
                                    var prevNonZero = patternShape.NonZeros[j - 1];
                                    var prevnonZeroSquare = board[i + prevNonZero.x][k + prevNonZero.y];
                                    possibleSwipeSquare = prevnonZeroSquare;
                                }

                                canAcceptMoreTypes = false;
                            }
                            
                            _possibleswipesquares.Add(nonZeroSquare);
                        }

                        if (possibleSwipeFound)
                        {
                            _possibleswipesquares.Remove(possibleSwipeSquare);//for gizmos
                            var nearSquares = possibleSwipeSquare.GetNearSquares();
                            foreach (var square in nearSquares)
                            {
                                if (square != null && square.TryGetByType(out Drop drop, null) && !_possibleswipesquares.Contains(square))
                                {
                                    if (drop.DropType == pickedType)
                                    {
                                        _possibleSwipeSquare = possibleSwipeSquare;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (_possibleSwipeSquare != null)
        {
            Rect rect = new Rect(_possibleSwipeSquare.CenterPosition.IncX(-50).IncY(-50), new Vector2(100,100));
            Handles.DrawSolidRectangleWithOutline(rect, Color.cyan, Color.white);

            foreach (var square in _possibleswipesquares)
            {
                Rect rect2 = new Rect(square.CenterPosition.IncX(-50).IncY(-50), new Vector2(100,100));
                Handles.DrawSolidRectangleWithOutline(rect2, Color.yellow, Color.white);
            }
        }
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