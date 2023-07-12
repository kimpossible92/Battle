using UnityEngine;
using UnityEngine.InputSystem;

// ref: https://www.youtube.com/watch?v=-0GFb9l3NHM
public class PlayerMovement : MonoBehaviour
{
    PlayerInputActions input;  // TODO: Move to own class
    Vector2 mousePos;  // TODO: Move to own class
    Vector2 inputVector; // TODO: Move to own class

    [SerializeField]
    private float movementSpeed = 6f;

    [SerializeField]
    private float rotateSpeed = 15f;

    [SerializeField]
    private Camera playerCamera;

    private void Awake()
    {
        // TODO: Move to own class and reference it here
        input = new PlayerInputActions();

        input.Player.Enable();

        input.Player.Mouse.performed += onMouse;
        input.Player.Movement.performed += onMovement;
        input.Player.Movement.canceled += onMovement;
    }

    private void FixedUpdate()
    {
        Vector3 targetVector = new Vector3(inputVector.x, 0, inputVector.y).normalized;
        MoveTowardTarget(targetVector);
        RotateTowardMouseVector();
    }

    private void MoveTowardTarget(Vector3 targetVector)
    {
        var speed = movementSpeed * Time.deltaTime;
        targetVector = Quaternion.Euler(0, playerCamera.gameObject.transform.eulerAngles.y, 0) * targetVector;
        var targetPosition = transform.position + targetVector * speed;
        transform.position = targetPosition;
    }

    private void RotateTowardMouseVector()
    {
        Ray ray = playerCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            transform.LookAt(target);

            // TODO: Figure out how to implement rotation speed
            // transform.rotation = Quaternion.RotateTowards(transform.rotation,
            //Quaternion.LookRotation(target), rotateSpeed); 
            // This is a little buggy
        }
    }

    // TODO: Move to own class
    private void onMovement(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    private void onMouse(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
    }
}
