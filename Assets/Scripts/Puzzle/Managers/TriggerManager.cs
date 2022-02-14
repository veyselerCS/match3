using DG.Tweening;
using Zenject;

public class TriggerManager : Manager
{
    [Inject] private SignalBus _signalBus;
    private BoardManager _boardManager;
    public override void Init()
    {
        _boardManager = _managerProvider.Get<BoardManager>();

        _dependencies.Add(_boardManager);
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
            if (square.BoardElement != null)
            {
                if (square.BoardElement is PowerUp powerUp)
                {
                    if(powerUp.Activated) continue;
                    
                    powerUp.Activate();
                }
                else
                {
                    if (square.BoardElement.Triggers.Contains(data.TriggerType))
                    {
                        square.BoardElement.BackToPool();
                        square.BoardElement = null;  
                    }
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