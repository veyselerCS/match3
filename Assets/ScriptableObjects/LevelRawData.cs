using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "LevelRawData", menuName = "Level Raw Data", order = 1)]
public class LevelRawData : ScriptableObject
{
    public int LevelNo;
    public int GridWidth;
    public int GridHeight;
    public int MoveCount;
    public string Squares;
}