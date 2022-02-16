public class Obstacle : BoardElement
{
    private ObstacleFactory _obstacleFactory;
    
    public ObstacleType ObstacleType;
    
    private void Start()
    {
        _obstacleFactory = ManagerProvider.Instance.Get<ObstacleFactory>();
    }

    public override void BackToPool()
    {
        _obstacleFactory.BackToPool(this);
    }
}