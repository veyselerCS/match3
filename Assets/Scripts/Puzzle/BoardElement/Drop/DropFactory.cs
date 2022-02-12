using System.Collections.Generic;
using UnityEngine;

public class DropFactory : Manager
{
    [SerializeField] private Drop BlueDropPrefab;
    [SerializeField] private Drop GreenDropPrefab;
    [SerializeField] private Drop RedDropPrefab;
    [SerializeField] private Drop YellowDropPrefab;

    [SerializeField] private int PoolSize = 75;
    [SerializeField] private Transform PoolParent;

    private SceneComponentService _sceneComponentService;

    private Dictionary<DropType, Drop> _dropDictionary = new Dictionary<DropType, Drop>();
    private Dictionary<DropType, List<Drop>> _dropPool = new Dictionary<DropType, List<Drop>>();

    public override void Init()
    {
        _sceneComponentService = _managerProvider.Get<SceneComponentService>();
        _dependencies.Add(_sceneComponentService);
    }

    public override void Begin()
    {
        _dropDictionary = new Dictionary<DropType, Drop>()
        {
            { DropType.Blue, BlueDropPrefab },
            { DropType.Green, GreenDropPrefab },
            { DropType.Red, RedDropPrefab },
            { DropType.Yellow, YellowDropPrefab }
        };    
        
        _dropPool = new Dictionary<DropType, List<Drop>>()
        {
            { DropType.Blue, new List<Drop>() },
            { DropType.Green, new List<Drop>() },
            { DropType.Red, new List<Drop>()},
            { DropType.Yellow,new List<Drop>()}
        };

        foreach (var dropPair in _dropDictionary)
        {
            for (int i = 0; i < PoolSize; i++)
            {
                var drop = Instantiate(dropPair.Value, PoolParent.transform);
                drop.gameObject.SetActive(false);
                _dropPool[dropPair.Key].Add(drop);
            }
        }
        
        SetReady();
    }

    public void BackToPool(Drop drop)
    {
        drop.gameObject.SetActive(false);
        drop.transform.SetParent(PoolParent);
        _dropPool[drop.DropType].Add(drop);
    }
    
    public Drop GetDropByDropType(DropType dropType)
    {
        var drop = _dropPool[dropType].TryDequeue(null);
        
        if (drop == null)
        {
            drop = Instantiate(_dropDictionary[dropType]);
        }
        
        drop.transform.SetParent(_sceneComponentService.BoardElementParent.transform);
        return drop;
    }
}