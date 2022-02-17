using UnityEngine;

public static class RectTransformExtensions
{
    public static Vector3 GetBottomLeft(this RectTransform rectTransform)
    {
        var size = rectTransform.sizeDelta;
        var pivot = rectTransform.pivot;
        return rectTransform.position - new Vector3(pivot.x * size.x * rectTransform.lossyScale.x, pivot.y * size.y * rectTransform.lossyScale.y);
    }
}