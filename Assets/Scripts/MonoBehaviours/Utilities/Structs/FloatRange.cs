using System;
using UnityEngine;

/// <summary>
/// Data structure for floating point ranges.
/// </summary>
[Serializable]
public struct FloatRange
{
    #region Fields
    /// <summary>
    /// The left end of the range.
    /// </summary>
    [Tooltip("The left end of the range.")]
    public float min;
    /// <summary>
    /// The right end of the range.
    /// </summary>
    [Tooltip("The right end of the range.")]
    public float max;
    #endregion
    #region Properties
    /// <summary>
    /// The length of the range.
    /// </summary>
    public float Length { get { return max - min; } }
    #endregion
    #region Utility Methods
    /// <summary>
    /// Ensures that the range is non-decreasing.
    /// </summary>
    public void ClampIncreasing()
    {
        if (max < min)
            max = min;
    }
    #endregion
}
