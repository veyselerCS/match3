using System;
using UnityEngine;

public class SceneComponentService : MonoBehaviour
{
    public GameObject BoardParent;
    public GameObject BoardElementParent;
    
    public static SceneComponentService Instance;

    private void Awake()
    {
        Instance = this;
    }
}