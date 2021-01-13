using UnityEngine;
using UnityEditor;

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
}