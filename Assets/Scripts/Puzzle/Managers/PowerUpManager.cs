using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PowerUpManager : SingletonManager<PowerUpManager>
{
    private BoardManager _boardManager;
    private PowerUpFactory _powerUpFactory;
    private SceneComponentService _sceneComponentService;
    
    private void Start()
    {
        _boardManager = BoardManager.Instance;
        _powerUpFactory = PowerUpFactory.Instance;
        _sceneComponentService = SceneComponentService.Instance;
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
                        Destroy(dropSquare.BoardElement.gameObject);
                        dropSquare.BoardElement = null;
                    }));
            }

            if (index == involvedPositions.Count - 1)
            {
                matchSequence.AppendCallback(() =>
                {
                    var powerUpPrefab = _powerUpFactory.GetPowerUpByPowerUpType(type);
                    var powerUp = Instantiate(powerUpPrefab, _sceneComponentService.BoardElementParent.transform);
                    powerUp.transform.position = mergeDropSquare.CenterPosition;
                    Destroy(mergeDropSquare.BoardElement.gameObject);
                    mergeDropSquare.BoardElement = powerUp;
                });
            }
        }
    }
}