using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "LevelRawData", menuName = "Level Raw Data", order = 1)]
public class LevelRawData : ScriptableObject
{
    public string LevelNo;
    public string GridWidth;
    public string GridHeight;
    public string MoveCount;
    public string Squares;
}