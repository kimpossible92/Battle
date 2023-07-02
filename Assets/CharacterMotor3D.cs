using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum bodyPosition
{
    Idle,
    Walk,
    Run,
    Dance,
    Emoution,
    Death
}
public class CharacterMotor3D : MonoBehaviour
{
    public enum CharacterTypes { Player, AI }
    public enum FacingDirections { Left, Right }
    public enum SpawnFacingDirections { Default, Left, Right }
    public bool FlipModelOnDirectionChange = true;
    public FacingDirections InitialFacingDirection = FacingDirections.Right;
    public SpawnFacingDirections DirectionOnSpawn = SpawnFacingDirections.Default;
    public Vector3 ModelFlipValue = new Vector3(-1, 1, 1);
    public bool RotateModelOnDirectionChange;
    public Vector3 ModelRotationValue = new Vector3(0f, 0f, 0f);
    public float ModelRotationSpeed = 0f;
    public bodyPosition currentbodyPosition;
    public GameObject CharacterModel;
    private int idleSpritePosition;
    public string PlayerID = "";
    public bool UseDefaultMecanim = true;
    public bool IsFacingRight { get; set; }
    public CharAbil[] _charAbilities; protected CController _controller;
    public int IdleSpritePosition { get => idleSpritePosition; set => idleSpritePosition = value; }
    public MMStateMachine<CharacterStates.MovementStates> MovementState { get; internal set; }
    public InputManager LinkedInputManager { get; protected set; }
    [SerializeField] private GameObject @objectModel;
    public List<Sprite> spritesModel;
    public MMStateMachine<CharacterStates.CharacterConditions> ConditionState;
    public CharacterStates CharacterState { get; protected set; }
    int countLayer = 0; public bool CanFlip { get; set; }
    protected SpriteRenderer _spriteRenderer;
    public Animator CharacterAnimator;
    public Animator _animator { get; protected set; }
    public CharacterMotor3D _character { get; protected set; }
    public List<string> _animatorParameters { get; set; }
    protected Vector3 _targetModelRotation;
    protected float _originalGravity;
    protected Health _health;
    protected bool _spawnDirectionForced = false;
    public bool SendStateChangeEvents = true, SendStateUpdateEvents = true;
    IEnumerator spriteLoader()
    {
        yield return new WaitForSeconds(1.0f); countLayer++; 
    }
    protected virtual void Awake()
    {
        Initialization();
        if (GetComponent<InputManager>() != null) { SetPlayerID("Player1"); }
        currentbodyPosition = 0;
    }
    public void LoadStates(MMStateMachine<CharacterStates.MovementStates> mMState)
    {
        MovementState = new MMStateMachine<CharacterStates.MovementStates>(gameObject, SendStateChangeEvents);
        
        mMState = MovementState;
    }
    public void LoadCondition(MMStateMachine<CharacterStates.CharacterConditions> mMStateCond)
    {
        ConditionState = new MMStateMachine<CharacterStates.CharacterConditions>(gameObject, SendStateChangeEvents);
        mMStateCond = ConditionState;
    }
    protected virtual void Initialization()
    {
        MovementState = new MMStateMachine<CharacterStates.MovementStates>(gameObject, SendStateChangeEvents);
        ConditionState = new MMStateMachine<CharacterStates.CharacterConditions>(gameObject, SendStateChangeEvents);
        
        if (InitialFacingDirection == FacingDirections.Left)
        {
            IsFacingRight = false;
        }
        else
        {
            IsFacingRight = true;
        }
        
        if (GetComponent<InputManager>() != null)
        {
            SetInput();
            SetInput(GetComponent<InputManager>());
        }
        if (Camera.main == null) { return; }
        //SceneCamera = Camera.main.GetComponent<CameraController>();
        CharacterState = new CharacterStates();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _controller = GetComponent<CController>();
        _charAbilities = GetComponents<CharAbil>();
        //_aiBrain = GetComponent<AIBrain>();
        _health = GetComponent<Health>();
        //_damageOnTouch = GetComponent<DamageOnTouch>();
        CanFlip = true;
        AssignAnimator();

        _originalGravity = _controller.Parameters.Gravity;
        ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        ForceSpawnDirection();
    }
    protected virtual void EveryFrame()
    {
        HandleCharacterStatus();

        EarlyProcessAbilities();
        ProcessAbilities();
        LateProcessAbilities();

        UpdateAnimators();
        RotateModel();
    }
    public virtual void SetPlayerID(string newPlayerID)
    {
        PlayerID = newPlayerID;
        SetInput();
    }
    protected virtual void HandleCharacterStatus()
    {

        if (ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
        {
            if (_health != null)
            {
                if (_health.DeathForce.x == 0f)
                {
                    _controller.SetHorizontalForce(0);
                    return;
                }
            }
            else
            {
                _controller.SetHorizontalForce(0);
                return;
            }
        }


        if (ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen)
        {
            _controller.GravityActive(false);
            _controller.SetForce(Vector3.zero);
        }
    }
    protected virtual void EarlyProcessAbilities()
    {
        foreach (CharAbil ability in _charAbilities)
        {
            if (ability.enabled && ability.AbilityInitialized)
            {
                ability.EarlyProcessAbility();
            }
        }
    }
    protected virtual void ProcessAbilities()
    {
        foreach (CharAbil ability in _charAbilities)
        {
            if (ability.enabled && ability.AbilityInitialized)
            {
                ability.ProcessAbility();
            }
        }
    }
    protected virtual void LateProcessAbilities()
    {
        foreach (CharAbil ability in _charAbilities)
        {
            if (ability.enabled && ability.AbilityInitialized)
            {
                ability.LateProcessAbility();
            }
        }
    }
    public virtual void SetInput()
    {
        if (!string.IsNullOrEmpty(PlayerID))
        {
            LinkedInputManager = null;
            InputManager[] foundInputManagers = FindObjectsOfType(typeof(InputManager)) as InputManager[];
            foreach (InputManager foundInputManager in foundInputManagers)
            {
                if (foundInputManager.PlayerID == PlayerID)
                {
                    LinkedInputManager = foundInputManager;
                }
            }
        }
    }
    public virtual void SetInput(InputManager inputManager)
    {
        LinkedInputManager = inputManager;
        UpdateInputManagersInAbilities();
    }
    protected virtual void UpdateInputManagersInAbilities()
    {
        if (_charAbilities == null)
        {
            print("_charAbilities");
            return;
        }
        for (int i = 0; i < _charAbilities.Length; i++)
        {
            _charAbilities[i].SetInputManager(LinkedInputManager);
        }
    }
    protected virtual void RotateModel()
    {
       if (!RotateModelOnDirectionChange)
       {
            return;
       }

        if (ModelRotationSpeed > 0f)
        {
            CharacterModel.transform.localEulerAngles = Vector3.Lerp(CharacterModel.transform.localEulerAngles, _targetModelRotation, Time.deltaTime * ModelRotationSpeed);
        }
        else
        {
            CharacterModel.transform.localEulerAngles = _targetModelRotation;
        }
    }
    private Vector3 dir;
    private Vector3 target;
    private Vector3 lastTarget; private float angleTarget;
    private void CalculateAngle(Vector3 temp)
    {
        dir = new Vector3(temp.x, CharacterModel.transform.position.y, temp.z) - transform.position;
        angleTarget = Vector3.Angle(dir, transform.forward);
    }
    private void LookAtThis()
    {
        if (target != lastTarget)
        {
            CalculateAngle(target);
            //if (angleTarget > 3)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), 220 * UnityEngine.Time.deltaTime);
        }
    }
    public void RotTowards(Vector3 vectorTo)
    {
        //target = vectorTo;
        CalculateAngle(vectorTo);
        if (angleTarget > 3)
           CharacterModel.transform.rotation = Quaternion.LookRotation(dir);

    }
    public virtual void Flip(bool IgnoreFlipOnDirectionChange = false)
    {
        if (!FlipModelOnDirectionChange && !RotateModelOnDirectionChange && !IgnoreFlipOnDirectionChange)
        {
            return;
        }

        if (!CanFlip)
        {
            return;
        }

        if (!FlipModelOnDirectionChange && !RotateModelOnDirectionChange && IgnoreFlipOnDirectionChange)
        {
            if (CharacterModel != null)
            {
                CharacterModel.transform.localScale = Vector3.Scale(CharacterModel.transform.localScale, ModelFlipValue);
            }
            else
            {
                if (_spriteRenderer != null)
                {
                    _spriteRenderer.flipX = !_spriteRenderer.flipX;
                }
            }
        }
        
        FlipModel();

        IsFacingRight = !IsFacingRight;

        foreach (CharAbil ability in _charAbilities)
        {
            if (ability.enabled)
            {
                ability.Flip();
            }
        }
    }
    public virtual void FlipModel()
    {
        if (FlipModelOnDirectionChange)
        {
            if (CharacterModel != null)
            {
                CharacterModel.transform.localScale = Vector3.Scale(CharacterModel.transform.localScale, ModelFlipValue);
            }
            else
            {
                if (_spriteRenderer != null)
                {
                    _spriteRenderer.flipX = !_spriteRenderer.flipX;
                }
            }
        }
        if (RotateModelOnDirectionChange)
        {
            if (CharacterModel != null)
            {
                _targetModelRotation += ModelRotationValue;
                _targetModelRotation.x = _targetModelRotation.x % 360;
                _targetModelRotation.y = _targetModelRotation.y % 360;
                _targetModelRotation.z = _targetModelRotation.z % 360;
            }

        }
    }

    public virtual void AssignAnimator()
    {
        if (CharacterAnimator != null)
        {
            _animator = CharacterAnimator;
        }
        else
        {
            _animator = GetComponent<Animator>();
        }

        if (_animator != null)
        {
            InitializeAnimatorParameters();
        }
    }
    protected virtual void ForceSpawnDirection()
    {
        if ((DirectionOnSpawn == SpawnFacingDirections.Default) || _spawnDirectionForced)
        {
            return;
        }
        else
        {
            _spawnDirectionForced = true;
            if (DirectionOnSpawn == SpawnFacingDirections.Left)
            {
                Face(FacingDirections.Left);
            }
            if (DirectionOnSpawn == SpawnFacingDirections.Right)
            {
                Face(FacingDirections.Right);
            }
        }
    }
    public virtual void Face(FacingDirections facingDirection)
    {
        if (!CanFlip)
        {
            return;
        }
        
        if (facingDirection == FacingDirections.Right)
        {
            if (!IsFacingRight)
            {
                Flip(true);
            }
        }
        else
        {
            if (IsFacingRight)
            {
                Flip(true);
            }
        }
    }
    protected virtual void InitializeAnimatorParameters()
    {
        if (_animator == null) { return; }
        _animatorParameters = new List<string>();
        MMAnimator.AddAnimatorParamaterIfExists(_animator, "Grounded", AnimatorControllerParameterType.Bool, _animatorParameters);
        MMAnimator.AddAnimatorParamaterIfExists(_animator, "xSpeed", AnimatorControllerParameterType.Float, _animatorParameters);
        MMAnimator.AddAnimatorParamaterIfExists(_animator, "ySpeed", AnimatorControllerParameterType.Float, _animatorParameters);
        MMAnimator.AddAnimatorParamaterIfExists(_animator, "CollidingLeft", AnimatorControllerParameterType.Bool, _animatorParameters);
        MMAnimator.AddAnimatorParamaterIfExists(_animator, "CollidingRight", AnimatorControllerParameterType.Bool, _animatorParameters);
        MMAnimator.AddAnimatorParamaterIfExists(_animator, "CollidingBelow", AnimatorControllerParameterType.Bool, _animatorParameters);
        MMAnimator.AddAnimatorParamaterIfExists(_animator, "CollidingAbove", AnimatorControllerParameterType.Bool, _animatorParameters);
        MMAnimator.AddAnimatorParamaterIfExists(_animator, "Idle", AnimatorControllerParameterType.Bool, _animatorParameters);
        MMAnimator.AddAnimatorParamaterIfExists(_animator, "Alive", AnimatorControllerParameterType.Bool, _animatorParameters);
        MMAnimator.AddAnimatorParamaterIfExists(_animator, "FacingRight", AnimatorControllerParameterType.Bool, _animatorParameters);
    }
    protected virtual void UpdateAnimators()
    {
        if ((UseDefaultMecanim) && (_animator != null))
        {
            MMAnimator.UpdateAnimatorBool(_animator, "Grounded", _controller.State.IsGrounded, _animatorParameters);
            MMAnimator.UpdateAnimatorBool(_animator, "Alive", (ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead), _animatorParameters);
            MMAnimator.UpdateAnimatorFloat(_animator, "xSpeed", _controller.Speed.x, _animatorParameters);
            MMAnimator.UpdateAnimatorFloat(_animator, "ySpeed", _controller.Speed.y, _animatorParameters);
            MMAnimator.UpdateAnimatorBool(_animator, "CollidingLeft", _controller.State.IsCollidingLeft, _animatorParameters);
            MMAnimator.UpdateAnimatorBool(_animator, "CollidingRight", _controller.State.IsCollidingRight, _animatorParameters);
            MMAnimator.UpdateAnimatorBool(_animator, "CollidingBelow", _controller.State.IsCollidingBelow, _animatorParameters);
            MMAnimator.UpdateAnimatorBool(_animator, "CollidingAbove", _controller.State.IsCollidingAbove, _animatorParameters);
            MMAnimator.UpdateAnimatorBool(_animator, "Idle", (MovementState.CurrentState == CharacterStates.MovementStates.Idle), _animatorParameters);
            MMAnimator.UpdateAnimatorBool(_animator, "FacingRight", IsFacingRight, _animatorParameters);

            foreach (CharAbil ability in _charAbilities)
            {
                if (ability.enabled && ability.AbilityInitialized)
                {
                    ability.UpdateAnimator();
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        EveryFrame();
    }
}
