using UnityEngine;

public class DropFactory : MonoBehaviour
{
    [SerializeField] private Drop BlueDropPrefab;
    [SerializeField] private Drop GreenDropPrefab;
    [SerializeField] private Drop RedDropPrefab;
    [SerializeField] private Drop YellowDropPrefab;

    public static DropFactory Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Drop GetDropByDropType(DropType dropType)
    {
        switch (dropType)
        {
            case DropType.Blue:
                var blueDrop = Instantiate(BlueDropPrefab);
                return blueDrop;        
            case DropType.Green:
                var greenDrop = Instantiate(GreenDropPrefab);
                return greenDrop;        
            case DropType.Red:
                var redDrop = Instantiate(RedDropPrefab);
                return redDrop;
            case DropType.Yellow:
                var yellowDrop = Instantiate(YellowDropPrefab);
                return yellowDrop;
        }

        return null;
    }
}