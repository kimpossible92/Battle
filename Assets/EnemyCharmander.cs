using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Gameplay.ShipSystems;
public enum TypeEnemyAttack
{
    tolchock,
    fire
}
public class EnemyCharmander : MonoBehaviour
{
    [System.Serializable]
    public struct RendererIndexData
    {
        public Renderer Renderer;
        public int MaterialIndex;

        public RendererIndexData(Renderer renderer, int index)
        {
            Renderer = renderer;
            MaterialIndex = index;
        }
    }
    RendererIndexData m_EyeRendererData;
    protected bool onoff = false;
    [SerializeField] GameObject Lamp;
    public bool islocked = false;
    int i1 = 0, i3 = 0;
    public Animator animator;
    [SerializeField] LayerMask buttonLayer;
    protected List<Transform> GetTransformsRaycast = new List<Transform>();
    protected Transform CenterPos;
    [SerializeField] GameObject FieldFire;
    protected int RotateNum = 0, oldrotNum = 0, newRotNum = 0;
    protected TypeEnemyAttack CurrentAttack;
    public NavMeshAgent agent;
    [SerializeField] Vector3 target;
    private float Health_ = 100f;
    Vector3 startVec;
    [SerializeField] UnityEngine.UI.Text GetText;
    [SerializeField] bool useparticle;
    [SerializeField] new ParticleSystem particleSystem;
    #region Npc_Controller
    public float patrolTime = 10;
    public float aggroRange = 10;
    public Transform[] waypoints;
    public AttackDefinition attack;
    public Transform SpellHotSpot;
    int index;
    float speed, agentSpeed;
    Transform player;
    private float timeOfLastAttack;
    private bool playerIsAlive;
    [SerializeField] LayerMask layerFps1;
    [SerializeField] LayerMask layerMaskFps2;
    private Transform target2;
    [SerializeField]
    private Transform AreaIn0;
    [SerializeField]
    private Transform[] AreaIn0123=new Transform[4];

    [SerializeField]
    private Transform _transform;
    #endregion
    public void StText(UnityEngine.UI.Text text1)
    {
        GetText = text1;
    }
    public void minusHealth(float _h)
    {
        Health_ -= _h;
    }
    private void loadthund()
    {

        particleSystem.gameObject.SetActive(false);
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agentSpeed = agent.speed;
        player = findClosestResourceWithTag("Player").transform;
        target2 = GameObject.FindGameObjectWithTag("Player").transform;
        _transform = transform;
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        index = Random.Range(0, waypoints.Length);

        InvokeRepeating("Tick", 0, 0.5f);

        if (waypoints.Length > 0)
        {
            InvokeRepeating("Patrol", Random.Range(0, patrolTime), patrolTime);
        }

        timeOfLastAttack = float.MinValue;
        playerIsAlive = true;
        //notplayer poka
        player.gameObject.GetComponent<DestructedEvent>().IDied += PlayerDied;
    }
    private void PlayerDied()
    {
        playerIsAlive = false;
    }

