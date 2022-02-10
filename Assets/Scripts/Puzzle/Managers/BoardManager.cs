using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : SingletonManager<BoardManager>
{
    [SerializeField] private Square _boardSquarePrefab;
    
    private InputManager _inputManager;
    private DropFactory _dropFactory;
    private SceneComponentService _sceneComponentService;

    public int BoardWidth = 9; 
    public int BoardHeight = 9;
    
    public List<List<Square>> Board = new List<List<Square>>();

    private Vector2 _squareSize;
    

    private void Start()
    {
        //init references
        _inputManager = InputManager.Instance;
        _dropFactory = DropFactory.Instance;
        _sceneComponentService = SceneComponentService.Instance;
        
        //set the size for grid layout
        _squareSize = _boardSquarePrefab.RectTransform.sizeDelta;

        var boardRectTransform = _sceneComponentService.BoardParent.GetComponent<RectTransform>();
        var boardElementRectTransform = _sceneComponentService.BoardElementParent.GetComponent<RectTransform>();
        
        boardRectTransform.sizeDelta = _squareSize * 9;
        boardElementRectTransform.sizeDelta = _squareSize * 9;
        
        //init board
        InitBoard();
        
        _inputManager.SetBoardSquareSize(_squareSize);
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
                Square square = Instantiate(_boardSquarePrefab, _sceneComponentService.BoardParent.transform);
                var squareTransform = square.transform;
                var squareScreenPosition = (squareTransform.position);
                var lossyScale = square.RectTransform.lossyScale;
                
                var dropPrefab = _dropFactory.GetDropByDropType((DropType)UnityEngine.Random.Range(0, 4));
                Drop drop = Instantiate(dropPrefab, _sceneComponentService.BoardElementParent.transform);

                var boardPosition =  squareScreenPosition - boardOffset + 
                                     new Vector3(_squareSize.x * (k + 1/2f) * lossyScale.x , _squareSize.y * (i+ 1/2f) * lossyScale.y, 0);
                
                var squarePosition = new Vector2Int(i, k);
                squareTransform.position = boardPosition;
                square.Coordinates = squarePosition;
                square.BoardElement = drop;
                
                drop.transform.position = boardPosition;
                drop.SquarePosition = squarePosition;
                Board[i].Add(square);
            }
        }

        for (int i = 0; i < BoardWidth; i++)
        {
            Board.Add(new List<Square>());
            for (int k = 0; k < BoardHeight; k++)
            {
                Square square = Instantiate(_boardSquarePrefab, _sceneComponentService.BoardParent.transform);
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
                square.gameObject.SetActive(false);
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
    
    private void OnDrawGizmos()
    {
        for (int i = 0; i < Board.Count; i++)
        {
            for (int k = 0; k < Board[i].Count; k++)
            {
                if (Board[i][k].BoardElement == null)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(Board[i][k].transform.position, Board[i][k].transform.position + new Vector3(25,25,0));
                }
                else
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(Board[i][k].transform.position, Board[i][k].transform.position + new Vector3(25,25,0));
                }
            }
        }
    }
}