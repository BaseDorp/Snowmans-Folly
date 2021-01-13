using UnityEngine;

/// <summary>
/// Provides utility extension methods for Cameras.
/// </summary>
public static class CameraExtensions
{
    /// <summary>
    /// Calculates the 2D world space rectangle that an othrographic camera is viewing.
    /// </summary>
    /// <param name="camera">The camera to retrieve the rect from.</param>
    /// <returns>A world space rectangle of the visible camera space.</returns>
    public static Rect GetWorldSpace2DRect(this Camera camera)
    {
        // Project each camera corner onto the 2D world space.
        Vector2 cornerA =
            camera.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 cornerB =
            camera.ViewportToWorldPoint(new Vector2(0, 1));
        Vector2 cornerC =
            camera.ViewportToWorldPoint(new Vector2(1, 0));
        Vector2 cornerD =
            camera.ViewportToWorldPoint(new Vector2(1, 1));
        // Find the bounding corners of the camera.
        Vector2 min = new Vector2
        {
            x = Mathf.Min(cornerA.x, cornerB.x, cornerC.x, cornerD.x),
            y = Mathf.Min(cornerA.y, cornerB.y, cornerC.y, cornerD.y)
        };
        Vector2 max = new Vector2
        {
            x = Mathf.Max(cornerA.x, cornerB.x, cornerC.x, cornerD.x),
            y = Mathf.Max(cornerA.y, cornerB.y, cornerC.y, cornerD.y)
        };
        // Construct and return the corresponding rect.
        return new Rect(min, max - min);
    }
}
