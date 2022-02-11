using System;
using UnityEditor;
using UnityEngine;
using Zenject;

public class InputManager : Manager
{
    [Inject] private SignalBus _signalBus;

    private BoardManager _boardManager;
    private CheatManager _cheatManager;
    private SceneComponentService _sceneComponentService;

    private Vector2 _squareSize;
    private Vector2Int _firstTouchedBoardPos;

    private bool _lock;
    private bool _swipeSent;
    
    public override void Init()
    {
        _boardManager = _managerProvider.Get<BoardManager>();
        _cheatManager = _managerProvider.Get<CheatManager>();
        _sceneComponentService = _managerProvider.Get<SceneComponentService>();
        
        _dependencies.Add(_boardManager);
        _dependencies.Add(_cheatManager);
        _dependencies.Add(_sceneComponentService);
    }
    
    public override void Begin()
    {
        _signalBus.Subscribe<SwipeEndSignal>(OnSwipeEndSignal);
        _squareSize = _sceneComponentService.BoardSquarePrefab.RectTransform.sizeDelta;
        SetReady();
    }

    private void Update()
    {
        if (_lock) return;

        if (_cheatManager.CheatMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _firstTouchedBoardPos = GetTouchBoardPosition();
                if (!_boardManager.IsInBoardLimits(_firstTouchedBoardPos)) return;

                var cheatElement = Instantiate(_cheatManager.PickedElement,
                    _sceneComponentService.BoardElementParent.transform);
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
        var offSet = touchPos - _sceneComponentService.BoardElementParent.transform.position;
        var lossyScale = _sceneComponentService.BoardElementParent.transform.lossyScale;
        return new Vector2Int((int)((offSet.y / _squareSize.y) * (1f / lossyScale.y)),
            (int)((offSet.x / _squareSize.x) * (1f / lossyScale.x)));
    }

    private void OnSwipeEndSignal(SwipeEndSignal data)
    {
        _lock = false;
    }
}