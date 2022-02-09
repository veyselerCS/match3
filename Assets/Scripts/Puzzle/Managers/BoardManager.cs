using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private Square _boardSquarePrefab;
    
    private InputManager _inputManager;
    private DropFactory _dropFactory;
    private SpawnerFactory _spawnerFactory;
    private SceneComponentService _sceneComponentService;
    
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
        _spawnerFactory = SpawnerFactory.Instance;
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
        for (int i = 0; i < 9; i++)
        {
            Board.Add(new List<Square>());
            for (int k = 0; k < 9; k++)
            {
                Square square = Instantiate(_boardSquarePrefab, _sceneComponentService.BoardParent.transform);
                var squareTransform = square.transform;
                var squareScreenPosition = (squareTransform.position);
                var lossyScale = square.RectTransform.lossyScale;
                
                var dropPrefab = _dropFactory.GetDropByDropType((DropType)UnityEngine.Random.Range(0, 4));
                Drop drop = Instantiate(dropPrefab, _sceneComponentService.BoardElementParent.transform);

                var boardPosition =  squareScreenPosition - boardOffset + 
                                     new Vector3(_squareSize.x * (k + 1/2f) * lossyScale.x , _squareSize.y * (i+ 1/2f) * lossyScale.y, 0);

                squareTransform.position = boardPosition;
                square.Coordinates = new Vector2Int(i, k);
                square.BoardElement = drop;
                
                drop.transform.position = boardPosition;

                Board[i].Add(square);
            }
        }

        for (int i = 0; i < 9; i++)
        {
            var spawnerPrefab = _spawnerFactory.GetSpawnerPrefab();
            Spawner spawner = Instantiate(spawnerPrefab, _sceneComponentService.BoardElementParent.transform);
            
            var spawnerTransform = spawner.transform;
            var pos = (spawnerTransform.position);
            var lossyScale = spawner.GetComponent<RectTransform>().lossyScale;
                
            var boardPosition =  pos - boardOffset + 
                                 new Vector3(_squareSize.x * (i + 1/2f) * lossyScale.x , _squareSize.y * (9+ 1/2f) * lossyScale.y, 0);
            
            spawner.transform.position = boardPosition;
            spawner.BoardPos = new Vector2Int(9, i);
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