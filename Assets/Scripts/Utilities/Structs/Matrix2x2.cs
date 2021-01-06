using System;
using UnityEngine;

// TODO this matrix implementation is missing some
// standard accessors/methods (not used in this project).

/// <summary>
/// A standard 2x2 matrix.
/// </summary>
public struct Matrix2x2 : IEquatable<Matrix2x2>
{
    #region Matrix Fields
    public float m00;
    public float m01;
    public float m10;
    public float m11;
    #endregion
    #region Constructors
    /// <summary>
    /// Creates a 2x2 matrix with the given values.
    /// </summary>
    /// <param name="column0">Passes the first matrix column values.</param>
    /// <param name="column1">Passes the second matrix column values.</param>
    public Matrix2x2(Vector2 column0, Vector2 column1)
    {
        m00 = column0.x;
        m10 = column0.y;
        m01 = column1.x;
        m11 = column1.y;
    }
    #endregion
    #region Utility Accessors
    /// <summary>
    /// Calculates and returns the inverse 2x2 matrix.
    /// </summary>
    public Matrix2x2 Inverse
    {
        get
        {
            float determinant = Determinant;
            return new Matrix2x2
            {
                m00 = determinant * m11,
                m01 = determinant * -m01,
                m10 = determinant * -m10,
                m11 = determinant * m00
            };
        }
    }
    /// <summary>
    /// Calculates and returns the matrix determinant.
    /// </summary>
    public float Determinant
    {
        get
        {
            // TODO does not catch division by zero.
            return 1f / (m00 * m11 - m01 * m10);
        }
    }
    #endregion
    #region IEquatable Implementation
    /// <summary>
    /// Checks whether these matrices hold the same values.
    /// </summary>
    /// <param name="other">The other matrix.</param>
    /// <returns>True when the matrix values are identical.</returns>
    public bool Equals(Matrix2x2 other)
    {
        // TODO floating point errors are not accounted
        // for in this implementation.
        return m00 == other.m00
            && m01 == other.m01
            && m10 == other.m10
            && m11 == other.m11;
    }
    #endregion
}
