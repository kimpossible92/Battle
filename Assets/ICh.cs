using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MvSt
{
    public float Accel = 25.0f;
    public float Deccel = 25.0f;
    public float MaxHorSpeed = 8.0f;
    public float JSpeed = 10.0f;
    public float JAbortSpeed = 10.0f;
}
[System.Serializable]
public class Grav
{
    public float Gravity = 20.0f;
    public float GroundedGravity = 5.0f;
    public float MaxFallSpeed = 40.0f;
}
[System.Serializable]
public class Rot1
{
    [Header("Control Rotation")]
    public float MinPitchAngle = -45.0f;
    public float MaxPitchAngle = 75.0f;

    [Header("Character Orientation")]
    [SerializeField] private bool _useCtrlRotation = false;
    [SerializeField] private bool _orientRotatToMovement = true;
    public float MinRotatSpeed = 600.0f; 
    public float MaxRotatSpeed = 1200.0f;

    public bool UseCtrlRotation { get { return _useCtrlRotation; } set { SetUseControlRotation(value); } }
    public bool OrientRotationToMovement { get { return _orientRotatToMovement; } set { SetOrientRotationToMovement(value); } }

    private void SetUseControlRotation(bool useControlRotation)
    {
        _useCtrlRotation = useControlRotation;
        _orientRotatToMovement = !_useCtrlRotation;
    }

    private void SetOrientRotationToMovement(bool orientRotationToMovement)
    {
        _orientRotatToMovement = orientRotationToMovement;
        _useCtrlRotation = !_orientRotatToMovement;
    }
}
public class ICh : MonoBehaviour
{
    #region Combo1
    public enum ComboTypes { Slash,Dash,FlyingAttack}
    public enum Actions { idle,Attack,jump,Defend,SpecialAttack}
    private ComboTypes _nextCombo;
    private Dictionary<int, int> animHashCodeContainer = new Dictionary<int, int>();
    private Animator _animator;
    private int _nextComboId;
    #endregion
    #region Combo2
    [SerializeField] private KeyCode attackKey;
    [SerializeField]
    private KeyCode jumpKey;
    private Coroutine comboChechk;
    private Actions actionpl; 
    Vector3 startVec;
    public void UpdateAnimator()
    {
        _animator.SetBool("Attack", (_nextCombo & ComboTypes.Slash) == ComboTypes.Slash);
        _animator.SetBool("Dash", (_nextCombo & ComboTypes.Dash) == ComboTypes.Dash);
        _animator.SetBool("FlyingAttack", (_nextCombo & ComboTypes.FlyingAttack) == ComboTypes.FlyingAttack);

    }
    void TrackActions()
    {
        if (Input.GetKey(attackKey)) { _nextCombo |= ComboTypes.Slash; }
        else if ((_nextCombo & ComboTypes.Slash) == ComboTypes.Slash && comboChechk == null) { _nextCombo ^= ComboTypes.Slash; }
        
        if (Input.GetKey(attackKey)) { _nextCombo |= ComboTypes.Dash; }
        else if ((_nextCombo & ComboTypes.Dash) == ComboTypes.Dash && comboChechk == null){ _nextCombo ^= ComboTypes.Dash; }
        
        if (Input.GetKey(attackKey)) { _nextCombo |= ComboTypes.FlyingAttack; }
        else if ((_nextCombo & ComboTypes.FlyingAttack) == ComboTypes.FlyingAttack && comboChechk == null) { _nextCombo ^= ComboTypes.FlyingAttack; }
        
        if (comboChechk == null) { StartCoroutine(Combo()); }
    }
    [HideInInspector]public bool isfoot = false;
    public void walkSound()
    {
        isfoot = true; /*print("walk");*/
        //GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
        //Invoke("NoWalk", 0.5f);
    }
    private void NoWalk()
    {
        isfoot = false;
    }
    IEnumerator Combo()
    {
        WaitForSeconds wait = new WaitForSeconds(1.0f);
        yield return wait;
        if(
            (_nextCombo & ComboTypes.Slash)==ComboTypes.Slash &&
            (_nextCombo & ComboTypes.Dash)==ComboTypes.Dash &&
            (_nextCombo & ComboTypes.FlyingAttack) == ComboTypes.FlyingAttack)
        {
            _animator.SetBool("Slash", true);
        }
    }
    [SerializeField] private float attackComboDiff=1f;
    [SerializeField] private float _lastAttackTime;
    private bool attackflag,comboFlag;
    IEnumerator ChckCombo()
    {
        while (true)
        {
            this.attackflag = false;
            this.comboFlag = false; 
            yield return new WaitForSecondsRealtime(this.attackComboDiff);
            if (!this.attackflag) { this.ResetCombo(); }
        }
    }
    public void Attack()
    {
        //if(Time.time - this._lastAttackTime > this.attackComboDiff) { this.ResetCombo(); }
        if (this.attackflag)
        {
            //this.ResetCombo();
            this.PlayNextCombo();
            this.comboFlag = false;
            Invoke("StopFlag", 2f);
        }
    }
    private void StopFlag()
    {
        StartCoroutine(ChckCombo());
        _animator.SetBool("Slash", attackflag);
    }
    private void PlayNextCombo()
    {
        this._animator.SetTrigger(this._attackComboAnimNames[this._nextComboId]);
        this._nextComboId = (this._nextComboId+1)% this._attackComboAnimNames.Length;
    }
    private void ResetCombo()
    {
        this._nextComboId = 0;
    }
    #endregion
    [SerializeField] private string[] _attackComboAnimNames;
    public Ctrl ctrl;
    public MvSt mvSt;
    public Grav grav;
    public int Deads=0;
    
