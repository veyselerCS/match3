using UnityEngine;

public class SwipeFailSignal
{
    public Vector2Int From;
    public Vector2Int To;

    public SwipeFailSignal(Vector2Int from, Vector2Int to)
    {
        From = from;
        To = to;
    }
}