using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class PowerUpManager : Manager
{
    [Inject] private SignalBus _signalBus;
    private BoardManager _boardManager;
    private PowerUpFactory _powerUpFactory;
    private SceneComponentService _sceneComponentService;
    private List<PowerUp> powerUpQueue = new List<PowerUp>();

    public override void Init()
    {
        _boardManager = _managerProvider.Get<BoardManager>();
        _powerUpFactory = _managerProvider.Get<PowerUpFactory>();
        _sceneComponentService = _managerProvider.Get<SceneComponentService>();
        
        _dependencies.Add(_boardManager);
        _dependencies.Add(_powerUpFactory);
        _dependencies.Add(_sceneComponentService);
    }

    public override void Begin()
    {
        _signalBus.Subscribe<TapSignal>(OnTapSignal);
        
        SetReady();
    }

    public void CreatePowerUp(Sequence matchSequence, Vector2Int mergePosition, List<Vector2Int> involvedPositions, PowerUpType type)
    {
        var board = _boardManager.Board;
        var mergeDropSquare = board.At(mergePosition);

        for (var index = 0; index < involvedPositions.Count; index++)
        {
            var dropPosition = involvedPositions[index];
            if (dropPosition != mergePosition)
            {
                var dropSquare = board.At(dropPosition);
                matchSequence.Join(dropSquare.BoardElement.transform.DOMove(mergeDropSquare.CenterPosition, 0.1f)
                    .OnComplete(() =>
                    {
                        if (dropSquare.BoardElement != null)
                        {
                            dropSquare.BoardElement.BackToPool();
                            dropSquare.BoardElement = null;
                        }
                    }));
            }

            if (index == involvedPositions.Count - 1)
            {
                matchSequence.AppendCallback(() =>
                {
                    PowerUp powerUp = _powerUpFactory.GetPowerUpByPowerUpType(type);
                    powerUp.transform.position = mergeDropSquare.CenterPosition;
                    powerUp.gameObject.SetActive(true);
                    mergeDropSquare.BoardElement.BackToPool();
                    mergeDropSquare.BoardElement = powerUp;
                    powerUp.SquarePosition = mergeDropSquare.Coordinates;

                    foreach (var square in involvedPositions)
                    {
                       board.At(square).Unlock();
                    }
                });
            }
        }
    }

    private HashSet<Square> powerUpTriggers = new HashSet<Square>();

    private void OnDrawGizmosSelected()
    {
        foreach (var square in powerUpTriggers)
        {      
            Rect rect = new Rect(square.CenterPosition - new Vector3(50,50),new Vector2(100,100));
            UnityEditor.Handles.DrawSolidRectangleWithOutline(rect, Color.black, Color.white);
        }
    }

    private void OnTapSignal(TapSignal data)
    {
        var board = _boardManager.Board;
        var square = board.At(data.On);
        var boardElement = square.BoardElement;
        if (boardElement != null && !square.Locked && boardElement is PowerUp powerUp)
        {
            powerUp.Activate();
        }
    }
}