using System;
using UnityEngine;
using Zenject;

public class Drop : BoardElement
{
    [SerializeField] public DropType DropType;

    private DropFactory _dropFactory;

    private void Start()
    {
        _dropFactory = ManagerProvider.Instance.Get<DropFactory>();
    }

    public override void BackToPool()
    {
        _dropFactory.BackToPool(this);
    }
}