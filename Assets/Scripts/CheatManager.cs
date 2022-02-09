using System;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            BoardManager.Instance.ResetBoard();
        }
    }
}