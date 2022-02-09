using UnityEngine;

public class DropFactory : SingletonManager<DropFactory>
{
    [SerializeField] private Drop BlueDropPrefab;
    [SerializeField] private Drop GreenDropPrefab;
    [SerializeField] private Drop RedDropPrefab;
    [SerializeField] private Drop YellowDropPrefab;

    public Drop GetDropByDropType(DropType dropType)
    {
        switch (dropType)
        {
            case DropType.Blue:
                return BlueDropPrefab;        
            case DropType.Green:
                return GreenDropPrefab;        
            case DropType.Red:
                return RedDropPrefab;
            case DropType.Yellow:
                return YellowDropPrefab;
        }

        return null;
    }
}