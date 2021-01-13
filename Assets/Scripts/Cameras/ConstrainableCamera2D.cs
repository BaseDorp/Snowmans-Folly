using UnityEngine;

/// <summary>
/// Base class for camera behaviours that can be constrained.
/// </summary>
public class ConstrainableCamera2D : MonoBehaviour
{
    #region Inspector Fields
    [Tooltip("The camera that is being constrained.")]
    [SerializeField] private new Camera camera = null;
    [Tooltip("The additional camera constraints applied to this camera.")]
    [SerializeField] private CameraConstraint[] constraints = null;
    #endregion
    #region Accessors
    /// <summary>
    /// The constrained camera component.
    /// </summary>
    public Camera Camera { get { return camera; } }
    #endregion
    #region Constraints Implementation
    protected virtual void Update()
    {
        // Apply the given constraints after any subclasses have
        // finished operating on the camera transform.
        foreach (CameraConstraint constraint in constraints)
            constraint.Constrain(camera.transform, camera.GetWorldSpace2DRect());
    }
    #endregion
}
