using System;
using System.Collections.Generic;
using UnityEngine;

#region Exposed Polynomial Enums
/// <summary>
/// Denotes a degree of polynomial function.
/// </summary>
public enum PolynomialDegree
{
    Linear,
    Cubic
}
#endregion

/// <summary>
/// Represents a scene instance of a polynomial curve.
/// </summary>
public sealed class PolynomialCurve2DInstance : MonoBehaviour, IValidateOnTransformChange
{
    /// <summary>
    /// The left world x position where the curve is meant to end.
    /// </summary>
    public float Left { get { return transform.position.x + curve.left; } }
    /// <summary>
    /// The right world x position where the curve is meant to end.
    /// </summary>
    public float Right { get { return transform.position.x + curve.right; } }

    [SerializeField]
    private PolynomialNode[] nodes = null;
    [Serializable]
    private struct PolynomialNode
    {
        public Transform transform;
        public PolynomialDegree type;
        public bool isStatic;
    }

    public float ValueAt(float worldX)
    {
        return curve.ValueAt(worldX - transform.position.x) + transform.position.y;
    }

    public Vector2 NormalAt(float worldX)
    {
        float slope = curve.SlopeAt(worldX - transform.position.x);
        return (new Vector2(-slope, 1f)).normalized;
    }


    [Header("Debug Parameters")]
    [SerializeField]
    private bool showPreview = true;
    [SerializeField]
    private float previewDetail = 0.5f;

#if DEBUG
    #region Gizmo Parameters
    private const float HANDLE_LENGTH = 1.0f;
    private readonly Color HANDLES_COLOR = Color.magenta;
    private readonly Color PREVIEW_COLOR = Color.yellow;
    #endregion
    #region Gizmo Drawing
    private void OnDrawGizmos()
    {
        // Do null checks to avoid warnings from curves
        // that are in the process of being assigned.
        if (showPreview && nodes != null)
        {
            if (rasterizedPath != null)
            {
                // Draw the rasterized path along the curve.
                Gizmos.color = PREVIEW_COLOR;
                for (int i = 1; i < rasterizedPath.Length; i++)
                    Gizmos.DrawLine(
                        (Vector2)transform.position + rasterizedPath[i],
                        (Vector2)transform.position + rasterizedPath[i - 1]);
            }
            // Draw tangent handles at each of the nodes along the curve.
            Gizmos.color = HANDLES_COLOR;
            foreach (PolynomialNode node in nodes)
            {
                Vector2 direction = new Vector2
                {
                    x = Mathf.Cos(Mathf.Deg2Rad * node.transform.eulerAngles.z),
                    y = Mathf.Sin(Mathf.Deg2Rad * node.transform.eulerAngles.z)
                };
                Gizmos.DrawLine(
                    (Vector2)node.transform.position + direction * HANDLE_LENGTH * 0.5f,
                    (Vector2)node.transform.position - direction * HANDLE_LENGTH * 0.5f);
            }
        }
    }
    #endregion
#endif

    private void FixedUpdate()
    {
        List<int> indicesToRegenerate = new List<int>();
        for (int i = 0; i < nodes.Length - 1; i++)
            if (!nodes[i].isStatic)
                indicesToRegenerate.Add(i);
        if (indicesToRegenerate.Count > 0)
            RegenerateCurve(indicesToRegenerate.ToArray());
    }


    private PolynomialCurve2D curve;
    private Vector2[] rasterizedPath;

    public void OnValidate()
    {
        // Prevent non-negative or zero precision.
        previewDetail.Clamp(0.1f, 5f);
        // Regenerate the curve and rasterized path
        // so that it can be displayed in the editor window.
        if (nodes.Length > 1)
        {
            RegenerateCurve();
            rasterizedPath = curve.Rasterize(previewDetail);
        }
    }

