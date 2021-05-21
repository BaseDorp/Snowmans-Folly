namespace SnowmansFolly.MonoBehaviours.Curvatures
{
    // TODO should not be in the Mono namespace.
    /// <summary>
    /// Immutable class for one segment of a polynomial curve.
    /// </summary>
    public sealed class PolynomialSegment2D
    {
        #region Fields
        private readonly float[] coefficients;
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
        /// Creates a new polynomial segment defined by endpoint restraints and polynomial coefficients.
        /// </summary>
        /// <param name="left">The left most x value that will yield a logical value.</param>
        /// <param name="right">The right most x value that will yield a logical value.</param>
        /// <param name="coefficients">The coefficients of the polynomial.</param>
        public PolynomialSegment2D(float left, float right, float[] coefficients)
        {
            Left = left;
            Right = right;
            this.coefficients = coefficients;
        }
        #endregion
        #region Polynomial Evaluation
        /// <summary>
        /// Gets the polynomial output at the given point on the curve.
        /// </summary>
        /// <param name="location">The location along the input axis.</param>
        /// <returns>The local value of the function.</returns>
        public float ValueAt(float location)
        {
            float accumulator = 0f;
            // Step through each power of x, minimizing
            // the number of operations required.
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
        /// <param name="location">The location along the input axis.</param>
        /// <returns>The local slope of the function.</returns>
        public float SlopeAt(float location)
        {
            float accumulator = 0f;
            // Step through each power of x, minimizing
            // the number of operations required.
            // In this case the derivative is calculated.
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
}
