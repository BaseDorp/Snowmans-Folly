using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SnowmansFolly.MonoBehaviours.Curvatures;

public enum TerrainSegmentType : byte
{
    Continuous,
    LeftCap,
    RightCap
}

public sealed class TerrainSegment : MonoBehaviour
{
    [SerializeField] private PolynomialSpline2DInstance surfaceCurve = null;
    [SerializeField] private TerrainSegmentType type = TerrainSegmentType.Continuous;

    public TerrainSegmentType SegmentType { get { return type; } }

    public PolynomialSpline2DInstance Curve { get { return surfaceCurve; } }


    public void SnapLeftTo(Vector2 position)
    {
        Vector2 currentPosition = new Vector2
        {
            x = surfaceCurve.Left,
            y = surfaceCurve.ValueAt(surfaceCurve.Left)
        };
        transform.position += (Vector3)(position - currentPosition);
    }

    public Vector2 DeltaPosition
    {
        get
        {
            return new Vector2
            {
                x = surfaceCurve.IntervalLength,
                y = surfaceCurve.ValueAt(surfaceCurve.Right)
                - surfaceCurve.ValueAt(surfaceCurve.Left)
            };
        }
    }
}