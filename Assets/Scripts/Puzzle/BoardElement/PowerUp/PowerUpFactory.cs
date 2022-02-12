using System.Collections.Generic;
using UnityEngine;

public class PowerUpFactory : Manager
{
    [SerializeField] PowerUp VerticalRocketPrefab;
    [SerializeField] PowerUp HorizontalRocketPrefab;
    [SerializeField] PowerUp PropellerPrefab;
    [SerializeField] PowerUp TNTPrefab;

    [SerializeField] private int PoolSize = 20;
    [SerializeField] private Transform PoolParent;

    private SceneComponentService _sceneComponentService;

    private Dictionary<PowerUpType, PowerUp> _powerUpDictionary = new Dictionary<PowerUpType, PowerUp>();
    private Dictionary<PowerUpType, List<PowerUp>> _powerUpPool = new Dictionary<PowerUpType, List<PowerUp>>();

    public override void Init()
    {
        _sceneComponentService = ManagerProvider.Instance.Get<SceneComponentService>();

        _dependencies.Add(_sceneComponentService);
    }

    public override void Begin()
    {
        _powerUpDictionary = new Dictionary<PowerUpType, PowerUp>()
        {
            { PowerUpType.VerticalRocket, VerticalRocketPrefab },
            { PowerUpType.HorizontalRocket, HorizontalRocketPrefab },
            { PowerUpType.Propeller, PropellerPrefab },
            { PowerUpType.TNT, TNTPrefab }
        };

        _powerUpPool = new Dictionary<PowerUpType, List<PowerUp>>()
        {
            { PowerUpType.VerticalRocket, new List<PowerUp>() },
            { PowerUpType.HorizontalRocket, new List<PowerUp>() },
            { PowerUpType.Propeller, new List<PowerUp>() },
            { PowerUpType.TNT, new List<PowerUp>() }
        };

        foreach (var powerUpPair in _powerUpDictionary)
        {
            for (int i = 0; i < PoolSize; i++)
            {
                var powerUp = Instantiate(powerUpPair.Value, PoolParent.transform);
                powerUp.gameObject.SetActive(false);
                _powerUpPool[powerUpPair.Key].Add(powerUp);
            }
        }

        SetReady();
    }

    public void BackToPool(PowerUp powerUp)
    {
        powerUp.gameObject.SetActive(false);
        powerUp.transform.SetParent(PoolParent);
        _powerUpPool[powerUp.Type].Add(powerUp);
    }

    public PowerUp GetPowerUpByPowerUpType(PowerUpType powerUpType)
    {
        var powerUp = _powerUpPool[powerUpType].TryPop(null);

        if (powerUp == null)
        {
            powerUp = Instantiate(_powerUpDictionary[powerUpType],
                _sceneComponentService.BoardElementParent.transform);
        }

        powerUp.IsActivated = false;
        return powerUp;
    }
}