using UnityEngine;

namespace SnowmansFolly.MonoBehaviours.Cameras
{
    // TODO this could be abstracted away from Unity.
    /// <summary>
    /// Constrains the camera limits inside a given orthographic 2D box.
    /// </summary>
    public sealed class CameraBoxConstraint : CameraConstraint
    {
        #region Inspector Fields
        [Tooltip("Defines the bounding box limits on the x axis.")]
        [SerializeField] private FloatRange limitsX = default;
        [Tooltip("Defines the bounding box limits on the y axis.")]
        [SerializeField] private FloatRange limitsY = default;
        #endregion
#if UNITY_EDITOR
        #region Inspector Validation
        private void OnValidate()
        {
            // Clamp limits.
            limitsX.ClampIncreasing();
            limitsY.ClampIncreasing();
        }
        #endregion
#endif
        #region Constraint Implementation
        /// <summary>
        /// Constrains the camera position to be within the box.
        /// </summary>
        /// <param name="toConstrain">The camera transform to constrain.</param>
        /// <param name="viewport">The viewport rectangle.</param>
        public override void Constrain(Transform toConstrain, Rect viewport)
        {
            // TODO might be better to abstract this to a general region
            // that can be snapped into.
            float deltaX = 0f, deltaY = 0f;
            // Check if the transform is beyond x bounds.
            if (viewport.xMin < limitsX.min)
                deltaX = limitsX.min - viewport.xMin;
            else if (viewport.xMax > limitsX.max)
                deltaX = limitsX.max - viewport.xMax;
            // Check if the transform is beyond y bounds.
            if (viewport.yMin < limitsY.min)
                deltaY = limitsY.min - viewport.yMin;
            else if (viewport.yMax > limitsY.max)
                deltaY = limitsY.max - viewport.yMax;
            // Update the transform positon.
            toConstrain.position = new Vector3
            {
                x = toConstrain.position.x + deltaX,
                y = toConstrain.position.y + deltaY,
                z = toConstrain.position.z
            };
        }
        #endregion
    }
}
