using System;
using UnityEngine;

public class SceneComponentService : MonoBehaviour
{
    public GameObject BoardParent;
    public GameObject BoardElementParent;

    public float spawner = 0.3f;
    public float fall = 0.5f;
    public static SceneComponentService Instance;

    private void Awake()
    {
        Instance = this;
    }
}