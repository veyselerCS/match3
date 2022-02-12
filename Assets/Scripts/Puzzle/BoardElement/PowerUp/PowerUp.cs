using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class PowerUp : BoardElement
{
    [Inject] protected SignalBus _signalBus;
    
    public PowerUpType Type;
    
    protected PowerUpFactory _powerUpFactory;
    protected BoardManager _boardManager;

    public virtual void Start()
    {
        _powerUpFactory = ManagerProvider.Instance.Get<PowerUpFactory>();
        _boardManager = ManagerProvider.Instance.Get<BoardManager>();
    }

    public override void BackToPool()
    {
        _powerUpFactory.BackToPool(this);
    }

    public abstract void Activate();
    
    public abstract List<Square> GetTriggerZone();
}