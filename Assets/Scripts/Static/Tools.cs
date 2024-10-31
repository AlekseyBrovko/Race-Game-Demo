using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Tools
{
    public static bool RandomByPersent(int persent)
    {
        float random = Random.Range(0f, 100f);
        if (random < persent)
            return true;

        return false;
    }

    public static void BezierFly(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, Transform missile)
    {
        Vector3 p01 = Vector3.Lerp(p0, p1, t);
        Vector3 p12 = Vector3.Lerp(p1, p2, t);
        Vector3 p23 = Vector3.Lerp(p2, p3, t);
        Vector3 p012 = Vector3.Lerp(p01, p12, t);
        Vector3 p123 = Vector3.Lerp(p12, p23, t);
        Vector3 p0123 = Vector3.Lerp(p012, p123, t);
        missile.position = p0123;
        missile.LookAt(p123);
    }

    public static void BezierFly(Vector3 p0, Vector3 p1, Vector3 p2, float t, Transform missile)
    {
        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 ab = Vector3.Lerp(a, b, t);
        missile.position = ab;
        missile.LookAt(b);
    }

    public static Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 p01 = Vector3.Lerp(p0, p1, t);
        Vector3 p12 = Vector3.Lerp(p1, p2, t);
        Vector3 p23 = Vector3.Lerp(p2, p3, t);
        Vector3 p012 = Vector3.Lerp(p01, p12, t);
        Vector3 p123 = Vector3.Lerp(p12, p23, t);
        Vector3 p0123 = Vector3.Lerp(p012, p123, t);
        return p0123;
    }

    public static Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 p01 = Vector3.Lerp(p0, p1, t);
        Vector3 p12 = Vector3.Lerp(p1, p2, t);
        Vector3 p012 = Vector3.Lerp(p01, p12, t);
        return p012;
    }

    public static Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, Transform transform)
    {
        Vector3 p01 = Vector3.Lerp(p0, p1, t);
        Vector3 p12 = Vector3.Lerp(p1, p2, t);
        Vector3 p23 = Vector3.Lerp(p2, p3, t);
        Vector3 p012 = Vector3.Lerp(p01, p12, t);
        Vector3 p123 = Vector3.Lerp(p12, p23, t);
        Vector3 p0123 = Vector3.Lerp(p012, p123, t);
        //transform.LookAt(p0123);
        transform.LookAt(p0123);
        //transform.Rotate(0, -90f, 0);
        return p0123;
    }

    public static Vector3 GetRandomVector3(float range)
    {
        Vector3 result = new Vector3(
            Random.Range(-range, range),
            Random.Range(-range, range),
            Random.Range(-range, range));
        return result;
    }

    public static float GetStartVelocityForBallisticObject(float angleInDegrees, Vector3 startPos, Vector3 targetPos)
    {
        Vector3 fromTo = targetPos - startPos;
        Vector3 fromToXZ = new Vector3(fromTo.x, 0, fromTo.z);

        float x = fromToXZ.magnitude;
        float y = fromTo.y;

        float angleInRad = angleInDegrees * Mathf.Deg2Rad;
        float v2 = (Physics.gravity.y * x * x) / (2f * (y - Mathf.Tan(angleInRad) * x) * Mathf.Pow(Mathf.Cos(angleInRad), 2));
        float result = Mathf.Sqrt(Mathf.Abs(v2));
        return result;
    }

    public static Transform GetNearestTransform(Vector3 position, List<Transform> transforms)
    {
        Transform resultTransform = null;
        float distance = float.PositiveInfinity;
        foreach (var transform in transforms)
        {
            Vector3 tempDirection = transform.position - position;
            float tempDistance = Vector3.SqrMagnitude(tempDirection);
            if (distance > tempDistance)
            {
                distance = tempDistance;
                resultTransform = transform;
            }
        }
        return resultTransform;
    }

    public static bool IsInVisible(Transform transform, Transform targetTransform, LayerMask layerMask)
    {
        var direction = (targetTransform.position + Vector3.up * 0.5f) - (transform.position + Vector3.up);
        var distance = Vector3.Magnitude(direction);
        if (Physics.Raycast(transform.position + Vector3.up, direction, distance, layerMask, QueryTriggerInteraction.Ignore))
            return false;
        return true;
    }

    public static List<Transform> GetSeveralNearestTransforms(Vector3 position, List<Transform> transforms, int amount)
    {
        List<Transform> result = new List<Transform>();
        List<TempNearestTransform> tempNearestTransforms = new List<TempNearestTransform>();

        foreach (var transform in transforms)
        {
            TempNearestTransform tempNearestTransform = new TempNearestTransform();
            tempNearestTransform.Transform = transform;
            tempNearestTransform.SqrDistance = Vector3.SqrMagnitude(transform.position - position);
            tempNearestTransforms.Add(tempNearestTransform);
        }

        tempNearestTransforms = tempNearestTransforms.OrderBy(x => x.SqrDistance).ToList();

        if (amount > transforms.Count)
            amount = transforms.Count;

        for (int i = 0; i < amount; i++)
            result.Add(tempNearestTransforms[i].Transform);            

        return result;
    }

    public static bool ComparisonWithThreshold(float value1, float value2, float threshold)
    {
        float result = Mathf.Abs(value1 - value2);
        if (result > threshold)
            return false;
        return true;
    }

    private class TempNearestTransform
    {
        public Transform Transform { get; set; }
        public float SqrDistance { get; set; }
    }
}