using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethods
{
    public static bool HasParameterOfType(this Animator self, string name, AnimatorControllerParameterType type)
    {
        if (name == null || name == "") { return false; }
        AnimatorControllerParameter[] parameters = self.parameters;
        foreach (AnimatorControllerParameter currParam in parameters)
        {
            if (currParam.type == type && currParam.name == name)
            {
                return true;
            }
        }
        return false;
    }
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }
    public static bool Intersects(this Rect thisRectangle, Rect otherRectangle)
    {
        return !((thisRectangle.x > otherRectangle.xMax) || (thisRectangle.xMax < otherRectangle.x) || (thisRectangle.y > otherRectangle.yMax) || (thisRectangle.yMax < otherRectangle.y));
    }
    public static bool Contains(this LayerMask mask, int layer)
    {
        return ((mask.value & (1 << layer)) > 0);
    }
    public static bool Contains(this LayerMask mask, GameObject gameobject)
    {
        return ((mask.value & (1 << gameobject.layer)) > 0);
    }

    static List<Component> m_ComponentCache = new List<Component>();
    public static Component GetComponentNoAlloc(this GameObject @this, System.Type componentType)
    {
        @this.GetComponents(componentType, m_ComponentCache);
        var component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
        m_ComponentCache.Clear();
        return component;
    }
    public static T GetComponentNoAlloc<T>(this GameObject @this) where T : Component
    {
        @this.GetComponents(typeof(T), m_ComponentCache);
        var component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
        m_ComponentCache.Clear();
        return component as T;
    }
    public static Vector2 Rotate(this Vector2 vector, float angleInDegrees)
    {
        float sin = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
        float tx = vector.x;
        float ty = vector.y;
        vector.x = (cos * tx) - (sin * ty);
        vector.y = (sin * tx) + (cos * ty);
        return vector;
    }
    public static float NormalizeAngle(this float angleInDegrees)
    {
        angleInDegrees = angleInDegrees % 360f;
        if (angleInDegrees < 0)
        {
            angleInDegrees += 360f;
        }
        return angleInDegrees;
    }

    public static void DestroyAllChildren(this Transform transform)
    {
        for (int t = transform.childCount - 1; t >= 0; t--)
        {
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(transform.GetChild(t).gameObject);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(transform.GetChild(t).gameObject);
            }
        }
    }


}
