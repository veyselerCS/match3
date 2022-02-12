using System.Collections.Generic;

public class TriggerSignal
{
    public List<Square> TriggeredSquares;
    public TriggerType Type;

    public TriggerSignal(List<Square> triggeredSquares, TriggerType type)
    {
        TriggeredSquares = triggeredSquares;
        Type = type;
    }
}