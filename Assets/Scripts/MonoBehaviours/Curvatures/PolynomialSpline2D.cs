using System;
using System.Collections.Generic;
using UnityEngine; // TODO remove this dependency (used for editor drawing).

namespace SnowmansFolly.MonoBehaviours.Curvatures
{
    // TODO are there benefits to this being immutable?
    // TODO should not be in the Mono namespace.
    /// <summary>
    /// Immutable class for joined segments of a polynomial curve.
    /// </summary>
    public sealed class PolynomialSpline2D
    {
        #region Fields
        /// <summary>
        /// The ranges that were used to assemble this curve.
        /// </summary>
        public PolynomialSegment2D[] Ranges { get; }
        /// <summary>
        /// The left end of this curve along the x-axis.
        /// </summary>
        public float Left { get; }
        /// <summary>
        /// The right end of this curve along the x-axis.
        /// </summary>
        public float Right { get; }
        #endregion
        #region Constructors
        /// <summary>
        /// Creates a new polynomial curve with the given pieces.
        /// </summary>
        /// <param name="ranges">The segments containing the polynomial definition.</param>
        public PolynomialSpline2D(PolynomialSegment2D[] ranges)
        {
            // Check for invalid input data.
            for (int i = 1; i < ranges.Length; i++)
                if (ranges[i].Left != ranges[i - 1].Right)
                    throw new ArgumentException(
                        "A curve must have joined endpoints on the given ranges.",
                        "ranges");
            Ranges = ranges;
            // Set the left and right edges of this polynomial curve.
            Left = ranges[0].Left;
            Right = ranges[ranges.Length - 1].Right;
        }
        #endregion
        #region Polynomial Evaluation
        /// <summary>
        /// Evaluates the height at a distance along the curve.
        /// </summary>
        /// <param name="location">The local location to evaluate at.</param>
        /// <returns>The output of the function at the given location.</returns>
        public float ValueAt(float location)
        {
            // Find the appropriate range.
            // This will extrapolate off the left end if before the start.
            for (int i = 0; i < Ranges.Length; i++)
                if (location < Ranges[i].Right)
                    return Ranges[i].ValueAt(location);
            // Otherwise extrapolate off the right end.
            return Ranges[Ranges.Length - 1].ValueAt(location);
        }
        /// <summary>
        /// Evaluates the slope at a distance along the curve.
        /// </summary>
        /// <param name="location">The location to evaluate at.</param>
        /// <returns>The slope of the function at the given location.</returns>
        public float SlopeAt(float location)
        {
            // TODO could be more DRY with above method.
            // Find the appropriate range.
            // This will extrapolate off the left end if before the start.
            for (int i = 0; i < Ranges.Length; i++)
                if (location < Ranges[i].Right)
                    return Ranges[i].SlopeAt(location);
            // Otherwise extrapolate off the right end.
            return Ranges[Ranges.Length - 1].SlopeAt(location);
        }
        #endregion
        #region Utility Methods
        /// <summary>
        /// Evaluates the polynomial into a polyline.
        /// </summary>
        /// <param name="stepWidth">How often should the curve be sampled along the input axis.</param>
        /// <returns>A collection of points along the polynomial.</returns>
        public Vector2[] Rasterize(float stepWidth)
        {
            List<Vector2> rasterizedCurve = new List<Vector2>();
            // Step along the curve and sample points.
            float distanceAlong = Left;
            while (distanceAlong <= Right)
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
            if (distanceAlong < Right)
            {
                rasterizedCurve.Add(new Vector2
                {
                    x = Right,
                    y = ValueAt(Right)
                });
            }
            return rasterizedCurve.ToArray();
        }
        #endregion
    }
}
