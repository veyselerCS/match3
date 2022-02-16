using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class PowerUp : BoardElement
{
    [Inject] protected SignalBus _signalBus;
    
    public bool Activated = false;
    public PowerUpType Type;
    
    protected BoardManager _boardManager;
    protected PowerUpFactory _powerUpFactory;
    protected PowerUpManager _powerUpManager;
    protected TargetManager _targetManager;

    public virtual void Start()
    {
        _boardManager = ManagerProvider.Instance.Get<BoardManager>();
        _powerUpFactory = ManagerProvider.Instance.Get<PowerUpFactory>();
        _powerUpManager = ManagerProvider.Instance.Get<PowerUpManager>();
        _targetManager = ManagerProvider.Instance.Get<TargetManager>();
    }

    public override void BackToPool()
    {
        Activated = false;
        _powerUpFactory.BackToPool(this);
    }

    public abstract void Activate();
    
    public abstract List<Square> GetTriggerZone();
}