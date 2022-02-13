using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public Dictionary<DropType, List<Vector2Int>> Drops = new Dictionary<DropType, List<Vector2Int>>()
    {
        { DropType.Blue , new List<Vector2Int>()},
        { DropType.Red , new List<Vector2Int>()},
        { DropType.Green , new List<Vector2Int>()},
        { DropType.Yellow , new List<Vector2Int>()}
    };
    public Dictionary<ObstacleType, List<Vector2Int>> Obstacles = new Dictionary<ObstacleType, List<Vector2Int>>()
    {
        { ObstacleType.Box , new List<Vector2Int>()},
        { ObstacleType.Sugar , new List<Vector2Int>()},
        { ObstacleType.Crisp , new List<Vector2Int>()},
        { ObstacleType.Cake , new List<Vector2Int>()}
    };

    public int LevelNo;
    public int GridWidth;
    public int GridHeight;
    public int MoveCount;

    public LevelData(int levelNo, int gridWidth, int gridHeight, int moveCount)
    {
        LevelNo = levelNo;
        GridWidth = gridWidth;
        GridHeight = gridHeight;
        MoveCount = moveCount;
    }
}