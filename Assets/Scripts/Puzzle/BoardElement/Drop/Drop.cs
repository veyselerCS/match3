using System;
using UnityEngine;
using Zenject;

public class Drop : BoardElement
{
    [Inject] private SignalBus _signalBus;
    [SerializeField] public DropType DropType;
    
    private void Start()
    {
        _signalBus.Subscribe<BoardElementPopSignal>(OnBoardElementPop);
    }

    private void OnBoardElementPop(BoardElementPopSignal data)
    {
    }
}