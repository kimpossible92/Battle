﻿using UnityEngine;
using System.Collections;
public static class MMMaths
{
    public static Vector2 Vector3ToVector2(Vector3 target)
    {
        return new Vector2(target.x, target.y);
    }
    
    public static Vector3 Vector2ToVector3(Vector2 target)
    {
        return new Vector3(target.x, target.y, 0);
    }
    public static Vector3 Vector2ToVector3(Vector2 target, float newZValue)
    {
        return new Vector3(target.x, target.y, newZValue);
    }
    
    public static Vector3 RoundVector3(Vector3 vector)
    {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }

    public static Vector2 RandomVector2(Vector2 minimum, Vector2 maximum)
    {
        return new Vector2(UnityEngine.Random.Range(minimum.x, maximum.x),
                                         UnityEngine.Random.Range(minimum.y, maximum.y));
    }
    public static Vector3 RandomVector3(Vector3 minimum, Vector3 maximum)
    {
        return new Vector3(UnityEngine.Random.Range(minimum.x, maximum.x),
                                         UnityEngine.Random.Range(minimum.y, maximum.y),
                                         UnityEngine.Random.Range(minimum.z, maximum.z));
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
    {
        angle = angle * (Mathf.PI / 180f);
        var rotatedX = Mathf.Cos(angle) * (point.x - pivot.x) - Mathf.Sin(angle) * (point.y - pivot.y) + pivot.x;
        var rotatedY = Mathf.Sin(angle) * (point.x - pivot.x) + Mathf.Cos(angle) * (point.y - pivot.y) + pivot.y;
        return new Vector3(rotatedX, rotatedY, 0);
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
    {
        Vector3 direction = point - pivot;
        direction = Quaternion.Euler(angle) * direction;
        point = direction + pivot;
        return point;
    }
    
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion quaternion)
    {
        Vector3 direction = point - pivot;
        direction = quaternion * direction;
        point = direction + pivot;
        return point;
    }
    
    public static Vector2 RotateVector2(Vector2 vector, float angle)
    {
        if (angle == 0)
        {
            return vector;
        }
        float sinus = Mathf.Sin(angle * Mathf.Deg2Rad);
        float cosinus = Mathf.Cos(angle * Mathf.Deg2Rad);

        float oldX = vector.x;
        float oldY = vector.y;
        vector.x = (cosinus * oldX) - (sinus * oldY);
        vector.y = (sinus * oldX) + (cosinus * oldY);
        return vector;
    }
    
    public static float AngleBetween(Vector2 vectorA, Vector2 vectorB)
    {
        float angle = Vector2.Angle(vectorA, vectorB);
        Vector3 cross = Vector3.Cross(vectorA, vectorB);

        if (cross.z > 0)
        {
            angle = 360 - angle;
        }

        return angle;
    }
    
    public static float DistanceBetweenPointAndLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        return Vector3.Magnitude(ProjectPointOnLine(point, lineStart, lineEnd) - point);
    }
    
    public static Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 rhs = point - lineStart;
        Vector3 vector2 = lineEnd - lineStart;
        float magnitude = vector2.magnitude;
        Vector3 lhs = vector2;
        if (magnitude > 1E-06f)
        {
            lhs = (Vector3)(lhs / magnitude);
        }
        float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
        return (lineStart + ((Vector3)(lhs * num2)));
    }
    public static int Sum(params int[] thingsToAdd)
    {
        int result = 0;
        for (int i = 0; i < thingsToAdd.Length; i++)
        {
            result += thingsToAdd[i];
        }
        return result;
    }
    
    public static int RollADice(int numberOfSides)
    {
        return (UnityEngine.Random.Range(1, numberOfSides + 1));
    }
    
    public static bool Chance(int percent)
    {
        return (UnityEngine.Random.Range(0, 100) <= percent);
    }

    public static float Approach(float from, float to, float amount)
    {
        if (from < to)
        {
            from += amount;
            if (from > to)
            {
                return to;
            }
        }
        else
        {
            from -= amount;
            if (from < to)
            {
                return to;
            }
        }
        return from;
    }
    
    public static float Remap(float x, float A, float B, float C, float D)
    {
        float remappedValue = C + (x - A) / (B - A) * (D - C);
        return remappedValue;
    }

    public static float ClampAngle(float angle, float minimumAngle, float maximumAngle)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, minimumAngle, maximumAngle);
    }
    
    public static float RoundToClosest(float value, float[] possibleValues)
    {
        if (possibleValues.Length == 0)
        {
            return 0f;
        }

        float closestValue = possibleValues[0];

        foreach (float possibleValue in possibleValues)
        {
            if (Mathf.Abs(closestValue - value) > Mathf.Abs(possibleValue - value))
            {
                closestValue = possibleValue;
            }
        }
        return closestValue;

    }
    
    public static Vector3 DirectionFromAngle(float angle, float additionalAngle)
    {
        angle += additionalAngle;

        Vector3 direction = Vector3.zero;
        direction.x = Mathf.Sin(angle * Mathf.Deg2Rad);
        direction.y = 0f;
        direction.z = Mathf.Cos(angle * Mathf.Deg2Rad);
        return direction;
    }
    public static Vector3 DirectionFromAngle2D(float angle, float additionalAngle)
    {
        angle += additionalAngle;

        Vector3 direction = Vector3.zero;
        direction.x = Mathf.Sin(angle * Mathf.Deg2Rad);
        direction.y = Mathf.Cos(angle * Mathf.Deg2Rad);
        direction.z = 0f;
        return direction;
    }
}
