using UnityEngine;

namespace SnowmansFolly.MonoBehaviours.CurvatureMovers
{
    // TODO abstract away from MonoBehaviour.
    /// <summary>
    /// A curve mover that tracks the location of another transform.
    /// </summary>
    public sealed class TrackingCurveMover : CurveMover2D
    {
        #region Inspector Fields
        [Tooltip("The target that this move attempts to follow.")]
        [SerializeField] private Transform toFollow = default;
        #endregion
        #region Tracking Implementation
        private void FixedUpdate()
        {
            // Track the object on fixed update.
            // TODO dependency inject the update loop type,
            // instead of assuming FixedUpdate.
            MoveTo(toFollow.position.x);
        }
        #endregion
    }
}
