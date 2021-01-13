using UnityEngine;

/// <summary>
/// Constrains the camera limits inside a given orthographic 2D box.
/// </summary>
public sealed class CameraBoxConstraint : CameraConstraint
{

    [SerializeField] private FloatRange limitsX = default;
    [SerializeField] private FloatRange limitsY = default;


    public override void Constrain(Transform toConstrain, Rect viewport)
    {
        float deltaX = 0f, deltaY = 0f;

        if (viewport.xMin < limitsX.min)
            deltaX = limitsX.min - viewport.xMin;
        else if (viewport.xMax > limitsX.max)
            deltaX = limitsX.max - viewport.xMax;

        if (viewport.yMin < limitsY.min)
            deltaY = limitsY.min - viewport.yMin;
        else if (viewport.yMax > limitsY.max)
            deltaY = limitsY.max - viewport.yMax;

        toConstrain.position = new Vector3
        {
            x = toConstrain.position.x + deltaX,
            y = toConstrain.position.y + deltaY,
            z = toConstrain.position.z
        };
    }
}
