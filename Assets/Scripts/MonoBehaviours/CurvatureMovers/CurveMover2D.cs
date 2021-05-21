using UnityEngine;
using SnowmansFolly.MonoBehaviours.Curvatures;

namespace SnowmansFolly.MonoBehaviours.CurvatureMovers
{
    // TODO abstract away from MonoBehaviour;
    // use observer to notify engine class.
    // TODO should abstract an ICurve interface so that
    // it does not need to be a polynomial.
    /// <summary>
    /// Base class for objects that move along a polynomial curve.
    /// </summary>
    public class CurveMover2D : MonoBehaviour
    {
        #region Inspector Fields
        [Tooltip("The object that is moved along the curve.")]
        [SerializeField] private Transform toMove = default;
        [Tooltip("The curve that this mover follows.")]
        [SerializeField] private PolynomialSpline2DInstance curve = default;
        [Tooltip("Whether the object moving along the curve aligns to the curve.")]
        [SerializeField] private bool alignToNormal = false;
        [Tooltip("Whether the objects is locked to the ends of the curve, or can extrapolate beyond the ends.")]
        [SerializeField] private bool lockToCurve = false;
        #endregion
        #region Properties
        /// <summary>
        /// The curve that this mover is bound to.
        /// </summary>
        public PolynomialSpline2DInstance Curve
        {
            get => curve;
            set
            {
                // TODO there should be logic here to
                // react to the new curve being set.
                curve = value;
            }
        }
        #endregion
        #region Common Methods
        // TODO this method should take in a Vector2; and offer a more
        // generalized approach instead of being axis specific.
        /// <summary>
        /// Moves the object to the point on the curve at location x.
        /// </summary>
        /// <param name="locationX">The location to move to.</param>
        public void MoveTo(float locationX)
        {
            // Lock to the curve if requested.
            if (lockToCurve)
                locationX.Clamp(curve.Left, curve.Right);
            // Move to the evaluated location on the curve.
            transform.position = new Vector2(
                locationX,
                curve.ValueAt(locationX));
            // Align the object if requested.
            if (alignToNormal)
                toMove.up = curve.NormalAt(locationX);
        }
        #endregion
    }
}
