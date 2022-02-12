public abstract class PowerUp : BoardElement
{
    public PowerUpType Type;
    
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

    public abstract void OnPowerUpActivated();
}