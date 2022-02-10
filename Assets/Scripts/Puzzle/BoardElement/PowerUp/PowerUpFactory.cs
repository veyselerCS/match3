using UnityEngine;

public class PowerUpFactory : SingletonManager<PowerUpFactory>
{
    [SerializeField] PowerUp VerticalRocketPrefab;

    public PowerUp GetPowerUpByPowerUpType(PowerUpType powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUpType.VerticalRocket:
                return VerticalRocketPrefab;        
        }

        return null;
    }
}