    IEnumerator GetTimerThreeSec()
    {
        yield return new WaitForSeconds(2.0f);
        //EveryFrame();
        yield return new WaitForSeconds(2.0f);
    }
    public IEnumerator GetEnumerator()
    {
        switch (islocked)
        {

            case true:
                while (i1 < 10)
                {
                    //print("0");
                    Lamp.transform.Translate(Vector2.down * 0.5f);
                    i1++;
                    yield return new WaitForSeconds(0.3f);
                }
                //islocked = false;
                break;
            case false:
                while (i1 >= 1)
                {
                    //print("1");
                    Lamp.transform.Translate(Vector2.up * 0.5f);
                    i1--;
                    yield return new WaitForSeconds(0.3f);
                }
                //islocked = true;
                break;
        }
    }
    protected int tick1;
    protected IEnumerator GetEnumReload(Vector3 tovec)
    {
        while (isFire1)
        {
            print(tick1);
            if (tick1 >= 225) { tick1 = 0; }
            tick1++;
            yield return new WaitForSeconds(1.0f);
        }
    }
    public void UpdDownWall()
    {
        Lamp.transform.Translate(Vector2.down * 2);
    }
    public void ButtonOnOff(bool value)
    {
        onoff = value;
    }
    Vector3 tStartPos1;
    // Use this for initialization
    void Start()
    {
        startVec = transform.position;
        animator = GetComponent<Animator>();
        isFire1 = true;
        //FieldFire.GetComponent<ParentFire>().enabled = false;
        //tStartPos1 = Lamp.transform.position;
        //GameMode.THIS.AddButtonLamps(this);
        animator.SetBool("walk3", true);
        for (int i = 0; i < 8; i++)
        {
            GetTransformsRaycast.Add(transform.Find("GameObject (" + i + ")"));
        }
        CenterPos = transform.Find(name);
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        //StartCoroutine(GetTimerThreeSec());
    }
    protected int tick = 0;
    protected const float koef = 0.2f;
    protected bool isFire1 = false; protected RaycastHit GetHit = new RaycastHit();
    protected void AddHits(RaycastHit hit)
    {
        if (GetHit.transform.position != hit.transform.position) { }
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
            //if (!go.GetComponent<ocupator>().isOccupied)
            //{
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
            //}
        }
        return closest;
    }
    public bool getdobivanie()
    {
        return dob;
    }
    public void SetDobivanie()
    {
        if (Health_ <= 1)
        {
            dob = false;
            transform.position = AreaIn0.position;
            Invoke("Resurect", 25.0f);
            //Destroy(gameObject);
        }
    }
    private void Resurect()
    {

        if (Health_ <= 1) { transform.position = AreaIn0123[Random.Range(0, AreaIn0123.Length)].position; }
        Health_ = 100;
    }
    bool dob = false;
    protected void EverFrame()
    {
        if (GetText != null)
        {
            
        }
        if (Health_ <= 0)
        {
            SetDobivanie();
        }
        if (findClosestResourceWithTag("Player") != null)
        {
            target = findClosestResourceWithTag("Player").transform.position;
            if (target != null)
            {

                agent.SetDestination(target);
                if (Vector3.Distance(transform.position, target) <= 1.6f)
                {

                    agent.SetDestination(transform.position);

                    InvokeRepeating("everyhealth", 2, 2);



                }
            }
        }
        if (particleSystem.gameObject.activeSelf == true)
        {
            Invoke("loadthund", 2f);
        }
    }
    private void everyhealth()
    {
        if (Vector3.Distance(transform.position, findClosestResourceWithTag("Player").transform.position) <= 1.6f)
        {
            findClosestResourceWithTag("Player").GetComponent<ICh>()._healthMinus();
            //animator.SetBool("Slash", true);
            //particleSystem.gameObject.SetActive(true);
        }
        else { animator.SetBool("Slash", false); animator.SetBool("walk3", true); }
    }

    
    public bool IsGrounded { get; private set; }
    // Update is called once per frame
    void Update()
    {
        speed = Mathf.Lerp(speed, agent.velocity.magnitude, Time.deltaTime * 10);
        animator.SetFloat("Speed", speed);
        //print(speed);
        float timeSinceLastAttack = Time.time - timeOfLastAttack;
        bool attackOnCooldown = timeSinceLastAttack < attack.Cooldown;

        agent.isStopped = attackOnCooldown;

        if (playerIsAlive)
        {
            float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
            bool attackInRange = distanceFromPlayer < attack.Range;

            if (!attackOnCooldown && attackInRange)
            {
                transform.LookAt(player.transform);
                timeOfLastAttack = Time.time;
                //InvokeRepeating("everyhealth", 2, 2);
                everyhealth();
                animator.SetTrigger("Attack");
            }
        }
        #region EvFrame
        if (GetText != null)
        {
            if (Health_ <= 0)
            {
                 dob = true; SetDobivanie();
            }
            else GetText.text = "enemy:" + Health_.ToString();
        }

        #endregion
        //EverFrame();
    }
    #region Spell
    public void Hit()
    {
        if (!playerIsAlive)
            return;

        if (attack is Weapon)
        {
            ((Weapon)attack).ExecuteAttack(gameObject, player.gameObject);
        }
        else if (attack is Spell)
        {
            ((Spell)attack).Cast(gameObject, SpellHotSpot.position, player.transform.position, LayerMask.NameToLayer("EnemySpells"));
        }
    }

    void Patrol()
    {
        index = index == waypoints.Length - 1 ? 0 : index + 1;
    }

    void Tick()
    {
        var currentPosition = _transform.position;
        var direction2 = (target2.position - currentPosition);
        agent.destination = waypoints[index].position;
        agent.speed = agentSpeed / 2;
        if (player != null && Vector3.Distance(transform.position, player.transform.position) < aggroRange)
        {
            agent.speed = agentSpeed;
            agent.destination = player.position;
        }
        if (MMDebug.Raycast3DBoolean(currentPosition, direction2, 20f, layerMaskFps2, Color.cyan))
        {
           
            return;
        }
        //else
        if(MMDebug.Raycast3DBoolean(currentPosition, direction2, 20f, layerFps1, Color.cyan))
        {
            _transform.LookAt(target2.position); agent.destination = _transform.position;
            if (GetComponent<WeaponSystem>() != null)
            {
                GetComponent<WeaponSystem>().TriggerFire();
                //print("fire");
            }
            return;
        }
        //agent.destination = waypoints[index].position;
        //agent.speed = agentSpeed / 2;

        //if (player != null && Vector3.Distance(transform.position, player.transform.position) < aggroRange)
        //{
        //    agent.speed = agentSpeed;
        //    agent.destination = player.position;
        //}
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
    #endregion
    private const float koef2 = 0.3f;
    private float distance1;
    protected void CharManderFire(Vector3 tovec)
    {
        GetComponent<CharacterMotor3D>().RotTowards(transform.position + tovec);
        if (CurrentAttack == TypeEnemyAttack.fire)
        {
            GameObject loadInst = Instantiate(FieldFire, FieldFire.transform.position, Quaternion.identity);
            loadInst.GetComponent<ParentFire>().enabled = true;
            loadInst.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            distance1 = tovec.x + tovec.z;
            if (distance1 < 0) { distance1 *= (-1); }
            else if (distance1 == 0) { distance1 = 3; }
            tovec *= 5;
            loadInst.transform.TweenPosition((distance1) * koef, transform.position + tovec + new Vector3(0, 0.3f, 0));
        }
        if (CurrentAttack == TypeEnemyAttack.tolchock)
        {
            distance1 = tovec.x + tovec.z;
            if (distance1 < 0) { distance1 *= (-1); }
            else if (distance1 == 0) { distance1 = 3; }
            transform.TweenPosition((distance1) * koef, transform.position + tovec + new Vector3(0, 0.3f, 0));
        }
    }
}
