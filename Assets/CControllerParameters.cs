using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Collider2D))]

[System.Serializable]
public class CControllerParameters
{
    [Header("Gravity")]
    /// Gravity
    public float Gravity = -30f;
    public float FallMultiplier = 1f;
    public float AscentMultiplier = 1f;

    [Header("Speed")]
    public Vector3 MaxVelocity = new Vector3(100f, 100f,100f);
    public float SpeedAccelerationOnGround = 20f;
    public float SpeedAccelerationInAir = 5f;
    public float SpeedFactor = 1f;

    [Header("Slopes")]
    [Range(0, 90)]
    public float MaximumSlopeAngle = 30f;
    public AnimationCurve SlopeAngleSpeedFactor = new AnimationCurve(new Keyframe(-90f, 1f), new Keyframe(0f, 1f), new Keyframe(90f, 1f));

    [Header("Physics2D Interaction [Experimental]")]
    public bool Physics3DInteraction = true;
    public float Physics3DPushForce = 2.0f;

    [Header("Gizmos")]
    public bool DrawRaycastsGizmos = true;
    public bool DisplayWarnings = true;
}