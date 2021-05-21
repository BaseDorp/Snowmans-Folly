using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnowmansFolly.Tests
{
    public sealed class LaunchRampTest : MonoBehaviour
    {
        [SerializeField] private Transform launchRampPivot = null;
        [SerializeField] private FloatRange pivotRange = default;
        [SerializeField] private float pivotInterval = 1f;

        // Update is called once per frame
        void Update()
        {
            float interpolant = Mathf.InverseLerp(-1f, 1f, Mathf.Sin(Time.time / pivotInterval));
            launchRampPivot.localEulerAngles = new Vector3
            {
                z = Mathf.Lerp(pivotRange.min, pivotRange.max, interpolant)
            };
        }
    }
}
