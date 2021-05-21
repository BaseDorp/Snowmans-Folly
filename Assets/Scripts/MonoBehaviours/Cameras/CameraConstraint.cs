using UnityEngine;

namespace SnowmansFolly.MonoBehaviours.Cameras
{
    // TODO this could be abstracted away from Unity.
    // TODO create an interface for CameraConstraint for easier composition.
    /// <summary>
    /// Implements a movement constraining function for a camera.
    /// </summary>
    public abstract class CameraConstraint : MonoBehaviour
    {
        #region Methods Implemented
        /// <summary>
        /// Adjusts the camera's transform such that it obeys a given constraint.
        /// </summary>
        /// <param name="toConstrain">The transform to constrain.</param>
        /// <param name="viewport">The rectangle drawn by the transform.</param>
        public abstract void Constrain(Transform toConstrain, Rect viewport);
        #endregion
    }
}
