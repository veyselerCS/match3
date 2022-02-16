using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class QuadraticBezier : MonoBehaviour
{
    public GameObject P0, P1, PControl;
    public GameObject PTest;
    public Vector3 P0Pos, P1Pos, PControlPos;
    public Color color = Color.white;
    public float width = 0.2f;
    public float TTest = 0.2f;
    public int numberOfPoints = 20;
    
    LineRenderer lineRenderer;
    
    void Start () 
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));

        P0Pos = P0.transform.position;
        P1Pos = P1.transform.position;
        PControlPos = PControl.transform.position;
    }
    
    
    
    public Vector3 Value(float t)
    {
        return Mathf.Pow(1.0f - t, 2) * P0Pos
               + 2.0f * (1.0f - t) * t * PControlPos
               + Mathf.Pow(t, 2) * P1Pos;
    }

    public Vector3 Derivative(float t)
    {
        return (2 * (1 - t)) * P0.transform.position
               + (2 - (4 * t)) * PControl.transform.position
               + 2 * t * P1.transform.position;
    }


    [Button("Update")]
    private void test()
    {
        UpdateP2(TTest, PTest.transform.position);
    }   
    
    [Button("Reset")]
    private void ResetPos()
    {
        P0Pos = P0.transform.position;
        P1Pos = P1.transform.position;
        PControlPos = PControl.transform.position;
    }

    public void UpdateP2(float t, Vector3 newP1)
    {
        Vector3 value = Value(t);
        Vector3 derivative = Derivative(t);

        P0Pos = value;
        P1Pos = newP1;
        PControlPos = (derivative - (2 * t * newP1) + (2 * (1 - t) * value))/(2 - 4*t);
    }


    private List<Vector3> CachedPositions = new List<Vector3>();
    
    void Update () 
    {
        if( lineRenderer == null || P0 == null || P1 == null || PControl == null)
        {
            return; // no points specified
        }

        // update line renderer
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        if (numberOfPoints > 0)
        {
            lineRenderer.positionCount = numberOfPoints;
        }

        float t;
        for(int i = 0; i < numberOfPoints; i++)
        {
            t = i / (numberOfPoints - 1.0f);
            lineRenderer.SetPosition(i, Value(t));
        }
    }
}