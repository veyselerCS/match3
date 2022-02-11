public class PowerUp : BoardElement
{
    public PowerUpType Type;
    
    private PowerUpFactory _powerUpFactory;

    private void Start()
    {
        _powerUpFactory = ManagerProvider.Instance.Get<PowerUpFactory>();
    }

    public override void BackToPool()
    {
        _powerUpFactory.BackToPool(this);
    }
}