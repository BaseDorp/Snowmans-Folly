using System;
using UnityEngine;

/// <summary>
/// Provides utility extension methods for floats.
/// </summary>
public static class FloatExtensions
{
    /// <summary>
    /// Clamps a floating point value in place into the given range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value for the float.</param>
    /// <param name="max">The maximum value for the float.</param>
    public static void Clamp(this ref float value, float min, float max)
    {
        value = Mathf.Clamp(value, min, max);
    }
    /// <summary>
    /// Wraps a floating point value into a given range.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <param name="min">The left end of the range to wrap into.</param>
    /// <param name="max">The right end of the range to wrap into.</param>
    public static void Wrap(this ref float value, float min, float max)
    {
        // Check arguments.
        float range = max - min;
        if (range <= 0f)
            throw new ArgumentException("Max value must be greater than min value.", "max");
        // Wrap the value from both directions.
        while (value < min)
            value += range;
        while (value > max)
            value -= range;
    }
    public static float Wrapped(this float value, float min, float max)
    {
        // Check arguments.
        float range = max - min;
        if (range <= 0f)
            throw new ArgumentException("Max value must be greater than min value.", "max");
        // Wrap the value from both directions.
        while (value < min)
            value += range;
        while (value > max)
            value -= range;
        return value;
    }
}
