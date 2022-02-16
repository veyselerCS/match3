using System;
using UnityEngine;

public class SceneComponentService : Manager
{
    public RectTransform BoardParent;
    public RectTransform BoardElementParent;
    public RectTransform TargetParent;
    public Square BoardSquarePrefab;
    
    public override void Init()
    {
    }

    public override void Begin()
    {

        SetReady();
    }
}