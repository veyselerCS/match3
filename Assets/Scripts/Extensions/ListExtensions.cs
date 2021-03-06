using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ListExtensions 
{
    public static bool IsEmpty<T>(this List<T> list)
    {
        return list.Count == 0;
    }

    public static T GetRandomElement<T>(this List<T> list)
    {
        var randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }
    
    public static T At<T>(this List<List<T>> list, Vector2Int pos)
    {
        return list[pos.x][pos.y];
    }
    
    public static T TryPop<T>(this List<T> list, T defaultValue)
    {
        if (list.Count == 0) return defaultValue;

        defaultValue = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        
        return defaultValue;
    }
    
    public static T Dequeue<T>(this List<T> list)
    {
        var element = list[0];
        list.RemoveAt(0);
        
        return element;
    }
    
    public static T TryDequeue<T>(this List<T> list, T defaultValue)
    {
        if (list.Count == 0) return defaultValue;

        defaultValue = list[0];
        list.RemoveAt(0);
        
        return defaultValue;
    }
}