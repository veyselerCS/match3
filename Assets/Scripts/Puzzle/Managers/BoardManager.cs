using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : Manager
{
    private DropFactory _dropFactory;
    private SceneComponentService _sceneComponentService;

    public int BoardWidth = 9; 
    public int BoardHeight = 9;
    
    public List<List<Square>> Board = new List<List<Square>>();

    private Vector2 _squareSize;

    public int MaxRight => BoardWidth - 1;
    public int MaxTop => BoardHeight - 1;


    public override void Init()
    {
        _dropFactory = _managerProvider.Get<DropFactory>();
        _sceneComponentService = _managerProvider.Get<SceneComponentService>();
        
       _dependencies.Add(_dropFactory);
       _dependencies.Add(_sceneComponentService);
    }

    public override void Begin()
    {
        _squareSize = _sceneComponentService.BoardSquarePrefab.RectTransform.sizeDelta;

        var boardRectTransform = _sceneComponentService.BoardParent.GetComponent<RectTransform>();
        var boardElementRectTransform = _sceneComponentService.BoardElementParent.GetComponent<RectTransform>();
        
        boardRectTransform.sizeDelta = _squareSize * 9;
        boardElementRectTransform.sizeDelta = _squareSize * 9;
        
        //init board
        InitBoard();
        SetReady();
    }

    private void InitBoard()
    {
        var boardRectTransform = _sceneComponentService.BoardParent.GetComponent<RectTransform>();
        
        Vector3 boardOffset = boardRectTransform.sizeDelta / 2 * boardRectTransform.lossyScale;
        Board.Clear();
        for (int i = 0; i < BoardWidth; i++)
        {
            Board.Add(new List<Square>());
            for (int k = 0; k < BoardHeight; k++)
            {
                Square square = Instantiate(_sceneComponentService.BoardSquarePrefab, _sceneComponentService.BoardParent.transform);
                var squareTransform = square.transform;
                var squareScreenPosition = (squareTransform.position);
                var lossyScale = square.RectTransform.lossyScale;
                
                var drop = _dropFactory.GetDropByDropType((DropType)UnityEngine.Random.Range(0, 4));
                var boardPosition =  squareScreenPosition - boardOffset + 
                                     new Vector3(_squareSize.x * (k + 1/2f) * lossyScale.x , _squareSize.y * (i+ 1/2f) * lossyScale.y, 0);
                
                var squarePosition = new Vector2Int(i, k);
                squareTransform.position = boardPosition;
                square.Coordinates = squarePosition;
                square.BoardElement = drop;
                
                drop.transform.position = boardPosition;
                drop.SquarePosition = squarePosition;
                drop.gameObject.SetActive(true);
                Board[i].Add(square);
            }
        }

        for (int i = 0; i < BoardWidth; i++)
        {
            Board.Add(new List<Square>());
            for (int k = 0; k < BoardHeight; k++)
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
                Board[BoardWidth + i].Add(square);
                square.gameObject.SetActive(true);
                //square
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