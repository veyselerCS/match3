public class Obstacle : BoardElement
{
    public ObstacleType ObstacleType;
    
    private ObstacleFactory _obstacleFactory;

    private void Start()
    {
        _obstacleFactory = ManagerProvider.Instance.Get<ObstacleFactory>();
    }

    public override void BackToPool()
    {
        _obstacleFactory.BackToPool(this);
    }
}