    public Rot1 rot1;
    public List<Vector3> RayPositions => _rayPositions;
    private List<Vector3> _rayPositions = new List<Vector3>{Vector3.zero,Vector3.zero,Vector3.zero, Vector3.zero };
    private CharacterController _characterController; // The Unity's CharacterController
    private ChAnim _characterAnimator;
    private float Health_ = 100f;
    private float _targetHorizontalSpeed;
    private float _horizontalSpeed;
    private float _verticalSpeed;
    [SerializeField] GameObject puli;
    private Vector2 _controlRotation;
    private Vector3 _movementInput;
    private Vector3 _lastMovementInput;
    private bool _hasMovementInput;
    [SerializeField] LayerMask layer; public AttackDefinition attack;
    private bool _jumpInput;
    [SerializeField] new ParticleSystem particleSystem;
    [SerializeField] UnityEngine.UI.Text GetTexthealth;
    public Vector3 Velocity => _characterController.velocity;
    public Vector3 HorizontalVelocity => _characterController.velocity.SetY(0.0f);
    public Vector3 VerticalVelocity => _characterController.velocity.Multiply(0.0f, 1.0f, 0.0f);
    protected bool dobivanie; private float timeOfLastAttack;
    protected void SetDobivanie()
    {
        //if(Input.GetKey(jumpKey)&& findClosestResourceWithTag("enemy").GetComponent<EnemyCharmander>().getdobivanie())
        //{
        //    findClosestResourceWithTag("enemy").GetComponent<EnemyCharmander>().SetDobivanie();
        //}
    }
    public bool IsGrounded { get; private set; }
    public void _healthMinus()
    {
        int Rand = Random.Range(0, 2);
        if (Rand == 0)
        {
            Health_-=5f;
        }
        else { Health_ -= 10f; }
    }
   
