using System;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject _boardSquarePrefab;
    [SerializeField] private GameObject _boardParent;
    [SerializeField] private RectTransform _boardRectTransform;

    private void Start()
    {
        //set the size for grid layout
        var boardSquare = Instantiate(_boardSquarePrefab, _boardParent.transform);
        var boardSquareRT = boardSquare.GetComponent<RectTransform>();

        _boardRectTransform.sizeDelta = boardSquareRT.sizeDelta * 9;
        Destroy(boardSquare);
        
        for (int i = 0; i < 9; i++)
        {
            for (int k = 0; k < 9; k++)
            {
                boardSquare = Instantiate(_boardSquarePrefab, _boardParent.transform);
                boardSquareRT = boardSquare.GetComponent<RectTransform>();
            }
        }
    }
}