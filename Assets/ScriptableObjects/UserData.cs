using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "UserData", menuName = "User Data", order = 2)]
public class UserData : ScriptableObject
{
    public int MaxLevel;
    public bool MaxLevelShown => _maxLevelShown == 1;
    
    public int _maxLevelShown;

    private void Awake()
    {
        ReadData();
    }

    private void UpdateData()
    {
        PlayerPrefs.SetInt("MaxLevel", MaxLevel);
        PlayerPrefs.SetInt("MaxLevelShown", _maxLevelShown);
    }

    private void ReadData()
    {
        if (PlayerPrefs.HasKey("MaxLevel"))
        {
            MaxLevel = PlayerPrefs.GetInt("MaxLevel");
            _maxLevelShown = PlayerPrefs.GetInt("MaxLevelShown");
            return;
        }

        MaxLevel = 1;
        _maxLevelShown = 0;
        
        PlayerPrefs.SetInt("MaxLevel", MaxLevel);
        PlayerPrefs.SetInt("MaxLevelShown", _maxLevelShown);
    }
}