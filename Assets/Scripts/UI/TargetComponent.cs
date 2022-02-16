using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetComponent : MonoBehaviour
{
    [SerializeField] public RectTransform RectTransform;
    [SerializeField] private TextMeshProUGUI RemainingAmountText;
    [SerializeField] public Image TargetImage;

    public int RemainingAmount;

    public void SetSprite(Sprite sprite)
    {
        TargetImage.sprite = sprite;
    }

    public void DecRemainingAmount()
    {
        RemainingAmount--;
        RemainingAmountText.text = RemainingAmount.ToString();
    }

    public void Init()
    {
        RemainingAmountText.text = RemainingAmount.ToString();
    }
}