using System.Collections.Generic;

public class TriggerSignal
{
    public List<Square> TriggeredSquares;
    public TriggerType TriggerType;

    public TriggerSignal(List<Square> triggeredSquares, TriggerType triggerType)
    {
        TriggeredSquares = triggeredSquares;
        TriggerType = triggerType;
    }
}