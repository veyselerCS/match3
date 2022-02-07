using System;
using UnityEditor;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private BoardManager _boardManager;
    
    public static InputManager Instance;
    
    private Vector2 _squareSize;
    private Vector2 _firstTouchedBoardPos;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _boardManager = BoardManager.Instance;
    }

    public void SetBoardSquareSize(Vector2 size)
    {
        _squareSize = size;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _firstTouchedBoardPos = GetTouchBoardPosition();
            Debug.LogWarning("Touch x : " + _firstTouchedBoardPos.x + " y : " + _firstTouchedBoardPos.y);
        }

        if (Input.GetMouseButton(0))
        {
            var currentTouchPos = GetTouchBoardPosition();
            if (currentTouchPos != _firstTouchedBoardPos)
            {
                Debug.LogWarning("Touch x : " + currentTouchPos.x + " y : " + currentTouchPos.y);
            }
        }
    }

    private Vector2Int GetTouchBoardPosition()
    {
        var touchPos = Input.mousePosition;
        var offSet = touchPos - _boardManager.transform.position;
        var lossyScale = _boardManager.transform.lossyScale;
        return new Vector2Int( (int)((offSet.x / _squareSize.x) * (1f / lossyScale.x)), (int)((offSet.y / _squareSize.y) * (1f / lossyScale.y)));
    }
    
    private void OnDrawGizmosSelected()
    {
    }
}