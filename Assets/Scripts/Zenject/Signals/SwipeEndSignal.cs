using UnityEngine;

public class SwipeEndSignal
{
    public Vector2Int From;
    public Vector2Int To;

    public SwipeEndSignal(Vector2Int from, Vector2Int to)
    {
        From = from;
        To = to;
    }
}