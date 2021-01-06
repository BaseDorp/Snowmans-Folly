using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CurveMover2D : MonoBehaviour
{
    [SerializeField] private Transform toMove = null;
    [SerializeField] private bool alignToNormal = false;
    [SerializeField] private bool lockToCurve = false;

    [SerializeField] private PolynomialCurve2DInstance curve = null;

    public void MoveTo(float locationX)
    {
        if (lockToCurve)
            locationX.Clamp(curve.Left, curve.Right);
        transform.position = new Vector2
        {
            x = locationX,
            y = curve.ValueAt(locationX)
        };

        if (alignToNormal)
        {
            toMove.up = curve.NormalAt(locationX);
        }
    }
}
