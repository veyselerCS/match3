using DG.Tweening;
using UnityEngine;
using Zenject;

public class Spawner : BoardElement
{
    [Inject] private SignalBus _signalBus;
    [SerializeField] public DropType DropType;

    public Vector2Int BoardPos;

    private DropFactory _dropFactory;
    private BoardManager _boardManager;
    private SceneComponentService _sceneComponentService;
    
    private void Start()
    {
        _boardManager = BoardManager.Instance;
        _dropFactory = DropFactory.Instance;
        _sceneComponentService = SceneComponentService.Instance;

        _signalBus.Subscribe<BoardElementFallSignal>(OnBoardElementFall);
    }
    
    private void OnBoardElementFall(BoardElementFallSignal data)
    {
        var board = _boardManager.Board;
        var squareToFall = board[BoardPos.x - 1][BoardPos.y];
        if (squareToFall.BoardElement == null)
        {
            var dropPrefab = _dropFactory.GetDropByDropType((DropType)UnityEngine.Random.Range(0, 4));
            Drop drop = Instantiate(dropPrefab, _sceneComponentService.BoardElementParent.transform);
            drop.transform.position = squareToFall.transform.position + new Vector3(0, 50f, 0);
            squareToFall.BoardElement = drop;
            drop.transform.DOMove(squareToFall.CenterPosition, 0.15f).SetEase(Ease.InOutQuad).OnComplete(
                () =>
                {
                    _signalBus.Fire<BoardElementSpawnedSignal>();
                });
        }
    }
    
    private Square GetFirstAvailableSquareInColumn(int col)
    {
        for (int i = 0; i < 9; i++)
        {
            if (AvailableForFall(i, col))
            {
                return _boardManager.Board[i][col];
            }
        }

        return null;
    }
    
    private bool AvailableForFall(int x, int y)
    {
        var board = _boardManager.Board;
        if (x < 0) return false;

        return board[x][y].BoardElement == null;
    }
}