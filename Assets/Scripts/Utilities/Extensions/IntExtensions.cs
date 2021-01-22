using UnityEngine;

/// <summary>
/// Provides utility extension methods for integers.
/// </summary>
public static class IntExtensions
{
    /// <summary>
    /// Clamps an integer value in place into the given range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value for the int.</param>
    /// <param name="max">The maximum value for the int.</param>
    public static void Clamp(this ref int value, int min, int max)
    {
        value = Mathf.Clamp(value, min, max);
    }
}
