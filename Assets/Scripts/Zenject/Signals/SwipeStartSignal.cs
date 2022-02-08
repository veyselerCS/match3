using System.Numerics;
using UnityEngine;

public class SwipeStartSignal
{
    public Vector2Int From;
    public Vector2Int To;

    public SwipeStartSignal(Vector2Int from, Vector2Int to)
    {
        From = from;
        To = to;
    }
}