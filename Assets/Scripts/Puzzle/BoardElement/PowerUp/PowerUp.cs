using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : BoardElement
{
    public PowerUpType Type;
    public bool IsActivated;
    
    private PowerUpFactory _powerUpFactory;
    protected BoardManager _boardManager;

    private void Start()
    {
        _powerUpFactory = ManagerProvider.Instance.Get<PowerUpFactory>();
        _boardManager = ManagerProvider.Instance.Get<BoardManager>();
    }

    public override void BackToPool()
    {
        _powerUpFactory.BackToPool(this);
    }

    public abstract List<Square> GetTriggerZone();
}