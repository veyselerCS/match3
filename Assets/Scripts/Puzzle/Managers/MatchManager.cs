using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
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
        _signalBus.Subscribe<FallEndSignal>(OnFallEndSignal);
    }

    private int spawnCount = 0;
    private bool spawning = false;

    private void OnSwipeEndSignal(SwipeEndSignal data)
    {
        CheckMatch();
    }

    private void OnFallEndSignal()
    {
        CheckMatch();
    }

    private HashSet<Vector2Int> _visited = new HashSet<Vector2Int>();
    
    Dictionary<int, int> xOccurences = new Dictionary<int, int>();
    Dictionary<int, int> yOccurences = new Dictionary<int, int>();
    List<Vector2Int> neighbourhood = new List<Vector2Int>();
    
    [Button("Check match")]
    private void CheckMatch()
    {
        var board = _boardManager.Board;
        _visited.Clear();
        for (int i = 0; i < board.Count; i++)
        {
            for (int k = 0; k < board[i].Count; k++)
            {
                var square = board[i][k];
                
                if(_visited.Contains(square.Coordinates)) continue;
                
                var boardElement = square.BoardElement;
                if (boardElement != null && boardElement is Drop drop)
                {
                    xOccurences.Clear();
                    yOccurences.Clear();
                    neighbourhood.Clear();

                    CheckNeighboursRec(square.Coordinates, xOccurences, yOccurences, neighbourhood, drop.DropType);
                    CheckPowerUp(xOccurences, yOccurences, neighbourhood);
                    foreach (var squareInNeighbourhood in neighbourhood)
                    {
                        _visited.Add(squareInNeighbourhood);
                    }
                }
            }
        }
        
        _signalBus.Fire<MatchEndSignal>();
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
        
        if(xCoord > _boardManager.BoardHeight - 1 || yCoord > _boardManager.BoardWidth - 1 || yCoord < 0 || xCoord < 0) return;
        if(neighbourhood.Contains(coordinates)) return;

        if (board[xCoord][yCoord].BoardElement != null && board[xCoord][yCoord].BoardElement is Drop drop && drop.DropType == dropType)
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

    private void CheckPowerUp(Dictionary<int, int> xOccurences, Dictionary<int, int> yOccurences, List<Vector2Int> neighbourhood)
    {
        var board = _boardManager.Board;

        bool popped;
        if (CheckSquareMatch(xOccurences, yOccurences))
        {
            foreach (var coordinate in neighbourhood)
            {
                var square = board[coordinate.x][coordinate.y];
                Destroy(square.BoardElement.gameObject);
                square.BoardElement = null;
            }
        }
        else if (CheckMatchVerticalByCount(xOccurences, yOccurences,4))
        {
            foreach (var coordinate in neighbourhood)
            {
                var square = board[coordinate.x][coordinate.y];
                Destroy(square.BoardElement.gameObject);
                square.BoardElement = null;
            }
        }
        else if (CheckMatchHorizontalByCount(xOccurences, yOccurences, 4))
        {
            foreach (var coordinate in neighbourhood)
            {
                var square = board[coordinate.x][coordinate.y];
                Destroy(square.BoardElement.gameObject);
                square.BoardElement = null;
            }
        }
        else if (CheckMatchVerticalByCount(xOccurences, yOccurences,3))
        {
            foreach (var coordinate in neighbourhood)
            {
                var square = board[coordinate.x][coordinate.y];
                Destroy(square.BoardElement.gameObject);
                square.BoardElement = null;
            }
        }
        else if (CheckMatchHorizontalByCount(xOccurences, yOccurences, 3))
        {
            foreach (var coordinate in neighbourhood)
            {
                var square = board[coordinate.x][coordinate.y];
                Destroy(square.BoardElement.gameObject);
                square.BoardElement = null;
            }
        }
    }
}