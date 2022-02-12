using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class PowerUpManager : Manager
{
    [Inject] private SignalBus _signalBus;
    private BoardManager _boardManager;
    private PowerUpFactory _powerUpFactory;
    private SceneComponentService _sceneComponentService;
    
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
                matchSequence.Join(dropSquare.BoardElement.transform.DOMove(mergeDropSquare.CenterPosition, 0.25f)
                    .OnComplete(() =>
                    {
                        dropSquare.BoardElement.BackToPool();
                        dropSquare.BoardElement = null;
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
                });
            }
        }
    }

    private void OnTapSignal(TapSignal data)
    {
        var board = _boardManager.Board;
        var boardElement = board.At(data.On).BoardElement;
        if (boardElement != null && boardElement is PowerUp powerUp)
        {
            Debug.LogWarning("Tap on powerup");
        }
    }
}