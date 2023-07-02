using UnityEngine;
using System.Collections;
using UnityEditor;

public static class MMDebug
{
    public static MMConsole _console;
    
    public static RaycastHit2D RayCast(Vector3 rayOriginPoint, Vector3 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo = false)
    {
        if (drawGizmo)
        {
            Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
        }
        return Physics2D.Raycast(rayOriginPoint, rayDirection, rayDistance, mask);
    }
    
    public static RaycastHit2D MonoRayCastNonAlloc(RaycastHit2D[] array, Vector2 rayOriginPoint, Vector2 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo = false)
    {
        if (drawGizmo)
        {
            Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
        }
        if (Physics2D.RaycastNonAlloc(rayOriginPoint, rayDirection, array, rayDistance, mask) > 0)
        {
            return array[0];
        }
        return new RaycastHit2D();
    }
    
    public static RaycastHit Raycast3D(Vector3 rayOriginPoint, Vector3 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo = false)
    {
        if (drawGizmo)
        {
            Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
        }
        RaycastHit hit;
        Physics.Raycast(rayOriginPoint, rayDirection, out hit, rayDistance, mask);
        return hit;
    }
    public static bool Raycast3DBoolean(Vector3 rayOriginPoint, Vector3 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo = false)
    {
        if (drawGizmo)
        {
            Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
        }
        RaycastHit hit;
        return Physics.Raycast(rayOriginPoint, rayDirection, out hit, rayDistance, mask);
    }
    public static void DebugLogTime(object message, string color = "")
    {
        string colorPrefix = "";
        string colorSuffix = "";
        if (color != "")
        {
            colorPrefix = "<color=" + color + ">";
            colorSuffix = "</color>";
        }
        Debug.Log(colorPrefix + Time.time + " " + message + colorSuffix);

    }
    
    public static void DebugOnScreen(string message)
    {
        InstantiateOnScreenConsole();
        _console.AddMessage(message);
    }

    public static void DebugOnScreen(string label, object value, int fontSize = 10)
    {
        InstantiateOnScreenConsole(fontSize);
        _console.AddMessage("<b>" + label + "</b> : " + value);
    }
    
    public static void InstantiateOnScreenConsole(int fontSize = 10)
    {
        if (_console == null)
        {
            GameObject newGameObject = new GameObject();
            newGameObject.name = "MMConsole";
            _console = newGameObject.AddComponent<MMConsole>();
            _console.SetFontSize(fontSize);
        }
    }
    
    public static void DrawGizmoArrow(Vector3 origin, Vector3 direction, Color color)
    {
        float arrowHeadLength = 3.00f;
        float arrowHeadAngle = 25.0f;

        Gizmos.color = color;
        Gizmos.DrawRay(origin, direction);

        DrawArrowEnd(true, origin, direction, color, arrowHeadLength, arrowHeadAngle);
    }

    public static void DebugDrawArrow(Vector3 origin, Vector3 direction, Color color)
    {
        float arrowHeadLength = 0.20f;
        float arrowHeadAngle = 35.0f;

        Debug.DrawRay(origin, direction, color);

        DrawArrowEnd(false, origin, direction, color, arrowHeadLength, arrowHeadAngle);
    }
    public static void DebugDrawArrow(Vector3 origin, Vector3 direction, Color color, float arrowLength, float arrowHeadLength = 0.20f, float arrowHeadAngle = 35.0f)
    {
        Debug.DrawRay(origin, direction * arrowLength, color);

        DrawArrowEnd(false, origin, direction * arrowLength, color, arrowHeadLength, arrowHeadAngle);
    }
    
    public static void DebugDrawCross(Vector3 spot, float crossSize, Color color)
    {
        Vector3 tempOrigin = Vector3.zero;
        Vector3 tempDirection = Vector3.zero;

        tempOrigin.x = spot.x - crossSize / 2;
        tempOrigin.y = spot.y - crossSize / 2;
        tempOrigin.z = spot.z;
        tempDirection.x = 1;
        tempDirection.y = 1;
        tempDirection.z = 0;
        Debug.DrawRay(tempOrigin, tempDirection * crossSize, color);

        tempOrigin.x = spot.x - crossSize / 2;
        tempOrigin.y = spot.y + crossSize / 2;
        tempOrigin.z = spot.z;
        tempDirection.x = 1;
        tempDirection.y = -1;
        tempDirection.z = 0;
        Debug.DrawRay(tempOrigin, tempDirection * crossSize, color);
    }
    
