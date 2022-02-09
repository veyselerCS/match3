using System;
using UnityEngine;

public class SingletonManager<T> : MonoBehaviour where T : SingletonManager<T>
{
    public static T Instance;

    private void Awake()
    {
        Instance = (T)this;
    }
}