using System.Collections.Generic;
using UnityEngine;

public class ObstacleFactory : Manager
{
    [SerializeField] private Obstacle BoxObstaclePrefab;
    [SerializeField] private Obstacle SugarObstaclePrefab;
    [SerializeField] private Obstacle CrispObstaclePrefab;
    [SerializeField] private Obstacle CakeObstaclePrefab;
    
    [SerializeField] private int PoolSize = 20;
    [SerializeField] private Transform PoolParent;

    private SceneComponentService _sceneComponentService;

    private Dictionary<ObstacleType, Obstacle> _obstacleDictionary = new Dictionary<ObstacleType, Obstacle>();
    private Dictionary<ObstacleType, List<Obstacle>> _obstaclePool = new Dictionary<ObstacleType, List<Obstacle>>();

    public override void Init()
    {
        _sceneComponentService = _managerProvider.Get<SceneComponentService>();
        _dependencies.Add(_sceneComponentService);
    }
    
    public override void Begin()
    {
        _obstacleDictionary = new Dictionary<ObstacleType, Obstacle>()
        {
            { ObstacleType.Box, BoxObstaclePrefab },
            { ObstacleType.Sugar, SugarObstaclePrefab },
            { ObstacleType.Crisp, CrispObstaclePrefab },
            { ObstacleType.Cake, CakeObstaclePrefab }
        };    
        
        _obstaclePool = new Dictionary<ObstacleType, List<Obstacle>>()
        {
            { ObstacleType.Box, new List<Obstacle>() },
            { ObstacleType.Sugar, new List<Obstacle>() },
            { ObstacleType.Crisp, new List<Obstacle>()},
            { ObstacleType.Cake,new List<Obstacle>()}
        };

        foreach (var obstaclePair in _obstacleDictionary)
        {
            for (int i = 0; i < PoolSize; i++)
            {
                var obstacle = Instantiate(obstaclePair.Value, PoolParent);
                obstacle.gameObject.SetActive(false);
                _obstaclePool[obstaclePair.Key].Add(obstacle);
            }
        }
        
        SetReady();
    }

    public void BackToPool(Obstacle obstacle)
    {
        obstacle.gameObject.SetActive(false);
        obstacle.transform.SetParent(PoolParent);
        _obstaclePool[obstacle.ObstacleType].Add(obstacle);
    }
    
    public Obstacle GetObstacleByObstacleType(ObstacleType obstacleType)
    {
        var obstacle = _obstaclePool[obstacleType].TryPop(null);
        
        if (obstacle == null)
        {
            obstacle = Instantiate(_obstacleDictionary[obstacleType], _sceneComponentService.BoardElementParent.transform);
        }
       
        obstacle.transform.SetParent(_sceneComponentService.BoardElementParent.transform);
        return obstacle;
    }
}