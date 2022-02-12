using System.Collections.Generic;

public class TriggerSignal
{
    public List<Square> TriggeredSquares;

    public TriggerSignal(List<Square> triggeredSquares)
    {
        TriggeredSquares = triggeredSquares;
    }
}