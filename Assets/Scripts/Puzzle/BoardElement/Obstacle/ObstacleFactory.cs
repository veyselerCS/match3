using UnityEngine;

public class ObstacleFactory : SingletonManager<ObstacleFactory>
{
    [SerializeField] private Obstacle BoxObstaclePrefab;
    [SerializeField] private Obstacle SugarObstaclePrefab;
    [SerializeField] private Obstacle CrispObstaclePrefab;
    [SerializeField] private Obstacle CakeObstaclePrefab;

    public Obstacle GetObstacleByObstacleType(ObstacleType obstacleType)
    {
        switch (obstacleType)
        {
            case ObstacleType.Box:
                return BoxObstaclePrefab;
            case ObstacleType.Sugar:
                return SugarObstaclePrefab;        
            case ObstacleType.Crisp:
                return CrispObstaclePrefab;
            case ObstacleType.Cake:
                return CakeObstaclePrefab;
        }

        return null;
    }
}