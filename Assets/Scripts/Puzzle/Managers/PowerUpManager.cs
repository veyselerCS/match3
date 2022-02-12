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
                    powerUp.SquarePosition = mergeDropSquare.Coordinates;
                });
            }
        }
    }

    private HashSet<Square> powerUpTriggers = new HashSet<Square>();
    public void ActivatePowerUp(Sequence sequence, List<PowerUp> powerUps)
    {
        powerUpTriggers.Clear();
        var board = _boardManager.Board;
        HashSet<PowerUp> activated = new HashSet<PowerUp>();
        List<Square> _triggered = new List<Square>();
        while (!powerUps.IsEmpty())
        {
            var powerUp = powerUps.Dequeue();
            if(activated.Contains(powerUp)) continue;
            
            activated.Add(powerUp);
            var triggerZone = powerUp.GetTriggerZone();
            foreach (var square in triggerZone)
            {
                powerUpTriggers.Add(square);
                if(!_triggered.Contains(square))
                    _triggered.Add(square);
                if (square.TryGetByType(out PowerUp nextPowerUp, null))
                {
                    powerUps.Add(nextPowerUp);
                }
            }
        }
        
        _signalBus.Fire(new TriggerSignal(_triggered));
    }

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
        var boardElement = board.At(data.On).BoardElement;
        if (boardElement != null && boardElement is PowerUp powerUp)
        {
            var sequence = DOTween.Sequence();
            ActivatePowerUp(sequence, new List<PowerUp>(){ powerUp});
        }
    }
}