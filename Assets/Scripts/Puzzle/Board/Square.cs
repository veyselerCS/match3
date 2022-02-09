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

    private void Start()
    {
        _boardManager = BoardManager.Instance;
        
        CenterPosition = transform.position;
        
    }


}