    private void Awake()
    {
        startVec = transform.position;
        ctrl.Init();
        ctrl.ich = this; timeOfLastAttack = float.MinValue;
        this._animator = this.GetComponent<Animator>();
        this.animHashCodeContainer.Add((int)ComboTypes.Slash, Animator.StringToHash(ComboTypes.Slash.ToString()));
        this.animHashCodeContainer.Add((int)ComboTypes.Dash, Animator.StringToHash(ComboTypes.Dash.ToString()));
        this.animHashCodeContainer.Add((int)ComboTypes.FlyingAttack, Animator.StringToHash(ComboTypes.FlyingAttack.ToString()));
        _characterController = GetComponent<CharacterController>();
        StartCoroutine(ChckCombo());
        GetComponent<CoverShooter.Actories>().SetAlive(true);
        _characterAnimator = GetComponent<ChAnim>();
    }
    private GameObject findClosestResourceWithTag(string tagtoCheck)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tagtoCheck);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            if (!go.GetComponent<ocupator>().isOccupied)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
        }
        return closest;
    }
    // Update is called once per frame
    private void Update()
    {
        //print(GetComponent<CoverShooter.Actories>().TopPosition);
        LayerMask _mask = LayerMask.GetMask("Impact");
        if (MMDebug.Raycast3DBoolean(transform.position, new Vector3(1, 0, 1), 100, _mask, Color.cyan)){ _rayPositions[0] = MMDebug.Raycast3D(transform.position, new Vector3(1, 0, 1), 100, _mask, Color.cyan, true).point; }
        if (MMDebug.Raycast3DBoolean(transform.position, new Vector3(-1, 0, 1), 100, _mask, Color.cyan)){ _rayPositions[1] = MMDebug.Raycast3D(transform.position, new Vector3(-1, 0, 1), 100, _mask, Color.cyan, true).point; }
        if (MMDebug.Raycast3DBoolean(transform.position, new Vector3(1, 0, -1), 100, _mask, Color.cyan)){ _rayPositions[2] = MMDebug.Raycast3D(transform.position, new Vector3(1, 0, -1), 100, _mask, Color.cyan, true).point; }
        if (MMDebug.Raycast3DBoolean(transform.position, new Vector3(-1, 0, -1), 100, _mask, Color.cyan)){ _rayPositions[3] = MMDebug.Raycast3D(transform.position, new Vector3(-1, 0, -1), 100, _mask, Color.cyan, true).point; }
            //SetDobivanie();
        if (Health_ <= 0)
        {
            transform.position = startVec;
            Deads += 1;
            Health_ = 100;
        }
        Cursor.lockState = CursorLockMode.Locked;
        if (GetTexthealth != null) GetTexthealth.text = Health_.ToString() + ":"+Deads;
        ctrl.OnIChUpdate();
        float timeSinceLastAttack = Time.time - timeOfLastAttack;
        bool attackOnCooldown = timeSinceLastAttack < attack.Cooldown;


        //if (Input.GetKey(attackKey))
        //{
        //    this.attackflag = true;
        //    if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("enemy").transform.position) <= 5f)
        //    {
        //        this.Attack(); 
        //    }
        //}
        foreach (var pl2 in GameObject.FindGameObjectsWithTag("enemy")) 
        {
            if (pl2.GetComponent<EnemyCharmander>()!=null && 
                pl2.GetComponent<EnemyCharmander>().getdobivanie()
                /*&& Vector3.Distance(transform.position, pl2.transform.position) <= 5f*/)
            {
                //_animator.SetTrigger("newslash");
                //pl2.GetComponent<EnemyCharmander>().SetDobivanie();
            }
        }
        if (Input.GetKey(attackKey))
        {
            foreach(var pl in GameObject.FindGameObjectsWithTag("enemy"))
            {
                float distanceFromPlayer = Vector3.Distance(transform.position, pl.transform.position);
                bool attackInRange = distanceFromPlayer < attack.Range;
                if (Vector3.Distance(transform.position, pl.transform.position) <= 5f)
                {
                    pl.GetComponent<EnemyCharmander>().StText(GameObject.Find("Canvas").transform.Find("enemyText").GetComponent<UnityEngine.UI.Text>());
                    transform.LookAt(pl.transform);
                    timeOfLastAttack = Time.time;
                    if (pl.GetComponent<EnemyCharmander>() != null) pl.GetComponent<EnemyCharmander>().minusHealth(3f);
                    if (pl.GetComponent<CoverShooter.AISight>() != null) pl.GetComponent<CoverShooter.AISight>().minusHealth();

                    //_animator.SetTrigger("slash2");
                }
            }
        }
        if (Input.GetKey(attackKey))
        {
            StartCoroutine(STTrig());
        }
        if (particleSystem.gameObject.activeSelf == true)
        {
            Invoke("loadthund", 2f);
        }
    }
    public IEnumerator STTrig()
    {
        _animator.SetTrigger("slash2");
        yield return new WaitForSeconds(1);
    }
    public void DoStomp(Vector3 destination)
    {
        //StopAllCoroutines();
        //StartCoroutine(GoToTargetAndStomp());
    }

    private IEnumerator GoToTargetAndStomp()
    {
        while (true)
        {
            //agent.destination = destination;
            yield return null;
        }
        //agent.isStopped = true;
        //_animator.SetTrigger("slash2");
    }
    private void loadthund()
    {
        
        particleSystem.gameObject.SetActive(false);
    }
    [SerializeField] GameObject pulki;
    private void FixedUpdate()
    {
        UpdateState();
        ctrl.OnIChFixedUpdate();
        if (Input.GetMouseButton(1))
        {
            RaycastHit hit,hit1,hit2;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                var grid = hit.collider.GetComponent<EnemyCharmander>();
                var grid2 = hit.collider.GetComponent<CoverShooter.AISight>();
                //var gr = Instantiate(pulki, hit.collider.transform.position, Quaternion.identity);
                //gr.transform.Translate(hit.collider.transform.position);
                GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
                if (GetComponent<Gameplay.ShipSystems.WeaponSystem>() != null)
                {
                    GetComponent<Gameplay.ShipSystems.WeaponSystem>().FirePlayer(hit.point);
                }
            }
            Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition + Vector3.left);
            if (Physics.Raycast(ray1, out hit1))
            {
                Transform objectHit = hit.transform;
                var grid = hit1.collider.GetComponent<EnemyCharmander>();
                var grid2 = hit1.collider.GetComponent<CoverShooter.AISight>();
                //var gr = Instantiate(pulki, hit1.collider.transform.position, Quaternion.identity);
                //gr.transform.Translate(hit1.collider.transform.position);
                GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
                if (GetComponent<Gameplay.ShipSystems.WeaponSystem>() != null)
                {
                    GetComponent<Gameplay.ShipSystems.WeaponSystem>().FirePlayer(hit1.point);
                }
            }
            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition + Vector3.right);
            if (Physics.Raycast(ray2, out hit2))
            {
                Transform objectHit = hit2.transform;
                var grid = hit2.collider.GetComponent<EnemyCharmander>();
                var grid2 = hit2.collider.GetComponent<CoverShooter.AISight>();
                //var gr = Instantiate(pulki, hit2.collider.transform.position, Quaternion.identity);
                //gr.transform.Translate(hit2.collider.transform.position);
                GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
                if (GetComponent<Gameplay.ShipSystems.WeaponSystem>() != null)
                {
                    GetComponent<Gameplay.ShipSystems.WeaponSystem>().FirePlayer(hit2.point);
                }
            }
        }
        if (Input.GetKey(KeyCode.KeypadEnter)|| Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                var grid = hit.collider.GetComponent<EnemyCharmander>();
                var grid2 = hit.collider.GetComponent<CoverShooter.AISight>();
                //var gr = Instantiate(pulki, hit.collider.transform.position, Quaternion.identity); 
                //gr.transform.Translate(hit.collider.transform.position);
                GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
                if (GetComponent<Gameplay.ShipSystems.WeaponSystem>() != null)
                {
                    GetComponent<Gameplay.ShipSystems.WeaponSystem>().FirePlayer(hit.point);
                }
            }
        }
    }
    private void Start()
    {
        startVec = transform.position;
    }
    private void UpdateState()
    {
        //if (Input.GetKey(KeyCode.W)||_horizontalSpeed!=0|| _verticalSpeed!=0) { walkSound(); }
        UpdateHorizontalSpeed();
        UpdateVerticalSpeed();

        Vector3 movement = _horizontalSpeed * GetMovementDirection() + _verticalSpeed * Vector3.up;
        _characterController.Move(movement * Time.deltaTime);

        OrientToTargetRotation(movement.SetY(0.0f));

        IsGrounded = _characterController.isGrounded;

        _characterAnimator.UpdateState();
    }

    public void SetMovementInput(Vector3 movementInput)
    {
        bool hasMovementInput = movementInput.sqrMagnitude > 0.0f;

        if (_hasMovementInput && !hasMovementInput)
        {
            _lastMovementInput = _movementInput;
        }

        _movementInput = movementInput;
        _hasMovementInput = hasMovementInput;
    }
    private void OrientToTargetRotation(Vector3 horizontalMovement)
    {
        if (rot1.OrientRotationToMovement && horizontalMovement.sqrMagnitude > 0.0f)
        {
            float rotationSpeed = Mathf.Lerp(
                rot1.MaxRotatSpeed, rot1.MinRotatSpeed, _horizontalSpeed / _targetHorizontalSpeed);

            Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else if (rot1.UseCtrlRotation)
        {
            Quaternion targetRotation = Quaternion.Euler(0.0f, _controlRotation.y, 0.0f);
            transform.rotation = targetRotation;
        }
    }
    private Vector3 GetMovementDirection()
    {
        //walkSound();
        Vector3 moveDir = _hasMovementInput ? _movementInput : _lastMovementInput;
        if (moveDir.sqrMagnitude > 1f)
        {
            moveDir.Normalize();
        }
        return moveDir;
    }
    public void SetJumpInput(bool jumpInput)
    {
        _jumpInput = jumpInput;
    }

    public Vector2 GetControlRotation()
    {
        return _controlRotation;
    }
    private void UpdateVerticalSpeed()
    {
        if (IsGrounded)
        {
            _verticalSpeed = -grav.GroundedGravity;

            if (_jumpInput)
            {
                _verticalSpeed = mvSt.JSpeed;
                IsGrounded = false;
            }
        }
        else
        {
            if (!_jumpInput && _verticalSpeed > 0.0f)
            {
                _verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -grav.MaxFallSpeed, mvSt.JAbortSpeed * Time.deltaTime);
            }

            _verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -grav.MaxFallSpeed, grav.Gravity * Time.deltaTime);
        }
    }

    public void SetControlRotation(Vector2 controlRotation)
    {
        float pitchAngle = controlRotation.x;
        pitchAngle %= 360.0f;
        pitchAngle = Mathf.Clamp(pitchAngle, rot1.MinPitchAngle, rot1.MaxPitchAngle);

        float yawAngle = controlRotation.y;
        yawAngle %= 360.0f;

        _controlRotation = new Vector2(pitchAngle, yawAngle);
    }

    private void UpdateHorizontalSpeed()
    {
        Vector3 movementInput = _movementInput;
        if (movementInput.sqrMagnitude > 1.0f)
        {
            movementInput.Normalize();
        }

        _targetHorizontalSpeed = movementInput.magnitude * mvSt.MaxHorSpeed;
        float acceleration = _hasMovementInput ? mvSt.Accel : mvSt.Deccel;

        _horizontalSpeed = Mathf.MoveTowards(_horizontalSpeed, _targetHorizontalSpeed, acceleration * Time.deltaTime);
    }
}
