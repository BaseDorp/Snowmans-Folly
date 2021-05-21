using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public static class GizmosHelper
{
    private const float ASYMPTOTE_DASH_LENGTH = 0.2f;

    public static void DrawAsymptote(float worldX)
    {
        Camera[] cameras = SceneView.GetAllSceneCameras();
        Rect boundingRect = new Rect();

        foreach (Camera camera in cameras)
        {
            Rect cameraRect = camera.GetWorldSpace2DRect();
            boundingRect.min = Vector2.Min(boundingRect.min, cameraRect.min);
            boundingRect.max = Vector2.Max(boundingRect.max, cameraRect.max);
        }

        if (worldX >= boundingRect.xMin
            && worldX <= boundingRect.xMax)
        {
            float height = boundingRect.yMin;
            while (height <= boundingRect.yMax)
            {
                Gizmos.DrawLine(new Vector2(worldX, height), new Vector2(worldX, height + ASYMPTOTE_DASH_LENGTH));
                height += 2f * ASYMPTOTE_DASH_LENGTH;
            }
        }
    }

    private const int CIRCLE_DIVISIONS = 32;
    public static void DrawCircle(Vector2 center, float radius)
    {
        Vector2 previous = Vector2.up * radius;
        for (int i = 1; i <= CIRCLE_DIVISIONS; i++)
        {
            float angle = (Mathf.PI * 2f) * ((float)i / CIRCLE_DIVISIONS);
            Vector2 current = new Vector2
            {
                x = Mathf.Sin(angle) * radius,
                y = Mathf.Cos(angle) * radius
            };
            Gizmos.DrawLine(center + previous, center + current);
            previous = current;
        }
    }
}
#endif