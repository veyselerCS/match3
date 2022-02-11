using UnityEngine;

public abstract class BoardElement : MonoBehaviour
{
    public Vector2Int SquarePosition;
    public bool isStable;
    public bool isSwappable;

    public abstract void BackToPool();
}