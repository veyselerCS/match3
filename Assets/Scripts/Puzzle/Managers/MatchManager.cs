using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MatchManager : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    
    private BoardManager _boardManager;

    public static MatchManager Instance; 
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _boardManager = BoardManager.Instance;
        _signalBus.Subscribe<SwipeEndSignal>(OnSwipeEndSignal);
    }
    
    private void OnSwipeEndSignal(SwipeEndSignal data)
    {
        var board = _boardManager.Board;

        Dictionary<int, int> xOccurences = new Dictionary<int, int>();
        Dictionary<int, int> yOccurences = new Dictionary<int, int>();
        List<Vector2Int> neighbourhood = new List<Vector2Int>();

        if (board[data.To.x][data.To.y].BoardElement is Drop drop)
        {
            CheckNeighboursRec(data.To, xOccurences, yOccurences, neighbourhood, drop.DropType);
        }

        if (CheckSquareMatch(xOccurences, yOccurences))
        {
            foreach (var coordinate in neighbourhood)
            {
                var square = board[coordinate.x][coordinate.y];
                Destroy(square.BoardElement.gameObject); 
            }
        }
        else if (CheckMatchVerticalByCount(xOccurences, yOccurences,4))
        {
            foreach (var coordinate in neighbourhood)
            {
                var square = board[coordinate.x][coordinate.y];
                Destroy(square.BoardElement.gameObject); 
            }
        }
        else if (CheckMatchHorizontalByCount(xOccurences, yOccurences, 4))
        {
            foreach (var coordinate in neighbourhood)
            {
                var square = board[coordinate.x][coordinate.y];
                Destroy(square.BoardElement.gameObject); 
            }
        }
        else if (CheckMatchVerticalByCount(xOccurences, yOccurences,3))
        {
            foreach (var coordinate in neighbourhood)
            {
                var square = board[coordinate.x][coordinate.y];
                Destroy(square.BoardElement.gameObject); 
            }
        }
        else if (CheckMatchHorizontalByCount(xOccurences, yOccurences, 3))
        {
            foreach (var coordinate in neighbourhood)
            {
                var square = board[coordinate.x][coordinate.y];
                Destroy(square.BoardElement.gameObject); 
            }
        }
    }

    private bool CheckSquareMatch(Dictionary<int, int> xOccurences, Dictionary<int, int> yOccurences)
    {
        if (xOccurences.Count == 2 && yOccurences.Count == 2)
        {
            return true;
        }

        return false;
    }

    private bool CheckMatchHorizontalByCount(Dictionary<int, int> xOccurences, Dictionary<int, int> yOccurences, int count)
    {
        if (yOccurences.Count >= count)
        {
            return true;
        }

        return false;
    }  
    
    private bool CheckMatchVerticalByCount(Dictionary<int, int> xOccurences, Dictionary<int, int> yOccurences, int count)
    {
        if (xOccurences.Count >= count)
        {
            return true;
        }

        return false;
    }
    
    private void CheckNeighboursRec(Vector2Int coordinates, Dictionary<int, int> xOccurences, Dictionary<int, int> yOccurences, List<Vector2Int> neighbourhood, DropType dropType)
    {
        var board = _boardManager.Board;
        
        int xCoord = coordinates.x;
        int yCoord = coordinates.y;
        
        if(xCoord > board.Count - 1 || yCoord > board.Count - 1 || yCoord < 0 || xCoord < 0) return;
        if(neighbourhood.Contains(coordinates)) return;

        if (board[xCoord][yCoord].BoardElement is Drop drop && drop.DropType == dropType)
        {
            xOccurences[xCoord] = xOccurences.ContainsKey(xCoord) ? xOccurences[xCoord] + 1 : 1;
            yOccurences[yCoord] = yOccurences.ContainsKey(yCoord) ? yOccurences[yCoord] + 1 : 1;
            neighbourhood.Add(coordinates);
            
            CheckNeighboursRec(new Vector2Int(xCoord + 1, yCoord), xOccurences, yOccurences, neighbourhood, dropType);
            CheckNeighboursRec(new Vector2Int(xCoord , yCoord + 1 ), xOccurences, yOccurences, neighbourhood, dropType);
            CheckNeighboursRec(new Vector2Int(xCoord - 1, yCoord), xOccurences, yOccurences, neighbourhood, dropType);
            CheckNeighboursRec(new Vector2Int(xCoord , yCoord - 1), xOccurences, yOccurences, neighbourhood, dropType);
        }
    }
}