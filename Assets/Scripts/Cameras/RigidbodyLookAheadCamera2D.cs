using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RigidbodyLookAheadCamera2D : ConstrainableCamera2D
{
    [SerializeField] private Rigidbody2D targetBody = null;
    [SerializeField] private SpeedSizeRangePair cameraZoomIntensity = default;
    [Serializable]
    private struct SpeedSizeRangePair
    {
        public FloatRange speedRange;
        public FloatRange cameraSizeRange;
        public Vector2 trailingAmplitude;
    }


    protected override void Update()
    {
        float interpolant =
            Mathf.Clamp01(Mathf.InverseLerp(
                cameraZoomIntensity.speedRange.min,
                cameraZoomIntensity.speedRange.max,
                targetBody.velocity.magnitude));

        Camera.orthographicSize =
            Mathf.Lerp(
                cameraZoomIntensity.cameraSizeRange.min,
                cameraZoomIntensity.cameraSizeRange.max,
                interpolant);

        transform.position = new Vector3
        {
            x = targetBody.transform.position.x
                + cameraZoomIntensity.trailingAmplitude.x * targetBody.velocity.x,
            y = targetBody.transform.position.y
                + cameraZoomIntensity.trailingAmplitude.y * targetBody.velocity.y,
            z = transform.position.z
        };

        base.Update();
    }
}
