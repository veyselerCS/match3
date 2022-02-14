using System;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

public class Square : MonoBehaviour
{
    public BoardElement BoardElement;
    public Vector3 CenterPosition;
    public Vector2Int Coordinates;
    public bool Locked
    {
        get
        {
            if(_lockCount < 0) Debug.LogWarning("LOCK ERROR");
            return _lockCount != 0;
        }
    }

    private int _lockCount;
    public BoardManager _boardManager;
    public RectTransform RectTransform;
    
    public Square Up
    {
        get
        {
            if (_boardManager.IsInBoardLimits(Coordinates.x + 1, Coordinates.y))
            {
              return _boardManager.Board[Coordinates.x + 1][Coordinates.y];
            }
            return null;
        }
    }   
    
    public Square Down
    {
        get
        {
            if (_boardManager.IsInBoardLimits(Coordinates.x - 1, Coordinates.y))
            {
                return _boardManager.Board[Coordinates.x - 1][Coordinates.y];
            }
            return null;
        }
    }  
    
    public Square Left
    {
        get
        {
            if (_boardManager.IsInBoardLimits(Coordinates.x, Coordinates.y - 1))
            {
                return _boardManager.Board[Coordinates.x][Coordinates.y - 1];
            }
            return null;
        }
    }  
    public Square Right
    {
        get
        {
            if (_boardManager.IsInBoardLimits(Coordinates.x, Coordinates.y + 1))
            {
                return _boardManager.Board[Coordinates.x][Coordinates.y + 1];
            }
            return null;
        }
    } 
    
    private void Start()
    {
        _boardManager = ManagerProvider.Instance.Get<BoardManager>();
    }

    public bool TryGetByType<T>(out T value, T defaultValue) where T : BoardElement
    {
        value = defaultValue;
        if (BoardElement == null) return false;
        
        if (BoardElement is T casted)
        {
            value = casted;
            return true;
        }

        return false;
    }

    public List<Square> GetNearSquares()
    {
        return new List<Square> { Up, Down, Left, Right };
    }

    public void Lock()
    {
        _lockCount++;
    }   
    
    public void Unlock()
    {
        _lockCount--;
    }
}


