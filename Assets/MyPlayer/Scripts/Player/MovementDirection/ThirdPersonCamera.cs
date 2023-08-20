//using ErbiumCamera;
using General;
using UnityEngine;

public class ThirdPersonCameraDirection :IMovementDirection
{
    
    public Vector3 GetDirection()
    {
        Vector3 forward = ThirdPersonCamera.getCameraForwardDirectionNormalized();
        Vector3 right = ThirdPersonCamera.getCameraRightDirectionNormalized();
        return forward * InputManager2.getVerInput() + right * InputManager2.getHorInput();
    }
    private static UnityEngine.Camera _camera;

 
}
public class ThirdPersonCamera : MonoBehaviour
{

    //private void Start()
    //{
    //    _camera = UnityEngine.Camera.main;
    //}

    public static Vector3 getCameraForwardDirection()
    {
        return Camera.main.transform.forward;
    }

    public static Vector3 getCameraRightDirection()
    {
        return Camera.main.transform.right;
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