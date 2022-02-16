using DG.Tweening;
using Zenject;

public class TriggerManager : Manager
{
    [Inject] private SignalBus _signalBus;
    private BoardManager _boardManager;
    private TargetManager _targetManager;
    public override void Init()
    {
        _boardManager = _managerProvider.Get<BoardManager>();
        _targetManager = _managerProvider.Get<TargetManager>();

        _dependencies.Add(_boardManager);
        _dependencies.Add(_targetManager);
    }

    public override void Begin()
    {
        _signalBus.Subscribe<TriggerSignal>(OnTrigger);
        SetReady();
    }

    public void OnTrigger(TriggerSignal data)
    {
        foreach (var square in data.TriggeredSquares)
        {
            var boardElement = square.BoardElement;
            if (boardElement != null && boardElement.Triggers.Contains(data.TriggerType))
            {
                if (boardElement is PowerUp powerUp)
                {
                    if(powerUp.Activated) continue;
                    
                    powerUp.Activate();
                }
                else if(boardElement is Obstacle obstacle)
                {
                    _targetManager.CheckTarget(square);
                }
                else
                {
                    square.BoardElement.BackToPool();
                    square.BoardElement = null;
                }
            }
        }
    }
}

public enum TriggerType
{
    NearMatch = 0,
    Special = 1
}