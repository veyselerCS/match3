using System;
using UnityEditor;
using UnityEngine;
using Zenject;

public class InputManager : Manager
{
    [Inject] private SignalBus _signalBus;

    private BoardManager _boardManager;
    private CheatManager _cheatManager;
    private DropFactory _dropFactory;
    private ObstacleFactory _obstacleFactory;
    private PowerUpFactory _powerUpFactory;
    private SceneComponentService _sceneComponentService;

    private Vector2 _squareSize;
    private Vector2Int _firstTouchedBoardPos = new Vector2Int(-1, -1);

    private bool _lock;
    private bool _swipeSent;
    
    public override void Init()
    {
        _boardManager = _managerProvider.Get<BoardManager>();
        _cheatManager = _managerProvider.Get<CheatManager>();
        _dropFactory = _managerProvider.Get<DropFactory>();
        _obstacleFactory = _managerProvider.Get<ObstacleFactory>();
        _powerUpFactory = _managerProvider.Get<PowerUpFactory>();
        _sceneComponentService = _managerProvider.Get<SceneComponentService>();
        
        _dependencies.Add(_boardManager);
        _dependencies.Add(_cheatManager);
        _dependencies.Add(_dropFactory);
        _dependencies.Add(_powerUpFactory);
        _dependencies.Add(_obstacleFactory);
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
                var currentTouchPos = GetTouchBoardPosition();
                if (!_boardManager.IsInBoardLimits(currentTouchPos)) return;
                
                _firstTouchedBoardPos = GetTouchBoardPosition();
                var square = _boardManager.Board.At(_firstTouchedBoardPos);

                BoardElement cheatElement = null;
                if (_cheatManager.PickedElement is Drop drop)
                {
                    cheatElement = _dropFactory.GetDropByDropType(drop.DropType);
                }
                else if (_cheatManager.PickedElement is Obstacle obstacle)
                {
                    cheatElement = _obstacleFactory.GetObstacleByObstacleType(obstacle.ObstacleType);
                }          
                else if (_cheatManager.PickedElement is PowerUp powerUp)
                {
                    cheatElement = _powerUpFactory.GetPowerUpByPowerUpType(powerUp.Type);

                }
                cheatElement.transform.position = square.CenterPosition;
                cheatElement.gameObject.SetActive(true);
                
                if (square.BoardElement)
                    Destroy(square.BoardElement.gameObject);
                
                square.BoardElement = cheatElement;
                cheatElement.SquarePosition = square.Coordinates;
            }

            return;
        }
    
        if (Input.GetMouseButtonDown(0))
        {
            var currentTouchPos = GetTouchBoardPosition();
            if (!_boardManager.IsInBoardLimits(currentTouchPos)) return;
            
            _firstTouchedBoardPos = GetTouchBoardPosition();
        }

        if (Input.GetMouseButtonUp(0))
        {
            var currentTouchPos = GetTouchBoardPosition();
            if (currentTouchPos == _firstTouchedBoardPos)
            {
                _signalBus.Fire(new TapSignal(currentTouchPos));
            }
        }
        
        if (Input.GetMouseButton(0) && !_swipeSent)
        {
            var board = _boardManager.Board;
            var currentTouchPos = GetTouchBoardPosition();
            
            if (!_boardManager.IsInBoardLimits(currentTouchPos)) return;
            if(board.At(_firstTouchedBoardPos).Locked || board.At(currentTouchPos).Locked) return;
            
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
        return _boardManager.GetBoardPosition(touchPos);
    }

    private void OnSwipeEndSignal(SwipeEndSignal data)
    {
        _lock = false;
    }
}