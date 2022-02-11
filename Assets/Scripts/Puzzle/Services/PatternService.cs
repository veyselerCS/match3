using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PatternService : Manager
{
    public List<PatternShape> PatternShapes = new List<PatternShape>();

    [SerializeField] private int SelectedIndex = 0;

    public override void Init()
    {
    }

    public override void Begin()
    {
        //ordering is important
        PatternShapes = new List<PatternShape>()
        {
            TShape0Degrees(),
            TShape90Degrees(),
            TShape180Degrees(),
            TShape270Degrees(),
            Square(),
            Vertical4(),
            Horizontal4(),
            Vertical3(),
            Horizontal3()
        };
        SetReady();
    }

    void OnDrawGizmosSelected()
    {
        if(PatternShapes.Count == 0) return;
        
        foreach (var nonZero in PatternShapes[SelectedIndex].NonZeros)
        {      
            Camera camera = Camera.current;
            Vector3 centerPoint = camera.ViewportToScreenPoint(new Vector3(0.5f, 0.8f, 0.5f));
            
            Rect rect = new Rect(centerPoint.SetX(centerPoint.x + 100*nonZero.y).SetY(centerPoint.y + 100*nonZero.x), new Vector2(100,100));
            UnityEditor.Handles.DrawSolidRectangleWithOutline(rect, Color.black, Color.white);
        }
    }


    private PatternShape TShape0Degrees()
    {
        List<Vector2Int> nonZeros = new List<Vector2Int>();
        nonZeros.Add(new Vector2Int(0, 1));
        nonZeros.Add(new Vector2Int(1, 1));
        nonZeros.Add(new Vector2Int(2, 0));
        nonZeros.Add(new Vector2Int(2, 1));
        nonZeros.Add(new Vector2Int(2, 2));
        return new PatternShape(nonZeros, MatchResultType.TNT);
    }

    private PatternShape TShape90Degrees()
    {
        List<Vector2Int> nonZeros = new List<Vector2Int>();
        nonZeros.Add(new Vector2Int(0, 0));
        nonZeros.Add(new Vector2Int(1, 0));
        nonZeros.Add(new Vector2Int(2, 0));
        nonZeros.Add(new Vector2Int(1, 1));
        nonZeros.Add(new Vector2Int(1, 2));
        return new PatternShape(nonZeros, MatchResultType.TNT);
    }

    private PatternShape TShape180Degrees()
    {
        List<Vector2Int> nonZeros = new List<Vector2Int>();
        nonZeros.Add(new Vector2Int(0, 0));
        nonZeros.Add(new Vector2Int(0, 1));
        nonZeros.Add(new Vector2Int(0, 2));
        nonZeros.Add(new Vector2Int(1, 1));
        nonZeros.Add(new Vector2Int(2, 1));
        return new PatternShape(nonZeros, MatchResultType.TNT);
    }

    private PatternShape TShape270Degrees()
    {
        List<Vector2Int> nonZeros = new List<Vector2Int>();
        nonZeros.Add(new Vector2Int(1, 0));
        nonZeros.Add(new Vector2Int(1, 1));
        nonZeros.Add(new Vector2Int(1, 2));
        nonZeros.Add(new Vector2Int(0, 2));
        nonZeros.Add(new Vector2Int(2, 2));
        return new PatternShape(nonZeros, MatchResultType.TNT);
    }

    private PatternShape Square()
    {
        List<Vector2Int> nonZeros = new List<Vector2Int>();
        nonZeros.Add(new Vector2Int(0, 0));
        nonZeros.Add(new Vector2Int(0, 1));
        nonZeros.Add(new Vector2Int(1, 0));
        nonZeros.Add(new Vector2Int(1, 1));
        return new PatternShape(nonZeros, MatchResultType.Propeller);
    }

    private PatternShape Vertical4()
    {
        List<Vector2Int> nonZeros = new List<Vector2Int>();
        nonZeros.Add(new Vector2Int(0, 0));
        nonZeros.Add(new Vector2Int(1, 0));
        nonZeros.Add(new Vector2Int(2, 0));
        nonZeros.Add(new Vector2Int(3, 0));
        return new PatternShape(nonZeros, MatchResultType.VerticalRocket);
    }

    private PatternShape Vertical3()
    {
        List<Vector2Int> nonZeros = new List<Vector2Int>();
        nonZeros.Add(new Vector2Int(0, 0));
        nonZeros.Add(new Vector2Int(1, 0));
        nonZeros.Add(new Vector2Int(2, 0));
        return new PatternShape(nonZeros, MatchResultType.DropPop);
    }

    private PatternShape Horizontal4()
    {
        List<Vector2Int> nonZeros = new List<Vector2Int>();
        nonZeros.Add(new Vector2Int(0, 0));
        nonZeros.Add(new Vector2Int(0, 1));
        nonZeros.Add(new Vector2Int(0, 2));
        nonZeros.Add(new Vector2Int(0, 3));
        return new PatternShape(nonZeros, MatchResultType.HorizontalRocket);
    }

    private PatternShape Horizontal3()
    {
        List<Vector2Int> nonZeros = new List<Vector2Int>();
        nonZeros.Add(new Vector2Int(0, 0));
        nonZeros.Add(new Vector2Int(0, 1));
        nonZeros.Add(new Vector2Int(0, 2));
        return new PatternShape(nonZeros, MatchResultType.DropPop);
    }
}

public class PatternShape
{
    public readonly List<Vector2Int> NonZeros;
    public readonly int PatternWidth;
    public readonly int PatternHeight;
    public readonly MatchResultType MatchResultType;

    public PatternShape(List<Vector2Int> nonZeros, MatchResultType matchResultType)
    {
        NonZeros = nonZeros;
        MatchResultType = matchResultType;

        int maxX = 0;
        int maxY = 0;
        foreach (var nonZero in NonZeros)
        {
            if (nonZero.x > maxX) maxX = nonZero.x;
            if (nonZero.y > maxY) maxY = nonZero.y;
        }

        PatternWidth = maxY;
        PatternHeight = maxX;
    }
}