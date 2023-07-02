using UnityEngine;
using System.Collections;

public class CControllerState
{
    public bool IsCollidingRight { get; set; }
    public bool IsCollidingLeft { get; set; }
    public bool IsCollidingAbove { get; set; }
    public bool IsCollidingBelow { get; set; }
    public bool HasCollisions { get { return IsCollidingRight || IsCollidingLeft || IsCollidingAbove || IsCollidingBelow; } }
    public float DistanceToLeftCollider;
    public float DistanceToRightCollider;
    public float LateralSlopeAngle { get; set; }
    public float BelowSlopeAngle { get; set; }
    public bool SlopeAngleOK { get; set; }
    public bool OnAMovingPlatform { get; set; }
    public bool IsGrounded { get { return IsCollidingBelow; } }
    public bool IsFalling { get; set; }
    public bool IsJumping { get; set; }
    public bool WasGroundedLastFrame { get; set; }
    public bool WasTouchingTheCeilingLastFrame { get; set; }
    public bool JustGotGrounded { get; set; }
    public bool ColliderResized { get; set; }
    public virtual void Reset()
    {
        IsCollidingLeft = false;
        IsCollidingRight = false;
        IsCollidingAbove = false;
        DistanceToLeftCollider = -1;
        DistanceToRightCollider = -1;
        SlopeAngleOK = false;
        JustGotGrounded = false;
        IsFalling = true;
        IsJumping = false;
        LateralSlopeAngle = 0;
    }
    public override string ToString()
    {
        return string.Format("(controller: collidingRight:{0} collidingLeft:{1} collidingAbove:{2} collidingBelow:{3} lateralSlopeAngle:{4} belowSlopeAngle:{5} isGrounded: {6}",
        IsCollidingRight,
        IsCollidingLeft,
        IsCollidingAbove,
        IsCollidingBelow,
        LateralSlopeAngle,
        BelowSlopeAngle,
        IsGrounded);
    }
}