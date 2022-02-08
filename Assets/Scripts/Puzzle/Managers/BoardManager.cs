using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private Square _boardSquarePrefab;
    [SerializeField] private GameObject _boardParent;
    [SerializeField] private GameObject _boardElementParent;
    
    [SerializeField] private RectTransform _boardRectTransform;
    [SerializeField] private RectTransform _boardElementParentRectTransform;
    
    private InputManager _inputManager;
    private DropFactory _dropFactory;
    
    public static BoardManager Instance;

    public List<List<Square>> Board = new List<List<Square>>();

    private Vector2 _squareSize;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //init references
        _inputManager = InputManager.Instance;
        _dropFactory = DropFactory.Instance;
        
        //set the size for grid layout
        _squareSize = _boardSquarePrefab.RectTransform.sizeDelta;
        
        _boardRectTransform.sizeDelta = _squareSize * 9;
        _boardElementParentRectTransform.sizeDelta = _squareSize * 9;
        
        //init board
        InitBoard();
        
        _inputManager.SetBoardSquareSize(_squareSize);
    }

    private void InitBoard()
    {
        Vector3 boardOffset = _boardRectTransform.sizeDelta / 2 * _boardRectTransform.lossyScale;
        for (int i = 0; i < 9; i++)
        {
            Board.Add(new List<Square>());
            for (int k = 0; k < 9; k++)
            {
                Square square = Instantiate(_boardSquarePrefab, _boardParent.transform);
                
                var dropPrefab = _dropFactory.GetDropByDropType((DropType)UnityEngine.Random.Range(0, 4));
                Drop drop = Instantiate(dropPrefab, _boardElementParent.transform);

                var squareTransform = square.transform;
                var squareScreenPosition = (squareTransform.position);
                var lossyScale = square.RectTransform.lossyScale;
                
                var boardPosition =  squareScreenPosition - boardOffset + 
                                     new Vector3(_squareSize.x * (k + 1/2f) * lossyScale.x , _squareSize.y * (i+ 1/2f) * lossyScale.y, 0);

                squareTransform.position = boardPosition;
                square.Coordinates = new Vector2Int(i, k);
                square.BoardElement = drop;
                
                drop.transform.position = boardPosition;

                Board[i].Add(square);
            }
        }
    }
}