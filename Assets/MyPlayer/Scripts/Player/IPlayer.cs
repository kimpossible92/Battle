using Characters;
//using Player.MovementDirection;
using UnityEngine;

public interface IPlayer : IPhysicsCharacter
{
    void ChangeMovementDirection(IMovementDirection movementDirection);
    void ChangeMovementDirection(CameraView cameraView);
}
public enum CameraView
{
    AlwaysForward,
    TopView,
    SideView
}
public interface IMovement
{
    void SetUp();
    void Move(Vector3 direction);
    void ChangeMovement(MovementEnum movement);
    void CleanUp();
}
public enum MovementEnum
{
    Ground,
    Midair,
    Crouch,
    Slide,
    Attack
}