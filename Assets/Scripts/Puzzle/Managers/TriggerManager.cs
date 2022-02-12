using DG.Tweening;
using Zenject;

public class TriggerManager : Manager
{
    [Inject] private SignalBus _signalBus;
    private BoardManager _boardManager;
    private Sequence _triggerSequence;

    public override void Init()
    {
        _boardManager = _managerProvider.Get<BoardManager>();

        _dependencies.Add(_boardManager);
    }

    public override void Begin()
    {
        _signalBus.Subscribe<TriggerSignal>(OnTrigger);
        _triggerSequence = null;
        SetReady();
    }

    public void OnTrigger(TriggerSignal data)
    {
        if (_triggerSequence == null) _triggerSequence = DOTween.Sequence();

        foreach (var square in data.TriggeredSquares)
        {
            if (square.BoardElement == null) return;

            if (square.TryGetByType(out PowerUp powerUp, null))
            {
                powerUp.OnActivated(_triggerSequence);
            }

            if (square.BoardElement != null)
            {
                square.BoardElement.BackToPool();
                square.BoardElement = null;
            }
            _triggerSequence.AppendCallback(() =>
            {

            });
        }

        _triggerSequence.OnComplete(() =>
        {
            _triggerSequence = null;
            _signalBus.Fire<MatchEndSignal>();
        });
    }
}

public enum TriggerType
{
    NearMatch = 0,
    Special = 1
}