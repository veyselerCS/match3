using System;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject _boardSquarePrefab;
    [SerializeField] private GameObject _boardParent;
    [SerializeField] private RectTransform _boardRectTransform;

    private InputManager _inputManager;
    
    public static BoardManager Instance;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //init references
        _inputManager = InputManager.Instance;
        
        //set the size for grid layout
        var boardSquare = Instantiate(_boardSquarePrefab, _boardParent.transform);
        var boardSquareRT = boardSquare.GetComponent<RectTransform>();
        var boardSquareSize = boardSquareRT.sizeDelta;
        
        _boardRectTransform.sizeDelta = boardSquareSize * 9;
        Destroy(boardSquare);
        
        //init board
        for (int i = 0; i < 9; i++)
        {
            for (int k = 0; k < 9; k++)
            {
                boardSquare = Instantiate(_boardSquarePrefab, _boardParent.transform);
            }
        }
        
        _inputManager.SetBoardSquareSize(boardSquareSize);
    }
}