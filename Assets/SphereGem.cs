using UnityEngine;
using System.Collections;

public class SphereGem : MonoBehaviour
{
    public enum PokemonAbility { Tolchok, Udar }
    public int color;
    public int rotationCount = 1;
    public int RotationNum = 1;
    float unitDestroyRadiusLoads;
    public Vector3 loadedVector;
    public float sradius;
    float speed = 1f;
    private Vector3 dir;
    private float angleTarget;
    public float walkLeftRight = 220;
    [SerializeField] GameObject GameColor;
    public float row, col;
    public string type;
    public bool isBonus;
    public int BonusMatchType;
    public bool isSwirl = false;
    public int seconds = 0;
    #region AIWAlk
    public enum WalkBehaviours { Patrol, MoveOnSight }
    
    public WalkBehaviours WalkBehaviour = WalkBehaviours.Patrol;

    [Header("Obstacle Detection")]
    public bool ChangeDirectionOnWall = true;
    public bool AvoidFalling = false;
    public Vector3 HoleDetectionOffset = new Vector3(0, 0, 0);
    public float HoleDetectionRaycastLength = 1f;
    [Header("Move on Sight")]
    public float ViewDistance = 7.5f;
    public float StopDistance = 1f;
    public Vector3 MoveOnSightRayOffset = new Vector3(0, 0, 0);
    [SerializeField] LayerMask MoveOnSightLayer, MoveOnSightObstaclesLayer, DeadLayer,ButtonLayer;
    protected CController _controller;
    protected CharacterMotor3D _character;
    protected Health _health;
    protected CharacterHorizontalMovement _characterHorizontalMovement;
    protected Vector2 _direction;
    protected Vector2 _startPosition;
    protected Vector2 _initialDirection;
    protected Vector3 _initialScale;
    protected float _distanceToTarget;
    [SerializeField] Sprite deadSprite;
    #endregion

