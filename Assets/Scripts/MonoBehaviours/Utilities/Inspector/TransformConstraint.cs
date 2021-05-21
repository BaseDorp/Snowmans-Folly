using UnityEngine;

/// <summary>
/// Applys ranged limits for the attached transform.
/// </summary>
public sealed class TransformConstraint : MonoBehaviour, IValidateOnTransformChange
{
    #region Inspector Fields
    [Tooltip("Toggles all constraints on or off for this transform.")]
    [SerializeField] private bool constraintsEnabled = false;
    [Header("Position Constraints")]
    [Tooltip("Toggles all position constraints on or off for this transform.")]
    [SerializeField] private bool constrainPosition = false;
    [Tooltip("Specifies the valid range for position values on the x-axis.")]
    [SerializeField] private FloatRange positionXConstraint = default;
    [Tooltip("Specifies the valid range for position values on the y-axis.")]
    [SerializeField] private FloatRange positionYConstraint = default;
    [Tooltip("Specifies the valid range for position values on the z-axis.")]
    [SerializeField] private FloatRange positionZConstraint = default;
    [Header("Rotation Constraints")]
    [Tooltip("Toggles all rotation constraints on or off for this transform.")]
    [SerializeField] private bool constrainRotation = false;
    [Tooltip("Specifies the valid range for rotation values on the x-axis.")]
    [SerializeField] private FloatRange rotationXConstraint = default;
    [Tooltip("Specifies the valid range for rotation values on the y-axis.")]
    [SerializeField] private FloatRange rotationYConstraint = default;
    [Tooltip("Specifies the valid range for rotation values on the z-axis.")]
    [SerializeField] private FloatRange rotationZConstraint = default;
    [Header("Scale Constraints")]
    [Tooltip("Toggles all scale constraints on or off for this transform.")]
    [SerializeField] private bool constrainScale = false;
    [Tooltip("Specifies the valid range for scale values on the x-axis.")]
    [SerializeField] private FloatRange scaleXConstraint = default;
    [Tooltip("Specifies the valid range for scale values on the y-axis.")]
    [SerializeField] private FloatRange scaleYConstraint = default;
    [Tooltip("Specifies the valid range for scale values on the z-axis.")]
    [SerializeField] private FloatRange scaleZConstraint = default;
    #endregion
    #region Constraint Enforcement
    public void OnValidate()
    {
        // Apply the constraints on each set of transform parameters.
        if (constraintsEnabled)
        {
            if (constrainPosition)
            {
                transform.localPosition = new Vector3
                {
                    x = Mathf.Clamp(transform.localPosition.x, positionXConstraint.min, positionXConstraint.max),
                    y = Mathf.Clamp(transform.localPosition.y, positionYConstraint.min, positionYConstraint.max),
                    z = Mathf.Clamp(transform.localPosition.z, positionZConstraint.min, positionZConstraint.max)
                };
            }
            if (constrainRotation)
            {
                // Rotations must be wrapped into the -180 to 180 range to account for
                // how unity internally stores the rotation values as non-negative.
                transform.localEulerAngles = new Vector3
                {
                    x = Mathf.Clamp(transform.localEulerAngles.x.Wrapped(-180f, 180f), rotationXConstraint.min, rotationXConstraint.max),
                    y = Mathf.Clamp(transform.localEulerAngles.y.Wrapped(-180f, 180f), rotationYConstraint.min, rotationYConstraint.max),
                    z = Mathf.Clamp(transform.localEulerAngles.z.Wrapped(-180f, 180f), rotationZConstraint.min, rotationZConstraint.max)
                };
            }
            if (constrainScale)
            {
                transform.localScale = new Vector3
                {
                    x = Mathf.Clamp(transform.localScale.x, scaleXConstraint.min, scaleXConstraint.max),
                    y = Mathf.Clamp(transform.localScale.y, scaleYConstraint.min, scaleYConstraint.max),
                    z = Mathf.Clamp(transform.localScale.z, scaleZConstraint.min, scaleZConstraint.max)
                };
            }
        }
    }
    #endregion
}
