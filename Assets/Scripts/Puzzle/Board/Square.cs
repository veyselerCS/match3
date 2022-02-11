using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class Square : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;

    public BoardElement BoardElement;
    public Vector3 CenterPosition;
    public Vector2Int Coordinates;
    public bool Locked;

    public BoardManager _boardManager;
    public RectTransform RectTransform;

    public Square Up
    {
        get { return _boardManager.Board[Coordinates.x + 1][Coordinates.y]; }
    }   
    
    public Square Down
    {
        get { return _boardManager.Board[Coordinates.x - 1][Coordinates.y]; }
    }  
    
    public Square Left
    {
        get { return _boardManager.Board[Coordinates.x ][Coordinates.y - 1]; }
    }  
    public Square Right
    {
        get { return _boardManager.Board[Coordinates.x][Coordinates.y + 1]; }
    } 
    
    private void Start()
    {
        _boardManager = ManagerProvider.Instance.Get<BoardManager>();
        
        CenterPosition = transform.position;
    }
}


