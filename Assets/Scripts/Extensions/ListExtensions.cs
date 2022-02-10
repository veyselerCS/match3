using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions 
{
    public static bool IsEmpty<T>(this List<T> list)
    {
        return list.Count == 0;
    }

    public static T At<T>(this List<List<T>> list, Vector2Int pos)
    {
        return list[pos.x][pos.y];
    }
}