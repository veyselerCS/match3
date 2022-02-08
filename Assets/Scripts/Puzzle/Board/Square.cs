using System;
using UnityEngine;

public class Square : MonoBehaviour
{
    public BoardElement BoardElement;
    public Vector3 CenterPosition;
    public Vector2Int Coordinates;
    public bool Locked;

    public RectTransform RectTransform;

    private void Start()
    {
        CenterPosition = transform.position;
    }
}
