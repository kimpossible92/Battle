using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterMotor3D))]
public class CharAbil : MonoBehaviour
{
    public enum pikachuAbility { Tolchok, Udar, ThunderCascad }
    public AudioClip AbilityStartSfx;
    public AudioClip AbilityInProgressSfx;
    public AudioClip AbilityStopSfx;
    public bool AbilityPermitted = true;
    public bool AbilityInitialized { get { return _abilityInitialized; } }
    public Animator _animator { get; protected set; }
    protected AudioSource _abilityInProgressSfx;
    protected bool _abilityInitialized = false;
    protected CharacterGravity _characterGravity;
    protected float _verticalInput;
    protected float _horizontalInput;
    private SpriteRenderer _spriteRenderer;
    private Health _health;
    protected CharacterMotor3D _character;
    protected InputManager _inputManager;
    private CharacterStates _state;
    protected CharacterHorizontalMovement _characterHorizontalMovement;
    public virtual string HelpBoxText() { return ""; }
    [SerializeField] bool RotateIdentity;
    protected MMStateMachine<CharacterStates.MovementStates> _movement;
    protected MMStateMachine<CharacterStates.CharacterConditions> _condition;
    protected Camera cameraMain;
    protected CController _controller;
    protected virtual void Start()
    {
        
    }
    protected virtual void Awake()
    {
        cameraMain = Camera.main;
        Initialization();
    }
    public virtual void SetInputManager(InputManager inputManager)
    {
        _inputManager = inputManager;
    }
    protected virtual void Initialization()
    {
        _character = GetComponent<CharacterMotor3D>();
        _controller = GetComponent<CController>();
        _characterHorizontalMovement = GetComponent<CharacterHorizontalMovement>();
        _characterGravity = GetComponent<CharacterGravity>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _health = GetComponent<Health>();
        //BindAnimator();
        //_sceneCamera = _character.SceneCamera;
        //_inputManager = _character.LinkedInputManager;
        _inputManager = GetComponent<InputManager>();
        _state = _character.CharacterState;
        if (GetComponent<InputManager>() == null) { _character.LoadStates(_movement);_character.LoadCondition(_condition); }
        else
        {
            _movement = _character.MovementState; _condition = _character.ConditionState;
        }
        AbilityPermitted = true;
        if (_character.MovementState == null) { }
        _abilityInitialized = true;
    }
    protected virtual void InternalHandleInput()
    {
        if (_inputManager == null) { return; }

        _verticalInput = _inputManager.PrimaryMovement.y;
        _horizontalInput = _inputManager.PrimaryMovement.x;
        if (GetComponent<CharacterGravity>() != null)
        {
            _verticalInput = -_verticalInput;
            _horizontalInput = -_horizontalInput;
            
            if (!_characterGravity.ShouldReverseInput())
            {               
                if (GetComponent<CharacterGravity>().ReverseVerticalInputWhenUpsideDown)
                {
                    _verticalInput = -_verticalInput;
                }
                if (GetComponent<CharacterGravity>().ReverseHorizontalInputWhenUpsideDown)
                {
                    _horizontalInput = -_horizontalInput;
                }
            }
        }
        HandleInput();
    }
    public virtual void UpdateAnimator(){}
    public virtual void ProcessAbility(){}
    public virtual void LateProcessAbility(){}
    protected virtual void RegisterAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (_animator == null)
        {
            return;
        }
        if (_animator.HasParameterOfType(parameterName, parameterType))
        {
            _character._animatorParameters.Add(parameterName);
        }
    }
    protected virtual void HandleInput()
    {

    }
    public virtual void ResetInput()
    {
        _horizontalInput = 0f;
        _verticalInput = 0f;
    }
    public virtual void EarlyProcessAbility()
    {
        InternalHandleInput();
    }
    protected virtual void Update()
    {     
        InternalHandleInput();
    }
    public virtual void PermitAbility(bool abilityPermitted)
    {
        AbilityPermitted = abilityPermitted;
    }
    public virtual void Flip()
    {

    }
    public virtual void Reset()
    {

    }
    protected virtual void PlayAbilityStartSfx()
    {
        if (AbilityStartSfx != null)
        {
           
        }
    }
    protected virtual void PlayAbilityUsedSfx()
    {
        if (AbilityInProgressSfx != null)
        {
            if (_abilityInProgressSfx == null)
            {
                
            }
        }
    }
    protected virtual void StopAbilityUsedSfx()
    {
        if (AbilityInProgressSfx != null)
        {            
            _abilityInProgressSfx = null;
        }
    }
    protected virtual void PlayAbilityStopSfx()
    {
        if (AbilityStopSfx != null)
        {
        }
    }
    protected virtual void InitializeAnimatorParameters()
    {

    }
    protected virtual void OnRespawn()
    {
    }
    protected virtual void OnDeath()
    {
        StopAbilityUsedSfx();
    }
    protected virtual void OnHit()
    {

    }
    protected virtual void OnEnable()
    {
    }
    protected virtual void OnDisable()
    {
    }
}