    #region Curve Regeneration Method
    /// <summary>
    /// Regenerates the entire curvature with the current transform data.
    /// </summary>
    public void RegenerateCurve()
    {
        Polynomial2D[] ranges = new Polynomial2D[nodes.Length - 1];
        for (int i = 0; i < nodes.Length - 1; i++)
            ranges[i] = Generate(i);
        curve = new PolynomialCurve2D(ranges);
#if DEBUG
        if (showPreview)
            rasterizedPath = curve.Rasterize(previewDetail);
#endif
    }
    /// <summary>
    /// Regenerates a single segment of the curvature given the transform data.
    /// </summary>
    /// <param name="curveIndex">The index of the left hand node.</param>
    public void RegenerateCurve(int curveIndex)
    {
        Polynomial2D[] ranges = curve.ranges;
        ranges[curveIndex] = Generate(curveIndex);
        curve = new PolynomialCurve2D(ranges);
#if DEBUG
        if (showPreview)
            rasterizedPath = curve.Rasterize(previewDetail);
#endif
    }
    /// <summary>
    /// Regenerates multiple segments of the curvature given the transform data.
    /// </summary>
    /// <param name="curveIndices">The indices of the left hand nodes.</param>
    public void RegenerateCurve(int[] curveIndices)
    {
        Polynomial2D[] ranges = curve.ranges;
        foreach (int index in curveIndices)
            ranges[index] = Generate(index);
        curve = new PolynomialCurve2D(ranges);
#if DEBUG
        if (showPreview)
            rasterizedPath = curve.Rasterize(previewDetail);
#endif
    }
    #endregion
    #region Polynomial Generation
    private Polynomial2D Generate(int segmentIndex)
    {
        // This method switches to generate and return the
        // appropriate polynomial segment.
        // All transforms are considered relative to this scripts root,
        // this is to allow nested transforms for relative animation of the nodes.
        switch (nodes[segmentIndex].type)
        {
            case PolynomialDegree.Linear:
                return GenerateLinear(
                    transform.InverseTransformPoint(nodes[segmentIndex].transform.position),
                    transform.InverseTransformPoint(nodes[segmentIndex + 1].transform.position));
            case PolynomialDegree.Cubic:
                return GenerateCubic(
                    transform.InverseTransformPoint(nodes[segmentIndex].transform.position),
                    transform.InverseTransformPoint(nodes[segmentIndex + 1].transform.position),
                    Mathf.Tan(nodes[segmentIndex].transform.eulerAngles.z * Mathf.Deg2Rad),
                    Mathf.Tan(nodes[segmentIndex + 1].transform.eulerAngles.z * Mathf.Deg2Rad));
            default:
                throw new NotImplementedException();
        }
    }
    private Polynomial2D GenerateLinear(Vector2 start, Vector2 end)
    {
        // Use matrices to solve the linear system.
        // TODO this should probably be abstracted as utility methods.
        Matrix2x2 system = new Matrix2x2
        {
            m00 = start.x,
            m01 = 1f,
            m10 = end.x,
            m11 = 1f
        };
        system = system.Inverse;
        return new Polynomial2D(start.x, end.x, new float[]
        {
            system.m10 * start.y + system.m11 * end.y,
            system.m00 * start.y + system.m01 * end.y
        });
    }
    private Polynomial2D GenerateCubic(Vector2 start, Vector2 end, float startSlope, float endSlope)
    {
        // Use matrices to solve the cubic system.
        // TODO this should probably be abstracted as utility methods.
        Matrix4x4 system = new Matrix4x4
        {
            m00 = start.x * start.x * start.x,
            m01 = start.x * start.x,
            m02 = start.x,
            m03 = 1f,
            m10 = end.x * end.x * end.x,
            m11 = end.x * end.x,
            m12 = end.x,
            m13 = 1f,
            m20 = 3f * start.x * start.x,
            m21 = 2f * start.x,
            m22 = 1f,
            m23 = 0f,
            m30 = 3f * end.x * end.x,
            m31 = 2f * end.x,
            m32 = 1f,
            m33 = 0f
        };
        system = system.inverse;
        return new Polynomial2D(start.x, end.x, new float[]
        {
            system.m30 * start.y + system.m31 * end.y + system.m32 * startSlope + system.m33 * endSlope,
            system.m20 * start.y + system.m21 * end.y + system.m22 * startSlope + system.m23 * endSlope,
            system.m10 * start.y + system.m11 * end.y + system.m12 * startSlope + system.m13 * endSlope,
            system.m00 * start.y + system.m01 * end.y + system.m02 * startSlope + system.m03 * endSlope
        });
    }
    #endregion
}

