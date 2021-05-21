namespace SnowmansFolly.MonoBehaviours.Curvatures
{
    // TODO should be moved into the core namespace.
    // TODO should have more options for quadratic, quartic.
    /// <summary>
    /// Denotes a degree of polynomial function.
    /// </summary>
    public enum PolynomialDegree : byte
    {
        /// <summary>
        /// A first degree spline with no smoothing between points.
        /// </summary>
        Linear,
        /// <summary>
        /// A third degree spline with cubic smoothing between points.
        /// </summary>
        Cubic
    }
}
