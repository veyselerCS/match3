using System;
using UnityEditor;
using UnityEngine;
using Zenject;

public class InputManager : SingletonManager<InputManager>
{
    [Inject] private SignalBus _signalBus;
    
    private BoardManager _boardManager;
    private CheatManager _cheatManager;
    private SceneComponentService _sceneComponentService;
    
    private Vector2 _squareSize;
    private Vector2Int _firstTouchedBoardPos;

    private bool _lock;
    private bool _swipeSent;
    
    private void Start()
    {
        _boardManager = BoardManager.Instance;
        _cheatManager = CheatManager.Instance;
        _sceneComponentService = SceneComponentService.Instance;
        _signalBus.Subscribe<SwipeEndSignal>(OnSwipeEndSignal);
    }

    public void SetBoardSquareSize(Vector2 size)
    {
        _squareSize = size;
    }

    private void Update()
    {
        if(_lock) return;

        if (_cheatManager.CheatMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _firstTouchedBoardPos = GetTouchBoardPosition();
                if(!_boardManager.IsInBoardLimits(_firstTouchedBoardPos)) return;
                    
                var cheatElement = Instantiate(_cheatManager.PickedElement, _sceneComponentService.BoardElementParent.transform);
                cheatElement.transform.position = _boardManager.Board.At(_firstTouchedBoardPos).CenterPosition;
                if (_boardManager.Board.At(_firstTouchedBoardPos).BoardElement)
                    Destroy(_boardManager.Board.At(_firstTouchedBoardPos).BoardElement.gameObject);
                _boardManager.Board.At(_firstTouchedBoardPos).BoardElement = cheatElement;
            }
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            _firstTouchedBoardPos = GetTouchBoardPosition();
            Debug.LogWarning("Touch x : " + _firstTouchedBoardPos.x + " y : " + _firstTouchedBoardPos.y);
        }

        if (Input.GetMouseButton(0) && !_swipeSent)
        {
            var currentTouchPos = GetTouchBoardPosition();
            if (currentTouchPos != _firstTouchedBoardPos)
            {
                _lock = true;
                _swipeSent = true;
                
                _signalBus.Fire(new SwipeStartSignal(_firstTouchedBoardPos, currentTouchPos));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _swipeSent = false;
        }
    }

    private Vector2Int GetTouchBoardPosition()
    {
        var touchPos = Input.mousePosition;
        var offSet = touchPos - _boardManager.transform.position;
        var lossyScale = _boardManager.transform.lossyScale;
        return new Vector2Int((int)((offSet.y / _squareSize.y) * (1f / lossyScale.y)), (int)((offSet.x / _squareSize.x) * (1f / lossyScale.x)));
    }

    private void OnSwipeEndSignal(SwipeEndSignal data)
    {
        _lock = false;
    }
}