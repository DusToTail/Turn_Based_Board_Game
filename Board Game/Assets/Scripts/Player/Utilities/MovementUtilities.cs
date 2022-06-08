using UnityEngine;

/// <summary>
/// English: A Utility class that is designed to manipulate position and rotation of transforms
/// </summary>
public class MovementUtilities
{
    public static void LinearLerp(Transform moveTransform, Transform fromPosition, Transform toPosition, float t, bool lerpRotation = false)
    {
        if(moveTransform == null) { return; }
        if (fromPosition == null) { return; }
        if (toPosition == null) { return; }

        moveTransform.position = Vector3.Lerp(fromPosition.position, toPosition.position, t);

        if (!lerpRotation) { return; }
        moveTransform.rotation = Quaternion.Lerp(fromPosition.rotation, toPosition.rotation, t);
    }

    public static void QuadraticBezierLerp(Transform moveTransform, Transform fromPosition, Transform toPosition, Transform controlPoint, float t, bool lerpRotation = false)
    {
        if(moveTransform == null) { return; }
        if (fromPosition == null) { return; }
        if (toPosition == null) { return; }
        if(controlPoint == null) { return; }

        Vector3 point1 = Vector3.Lerp(fromPosition.position, controlPoint.position, t);
        Vector3 point2 = Vector3.Lerp(controlPoint.position, toPosition.position, t);
        Vector3 lerpedPosition = Vector3.Lerp(point1, point2, t);
        moveTransform.position = lerpedPosition;

        if(!lerpRotation) { return; }

        Quaternion rotation1 = Quaternion.Lerp(fromPosition.rotation, controlPoint.rotation, t);
        Quaternion rotation2 = Quaternion.Lerp(controlPoint.rotation, toPosition.rotation, t);
        Quaternion lerpedRotation = Quaternion.Lerp(rotation1, rotation2, t);
        moveTransform.rotation = lerpedRotation;

    }

    public static void CubicBezierLerp(Transform moveTransform, Transform fromPosition, Transform toPosition, Transform[] controlPoints, float t, bool lerpRotation = false)
    {
        if (moveTransform == null) { return; }
        if (fromPosition == null) { return; }
        if (toPosition == null) { return; }
        if (controlPoints == null) { return; }
        if(controlPoints.Length < 2) { return; }

        Vector3 point1 = Vector3.Lerp(fromPosition.position, controlPoints[0].position, t);
        Vector3 point2 = Vector3.Lerp(controlPoints[0].position, controlPoints[1].position, t);
        Vector3 point3 = Vector3.Lerp(controlPoints[1].position, toPosition.position, t);
        Vector3 point12 = Vector3.Lerp(point1, point2, t);
        Vector3 point23 = Vector3.Lerp(point2, point3, t);
        Vector3 lerpedPosition = Vector3.Lerp(point12, point23, t);
        moveTransform.position = lerpedPosition;

        if (!lerpRotation) { return; }

        Quaternion rotation1 = Quaternion.Lerp(fromPosition.rotation, controlPoints[0].rotation, t);
        Quaternion rotation2 = Quaternion.Lerp(controlPoints[0].rotation, controlPoints[1].rotation, t);
        Quaternion rotation3 = Quaternion.Lerp(controlPoints[1].rotation, toPosition.rotation, t);
        Quaternion rotation12 = Quaternion.Lerp(rotation1, rotation2, t);
        Quaternion rotation23 = Quaternion.Lerp(rotation2, rotation3, t);
        Quaternion lerpedRotation = Quaternion.Lerp(rotation12, rotation23, t);
        moveTransform.rotation = lerpedRotation;
    }

    public static void RotationLerp(Transform fromTransform, Transform toTransform, float t)
    {
        fromTransform.rotation = Quaternion.Lerp(fromTransform.rotation, toTransform.rotation, t);
    }

    public static Vector3 GetLinearLerp(Transform fromPosition, Transform toPosition, float t)
    {
        if (fromPosition == null) { return Vector3.zero; }
        if (toPosition == null) { return Vector3.zero; }

        return Vector3.Lerp(fromPosition.position, toPosition.position, t);
    }

    public static Vector3 GetQuadraticBezierLerp(Transform fromPosition, Transform toPosition, Transform controlPoint, float t)
    {
        if (fromPosition == null) { return Vector3.zero; }
        if (toPosition == null) { return Vector3.zero; }
        if (controlPoint == null) { return Vector3.zero; }

        Vector3 point1 = Vector3.Lerp(fromPosition.position, controlPoint.position, t);
        Vector3 point2 = Vector3.Lerp(controlPoint.position, toPosition.position, t);
        Vector3 lerpedPosition = Vector3.Lerp(point1, point2, t);
        return lerpedPosition;
    }

    public static Vector3 GetCubicBezierLerp(Transform fromPosition, Transform toPosition, Transform[] controlPoints, float t)
    {
        if (fromPosition == null) { return Vector3.zero; }
        if (toPosition == null) { return Vector3.zero; }
        if (controlPoints == null) { return Vector3.zero; }
        if (controlPoints.Length < 2) { return Vector3.zero; }

        Vector3 point1 = Vector3.Lerp(fromPosition.position, controlPoints[0].position, t);
        Vector3 point2 = Vector3.Lerp(controlPoints[0].position, controlPoints[1].position, t);
        Vector3 point3 = Vector3.Lerp(controlPoints[1].position, toPosition.position, t);
        Vector3 point12 = Vector3.Lerp(point1, point2, t);
        Vector3 point23 = Vector3.Lerp(point2, point3, t);
        Vector3 lerpedPosition = Vector3.Lerp(point12, point23, t);
        return lerpedPosition;

    }
}
