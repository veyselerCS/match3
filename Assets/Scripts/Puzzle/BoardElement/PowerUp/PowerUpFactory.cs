using UnityEngine;

public class PowerUpFactory : SingletonManager<PowerUpFactory>
{
    [SerializeField] PowerUp VerticalRocketPrefab;
    [SerializeField] PowerUp HorizontalRocketPrefab;
    [SerializeField] PowerUp PropellerPrefab;
    [SerializeField] PowerUp TNTPrefab;

    public PowerUp GetPowerUpByPowerUpType(PowerUpType powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUpType.VerticalRocket:
                return VerticalRocketPrefab;    
            case PowerUpType.HorizontalRocket:
                return HorizontalRocketPrefab;    
            case PowerUpType.Propeller:
                return PropellerPrefab;     
            case PowerUpType.TNT:
                return TNTPrefab;
            default:
                return VerticalRocketPrefab;
        }
    }
}