using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMann : MonoBehaviour
{
    private static UnityEngine.Camera _camera;

    private void Start()
    {
        _camera = UnityEngine.Camera.main;
    }

    public static Vector3 getCameraForwardDirection()
    {
        return _camera.transform.forward;
    }

    public static Vector3 getCameraRightDirection()
    {
        return _camera.transform.right;
    }

    public static Vector3 getCameraForwardDirectionNormalized()
    {
        Vector3 forward = getCameraForwardDirection();
        forward.y = 0;
        return forward.normalized;
    }

    public static Vector3 getCameraRightDirectionNormalized()
    {
        Vector3 right = getCameraRightDirection();
        right.y = 0;
        return right.normalized;
    }
}
