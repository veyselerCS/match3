using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CheatManager : SingletonManager<CheatManager>
{
    [SerializeField] private List<BoardElement> BoardElements;
    [SerializeField] private GameObject BoardElementsParent;
    [SerializeField] private Toggle CheatToggle;

    public bool CheatMode;
    public BoardElement PickedElement;
    private void Start()
    {
        BoardElements = BoardElementsParent.GetComponentsInChildren<BoardElement>().ToList();
        foreach (var boardElement in BoardElements)
        {
            Button button;
            (button = boardElement.gameObject.GetComponent<Button>()).onClick.AddListener(() =>
            {
                PickedElement = boardElement;
            });
        }

        PickedElement = BoardElements[0];
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.C))
        {
            CheatMode = !CheatMode;
        }

        CheatToggle.isOn = CheatMode;
    }
}