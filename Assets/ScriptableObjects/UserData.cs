using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "UserData", menuName = "User Data", order = 2)]
public class UserData : ScriptableObject
{
    public int MaxLevel;
    public int MaxLevelShown;
    
    public bool IsMaxLevelShown => MaxLevelShown == 1;
    
    private void Awake()
    {
        ReadData();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("MaxLevel", MaxLevel);
        PlayerPrefs.SetInt("MaxLevelShown", MaxLevelShown);
    }

    private void ReadData()
    {
        if (PlayerPrefs.HasKey("MaxLevel"))
        {
            MaxLevel = PlayerPrefs.GetInt("MaxLevel");
            MaxLevelShown = PlayerPrefs.GetInt("MaxLevelShown");
            return;
        }

        MaxLevel = 1;
        MaxLevelShown = 0;
        
        PlayerPrefs.SetInt("MaxLevel", MaxLevel);
        PlayerPrefs.SetInt("MaxLevelShown", MaxLevelShown);
    }
}