    private static void DrawArrowEnd(bool drawGizmos, Vector3 arrowEndPosition, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 40.0f)
    {
        if (direction == Vector3.zero)
        {
            return;
        }
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
        Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
        Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;
        if (drawGizmos)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(arrowEndPosition + direction, right * arrowHeadLength);
            Gizmos.DrawRay(arrowEndPosition + direction, left * arrowHeadLength);
            Gizmos.DrawRay(arrowEndPosition + direction, up * arrowHeadLength);
            Gizmos.DrawRay(arrowEndPosition + direction, down * arrowHeadLength);
        }
        else
        {
            Debug.DrawRay(arrowEndPosition + direction, right * arrowHeadLength, color);
            Debug.DrawRay(arrowEndPosition + direction, left * arrowHeadLength, color);
            Debug.DrawRay(arrowEndPosition + direction, up * arrowHeadLength, color);
            Debug.DrawRay(arrowEndPosition + direction, down * arrowHeadLength, color);
        }
    }
    
    public static void DrawHandlesBounds(Bounds bounds, Color color)
    {
#if UNITY_EDITOR
        Vector3 boundsCenter = bounds.center;
        Vector3 boundsExtents = bounds.extents;

        Vector3 v3FrontTopLeft = new Vector3(boundsCenter.x - boundsExtents.x, boundsCenter.y + boundsExtents.y, boundsCenter.z - boundsExtents.z);  // Front top left corner
        Vector3 v3FrontTopRight = new Vector3(boundsCenter.x + boundsExtents.x, boundsCenter.y + boundsExtents.y, boundsCenter.z - boundsExtents.z);  // Front top right corner
        Vector3 v3FrontBottomLeft = new Vector3(boundsCenter.x - boundsExtents.x, boundsCenter.y - boundsExtents.y, boundsCenter.z - boundsExtents.z);  // Front bottom left corner
        Vector3 v3FrontBottomRight = new Vector3(boundsCenter.x + boundsExtents.x, boundsCenter.y - boundsExtents.y, boundsCenter.z - boundsExtents.z);  // Front bottom right corner
        Vector3 v3BackTopLeft = new Vector3(boundsCenter.x - boundsExtents.x, boundsCenter.y + boundsExtents.y, boundsCenter.z + boundsExtents.z);  // Back top left corner
        Vector3 v3BackTopRight = new Vector3(boundsCenter.x + boundsExtents.x, boundsCenter.y + boundsExtents.y, boundsCenter.z + boundsExtents.z);  // Back top right corner
        Vector3 v3BackBottomLeft = new Vector3(boundsCenter.x - boundsExtents.x, boundsCenter.y - boundsExtents.y, boundsCenter.z + boundsExtents.z);  // Back bottom left corner
        Vector3 v3BackBottomRight = new Vector3(boundsCenter.x + boundsExtents.x, boundsCenter.y - boundsExtents.y, boundsCenter.z + boundsExtents.z);  // Back bottom right corner


        Handles.color = color;

        Handles.DrawLine(v3FrontTopLeft, v3FrontTopRight);
        Handles.DrawLine(v3FrontTopRight, v3FrontBottomRight);
        Handles.DrawLine(v3FrontBottomRight, v3FrontBottomLeft);
        Handles.DrawLine(v3FrontBottomLeft, v3FrontTopLeft);

        Handles.DrawLine(v3BackTopLeft, v3BackTopRight);
        Handles.DrawLine(v3BackTopRight, v3BackBottomRight);
        Handles.DrawLine(v3BackBottomRight, v3BackBottomLeft);
        Handles.DrawLine(v3BackBottomLeft, v3BackTopLeft);

        Handles.DrawLine(v3FrontTopLeft, v3BackTopLeft);
        Handles.DrawLine(v3FrontTopRight, v3BackTopRight);
        Handles.DrawLine(v3FrontBottomRight, v3BackBottomRight);
        Handles.DrawLine(v3FrontBottomLeft, v3BackBottomLeft);
#endif
    }

    public static void DrawSolidRectangle(Vector3 position, Vector3 size, Color borderColor, Color solidColor)
    {
#if UNITY_EDITOR

        Vector3 halfSize = size / 2f;

        Vector3[] verts = new Vector3[4];
        verts[0] = new Vector3(halfSize.x, halfSize.y, halfSize.z);
        verts[1] = new Vector3(-halfSize.x, halfSize.y, halfSize.z);
        verts[2] = new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
        verts[3] = new Vector3(halfSize.x, -halfSize.y, halfSize.z);
        Handles.DrawSolidRectangleWithOutline(verts, solidColor, borderColor);

#endif
    }


    public static void DrawGizmoPoint(Vector3 position, float size, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(position, size);
    }
    public static void DrawCube(Vector3 position, Color color, Vector3 size)
    {
        Vector3 halfSize = size / 2f;

        Vector3[] points = new Vector3[]
        {
                position + new Vector3(halfSize.x,halfSize.y,halfSize.z),
                position + new Vector3(-halfSize.x,halfSize.y,halfSize.z),
                position + new Vector3(-halfSize.x,-halfSize.y,halfSize.z),
                position + new Vector3(halfSize.x,-halfSize.y,halfSize.z),
                position + new Vector3(halfSize.x,halfSize.y,-halfSize.z),
                position + new Vector3(-halfSize.x,halfSize.y,-halfSize.z),
                position + new Vector3(-halfSize.x,-halfSize.y,-halfSize.z),
                position + new Vector3(halfSize.x,-halfSize.y,-halfSize.z),
        };

        Debug.DrawLine(points[0], points[1], color);
        Debug.DrawLine(points[1], points[2], color);
        Debug.DrawLine(points[2], points[3], color);
        Debug.DrawLine(points[3], points[0], color);
    }
    
    public static void DrawGizmoRectangle(Vector2 center, Vector2 size, Color color)
    {
        Gizmos.color = color;

        Vector3 v3TopLeft = new Vector3(center.x - size.x / 2, center.y + size.y / 2, 0);
        Vector3 v3TopRight = new Vector3(center.x + size.x / 2, center.y + size.y / 2, 0); ;
        Vector3 v3BottomRight = new Vector3(center.x + size.x / 2, center.y - size.y / 2, 0); ;
        Vector3 v3BottomLeft = new Vector3(center.x - size.x / 2, center.y - size.y / 2, 0); ;

        Gizmos.DrawLine(v3TopLeft, v3TopRight);
        Gizmos.DrawLine(v3TopRight, v3BottomRight);
        Gizmos.DrawLine(v3BottomRight, v3BottomLeft);
        Gizmos.DrawLine(v3BottomLeft, v3TopLeft);
    }

    public static void DrawRectangle(Rect rectangle, Color color)
    {
        Vector3 pos = new Vector3(rectangle.x + rectangle.width / 2, rectangle.y + rectangle.height / 2, 0.0f);
        Vector3 scale = new Vector3(rectangle.width, rectangle.height, 0.0f);

        MMDebug.DrawRectangle(pos, color, scale);
    }

    public static void DrawRectangle(Vector3 position, Color color, Vector3 size)
    {
        Vector3 halfSize = size / 2f;

        Vector3[] points = new Vector3[]
        {
                position + new Vector3(halfSize.x,halfSize.y,halfSize.z),
                position + new Vector3(-halfSize.x,halfSize.y,halfSize.z),
                position + new Vector3(-halfSize.x,-halfSize.y,halfSize.z),
                position + new Vector3(halfSize.x,-halfSize.y,halfSize.z),
        };

        Debug.DrawLine(points[0], points[1], color);
        Debug.DrawLine(points[1], points[2], color);
        Debug.DrawLine(points[2], points[3], color);
        Debug.DrawLine(points[3], points[0], color);
    }
    
    public static void DrawPoint(Vector3 position, Color color, float size)
    {
        Vector3[] points = new Vector3[]
        {
                position + (Vector3.up * size),
                position - (Vector3.up * size),
                position + (Vector3.right * size),
                position - (Vector3.right * size),
                position + (Vector3.forward * size),
                position - (Vector3.forward * size)
        };

        Debug.DrawLine(points[0], points[1], color);
        Debug.DrawLine(points[2], points[3], color);
        Debug.DrawLine(points[4], points[5], color);
        Debug.DrawLine(points[0], points[2], color);
        Debug.DrawLine(points[0], points[3], color);
        Debug.DrawLine(points[0], points[4], color);
        Debug.DrawLine(points[0], points[5], color);
        Debug.DrawLine(points[1], points[2], color);
        Debug.DrawLine(points[1], points[3], color);
        Debug.DrawLine(points[1], points[4], color);
        Debug.DrawLine(points[1], points[5], color);
        Debug.DrawLine(points[4], points[2], color);
        Debug.DrawLine(points[4], points[3], color);
        Debug.DrawLine(points[5], points[2], color);
        Debug.DrawLine(points[5], points[3], color);

    }
}