using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterStates
{
    /// The possible character conditions
    public enum CharacterConditions
    {
        Normal,
        ControlledMovement,
        Frozen,
        Paused,
        Dead
    }
    
    public enum MovementStates
    {
        Null,
        Idle,
        Walking,
        Falling,
        Running,
        Crouching,
        Crawling,
        Dashing,
        LookingUp,
        WallClinging,
        Jetpacking,
        Diving,
        Gripping,
        Dangling,
        Jumping,
        Pushing,
        DoubleJumping,
        WallJumping,
        LadderClimbing,
        SwimmingIdle,
        Gliding,
        Flying,
        FollowingPath,
        LedgeHanging,
        LedgeClimbing
    }
}