/// <summary>
/// Represents a collection of joined curves.
/// </summary>
public struct PolynomialCurve2D
{
    #region Fields
    /// <summary>
    /// The ranges that were used to assemble this curve.
    /// </summary>
    public readonly Polynomial2D[] ranges;
    /// <summary>
    /// The left end of this curve along the x-axis.
    /// </summary>
    public readonly float left;
    /// <summary>
    /// The right end of this curve along the x-axis.
    /// </summary>
    public readonly float right;
    #endregion
    #region Constructors
    /// <summary>
    /// Creates a new polynomial curve with the given pieces.
    /// </summary>
    /// <param name="ranges">The segments containing the polynomial definition.</param>
    public PolynomialCurve2D(Polynomial2D[] ranges)
    {
        // Check for invalid input data.
        for (int i = 1; i < ranges.Length; i++)
            if (ranges[i].left != ranges[i - 1].right)
                throw new ArgumentException("A curve must have joined endpoints on the given ranges.", "ranges");
        this.ranges = ranges;
        // Set the left and right edges of this polynomial curve.
        left = ranges[0].left;
        right = ranges[ranges.Length - 1].right;
    }
    #endregion
    #region Polynomial Evaluation
    /// <summary>
    /// Evaluates the height at a distance along the curve.
    /// </summary>
    /// <param name="location">The location to evaluate at.</param>
    /// <returns>The output of the function at the given location.</returns>
    public float ValueAt(float location)
    {
        for (int i = 0; i < ranges.Length; i++)
            if (location < ranges[i].right)
                return ranges[i].ValueAt(location);
        return ranges[ranges.Length - 1].ValueAt(location);
    }
    /// <summary>
    /// Evaluates the slope at a distance along the curve.
    /// </summary>
    /// <param name="location">The location to evaluate at.</param>
    /// <returns>The slope of the function at the given location.</returns>
    public float SlopeAt(float location)
    {
        for (int i = 0; i < ranges.Length; i++)
            if (location < ranges[i].right)
                return ranges[i].SlopeAt(location);
        return ranges[ranges.Length - 1].SlopeAt(location);
    }
    #endregion
    #region Utility Methods
    /// <summary>
    /// Evaluates the polynomial into a polyline.
    /// </summary>
    /// <param name="stepWidth">How often should the curve be sampled along the x-axis.</param>
    /// <returns>A collection of points along the polynomial.</returns>
    public Vector2[] Rasterize(float stepWidth)
    {
        List<Vector2> rasterizedCurve = new List<Vector2>();
        // Step along the curve and sample points.
        float distanceAlong = left;
        while (distanceAlong <= right)
        {
            rasterizedCurve.Add(new Vector2
            {
                x = distanceAlong,
                y = ValueAt(distanceAlong)
            });
            distanceAlong += stepWidth;
        }
        // Add a clean endpoint if the interval
        // was not evenly divisible by the step width.
        if (distanceAlong < right)
        {
            rasterizedCurve.Add(new Vector2
            {
                x = right,
                y = ValueAt(right)
            });
        }
        return rasterizedCurve.ToArray();
    }
    #endregion
}

/// <summary>
/// Represents a curvature segment on a polynomial curve.
/// </summary>
public struct Polynomial2D
{
    #region Fields
    private readonly float[] coefficients;
    /// <summary>
    /// The left end of this curve along the x-axis.
    /// </summary>
    public readonly float left;
    /// <summary>
    /// The right end of this curve along the x-axis.
    /// </summary>
    public readonly float right;
    #endregion
    #region Constructors
    /// <summary>
    /// Creates a new polynomial range defined by endpoint restraints and polynomial coefficients.
    /// </summary>
    /// <param name="left">The left most x value that will yield a logical value.</param>
    /// <param name="right">The right most x value that will yield a logical value.</param>
    /// <param name="coefficients"></param>
    public Polynomial2D(float left, float right, float[] coefficients)
    {
        this.left = left;
        this.right = right;
        this.coefficients = coefficients;
    }
    #endregion
    #region Polynomial Evaluation
    /// <summary>
    /// Gets the polynomial output at the given point on the curve.
    /// </summary>
    /// <param name="location">The location along the x-axis.</param>
    /// <returns>The local value of the function.</returns>
    public float ValueAt(float location)
    {
        float accumulator = 0f;
        // Increase the power using a variable to
        // decrease operation repetitions.
        float baseValue = 1f;
        for (int i = 0; i < coefficients.Length; i++)
        {
            accumulator += baseValue * coefficients[i];
            baseValue *= location;
        }
        // Return the evaluated y position on the curve.
        return accumulator;
    }
    /// <summary>
    /// Gets the polynomial slope at the given point on the curve.
    /// </summary>
    /// <param name="location">The location along the x-axis.</param>
    /// <returns>The local slope of the function.</returns>
    public float SlopeAt(float location)
    {
        float accumulator = 0f;
        // Increase the power using a variable to
        // decrease operation repetitions.
        float baseValue = 1f;
        for (int i = 1; i < coefficients.Length; i++)
        {
            accumulator += i * baseValue * coefficients[i];
            baseValue *= location;
        }
        // Return the evaluated slope on the curve.
        return accumulator;
    }
    #endregion
}
