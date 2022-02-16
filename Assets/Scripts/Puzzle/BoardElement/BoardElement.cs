using System.Collections.Generic;
using UnityEngine;

public abstract class BoardElement : MonoBehaviour
{
    public bool IsStable;
    public bool IsSwappable;
    
    public int PropellerTargetScore;
    
    public Vector2Int SquarePosition;
    
    public List<TriggerType> Triggers;
    
    public abstract void BackToPool();
}