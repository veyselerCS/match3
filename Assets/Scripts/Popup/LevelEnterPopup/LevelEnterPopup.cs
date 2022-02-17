using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelEnterPopup : BasePopup<LevelEnterPopup.Data>
{
    [SerializeField] private RectTransform LevelsParent;
    [SerializeField] private LevelEnterPopupItem LevelEnterPopupTemplate;

    private DataManager _dataManager;

    public override void Init()
    {
        _dataManager = ManagerProvider.Instance.Get<DataManager>();
    }

    public override void Show()
    {
        var maxLevel = _dataManager.UserData.MaxLevel;
        
        foreach (var level in PopupData.Levels)
        {
            LevelEnterPopupItem item = Instantiate(LevelEnterPopupTemplate, LevelsParent.transform);
            item.Init(level.LevelNo, level.MoveCount, level.LevelNo == maxLevel && !_dataManager.UserData.IsMaxLevelShown, level.LevelNo <= maxLevel);
            item.gameObject.SetActive(true);
        }
    }

    public class Data : BasePopupData
    {
        public List<LevelData> Levels;

        public Data(List<LevelData> levels)
        {
            Name = nameof(LevelEnterPopup);
            Levels = levels;
        }
    }
}