    protected float aiwalkDistance1;
    protected float aiwalkDistance2;
    void CalculateAngle(Vector3 temp)
    {
        dir = new Vector3(temp.x, transform.position.y, temp.z) - transform.position;
        angleTarget = Vector3.Angle(dir, transform.forward);
    }
    void LookAtThis(Vector3 targ)
    {
        CalculateAngle(transform.position + targ);
        if (angleTarget > 1)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), walkLeftRight * Time.deltaTime);
        }
    }
    void loadColor()
    {

    }
    protected virtual void Awake()
    {
        StartingSprite = GetComponent<SpriteRenderer>().sprite;
        aiwalkDistance1 = transform.position.x-15;//
        aiwalkDistance2 = transform.position.x+15;//
    }
    #region AIWalk
    protected virtual void CheckForWalls()
    {
        if (!ChangeDirectionOnWall)
        {
            return;
        }
        
        if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
        {
            ChangeDirection();
        }
        #region CheckForHoles
        if (!AvoidFalling || !_controller.State.IsGrounded)
        {
            return;
        }
        Vector2 raycastOrigin = new Vector2(transform.position.x + _direction.x * (HoleDetectionOffset.x + Mathf.Abs(GetComponent<BoxCollider2D>().bounds.size.x) / 2), transform.position.y + HoleDetectionOffset.y - (transform.localScale.y / 2));
        RaycastHit2D raycast = MMDebug.RayCast(raycastOrigin, -transform.up, HoleDetectionRaycastLength, _controller.PlatformMask | _controller.MovingPlatformMask | _controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask, Color.red, true);
        
        if (!raycast)
        {
            //ChangeDirection();
        }
        #endregion
    }
    protected virtual void Initialization()
    {
        _controller = GetComponent<CController>();
        _character = GetComponent<CharacterMotor3D>();//
        _characterHorizontalMovement = GetComponent<CharacterHorizontalMovement>();
        _health = GetComponent<Health>();
        _startPosition = transform.position;
        _direction = _character.IsFacingRight ? Vector2.right : Vector2.left;

        _initialDirection = _direction;
        _initialScale = transform.localScale;
        WalkBehaviour = WalkBehaviours.MoveOnSight;
    }
    protected virtual void CheckForTarget()
    {
        if (WalkBehaviour != WalkBehaviours.MoveOnSight)
        {
            return;
        }
        bool hit = false;

        _distanceToTarget = 0;
        Vector2 raycastOrigin = transform.position + MoveOnSightRayOffset;
        RaycastHit2D raycast = MMDebug.RayCast(raycastOrigin, Vector2.left, ViewDistance, MoveOnSightLayer, Color.yellow, true);
        if (raycast)
        {
            hit = true;
            _direction = Vector2.left;
            _distanceToTarget = raycast.distance;
        }
        raycast = MMDebug.RayCast(raycastOrigin, Vector2.right, ViewDistance, MoveOnSightLayer, Color.yellow, true);
        if (raycast)
        {
            hit = true;
            _direction = Vector2.right;
            _distanceToTarget = raycast.distance;
        }
        if ((!hit) || (_distanceToTarget <= StopDistance))
        {
            _direction = Vector2.zero;
        }
        else
        {
            RaycastHit2D raycastObstacle = MMDebug.RayCast(raycastOrigin, _direction, 1.5f, MoveOnSightObstaclesLayer, Color.gray, true);
            if (raycastObstacle && _distanceToTarget > raycastObstacle.distance)
            {
                _direction = Vector2.zero;
            }
        }
    }
    protected virtual void ChangeDirection()
    {
        _direction = -_direction;
    }

    protected virtual void OnRevive()
    {
        _direction = _character.IsFacingRight ? Vector2.right : Vector2.left;
        transform.localScale = _initialScale;
        transform.position = _startPosition;
    }
    #endregion
    protected Sprite StartingSprite;
    // Update is called once per frame.
    protected virtual void Update()
    {
        RaycastHit2D rayButton = MMDebug.RayCast(transform.position, Vector2.down, 1.5f, ButtonLayer, Color.blue, true);
        if(rayButton)
        {
            if (rayButton.collider.transform.GetComponent<EnemyCharmander>() != null)
            {
                rayButton.collider.transform.GetComponent<EnemyCharmander>().ButtonOnOff(true);
            }
        }
        if (MMDebug.RayCast(transform.position, Vector2.down, 0.5f, MoveOnSightObstaclesLayer, Color.black, true))
        {
            GetComponent<Animator>().SetBool("dead", true);
        }
        else {
            GetComponent<Animator>().SetBool("dead", false);
        }
        if (_direction.x != 0)
        {
            GetComponent<Animator>().SetBool("Walk", true);
            RaycastHit2D hit2D = MMDebug.RayCast(transform.position, Vector2.right, 1.8f, DeadLayer, Color.red, true);
            if (hit2D)
            {
                GameMode.THIS.loadisgameover(true);
            }
            if (MMDebug.RayCast(transform.position, -Vector2.right, 1.8f, DeadLayer, Color.red, true))
            {
                GameMode.THIS.loadisgameover(true);
            }
        }
        else { GetComponent<Animator>().SetBool("Walk", false); }
        if (_character == null)
        {
            return;
        }
        if ((_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
            || (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen))
        {
            return;
        }
        CheckForTarget();
        CheckForWalls();
        //print(_direction.x);
        _characterHorizontalMovement.SetHorizontalMove(_direction.x);
    }
    protected virtual void Start()
    {
        Initialization();
        GameMode.THIS.AddEnemy(gameObject);GameMode.THIS.AddPositionsEnemy(transform.position);
    }
    IEnumerator GetROtationCoroutine()
    {
        while (rotationCount != 0)
        {
            if (RotationNum == 1)
            {
                transform.TweenPosition(15/2, new Vector3(aiwalkDistance2, transform.position.y, transform.position.z));//
            }
            if (RotationNum == 2)
            {
                transform.TweenPosition(15/2, new Vector3(aiwalkDistance1, transform.position.y, transform.position.z));
            }
            if (RotationNum == 3)
            {
                transform.TweenPosition(0.5f, new Vector3(transform.position.x, transform.position.y, 100f));
            }
            if (RotationNum == 4)
            {
                transform.TweenPosition(0.5f, new Vector3(transform.position.x, transform.position.y, 0f));
            }
            yield return null;
            rotationCount++;
        }
    }

    public void RotAll()
    {

        if (RotationNum == 1)
        {
            Vector3 targetDirection = (transform.position + new Vector3(100, 0, 0)) - transform.position;
            float singleStep = speed * Time.deltaTime;
            Vector3 newdir = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newdir);

        }
        if (RotationNum == 2)
        {
            Vector3 targetDirection = (transform.position + new Vector3(-100, 0, 0)) - transform.position;
            float singleStep = speed * Time.deltaTime;
            Vector3 newdir = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newdir);

        }
        if (RotationNum == 3)
        {
            Vector3 targetDirection = (transform.position + new Vector3(0, 0, 100)) - transform.position;
            float singleStep = speed * Time.deltaTime;
            Vector3 newdir = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newdir);

        }
        if (RotationNum == 4)
        {
            Vector3 targetDirection = (transform.position + new Vector3(0, 0, -100)) - transform.position;
            float singleStep = speed * Time.deltaTime;
            Vector3 newdir = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            Debug.DrawRay(transform.position, newdir, Color.red);
            transform.rotation = Quaternion.LookRotation(newdir);
        }
    }
    private void Rotloader()
    {
        int randload = Random.Range(1, 5);
        if (randload != RotationNum)
        {
            RotationNum = randload;
        }
        else
        {
            Rotloader();
        }
    }
    public bool isequal(SphereGem hitCandy)
    {
        return hitCandy != null && hitCandy.type == type && hitCandy.type != "ingredient" + 0 && hitCandy.type != "ingredient" + 1;
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "wall")
        {
            StopCoroutine(GetROtationCoroutine());
            StartCoroutine(GetROtationCoroutine());
            if (RotationNum == 4)
            {
                RotationNum = 3;
            }
            else if (RotationNum == 3) RotationNum = 4;
            else if (RotationNum == 2) RotationNum = 1;
            else RotationNum = 2;
        }
        if (col.gameObject.tag == "sphere")
        {
            if (
                name != col.gameObject.name
                )
            {
                transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
            }
            else
            {
                if (RotationNum == 4)
                {
                    RotationNum = 3;
                }
                else if (RotationNum == 3) RotationNum = 4;
                else if (RotationNum == 2) RotationNum = 1;
                else RotationNum = 2;
            }
        }
    }
}
