using System.Collections.Generic;
using UnityEngine;

public abstract class BoardElement : MonoBehaviour
{
    public Vector2Int SquarePosition;
    public bool IsStable;
    public bool IsSwappable;
    public bool IsTappable;
    public List<TriggerType> Triggers;
    public abstract void BackToPool();
}