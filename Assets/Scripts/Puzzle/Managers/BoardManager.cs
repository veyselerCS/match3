using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : Manager
{
    private DropFactory _dropFactory;
    private ObstacleFactory _obstacleFactory;
    private SceneComponentService _sceneComponentService;
    private PuzzleLoadManager _puzzleLoadManager;
    
    [HideInInspector]
    public int BoardWidth; 
    [HideInInspector]
    public int BoardHeight;
    
    public List<List<Square>> Board = new List<List<Square>>();
    public int MaxRight => BoardWidth - 1;
    public int MaxTop => BoardHeight - 1;
    
    private Vector2 _squareSize;
    private LevelData _levelData;

    public override void Init()
    {
        _dropFactory = _managerProvider.Get<DropFactory>();
        _obstacleFactory = _managerProvider.Get<ObstacleFactory>();
        _sceneComponentService = _managerProvider.Get<SceneComponentService>();
        _puzzleLoadManager = _managerProvider.Get<PuzzleLoadManager>();
        
       _dependencies.Add(_dropFactory);
       _dependencies.Add(_sceneComponentService);
       _dependencies.Add(_puzzleLoadManager);
       _dependencies.Add(_obstacleFactory);
    }

    public override void Begin()
    {
        _levelData = _puzzleLoadManager.LevelToLoad;

        BoardWidth = _levelData.GridWidth;
        BoardHeight = _levelData.GridHeight;
        
        _squareSize = _sceneComponentService.BoardSquarePrefab.RectTransform.sizeDelta;

        var boardRectTransform = _sceneComponentService.BoardParent.GetComponent<RectTransform>();
        var boardElementRectTransform = _sceneComponentService.BoardElementParent.GetComponent<RectTransform>();

        var boardRTSize = new Vector2(_squareSize.x * BoardWidth, _squareSize.y * BoardHeight);
        boardRectTransform.sizeDelta = boardRTSize;
        boardElementRectTransform.sizeDelta = boardRTSize;
        
        //init board
        InitBoard();
        SetReady();
    }

    private void InitBoard()
    {
        Board.Clear();
        var boardRectTransform = _sceneComponentService.BoardParent.GetComponent<RectTransform>();
        
        //create empty squares
        Vector3 boardOffset = boardRectTransform.sizeDelta / 2 * boardRectTransform.lossyScale;
        for (int i = 0; i < BoardHeight; i++)
        {
            Board.Add(new List<Square>());
            for (int k = 0; k < BoardWidth; k++)
            {
                Square square = Instantiate(_sceneComponentService.BoardSquarePrefab, _sceneComponentService.BoardParent.transform);
                var squareTransform = square.transform;
                var squareScreenPosition = (squareTransform.position);
                var lossyScale = square.RectTransform.lossyScale;
                
                var boardPosition =  squareScreenPosition - boardOffset + 
                                     new Vector3(_squareSize.x * (k + 1/2f) * lossyScale.x , _squareSize.y * (i+ 1/2f) * lossyScale.y, 0);
                
                var squarePosition = new Vector2Int(i, k);
                
                squareTransform.position = boardPosition;
                square.Coordinates = squarePosition;
                square.CenterPosition = boardPosition;
                square.BoardElement = null;
                
                Board[i].Add(square);
            }
        }
        
        //add fall squares
        for (int i = 0; i < BoardHeight; i++)
        {
            Board.Add(new List<Square>());
            for (int k = 0; k < BoardWidth; k++)
            {
                Square square = Instantiate(_sceneComponentService.BoardSquarePrefab, _sceneComponentService.BoardParent.transform);
                var squareTransform = square.transform;
                var squareScreenPosition = (squareTransform.position);
                var lossyScale = square.RectTransform.lossyScale;
                
                var boardPosition =  squareScreenPosition - boardOffset + 
                                     new Vector3(_squareSize.x * (k + 1/2f) * lossyScale.x , _squareSize.y * (BoardHeight + i+ 1/2f) * lossyScale.y, 0);
                
                var squarePosition = new Vector2Int(i, k);
                squareTransform.position = boardPosition;
                square.Coordinates = squarePosition;
                square.CenterPosition = boardPosition;
                square.BoardElement = null;
                Board[BoardHeight + i].Add(square);
                square.gameObject.SetActive(false);
                //square
            }
        }
        
        //add drops
        foreach (var dropTypePositionPair in _levelData.Drops)
        {
            foreach (var position in dropTypePositionPair.Value)
            {
                var drop = _dropFactory.GetDropByDropType(dropTypePositionPair.Key);
                var square = Board.At(position);
                
                drop.transform.SetParent(_sceneComponentService.BoardElementParent.transform);
                drop.transform.position = square.CenterPosition;
                drop.SquarePosition = position;
                drop.gameObject.SetActive(true);
                square.BoardElement = drop;
            }
        }       
        
        //add obstacles
        foreach (var obstacleTypePositionPair in _levelData.Obstacles)
        {
            foreach (var position in obstacleTypePositionPair.Value)
            {
                var obstacle = _obstacleFactory.GetObstacleByObstacleType(obstacleTypePositionPair.Key);
                var square = Board.At(position);
                
                obstacle.transform.SetParent(_sceneComponentService.BoardElementParent.transform);
                obstacle.transform.position = square.CenterPosition;
                obstacle.SquarePosition = position;
                obstacle.gameObject.SetActive(true);
                square.BoardElement = obstacle;
            }
        }
    }

    public void ResetBoard()
    {
        for (int i = _sceneComponentService.BoardParent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(_sceneComponentService.BoardParent.transform.GetChild(i).gameObject);
        }
        
        for (int i = _sceneComponentService.BoardElementParent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(_sceneComponentService.BoardElementParent.transform.GetChild(i).gameObject);
        }
        
        InitBoard();
    }
    
    public void DeleteBoard()
    {
        for (int i = 0; i < BoardWidth; i++)
        {
            for (int k = 0; k < BoardHeight; k++)
            {
                if (Board[i][k].BoardElement != null)
                    Destroy(Board[i][k].BoardElement.gameObject);
            }
        }
    }

    public bool IsInBoardLimits(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < BoardHeight && pos.y < BoardWidth;
    }   
    
    public bool IsInBoardLimits(int x, int y)
    {
        return x >= 0 && y >= 0 && x < BoardHeight && y < BoardWidth;
    }
    
    private void OnDrawGizmos()
    {
        return;
        Color locked = new Color(0, 0, 0, 0.2f);
        Color nulled = new Color(0, 0, 255, 0.2f);
        Color fulled = new Color(255, 0, 0, 0.2f);

        Color color;
        for (int i = 0; i < Board.Count; i++)
        {
            for (int k = 0; k < Board[i].Count; k++)
            {
                var square = Board[i][k];
                if (square == null)
                {
                    color = nulled;
                    if(square.Locked) color = locked;
                    Rect rect = new Rect(square.CenterPosition.IncX(-50).IncY(-50), new Vector2(100,100));
                    Handles.DrawSolidRectangleWithOutline(rect, color, Color.white);
                }
                else
                {
                    color = fulled;
                    if(Board[i][k].Locked) color = locked;
                    Rect rect = new Rect(square.CenterPosition.IncX(-50).IncY(-50), new Vector2(100,100));
                    Handles.DrawSolidRectangleWithOutline(rect, color, Color.white);
                }
            }
        }
    }

    public Vector2Int GetBoardPosition(Vector3 pos)
    {
        var offSet = pos - _sceneComponentService.BoardElementParent.transform.position;
        var lossyScale = _sceneComponentService.BoardElementParent.transform.lossyScale;
        return new Vector2Int((int)((offSet.y / _squareSize.y) * (1f / lossyScale.y)),
            (int)((offSet.x / _squareSize.x) * (1f / lossyScale.x)));
    }
}