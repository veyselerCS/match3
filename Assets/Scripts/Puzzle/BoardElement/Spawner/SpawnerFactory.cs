using System;
using UnityEngine;

public class SpawnerFactory : MonoBehaviour
{
    [SerializeField] private Spawner SpawnerPrefab;

    public static SpawnerFactory Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Spawner GetSpawnerPrefab()
    {
        return SpawnerPrefab;
    }
}