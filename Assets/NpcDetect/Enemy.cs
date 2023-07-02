using Gameplay.ShipSystems;
using System.Collections;
using UnityEngine;
public class Enemy2 : MonoBehaviour
{
    private Coroutine _watching;

    [SerializeField] private DetectionArea DetectionArea;

    [SerializeField] private float Distance = 1f;
    [SerializeField] private float FieldOfView = 30;
    [SerializeField] private Vector3 Center = Vector3.zero;
    public string targetTag = "Player";
    public int rays = 6;
    public int distance = 15;
    public float angle = 20;
    public Vector3 offset;
    private Transform target;
    [SerializeField]
    private Transform _transform;
    #region fps1
    [SerializeField] LayerMask layerFps1;
    [SerializeField]LayerMask layerMaskFps2;
    protected TypeEnemyAttack CurrentAttack; 
    public AttackDefinition attack;
    private float timeOfLastAttack;
    private bool playerIsAlive;
    public float patrolTime = 10;
    public float aggroRange = 6.6f;
    public Transform[] waypoints;
    int index;
    public UnityEngine.AI.NavMeshAgent agent;
    float speed, agentSpeed;
    Transform player;
    #endregion
    private void Start()
    {
        timeOfLastAttack = float.MinValue;
        playerIsAlive = true;
        index = Random.Range(0, waypoints.Length);
        if (_transform == null)
            _transform = gameObject.GetComponent<Transform>();
        if (DetectionArea != null)
            DetectionArea.Create(Distance, Center);
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        player = GameObject.FindGameObjectWithTag(targetTag).transform;
        //agent.destination = waypoints[index].position;
        //agent.speed = agentSpeed / 2; print(agent.destination);
        if (waypoints.Length > 0)
        {
            InvokeRepeating("Patrol", Random.Range(0, 0.8f), 10f);
        }
        InvokeRepeating("tick2", 0, 0.5f);


    }
    private void Update()
    {
        //playerIsAlive = GetComponent<EnemyCharmander>()._isAlive;
        //float timeSinceLastAttack = Time.time - timeOfLastAttack;
        //bool attackOnCooldown = timeSinceLastAttack < attack.Cooldown;

        //agent.isStopped = attackOnCooldown;

        //if (playerIsAlive)
        //{
        //    float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        //    bool attackInRange = distanceFromPlayer < attack.Range;

        //    if (!attackOnCooldown && attackInRange)
        //    {
        //        transform.LookAt(player.transform);
        //        timeOfLastAttack = Time.time;
        //        GetComponent<Animator>().SetTrigger("Attack");
        //    }
        //}
    }
    bool RayToScan()
    {
        bool result = false;
        bool a = false;
        bool b = false;
        float j = 0;
        for (int i = 0; i < rays; i++)
        {
            var x = Mathf.Sin(j);
            var y = Mathf.Cos(j);

            j += angle * Mathf.Deg2Rad / rays;

            Vector3 dir = transform.TransformDirection(new Vector3(x, 0, y));
            if (GetRaycast(dir)) a = true;

            if (x != 0)
            {
                dir = transform.TransformDirection(new Vector3(-x, 0, y));
                if (GetRaycast(dir)) b = true;
            }
        }

        if (a || b) result = true;
        return result;
    }
    bool GetRaycast(Vector3 dir)
    {
        bool result = false;
        RaycastHit hit = new RaycastHit();
        Vector3 pos = transform.position + offset;
        if (Physics.Raycast(pos, dir, out hit, distance))
        {
            if (hit.transform == target)
            {
                result = true;
                Debug.DrawLine(pos, hit.point, Color.green);
            }
            else
            {
                Debug.DrawLine(pos, hit.point, Color.blue);
            }
        }
        else
        {
            Debug.DrawRay(pos, dir * distance, Color.red);
        }
        return result;
    }

    public void Tick()
    {
        

    }
    public void WatchDetect()
    {

        //yield return new WaitForFixedUpdate();
    }
    void Patrol()
    {
        index = index == waypoints.Length - 1 ? 0 : index + 1;
    }
    void tick2()
    {
        var currentPosition = _transform.position;
        foreach (var detectableObjects in DetectionArea.GetDetectableInArea())
        {
            
            var direction = (detectableObjects.GetPosition() - currentPosition);
            var direction2 = (target.position - currentPosition);
            var distance = Vector3.Distance(detectableObjects.GetPosition(), currentPosition);

            if (!Physics.Raycast(currentPosition, direction, out var hit, distance))
            {
                detectableObjects.UnDetect();

                continue;
            }

            if (hit.transform.gameObject != detectableObjects.GetGameObject())
            {
                detectableObjects.UnDetect();
                continue;
            }

            if (!(Vector3.Angle(_transform.right, direction.normalized) < FieldOfView / 2f))
            {
                detectableObjects.UnDetect();
                continue;
            }
            if (MMDebug.Raycast3DBoolean(currentPosition, direction2, GetComponent<SphereDetectionArea>().distance, layerMaskFps2, Color.cyan))
            {
            }
            else if (MMDebug.Raycast3DBoolean(currentPosition, direction2, GetComponent<SphereDetectionArea>().distance, layerFps1, Color.cyan))
            {
                _transform.LookAt(target);
                if (GetComponent<WeaponSystem>() != null)
                {
                    GetComponent<WeaponSystem>().TriggerFire(); print("tick");
                }

                detectableObjects.Detect();
                //if (ht2.collider.tag != "Player")
                //{

                //return;
                //}
                //detectableObjects.UnDetect();
                //continue;

            }
            else {
                
                //agent.destination = waypoints[index].position;
                //agent.speed = agentSpeed / 2;

                //if (player != null && Vector3.Distance(transform.position, player.transform.position) < aggroRange)
                //{
                //    agent.speed = agentSpeed;
                //    agent.destination = player.position;
                //}
            }
        }
    }
}
