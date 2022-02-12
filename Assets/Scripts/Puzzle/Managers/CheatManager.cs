using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CheatManager : Manager
{
    [SerializeField] private List<BoardElement> BoardElements;
    [SerializeField] private List<GameObject> BoardElementsParents;
    [SerializeField] private Toggle CheatToggle;
    [SerializeField] private GameObject CheatParent;

    private BoardManager _boardManager;
    private MatchManager _matchManager;
    
    public bool CheatMode;
    public BoardElement PickedElement;
    public override void Init()
    {
        _boardManager = _managerProvider.Get<BoardManager>();
        _matchManager = _managerProvider.Get<MatchManager>();
        
        _dependencies.Add(_boardManager);
        _dependencies.Add(_matchManager);
    }

    public override void Begin()
    {
        foreach (var BoardElementsParent in  BoardElementsParents)
        {
            var boardElements = BoardElementsParent.GetComponentsInChildren<BoardElement>().ToList();
            foreach (var boardElement in boardElements)
            {
                Button button;
                (button = boardElement.gameObject.GetComponent<Button>()).onClick.AddListener(() =>
                {
                    PickedElement = boardElement;
                });
                BoardElements.Add(boardElement);
            }
        }
        
        CheatParent.SetActive(true);
        PickedElement = BoardElements[0];
        SetReady();
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.C))
        {
            CheatMode = !CheatMode;
        }       
        
        if(Input.GetKeyUp(KeyCode.D))
        {
            _boardManager.DeleteBoard();
        }      
        
        if(Input.GetKeyUp(KeyCode.M))
        {
            _matchManager.CheckMatch();
        }

        CheatToggle.isOn = CheatMode;
    }
}