using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnowmansFolly.MonoBehaviours.Curvatures
{
    /// <summary>
    /// A scene instance of a polynomial spline,
    /// adjusted for the local transform.
    /// </summary>
    public sealed class PolynomialSpline2DInstance : MonoBehaviour, IValidateOnTransformChange
    {
        #region Fields
        private PolynomialSpline2D curve;
        #endregion
        #region Inspector Fields
        [Tooltip("The nodes that form the spline.")]
        [SerializeField] private PolynomialNode[] nodes = default;
#if UNITY_EDITOR
        [Header("Debug Parameters")]
        [Tooltip("Whether to render a preview for this spline.")]
        [SerializeField] private bool showPreview = true;
        [Tooltip("The amount of detail in the preview curve for the spline.")]
        [Range(0.1f, 10.0f)][SerializeField] private float previewDetail = 0.5f;
#endif
        #endregion
        #region Inspector POCOs
        [Serializable]
        private class PolynomialNode
        {
            [Tooltip("The transform driving of the node.")]
            public Transform transform = default;
            [Tooltip("The curve degree following this node. Ignored if this is the last node.")]
            public PolynomialDegree type = default;
            [Tooltip("Mark this as true if the curve does not change locally; this means it never recalculates.")]
            public bool isStatic = true;
        }
        #endregion
#if UNITY_EDITOR
        #region Inspector Validation
        public void OnValidate()
        {
            // Regenerate the curve and rasterized path
            // so that it can be displayed in the editor window.
            if (!(nodes is null) && nodes.Length > 1)
            {
                RegenerateCurve();
                rasterizedPath = curve.Rasterize(previewDetail);
            }
        }
        #endregion
        #region Gizmo Parameters
        private const float HANDLE_LENGTH = 1.0f;
        private readonly Color HANDLES_COLOR = Color.magenta;
        private readonly Color PREVIEW_COLOR = Color.yellow;
        private Vector2[] rasterizedPath;
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
        #region Spline Properties
        /// <summary>
        /// The left world x position where the curve is meant to end.
        /// </summary>
        public float Left => transform.position.x + curve.Left;
        /// <summary>
        /// The right world x position where the curve is meant to end.
        /// </summary>
        public float Right => transform.position.x + curve.Right;
        /// <summary>
        /// Gets the length of the interval along the x axis.
        /// </summary>
        public float IntervalLength => curve.Right - curve.Left;
        #endregion
        #region Spline Methods
        /// <summary>
        /// Gets the spline value at a world x location.
        /// </summary>
        /// <param name="worldX">The world x location to sample.</param>
        /// <returns>The world y value at the sampled spline location.</returns>
        public float ValueAt(float worldX)
        {
            // Calculate the curve value in local space.
            return curve.ValueAt(
                worldX - transform.position.x)
                + transform.position.y;
        }
        /// <summary>
        /// Gets the spline normal at a world x location.
        /// </summary>
        /// <param name="worldX">The world x location to sample.</param>
        /// <returns>The slope normal at the sampled spline location.</returns>
        public Vector2 NormalAt(float worldX)
        {
            // Get the local slope.
            float slope = curve.SlopeAt(worldX - transform.position.x);
            // Turn the slope into a normal vector.
            return (new Vector2(-slope, 1f)).normalized;
        }
        #endregion
        #region Spline Initialization and Update Implementation
        private void Awake()
        {
            // Generate the curve on awake.
            RegenerateCurve();
        }
        private void FixedUpdate()
        {
            // TODO having to check this on tick
            // by default is poor; the tick requirement
            // should be injected and bound appropriately.
            List<int> indicesToRegenerate = new List<int>();
            for (int i = 0; i < nodes.Length - 1; i++)
                if (!nodes[i].isStatic)
                    indicesToRegenerate.Add(i);
            // Regenerate nodes that require regeneration.
            if (indicesToRegenerate.Count > 0)
                RegenerateCurve(indicesToRegenerate.ToArray());
        }
        #endregion
        #region Curve Regeneration Method
        /// <summary>
        /// Regenerates the entire curvature with the current transform data.
        /// </summary>
        public void RegenerateCurve()
        {
            PolynomialSegment2D[] ranges = new PolynomialSegment2D[nodes.Length - 1];
            for (int i = 0; i < nodes.Length - 1; i++)
                ranges[i] = Generate(i);
            curve = new PolynomialSpline2D(ranges);
#if UNITY_EDITOR
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
            PolynomialSegment2D[] ranges = curve.Ranges;
            ranges[curveIndex] = Generate(curveIndex);
            curve = new PolynomialSpline2D(ranges);
#if UNITY_EDITOR
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
            PolynomialSegment2D[] ranges = curve.Ranges;
            foreach (int index in curveIndices)
                ranges[index] = Generate(index);
            curve = new PolynomialSpline2D(ranges);
#if UNITY_EDITOR
            if (showPreview)
                rasterizedPath = curve.Rasterize(previewDetail);
#endif
        }
        #endregion
        #region Polynomial Generation
        private PolynomialSegment2D Generate(int segmentIndex)
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
        private PolynomialSegment2D GenerateLinear(Vector2 start, Vector2 end)
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
            return new PolynomialSegment2D(start.x, end.x, new float[]
            {
                system.m10 * start.y + system.m11 * end.y,
                system.m00 * start.y + system.m01 * end.y
            });
        }
        private PolynomialSegment2D GenerateCubic(Vector2 start, Vector2 end, float startSlope, float endSlope)
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
            return new PolynomialSegment2D(start.x, end.x, new float[]
            {
                system.m30 * start.y + system.m31 * end.y + system.m32 * startSlope + system.m33 * endSlope,
                system.m20 * start.y + system.m21 * end.y + system.m22 * startSlope + system.m23 * endSlope,
                system.m10 * start.y + system.m11 * end.y + system.m12 * startSlope + system.m13 * endSlope,
                system.m00 * start.y + system.m01 * end.y + system.m02 * startSlope + system.m03 * endSlope
            });
        }
        #endregion
    }
}
