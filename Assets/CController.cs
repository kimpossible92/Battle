using UnityEngine;
using System.Collections;
using Unity.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class CController : MonoBehaviour
{
    public CControllerState State { get; protected set; }
    [Header("Default Parameters")]
    public CControllerParameters DefaultParameters;
    public CControllerParameters Parameters { get { return _overrideParameters ?? DefaultParameters; } }

    [Header("Collision Masks")]
    public LayerMask PlatformMask = 0;
    public LayerMask MovingPlatformMask = 0;
    public LayerMask OneWayPlatformMask = 0;
    public LayerMask MovingOneWayPlatformMask = 0;
    public LayerMask MidHeightOneWayPlatformMask = 0;
    public enum RaycastDirections { up, down, left, right };
    public enum DetachmentMethods { Layer, Object }
    public DetachmentMethods DetachmentMethod = DetachmentMethods.Layer;
    [ReadOnly]
    public GameObject StandingOn;
    public GameObject StandingOnLastFrame { get; protected set; }
    public Collider StandingOnCollider { get; protected set; }
    public Vector3 Speed { get { return _speed; } }
    public Vector3 ForcesApplied { get; protected set; }
    public GameObject CurrentWallCollider { get; protected set; }

    [Header("Raycasting")]
    public int NumberOfHorizontalRays = 8;
    public int NumberOfVerticalRays = 8;	
    public float RayOffset = 0.05f;
    public float CrouchedRaycastLengthMultiplier = 1f;
    public bool CastRaysOnBothSides = false;

    [Header("Stickiness")]
    public bool StickToSlopes = false;
    public float StickyRaycastLength = 0f;
    public float StickToSlopesOffsetY = 0.2f;

    [Header("Safety")]
    public bool AutomaticGravitySettings = true;

    public Vector3 ColliderSize
    {
        get
        {
            return Vector3.Scale(transform.localScale, _boxCollider.size);
        }
    }

    public Vector3 ColliderCenterPosition
    {
        get
        {
            return _boxCollider.bounds.center;
        }
    }

    public virtual Vector3 ColliderBottomPosition
    {
        get
        {
            _colliderBottomCenterPosition.x = _boxCollider.bounds.center.x;
            _colliderBottomCenterPosition.y = _boxCollider.bounds.min.y;
            _colliderBottomCenterPosition.z = 0;
            return _colliderBottomCenterPosition;
        }
    }

    public virtual Vector3 ColliderLeftPosition
    {
        get
        {
            _colliderLeftCenterPosition.x = _boxCollider.bounds.min.x;
            _colliderLeftCenterPosition.y = _boxCollider.bounds.center.y;
            _colliderLeftCenterPosition.z = 0;
            return _colliderLeftCenterPosition;
        }
    }

    public virtual Vector3 ColliderTopPosition
    {
        get
        {
            _colliderTopCenterPosition.x = _boxCollider.bounds.center.x;
            _colliderTopCenterPosition.y = _boxCollider.bounds.max.y;
            _colliderTopCenterPosition.z = 0;
            return _colliderTopCenterPosition;
        }
    }

    public virtual Vector3 ColliderRightPosition
    {
        get
        {
            _colliderRightCenterPosition.x = _boxCollider.bounds.max.x;
            _colliderRightCenterPosition.y = _boxCollider.bounds.center.y;
            _colliderRightCenterPosition.z = 0;
            return _colliderRightCenterPosition;
        }
    }

    public float Friction
    {
        get
        {
            return _friction;
        }
    }

    public virtual Vector3 BoundsTopLeftCorner
    {
        get
        {
            return _boundsTopLeftCorner;
        }
    }

    public virtual Vector3 BoundsBottomLeftCorner
    {
        get
        {
            return _boundsBottomLeftCorner;
        }
    }

    public virtual Vector3 BoundsTopRightCorner
    {
        get
        {
            return _boundsTopRightCorner;
        }
    }

    public virtual Vector3 BoundsBottomRightCorner
    {
        get
        {
            return _boundsBottomRightCorner;
        }
    }

    public virtual Vector3 BoundsTop
    {
        get
        {
            return (_boundsTopLeftCorner + _boundsTopRightCorner) / 2;
        }
    }

    public virtual Vector3 BoundsBottom
    {
        get
        {
            return (_boundsBottomLeftCorner + _boundsBottomRightCorner) / 2;
        }
    }

    public virtual Vector3 BoundsRight
    {
        get
        {
            return (_boundsTopRightCorner + _boundsBottomRightCorner) / 2;
        }
    }

    public virtual Vector3 BoundsLeft
    {
        get
        {
            return (_boundsTopLeftCorner + _boundsBottomLeftCorner) / 2;
        }
    }

    public virtual Vector3 BoundsCenter
    {
        get
        {
            return _boundsCenter;
        }
    }


    public virtual Vector3 ExternalForce
    {
        get
        {
            return _externalForce;
        }
    }
    protected CControllerParameters _overrideParameters;
    protected Vector3 _speed;
    protected float _friction = 0;
    protected float _fallSlowFactor;
    protected float _currentGravity = 0;
    protected Vector3 _externalForce;
    protected Vector3 _newPosition;
    protected Transform _transform;
    protected BoxCollider _boxCollider;
    protected LayerMask _platformMaskSave;
    protected LayerMask _raysBelowLayerMaskPlatforms;
    protected LayerMask _raysBelowLayerMaskPlatformsWithoutOneWay;
    protected LayerMask _raysBelowLayerMaskPlatformsWithoutMidHeight;
    protected int _savedBelowLayer;
    protected MMPathMovement _movingPlatform = null;
    protected float _movingPlatformCurrentGravity;
    protected bool _gravityActive = true;
    protected Collider _ignoredCollider = null;

    protected const float _smallValue = 0.0001f;
    protected const float _obstacleHeightTolerance = 0.05f;
    protected const float _movingPlatformsGravity = -500;

    protected Vector3 _originalColliderSize;
    protected Vector3 _originalColliderOffset;
    protected Vector3 _originalSizeRaycastOrigin;

    protected Vector3 _crossBelowSlopeAngle;

    protected RaycastHit[] _sideHitsStorage;
    protected RaycastHit[] _belowHitsStorage;
    protected RaycastHit[] _aboveHitsStorage;
    protected RaycastHit _stickRaycast;
    protected float _movementDirection;
    protected float _storedMovementDirection = 1;
    protected const float _movementDirectionThreshold = 0.0001f;

    protected Vector3 _horizontalRayCastFromBottom = Vector3.zero;
    protected Vector3 _horizontalRayCastToTop = Vector3.zero;
    protected Vector3 _verticalRayCastFromLeft = Vector3.zero;
    protected Vector3 _verticalRayCastToRight = Vector3.zero;
    protected Vector3 _aboveRayCastStart = Vector3.zero;
    protected Vector3 _aboveRayCastEnd = Vector3.zero;
    protected Vector3 _stickRayCastOrigin = Vector3.zero;

    protected Vector3 _colliderBottomCenterPosition;
    protected Vector3 _colliderLeftCenterPosition;
    protected Vector3 _colliderRightCenterPosition;
    protected Vector3 _colliderTopCenterPosition;

    protected MMPathMovement _movingPlatformTest;
    protected SurfaceModifier _frictionTest;

    protected RaycastHit2D[] _raycastNonAlloc = new RaycastHit2D[0];

    protected Vector3 _boundsTopLeftCorner;
    protected Vector3 _boundsBottomLeftCorner;
    protected Vector3 _boundsTopRightCorner;
    protected Vector3 _boundsBottomRightCorner;
    protected Vector3 _boundsCenter;
    protected float _boundsWidth;
    protected float _boundsHeight;
    protected bool[] aboveHitsStorage2;
    protected bool[] belowHitsStorage2;
    protected List<RaycastHit> _contactList;
    protected float _movementDirection2, _storedMovementDirection2;
    protected virtual void Awake()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        _transform = transform;
        _boxCollider = (BoxCollider)GetComponent<BoxCollider>();
        _originalColliderSize = _boxCollider.size;
        _originalColliderOffset = _boxCollider.center;
        
        if ((_boxCollider.center.x != 0) && (Parameters.DisplayWarnings))
        {
            Debug.LogWarning("The boxcollider for " + gameObject.name + " should have an x offset set to zero.");
        }
        
        _contactList = new List<RaycastHit>();
        State = new CControllerState();
	
        _platformMaskSave = PlatformMask;
        PlatformMask |= OneWayPlatformMask;
        PlatformMask |= MovingPlatformMask;
        PlatformMask |= MovingOneWayPlatformMask;
        PlatformMask |= MidHeightOneWayPlatformMask;

        _sideHitsStorage = new RaycastHit[NumberOfHorizontalRays];

        _belowHitsStorage = new RaycastHit[NumberOfVerticalRays];
        _aboveHitsStorage = new RaycastHit[NumberOfVerticalRays];
        aboveHitsStorage2 = new bool[NumberOfVerticalRays];
        belowHitsStorage2 = new bool[NumberOfVerticalRays];
        CurrentWallCollider = null;
        State.Reset();
        SetRaysParameters();

        if (AutomaticGravitySettings)
        {
            CharacterGravity characterGravity = this.gameObject.GetComponentNoAlloc<CharacterGravity>();
            if (characterGravity == null)
            {
                this.transform.rotation = Quaternion.identity;
            }
        }
    }
    #region CollisionsController
    protected virtual void SetStates()
    {
        if (!State.WasGroundedLastFrame && State.IsCollidingBelow)
        {
            State.JustGotGrounded = true;
        }

        if (State.IsCollidingLeft || State.IsCollidingRight || State.IsCollidingBelow || State.IsCollidingAbove)
        {
            OnCorgiColliderHit();
        }
    }
    protected virtual void FrameInitialization()
    {
        _contactList.Clear();
        _newPosition = Speed * Time.deltaTime;
        State.WasGroundedLastFrame = State.IsCollidingBelow;
        StandingOnLastFrame = StandingOn;
        State.WasTouchingTheCeilingLastFrame = State.IsCollidingAbove;
        CurrentWallCollider = null;
        State.Reset();
    }
    protected virtual void Update()
    {
        EveryFrame();
    }
    protected virtual void EveryFrame()
    {
        //ApplyGravity();
        FrameInitialization();

        SetRaysParameters();
        HandleMovingPlatforms();
        
        ForcesApplied = _speed;
        

        DetermineMovementDirection();
        if (CastRaysOnBothSides)
        {
            CastRaysToTheLeft();
            CastRaysToTheRight();
        }
        else
        {
            if (_movementDirection == -1)
            {
                CastRaysToTheLeft();
            }
            else
            {
                CastRaysToTheRight();
            }
        }
        CastRaysBelow();
        CastRaysAbove();

        _transform.Translate(_newPosition, Space.Self);
        //print(_newPosition);
        SetRaysParameters();
        ComputeNewSpeed();
        SetStates();

        _externalForce.x = 0;
        _externalForce.y = 0;

        FrameExit();
    }

    protected virtual void DetermineMovementDirectionZ()
    {
        _movementDirection2 = _storedMovementDirection2;
        if ((_speed.z < -_movementDirectionThreshold) || (_externalForce.z < -_movementDirectionThreshold))
        {
            _movementDirection2 = -1;
        }
        else if ((_speed.z > _movementDirectionThreshold) || (_externalForce.z > _movementDirectionThreshold))
        {
            _movementDirection2 = 1;
        }

        if (_movingPlatform != null)
        {
            if (Mathf.Abs(_movingPlatform.CurrentSpeed.z) > Mathf.Abs(_speed.z))
            {
                _movementDirection2 = Mathf.Sign(_movingPlatform.CurrentSpeed.z);
            }
        }
        _storedMovementDirection2 = _movementDirection;
    }
    #region DeterminApplyHandleMovement
    protected virtual void DetermineMovementDirection()
    {
        _movementDirection = _storedMovementDirection;
        if ((_speed.x < -_movementDirectionThreshold) || (_externalForce.x < -_movementDirectionThreshold))
        {
            _movementDirection = -1;
        }
        else if ((_speed.x > _movementDirectionThreshold) || (_externalForce.x > _movementDirectionThreshold))
        {
            _movementDirection = 1;
        }

        if (_movingPlatform != null)
        {
            if (Mathf.Abs(_movingPlatform.CurrentSpeed.x) > Mathf.Abs(_speed.x))
            {
                _movementDirection = Mathf.Sign(_movingPlatform.CurrentSpeed.x);
            }
        }
        _storedMovementDirection = _movementDirection;
        DetermineMovementDirectionZ();
    }

    protected virtual void ApplyGravity()
    {
        _currentGravity = Parameters.Gravity;
        if (_speed.y > 0)
        {
            _currentGravity = _currentGravity / Parameters.AscentMultiplier;
        }
        if (_speed.y < 0)
        {
            _currentGravity = _currentGravity * Parameters.FallMultiplier;
        }


        if (_gravityActive)
        {
            _speed.y += (_currentGravity + _movingPlatformCurrentGravity) * Time.deltaTime;
        }

        if (_fallSlowFactor != 0)
        {
            _speed.y *= _fallSlowFactor;
        }
    }
    protected virtual void HandleMovingPlatforms()
    {
        if (_movingPlatform != null)
        {
            if (!float.IsNaN(_movingPlatform.CurrentSpeed.x) && !float.IsNaN(_movingPlatform.CurrentSpeed.y) && !float.IsNaN(_movingPlatform.CurrentSpeed.z))
            {
                _transform.Translate(_movingPlatform.CurrentSpeed * Time.deltaTime);
            }

            if ((Time.timeScale == 0) || float.IsNaN(_movingPlatform.CurrentSpeed.x) || float.IsNaN(_movingPlatform.CurrentSpeed.y) || float.IsNaN(_movingPlatform.CurrentSpeed.z))
            {
                return;
            }

            if ((Time.deltaTime <= 0))
            {
                return;
            }

            State.OnAMovingPlatform = true;

            GravityActive(false);

            _movingPlatformCurrentGravity = _movingPlatformsGravity;

            _newPosition.y = _movingPlatform.CurrentSpeed.y * Time.deltaTime;

            _speed = -_newPosition / Time.deltaTime;
            _speed.x = -_speed.x;

            SetRaysParameters();
        }
        else {  }
    }
    #endregion
    protected virtual void CastRaysAbove()
    {
        if (_newPosition.y < 0)
            return;

        float rayLength = State.IsGrounded ? RayOffset : _newPosition.y;
        rayLength += _boundsHeight / 2;

        bool hitConnected = false;

        _aboveRayCastStart = (_boundsBottomLeftCorner + _boundsTopLeftCorner) / 2;
        _aboveRayCastEnd = (_boundsBottomRightCorner + _boundsTopRightCorner) / 2;

        _aboveRayCastStart += (Vector3)transform.right * _newPosition.x;
        _aboveRayCastEnd += (Vector3)transform.right * _newPosition.x;

        if (_aboveHitsStorage.Length != NumberOfVerticalRays)
        {
            _aboveHitsStorage = new RaycastHit[NumberOfVerticalRays];
        }

        float smallestDistance = float.MaxValue;

        for (int i = 0; i < NumberOfVerticalRays; i++)
        {
            Vector3 rayOriginPoint = Vector3.Lerp(_aboveRayCastStart, _aboveRayCastEnd, (float)i / (float)(NumberOfVerticalRays - 1));
            Color color1 = Color.cyan;
            _aboveHitsStorage[i] = MMDebug.Raycast3D(rayOriginPoint, (transform.up), rayLength, PlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask, color1, Parameters.DrawRaycastsGizmos);
            aboveHitsStorage2[i] = MMDebug.Raycast3DBoolean(rayOriginPoint, (transform.up), rayLength, PlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask, color1, Parameters.DrawRaycastsGizmos);
            if (aboveHitsStorage2[i])
            {
                hitConnected = true;
                if (_aboveHitsStorage[i].collider == _ignoredCollider)
                {
                    break;
                }
                if (_aboveHitsStorage[i].distance < smallestDistance)
                {
                    smallestDistance = _aboveHitsStorage[i].distance;
                }
            }
        }

        if (hitConnected)
        {
            _newPosition.y = smallestDistance - _boundsHeight / 2;

            if ((State.IsGrounded) && (_newPosition.y < 0))
            {
                _newPosition.y = 0;
            }

            State.IsCollidingAbove = true;

            if (!State.WasTouchingTheCeilingLastFrame)
            {
                _speed = new Vector3(_speed.x, 0f,0f);
            }
        }
    }
    protected virtual void FrameExit()
    {
        if (StandingOnLastFrame != null)
        {
            StandingOnLastFrame.layer = _savedBelowLayer;
        }
    }

    public virtual void SetRaysParameters()
    {
        float top = _boxCollider.center.y + (_boxCollider.size.y / 2f);
        float bottom = _boxCollider.center.y - (_boxCollider.size.y / 2f);
        float left = _boxCollider.center.x - (_boxCollider.size.x / 2f);
        float right = _boxCollider.center.x + (_boxCollider.size.x / 2f);
        float up = _boxCollider.center.z + (_boxCollider.size.z / 2f);
        float back = _boxCollider.center.z - (_boxCollider.size.z / 2f);

        _boundsTopLeftCorner.x = left;
        _boundsTopLeftCorner.y = top;
        _boundsTopLeftCorner.z = up;

        _boundsTopRightCorner.x = right;
        _boundsTopRightCorner.y = top;
        _boundsTopRightCorner.z = up;

        _boundsBottomLeftCorner.x = left;
        _boundsBottomLeftCorner.y = bottom;
        _boundsBottomLeftCorner.z = back;

        _boundsBottomRightCorner.x = right;
        _boundsBottomRightCorner.y = bottom;
        _boundsBottomRightCorner.z = back;

        _boundsTopLeftCorner = transform.TransformPoint(_boundsTopLeftCorner);
        _boundsTopRightCorner = transform.TransformPoint(_boundsTopRightCorner);
        _boundsBottomLeftCorner = transform.TransformPoint(_boundsBottomLeftCorner);
        _boundsBottomRightCorner = transform.TransformPoint(_boundsBottomRightCorner);
        _boundsCenter = _boxCollider.bounds.center;

        //_boundsWidth = Vector2.Distance(_boundsBottomLeftCorner, _boundsBottomRightCorner);
        //_boundsHeight = Vector2.Distance(_boundsBottomLeftCorner, _boundsTopLeftCorner);
    }

    public virtual void SetIgnoreCollider(Collider newIgnoredCollider)
    {
        _ignoredCollider = newIgnoredCollider;
    }

    public virtual IEnumerator DisableCollisions(float duration)
    {
        CollisionsOff();
        yield return new WaitForSeconds(duration);
        CollisionsOn();
    }
    
    public virtual void CollisionsOn()
    {
        PlatformMask = _platformMaskSave;
        PlatformMask |= OneWayPlatformMask;
        PlatformMask |= MovingPlatformMask;
        PlatformMask |= MovingOneWayPlatformMask;
        PlatformMask |= MidHeightOneWayPlatformMask;
    }
    
    public virtual void CollisionsOff()
    {
        PlatformMask = 0;
    }
    
    public virtual IEnumerator DisableCollisionsWithOneWayPlatforms(float duration)
    {
        if (DetachmentMethod == DetachmentMethods.Layer)
        {
            CollisionsOffWithOneWayPlatformsLayer();
            yield return new WaitForSeconds(duration);
            CollisionsOn();
        }
        else
        {
            SetIgnoreCollider(StandingOnCollider);
            yield return new WaitForSeconds(duration);
            SetIgnoreCollider(null);
        }
    }
    
    public virtual IEnumerator DisableCollisionsWithMovingPlatforms(float duration)
    {
        if (DetachmentMethod == DetachmentMethods.Layer)
        {
            CollisionsOffWithMovingPlatformsLayer();
            yield return new WaitForSeconds(duration);
            CollisionsOn();
        }
        else
        {
            SetIgnoreCollider(StandingOnCollider);
            yield return new WaitForSeconds(duration);
            SetIgnoreCollider(null);
        }
    }
    public virtual void SetForce(Vector2 force)
    {
        _speed = force;
        _externalForce = force;
    }
    public virtual void GravityActive(bool state)
    {
        if (state)
        {
            _gravityActive = true;
        }
        else
        {
            _gravityActive = false;
        }
    }
    public virtual void SlowFall(float factor)
    {
        _fallSlowFactor = factor;
    }

    public virtual void SetHorizontalForce(float x)
    {
        _speed.x = x;
        _externalForce.x = x;
    }
    public virtual void SetVerticalForce(float z)
    {
        _speed.z = z;
        _externalForce.z = z;

    }
    public virtual void SetVerticalForce2(float y)
    {
        _speed.y = y;
        _externalForce.y = y;

    }
    protected virtual void CastRaysToTheLeft()
    {
        CastRaysToTheSides(-1);
    }

    protected virtual void CastRaysToTheRight()
    {
        CastRaysToTheSides(1);
    }
    
    protected virtual void CastRaysToTheSides(float raysDirection)
    {
        _horizontalRayCastFromBottom = (_boundsBottomRightCorner + _boundsBottomLeftCorner) / 2;
        _horizontalRayCastToTop = (_boundsTopLeftCorner + _boundsTopRightCorner) / 2;
        _horizontalRayCastFromBottom = _horizontalRayCastFromBottom + (Vector3)transform.up * _obstacleHeightTolerance;
        _horizontalRayCastToTop = _horizontalRayCastToTop - (Vector3)transform.up * _obstacleHeightTolerance;
        
        float horizontalRayLength = Mathf.Abs(_speed.x * Time.deltaTime) + _boundsWidth / 2 + RayOffset * 2;
        
        if (_sideHitsStorage.Length != NumberOfHorizontalRays)
        {
            _sideHitsStorage = new RaycastHit[NumberOfHorizontalRays];
        }
        
        for (int i = 0; i < NumberOfHorizontalRays; i++)
        {
            Vector3 rayOriginPoint = Vector3.Lerp(_horizontalRayCastFromBottom, _horizontalRayCastToTop, (float)i / (float)(NumberOfHorizontalRays - 1));
            
            if (State.WasGroundedLastFrame && i == 0)
            {
                Color color1 = Color.white;
                _sideHitsStorage[i] = MMDebug.Raycast3D(rayOriginPoint, raysDirection * (transform.right), horizontalRayLength, PlatformMask, color1, Parameters.DrawRaycastsGizmos);
            }
            else
            {
                Color color2 = Color.white;
                _sideHitsStorage[i] = MMDebug.Raycast3D(rayOriginPoint, raysDirection * (transform.right), horizontalRayLength, PlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask, color2, Parameters.DrawRaycastsGizmos);
            }
            if (_sideHitsStorage[i].distance > 0)
            {
                if (_sideHitsStorage[i].collider == _ignoredCollider)
                {
                    break;
                }
                
                float hitAngle = Mathf.Abs(Vector3.Angle(_sideHitsStorage[i].normal, transform.up));
                
                if (_movementDirection == raysDirection)
                {
                    State.LateralSlopeAngle = hitAngle;
                }
                if (hitAngle > Parameters.MaximumSlopeAngle)
                {
                    if (raysDirection < 0)
                    {
                        State.IsCollidingLeft = true;
                        State.DistanceToLeftCollider = _sideHitsStorage[i].distance;
                    }
                    else
                    {
                        State.IsCollidingRight = true;
                        State.DistanceToRightCollider = _sideHitsStorage[i].distance;
                    }

                    if (_movementDirection == raysDirection)
                    {
                        CurrentWallCollider = _sideHitsStorage[i].collider.gameObject;
                        State.SlopeAngleOK = false;

                        float distance = MMMaths.DistanceBetweenPointAndLine(_sideHitsStorage[i].point, _horizontalRayCastFromBottom, _horizontalRayCastToTop);
                        if (raysDirection <= 0)
                        {
                            _newPosition.x = -distance
                                + _boundsWidth / 2
                                + RayOffset * 2;
                        }
                        else
                        {
                            _newPosition.x = distance
                                - _boundsWidth / 2
                                - RayOffset * 2;
                        }
                        
                        if (!State.IsGrounded && (Speed.y != 0))
                        {
                            _newPosition.x = 0;
                        }

                        _contactList.Add(_sideHitsStorage[i]);
                        _speed.x = 0;
                    }

                    break;
                }
            }
        }


    }
    public virtual void CollisionsOffWithOneWayPlatformsLayer()
    {

        PlatformMask -= OneWayPlatformMask;
        PlatformMask -= MovingOneWayPlatformMask;
        PlatformMask -= MidHeightOneWayPlatformMask;
    }
    
    public virtual void CollisionsOffWithMovingPlatformsLayer()
    {
        PlatformMask -= MovingPlatformMask;
        PlatformMask -= MovingOneWayPlatformMask;
    }
    #endregion

    public virtual void AddForce(Vector3 force)
    {
        _speed += force;
        _externalForce += force;
    }

    public virtual void AddHorizontalForce(float x)
    {
        _speed.x += x;
        _externalForce.x += x;
    }
    public virtual void AddVertForce(float z)
    {
        _speed.z += z;
        _externalForce.z += z;
    }
    public virtual float Width()
    {
        return _boundsWidth;
    }
    public virtual float Height()
    {
        return _boundsHeight;
    }
    public virtual void DetachFromMovingPlatform()
    {
        if (_movingPlatform == null)
        {
            return;
        }
        GravityActive(true);
        State.OnAMovingPlatform = false;
        _movingPlatform = null;
        _movingPlatformCurrentGravity = 0;
    }
    protected virtual void OnCorgiColliderHit()
    {
        foreach (RaycastHit hit in _contactList)
        {
            if (Parameters.Physics3DInteraction)
            {
                Rigidbody body = hit.collider.attachedRigidbody;
                if (body == null || body.isKinematic 
                    //|| body.bodyType == RigidbodyType2D.Static
                    )
                {
                    return;
                }
                Vector3 pushDirection = new Vector3(_externalForce.x, 0, 0);
                body.velocity = pushDirection.normalized * Parameters.Physics3DPushForce;
            }
        }
    }
    public virtual void AddVerticalForce(float y)
    {
        _speed.y += y;
        _externalForce.y += y;
    }
    protected virtual void CastRaysBelow()
    {
        State.IsCollidingBelow = true;
        _friction = 0;

        if (_newPosition.y < -_smallValue)
        {
            State.IsFalling = true; 
        }
        else
        {
            State.IsFalling = false;
        }

        if ((Parameters.Gravity > 0) && (!State.IsFalling))
        {
            State.IsCollidingBelow = false; 
            return;
        }
        float rayLength = _boundsHeight / 2 + RayOffset;

        if (State.OnAMovingPlatform)
        {
            rayLength *= 2;
        }

        if (_newPosition.y < 0)
        {
            rayLength += Mathf.Abs(_newPosition.y);
        }

        _verticalRayCastFromLeft = (_boundsBottomLeftCorner + _boundsTopLeftCorner) / 2;
        _verticalRayCastToRight = (_boundsBottomRightCorner + _boundsTopRightCorner) / 2;
        _verticalRayCastFromLeft += (Vector3)transform.up * RayOffset;
        _verticalRayCastToRight += (Vector3)transform.up * RayOffset;
        _verticalRayCastFromLeft += (Vector3)transform.right * _newPosition.x;
        _verticalRayCastToRight += (Vector3)transform.right * _newPosition.x;

        if (_belowHitsStorage.Length != NumberOfVerticalRays)
        {
            _belowHitsStorage = new RaycastHit[NumberOfVerticalRays];
        }

        _raysBelowLayerMaskPlatforms = PlatformMask;

        _raysBelowLayerMaskPlatformsWithoutOneWay = PlatformMask & ~MidHeightOneWayPlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask;
        _raysBelowLayerMaskPlatformsWithoutMidHeight = _raysBelowLayerMaskPlatforms & ~MidHeightOneWayPlatformMask;
        if (StandingOnLastFrame != null)
        {
            _savedBelowLayer = StandingOnLastFrame.layer;
            if (MidHeightOneWayPlatformMask.Contains(StandingOnLastFrame.layer))
            {
                //StandingOnLastFrame.layer = LayerMask.NameToLayer("Platforms");
            }
        }
        if (State.WasGroundedLastFrame)
        {
            if (StandingOnLastFrame != null)
            {
                if (!MidHeightOneWayPlatformMask.Contains(StandingOnLastFrame.layer))
                {
                    _raysBelowLayerMaskPlatforms = _raysBelowLayerMaskPlatformsWithoutMidHeight;
                }
            }
        }

        float smallestDistance = float.MaxValue;
        int smallestDistanceIndex = 0;
        bool hitConnected = false;
        //print("dasdsa2");
        for (int i = 0; i < NumberOfVerticalRays; i++)
        {
            Vector3 rayOriginPoint = Vector3.Lerp(_verticalRayCastFromLeft, _verticalRayCastToRight, (float)i / (float)(NumberOfVerticalRays - 1));
            
            if ((_newPosition.y > 0) && (!State.WasGroundedLastFrame))
            {
                _belowHitsStorage[i] = MMDebug.Raycast3D(rayOriginPoint, -transform.up, rayLength, _raysBelowLayerMaskPlatformsWithoutOneWay, Color.blue, Parameters.DrawRaycastsGizmos);
                belowHitsStorage2[i] = MMDebug.Raycast3DBoolean(rayOriginPoint, -transform.up, rayLength, _raysBelowLayerMaskPlatformsWithoutOneWay, Color.blue, Parameters.DrawRaycastsGizmos);
            }
            else
            {
                _belowHitsStorage[i] = MMDebug.Raycast3D(rayOriginPoint, -transform.up, rayLength, _raysBelowLayerMaskPlatforms, Color.blue, Parameters.DrawRaycastsGizmos);
                belowHitsStorage2[i] = MMDebug.Raycast3DBoolean(rayOriginPoint, -transform.up, rayLength, _raysBelowLayerMaskPlatforms, Color.blue, Parameters.DrawRaycastsGizmos);
            }
            
            float distance = MMMaths.DistanceBetweenPointAndLine(_belowHitsStorage[smallestDistanceIndex].point, _verticalRayCastFromLeft, _verticalRayCastToRight);
            if (distance < _smallValue)
            {
                break;
            }

            if (belowHitsStorage2[i])
            {
                if (_belowHitsStorage[i].collider == _ignoredCollider)
                {
                    continue;
                }

                hitConnected = true;
                State.BelowSlopeAngle = Vector3.Angle(_belowHitsStorage[i].normal, transform.up);
                _crossBelowSlopeAngle = Vector3.Cross(transform.up, _belowHitsStorage[i].normal);
                if (_crossBelowSlopeAngle.z < 0)
                {
                    State.BelowSlopeAngle = -State.BelowSlopeAngle;
                }

                if (_belowHitsStorage[i].distance < smallestDistance)
                {
                    smallestDistanceIndex = i;
                    smallestDistance = _belowHitsStorage[i].distance;
                }
            }
        }
        //print(hitConnected);
        if (hitConnected)
        {
            StandingOn = _belowHitsStorage[smallestDistanceIndex].collider.gameObject;
            StandingOnCollider = _belowHitsStorage[smallestDistanceIndex].collider;

            // if the character is jumping onto a (1-way) platform but not high enough, we do nothing
            if (
                !State.WasGroundedLastFrame
                && (smallestDistance < _boundsHeight / 2)
                && (
                    OneWayPlatformMask.Contains(StandingOn.layer)
                    ||
                    MovingOneWayPlatformMask.Contains(StandingOn.layer)
                )
            )
            {
                State.IsCollidingBelow = false;
                return;
            }

            State.IsFalling = false;
            State.IsCollidingBelow = true;

            // if we're applying an external force (jumping, jetpack...) we only apply that
            if (_externalForce.y > 0 && _speed.y > 0)
            {
                _newPosition.y = _speed.y * Time.deltaTime;
                State.IsCollidingBelow = false;
            }
            // if not, we just adjust the position based on the raycast hit
            else
            {
                float distance = MMMaths.DistanceBetweenPointAndLine(_belowHitsStorage[smallestDistanceIndex].point, _verticalRayCastFromLeft, _verticalRayCastToRight);

                _newPosition.y = -distance
                    + _boundsHeight / 2
                    + RayOffset;
            }

            if (!State.WasGroundedLastFrame && _speed.y > 0)
            {
                _newPosition.y += _speed.y * Time.deltaTime;
            }

            if (Mathf.Abs(_newPosition.y) < _smallValue)
                _newPosition.y = 0;
            
            _frictionTest = _belowHitsStorage[smallestDistanceIndex].collider.gameObject.GetComponentNoAlloc<SurfaceModifier>();
            if (_frictionTest != null)
            {
                _friction = _belowHitsStorage[smallestDistanceIndex].collider.GetComponent<SurfaceModifier>().Friction;
            }
            
            _movingPlatformTest = _belowHitsStorage[smallestDistanceIndex].collider.gameObject.GetComponentNoAlloc<MMPathMovement>();
            if (_movingPlatformTest != null && State.IsGrounded)
            {
                _movingPlatform = _movingPlatformTest.GetComponent<MMPathMovement>();
            }
            else
            {
                DetachFromMovingPlatform();
            }
        }
        else
        {
            State.IsCollidingBelow = true;
            //State.IsCollidingBelow = false;
            if (State.OnAMovingPlatform)
            {
                DetachFromMovingPlatform();
            }
        }

        if (StickToSlopes)
        {
            StickToSlope();
        }
    }
    protected virtual void ComputeNewSpeed()
    {
        if (Time.deltaTime > 0)
        {
            _speed = _newPosition / Time.deltaTime;
        }
        if (State.IsGrounded)
        {
            _speed.x *= Parameters.SlopeAngleSpeedFactor.Evaluate(Mathf.Abs(State.BelowSlopeAngle) * Mathf.Sign(_speed.y));
            _speed.z *= Parameters.SlopeAngleSpeedFactor.Evaluate(Mathf.Abs(State.BelowSlopeAngle) * Mathf.Sign(_speed.y));
        }

        if (!State.OnAMovingPlatform)
        {
            _speed.x = Mathf.Clamp(_speed.x, -Parameters.MaxVelocity.x, Parameters.MaxVelocity.x);
            _speed.y = Mathf.Clamp(_speed.y, -Parameters.MaxVelocity.y, Parameters.MaxVelocity.y);
            _speed.z = Mathf.Clamp(_speed.z, -Parameters.MaxVelocity.z, Parameters.MaxVelocity.z);
        }
    }
    protected virtual void StickToSlope()
    {
        if ((_newPosition.y >= StickToSlopesOffsetY)
            || (_newPosition.y <= -StickToSlopesOffsetY)
            || (State.IsJumping)
            || (!StickToSlopes)
            || (!State.WasGroundedLastFrame)
            || (_externalForce.y > 0)
            || (_movingPlatform != null))
        {
            return;
        }

        float rayLength = 0;
        if (StickyRaycastLength == 0)
        {
            rayLength = _boundsWidth * Mathf.Abs(Mathf.Tan(Parameters.MaximumSlopeAngle));
            rayLength += _boundsHeight / 2 + RayOffset;
        }
        else
        {
            rayLength = StickyRaycastLength;
        }

        _stickRayCastOrigin.x = (_newPosition.x > 0) ? _boundsBottomLeftCorner.x : _boundsTopRightCorner.x;
        _stickRayCastOrigin.x += _newPosition.x;

        _stickRayCastOrigin.y = _boundsCenter.y + RayOffset;
        Color color1 = Color.blue;
        _stickRaycast = MMDebug.Raycast3D(_stickRayCastOrigin, -transform.up, rayLength, PlatformMask, color1, Parameters.DrawRaycastsGizmos);
        stickRaycast2 = MMDebug.Raycast3DBoolean(_stickRayCastOrigin, -transform.up, rayLength, PlatformMask, color1, Parameters.DrawRaycastsGizmos);

        if (stickRaycast2)
        {
            if (_stickRaycast.collider == _ignoredCollider)
            {
                return;
            }

            _newPosition.y = -Mathf.Abs(_stickRaycast.point.y - _stickRayCastOrigin.y)
                + _boundsHeight / 2
                + RayOffset;

            State.IsCollidingBelow = true;
        }
    }
    protected bool stickRaycast2;
    // Use this for initialization
    void Start()
    {

    }
}
