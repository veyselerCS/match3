using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LevelEnterPopupItem : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;

    [SerializeField] private TextMeshProUGUI LevelText;
    [SerializeField] private TextMeshProUGUI MoveText;
    [SerializeField] private RectTransform GrayButtonRectTransform;
    [SerializeField] private Button GreenButton;

    private PuzzleLoadManager _puzzleLoadManager;
    private PopupManager _popupManager;
    
    private int _levelNo;
    private bool _shouldAnimate;

    private void Start()
    {
        _puzzleLoadManager = ManagerProvider.Instance.Get<PuzzleLoadManager>();
        _popupManager = ManagerProvider.Instance.Get<PopupManager>();
    }

    public void Init(int levelNo, int moveCount, bool shouldAnimate, bool alreadyActive)
    {
        _levelNo = levelNo;
        
        LevelText.text = levelNo.ToString();
        MoveText.text = moveCount.ToString();
        _shouldAnimate = shouldAnimate;

        GrayButtonRectTransform.gameObject.SetActive(!alreadyActive);
        GreenButton.onClick.AddListener(() =>
        {
            _popupManager.Hide("LevelEnterPopup");
            _puzzleLoadManager.LoadLevel(_levelNo);
        });
    }

    private void OnEnable()
    {
        if (_shouldAnimate)
        {
            GrayButtonRectTransform
                .DOSizeDelta(new Vector2(0, GrayButtonRectTransform.sizeDelta.y), 0.5f)
                .OnComplete(() => GrayButtonRectTransform.gameObject.SetActive(false))
                .SetLink(GrayButtonRectTransform.gameObject);
        }
    }
}