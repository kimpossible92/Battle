using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CharacterHorizontalMovement : CharAbil
{
    public float MovementSpeed { get; set; }
    [Header("Speed")]
    public float WalkSpeed = 6f;
    [ReadOnly]
    public float MovementSpeedMultiplier = 1f;
    [ReadOnly]
    public float PushSpeedMultiplier = 1f;
    public float HorizontalMovementForce { get { return _horizontalMovementForce; } }
    public bool MovementForbidden { get; set; }

    [Header("Input")]
    public bool InstantAcceleration = false, InstantAcceleration2 = false;
    public float InputThreshold = 0.1f;

    [Header("Effects")]
    public ParticleSystem TouchTheGroundEffect;
    public AudioClip TouchTheGroundSfx;

    [SerializeField] bool Ability_Permited = false;
    protected float _horizontalMovement;
    protected float _horizontalMovementForce, _verticalMovementForce;
    protected float _normalizedHorizontalSpeed, _normalizeVertSpeed;
    protected int CurrentStateBot;
    [SerializeField] LayerMask lestnica_mask, ButtonLayer;
    protected override void Awake()
    {
        base.Awake();
        AbilityPermitted = true;
        Initialization();
        InitializeAnimatorParameters();
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void Initialization()
    {
        _animator = GetComponent<Animator>();
        _character = GetComponent<CharacterMotor3D>();
        _controller = GetComponent<CController>();
        _characterHorizontalMovement = GetComponent<CharacterHorizontalMovement>();
        _characterGravity = GetComponent<CharacterGravity>();
        _character.LoadStates(_movement); _character.LoadCondition(_condition);
        base.Initialization();
        if (GetComponent<InputManager>() != null)
        {
            MovementSpeed = WalkSpeed;
            MovementSpeedMultiplier = 1f;
            MovementForbidden = false;
            if (_movement != null)
            {
                _movement.
                ChangeState(
                CharacterStates.MovementStates.Idle);
            }
            else { }
            HandleInput();
        }
        else
        {
            MovementSpeed = WalkSpeed;
            MovementSpeedMultiplier = 1f;
            MovementForbidden = false;
            _character = GetComponent<CharacterMotor3D>();
            _character.LoadStates(_movement);
            _character.LoadCondition(_condition);
            //_movement = _character.MovementState;
            _animator = GetComponent<Animator>();
            //_movement.ChangeState(CharacterStates.MovementStates.Idle);
            CurrentStateBot = (int)CharacterStates.MovementStates.Idle;
        }
        _characterGravity.ReverseHorizontalInputWhenUpsideDown = true;

    }
    void _movementStates()
    {
        if (_movement.CurrentState != CharacterStates.MovementStates.Walking && _abilityInProgressSfx != null)
        {
            StopAbilityUsedSfx();
        }
        if (_movement.CurrentState == CharacterStates.MovementStates.Walking && _abilityInProgressSfx == null)
        {
            PlayAbilityUsedSfx();
        }
    }
    protected void BotHandleHorizontalMovement()
    {
        if (MovementForbidden)
        {
            _horizontalMovement = _controller.Speed.x * Time.deltaTime;
        }

        if (_horizontalMovement > InputThreshold)
        {
            _normalizedHorizontalSpeed = _horizontalMovement;
            if (!_character.IsFacingRight)
            {
               // _character.Flip();
            }
        }
        else if (_horizontalMovement < -InputThreshold)
        {
            _normalizedHorizontalSpeed = _horizontalMovement;
            if (_character.IsFacingRight)
            {
                //_character.Flip();
            }
        }
        else
        {
            _normalizedHorizontalSpeed = 0;
        }
        if (_controller.State.IsGrounded
             && (_normalizedHorizontalSpeed != 0) && CurrentStateBot == (int)CharacterStates.MovementStates.Idle/*1*/)
        {
            CurrentStateBot = (int)CharacterStates.MovementStates.Walking;
            PlayAbilityStartSfx();
            PlayAbilityUsedSfx();
        }
        if ((CurrentStateBot == (int)CharacterStates.MovementStates.Walking/*2*/)
            && (_normalizedHorizontalSpeed == 0))
        {
            CurrentStateBot = (int)CharacterStates.MovementStates.Idle;
            PlayAbilityStopSfx();
        }
        if (InstantAcceleration)
        {
            if (_normalizedHorizontalSpeed > 0f) { _normalizedHorizontalSpeed = 1f; }
            if (_normalizedHorizontalSpeed < 0f) { _normalizedHorizontalSpeed = -1f; }
        }
        float movementFactor = _controller.State.IsGrounded ? _controller.Parameters.SpeedAccelerationOnGround : _controller.Parameters.SpeedAccelerationInAir;
        float movementSpeed = _normalizedHorizontalSpeed * MovementSpeed * _controller.Parameters.SpeedFactor * MovementSpeedMultiplier * PushSpeedMultiplier;

        if (!InstantAcceleration)
        {
            _horizontalMovementForce = Mathf.Lerp(_controller.Speed.x, movementSpeed, Time.deltaTime * movementFactor);
        }
        else
        {
            _horizontalMovementForce = movementSpeed;
        }
        _horizontalMovementForce = HandleFriction(_horizontalMovementForce);
        _controller.SetHorizontalForce(_horizontalMovementForce);
    }
    protected virtual void HandleHorizontalMovement()
    {
        //if (AbilityPermitted == false) { print(""); }

        if (GetComponent<InputManager>() != null)
        {
            if (AbilityPermitted == false
            || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
            {
                return;
            }
        }
        if (GetComponent<InputManager>() != null)
        {
            CheckJustGotGrounded();
        }
        if (MovementForbidden)
        {
            _horizontalMovement = _controller.Speed.x * Time.deltaTime;
        }

        if (_verticalMovement > InputThreshold)
        {
            _normalizeVertSpeed = _verticalMovement;//
            if (!_character.IsFacingRight)
            {
                //_character.Flip();
            }
        }
        else if (_verticalMovement < -InputThreshold)
        {
            _normalizeVertSpeed = _verticalMovement;
            if (_character.IsFacingRight)
            {
                //_character.Flip();
            }
        }
        else { _normalizeVertSpeed = 0; }
        if (_horizontalMovement > InputThreshold)
        {
            _normalizedHorizontalSpeed = _horizontalMovement;
            if (!_character.IsFacingRight)
            {
                //_character.Flip();
            }
        }
        else if (_horizontalMovement < -InputThreshold)
        {
            _normalizedHorizontalSpeed = _horizontalMovement;
            if (_character.IsFacingRight)
            {
                //_character.Flip();
            }
        }
        else
        {
            _normalizedHorizontalSpeed = 0;
        }
        if (_movement.CurrentState == CharacterStates.MovementStates.Idle)
        {

        }

        if ((_controller.State.IsGrounded)
        && (_normalizedHorizontalSpeed != 0)
        && ((_movement.CurrentState == CharacterStates.MovementStates.Idle)
            || (_movement.CurrentState == CharacterStates.MovementStates.Dangling)))
        {
            _movement.ChangeState(CharacterStates.MovementStates.Walking);
            PlayAbilityStartSfx();
            PlayAbilityUsedSfx();
        }

        if ((_movement.CurrentState == CharacterStates.MovementStates.Walking)
            && (_normalizedHorizontalSpeed == 0))
        {
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            PlayAbilityStopSfx();
        }

        if (!_controller.State.IsGrounded
            && (
                (_movement.CurrentState == CharacterStates.MovementStates.Walking)
                 || (_movement.CurrentState == CharacterStates.MovementStates.Idle)
                ))
        {
            _movement.ChangeState(CharacterStates.MovementStates.Falling);//
        }

        if (InstantAcceleration)
        {
            if (_normalizedHorizontalSpeed > 0f) { _normalizedHorizontalSpeed = 1f; }
            if (_normalizedHorizontalSpeed < 0f) { _normalizedHorizontalSpeed = -1f; }
        }
        if (InstantAcceleration2)
        {
            if (_normalizeVertSpeed > 0f) { _normalizeVertSpeed = 1f; }
            if (_normalizeVertSpeed < 0f) { _normalizeVertSpeed = -1f; }
        }
        float movementFactor = _controller.State.IsGrounded ? _controller.Parameters.SpeedAccelerationOnGround : _controller.Parameters.SpeedAccelerationInAir;
        float movementSpeed = _normalizedHorizontalSpeed * MovementSpeed * _controller.Parameters.SpeedFactor * MovementSpeedMultiplier * PushSpeedMultiplier;
        float movementVertSpeed = _normalizeVertSpeed * MovementSpeed * _controller.Parameters.SpeedFactor * MovementSpeedMultiplier * PushSpeedMultiplier;

        if (!InstantAcceleration)
        {
            _horizontalMovementForce = Mathf.Lerp(_controller.Speed.x, movementSpeed, Time.deltaTime * movementFactor);
            //_verticalMovementForce = Mathf.Lerp(_controller.Speed.z, movementSpeed, Time.deltaTime * movementFactor);
        }
        else
        {
            _horizontalMovementForce = movementSpeed;//_verticalMovementForce = movementSpeed;
        }

        _horizontalMovementForce = HandleFriction(_horizontalMovementForce);

        _controller.SetHorizontalForce(_horizontalMovementForce);
        if (!InstantAcceleration2)
        {
            _verticalMovementForce = Mathf.Lerp(_controller.Speed.z, movementVertSpeed, Time.deltaTime * movementFactor);
        }
        else
        {
            //_horizontalMovementForce = movementSpeed;//
            _verticalMovementForce = movementVertSpeed;
        }
        _controller.SetVerticalForce(_verticalMovementForce);
    }
    int lcount = 1;
    IEnumerator GetROtationCoroutine()
    {
        while (lcount != 0)
        {
            transform.TweenPosition(WalkSpeed, transform.position + new Vector3(_horizontalMovement, 0, 0));
            lcount++;
            yield return null;
        }
    }
    public override void ProcessAbility()
    {
        base.ProcessAbility();
        if (GetComponent<InputManager>() != null)
        {
            HandleHorizontalMovement();
        }
        else
        {
            BotHandleHorizontalMovement();
        }
    }
    float _verticalMovement;
    protected override void HandleInput()
    {
        _horizontalMovement = _horizontalInput;
        _verticalMovement = _verticalInput;
    }
    public virtual void SetHorizontalMove(float value)
    {
        _horizontalMovement = value;
    }
    public virtual void ResetHorizontalSpeed()
    {
        MovementSpeed = WalkSpeed;
    }
    protected virtual float HandleFriction(float force)
    {
        if (_controller.Friction > 1)
        {
            force = force / _controller.Friction;
        }
        if (_controller.Friction < 1 && _controller.Friction > 0)
        {
            force = Mathf.Lerp(_controller.Speed.x, force, Time.deltaTime * _controller.Friction * 10);
        }

        return force;
    }
    protected virtual void CheckJustGotGrounded()
    {
        if (_controller.State.JustGotGrounded)
        {
            if (_controller.State.ColliderResized)
            {
                _movement.ChangeState(CharacterStates.MovementStates.Crouching);
            }
            else
            {
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
            }

            _controller.SlowFall(0f);
            if (TouchTheGroundEffect != null)
            {
                Instantiate(TouchTheGroundEffect, _controller.BoundsBottom, transform.rotation);
            }
        }
    }
    protected override void InitializeAnimatorParameters()
    {
        if (GetComponent<InputManager>() != null)
        {
            RegisterAnimatorParameter("Speed", AnimatorControllerParameterType.Float);
            RegisterAnimatorParameter("Walking", AnimatorControllerParameterType.Bool);
        }
    }
    public override void UpdateAnimator()
    {
        if (GetComponent<InputManager>() != null)
        {
            MMAnimator.UpdateAnimatorFloat(_animator, "Speed", Mathf.Abs(_normalizedHorizontalSpeed), _character._animatorParameters);
            MMAnimator.UpdateAnimatorBool(_animator, "Walking", (_movement.CurrentState == CharacterStates.MovementStates.Walking),
                _character._animatorParameters);
        }
    }

    bool checkUpd = false;
    IEnumerator GetEnumeratorlestnica1(Vector2 rayDir, float updown, float distances, Vector2 addvector, bool iskinematic)
    {
        while (MMDebug.RayCast((Vector2)transform.position + addvector, rayDir, distances, lestnica_mask, Color.green, true))
        {
            GetComponent<Rigidbody>().isKinematic = iskinematic;
            transform.Translate(new Vector3(0, updown, 0));//
            //if (iskinematic) { yield return new WaitForSeconds(0.1f); }
            yield return null;
        }
    }
    bool islock = false; 
    float m_HorizontalMovement;
    float m_VerticalMovement;
    int loadAnim()
    {
        if (_verticalInput < 0 || _verticalInput > 0)
        { return 3; }
        if (_verticalInput > 0) { return 2; }
        if (_verticalInput < 0) { return 1; }
        return 0;
    }
    // Update is called once per frame
    void Update()
    {
        m_HorizontalMovement = Input.GetAxis("Horizontal");
        m_VerticalMovement = Input.GetAxis("Vertical");
        if (m_VerticalMovement >0.00000000000000000001f) { m_HorizontalMovement = m_VerticalMovement; }
        if (m_VerticalMovement <-0.00000000000000000001f) { m_HorizontalMovement = -m_VerticalMovement; }
        if (m_HorizontalMovement < -0.00000000000000000001f) { m_HorizontalMovement = -m_HorizontalMovement;  }
        _animator.SetFloat("speed", m_HorizontalMovement);
    }
    protected virtual void OnRevive()
    {
        Initialization();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
