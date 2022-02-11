using System;
using UnityEngine;

public class SceneComponentService : Manager
{
    public GameObject BoardParent;
    public GameObject BoardElementParent;
    public Square BoardSquarePrefab;

    public override void Init()
    {
    }

    public override void Begin()
    {
        SetReady();
    }
}