using System;
using UnityEngine;

namespace SnowmansFolly.MonoBehaviours.Cameras
{
    // TODO this could be abstracted away from Unity.
    /// <summary>
    /// Implements a camera whose base behaviour is to look at where a rigidbody
    /// will be based on its velocity.
    /// </summary>
    public sealed class RigidbodyLookAheadCamera2D : ConstrainableCamera2D
    {
        #region Inspector Fields
        [Tooltip("The rigidbody that this camera is following.")]
        [SerializeField] private Rigidbody2D targetBody = default;
        [Tooltip("Controls how the camera reacts to the rigidbody.")]
        [SerializeField] private SpeedSizeRangePair cameraZoomIntensity = default;
        #endregion
        #region Inspector POCOs
        [Serializable]
        private struct SpeedSizeRangePair
        {
            [Tooltip("The speed range that the camera acknowledges. Anything outside this range has clamped behaviour.")]
            public FloatRange speedRange;
            [Tooltip("The orthographic size of the camera from the slowest speed to the fastest speed.")]
            public FloatRange cameraSizeRange;
            [Tooltip("Controls how much the rigidbody velocity effects camera look ahead on each axis.")]
            public Vector2 trailingAmplitude;
        }
        #endregion
#if UNITY_EDITOR
        #region Inspector Validation
        private void OnValidate()
        {
            // Clamp the ranges to be increasing.
            cameraZoomIntensity.speedRange.ClampIncreasing();
            cameraZoomIntensity.cameraSizeRange.ClampIncreasing();
        }
        #endregion
#endif
        #region Rigidbody Following Implementation
        // This has to run in fixed update because the
        // rigidbody runs in fixed update. Otherwise stutter.
        private void FixedUpdate()
        {
            // Get the movement speed interpolant.
            float interpolant =
                Mathf.Clamp01(Mathf.InverseLerp(
                    cameraZoomIntensity.speedRange.min,
                    cameraZoomIntensity.speedRange.max,
                    targetBody.velocity.magnitude));
            // Set the camera size accordingly.
            Camera.orthographicSize =
                Mathf.Lerp(
                    cameraZoomIntensity.cameraSizeRange.min,
                    cameraZoomIntensity.cameraSizeRange.max,
                    interpolant);
            // Get a target position to move the camera towards.
            Vector3 target = new Vector3
            {
                x = targetBody.transform.position.x
                    + cameraZoomIntensity.trailingAmplitude.x * targetBody.velocity.x,
                y = targetBody.transform.position.y
                    + cameraZoomIntensity.trailingAmplitude.y * targetBody.velocity.y,
                z = transform.position.z
            };
            // Move towards the target camera position.
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                Time.fixedDeltaTime * (target - transform.position).magnitude);
            // Call the base update functionality; this prevents
            // desynchronization of the camera movement with the update loop.
            // TODO could be better abstracted so that the camera always
            // runs on either the fixed loop or update loop.
            Update();
        }
        #endregion
    }
}
