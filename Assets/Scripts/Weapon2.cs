using Gameplay.Weapons;
using Gameplay.Weapons.Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class Weapon2 : MonoBehaviour
{
    [SerializeField]
    private ProjectilePool _projectile;
    [SerializeField]
    private ProjectilePool _laser;
    [SerializeField]
    private ProjectilePool[] projectiles;
    int wpnum;
    [SerializeField]
    private Transform _barrel;

    [SerializeField]
    private float _cooldown;
    [SerializeField] LayerMask layerMaskFps2;

    private bool _readyToFire = true;
    private UnitBattleIdentity _battleIdentity;
    PlayerInput _playerInput;
    public void Update()
    {

    }
    public void newCooldown()
    {
        //_cooldown = 0.33f;
        Invoke("old", 8.0f);
    }
    private void old()
    {
        _cooldown = 0.1f;
    }
    public void setwp2()
    {
        wpnum = 2;
    }
    public void setwp()
    {
        wpnum = 1;
    }
    public void Init(UnitBattleIdentity battleIdentity)
    {
        _battleIdentity = battleIdentity;
    }

    bool isplayer = false;
    public void setpl1()
    {
        isplayer = true;
    }
    public float Power=10f;
    public void FirePlayer(Vector3 _position)
    {
        if (!_readyToFire)
            return;
        var RanVariant = Random.Range(0, 3);
        var target2 = _position;
        var currentPosition = transform.position;
        float _speed = 5.3f;
        var direction2 = (target2 - currentPosition);

        LayerMask _mask = LayerMask.GetMask("Impact");
        Ray ray = new Ray(transform.position, direction2);
        if (MMDebug.Raycast3DBoolean(currentPosition, direction2, 25, layerMaskFps2, Color.cyan))
        {
            _projectile = projectiles[wpnum];
            var proj = Instantiate(_projectile, transform.position, transform.rotation);
            proj.SetDirection(direction2, _speed, false);
        }
        StartCoroutine(Reload(_cooldown));
    }
    public void TriggerFire()
    {
        if (!_readyToFire)
            return;
        var RanVariant = Random.Range(0, 3);
       
        var target2 = GameObject.FindGameObjectWithTag("Player").transform;
        
        var currentPosition = transform.position;
        float _speed = 1.3f; 
        var direction2 = (target2.position - currentPosition);
        
        LayerMask _mask = LayerMask.GetMask("Impact");
        Ray ray = new Ray(transform.position, direction2);
        //RaycastHit hit;
        if (MMDebug.Raycast3DBoolean(currentPosition, direction2, 25, layerMaskFps2, Color.cyan))
        {
            //hit.rigidbody.AddForceAtPosition(ray.direction * Power, hit.point, ForceMode.Impulse);
            _projectile = projectiles[wpnum];
            var proj = Instantiate(_projectile, transform.position, transform.rotation);
            #region рекошет
            if (RanVariant == 0)
            {
                var dirs = new List<float>();
                foreach (var dir in target2.GetComponent<ICh>().RayPositions) { dirs.Add(Vector3.Distance(currentPosition,dir)); }

                //for(int i = 0; i < 4; i++) { dirs[i] = target2.GetComponent<ICh>().RayPositions[i]-currentPosition; }
                if (dirs.Count > 2)
                {
                    var MinDir = dirs.Min();
                    for(int i = 0; i < 4; i++)
                    {
                        if(Vector3.Distance(currentPosition, target2.GetComponent<ICh>().RayPositions[i]) == MinDir)
                        {
                            proj.SetDirection(target2.GetComponent<ICh>().RayPositions[i]-currentPosition, _speed, true);
                            return;
                        }
                    }
                }
            }
            #endregion
            proj.SetDirection(direction2, _speed,false);
            //proj.GetComponent<NavMeshAgent>().destination = target2.position;
            //target2.GetComponent<ICh>()._healthMinus();
            //print("fire");
            
            
            
            //proj.GetComponent<Rigidbody>().AddForce(hit.point - transform.position * proj.GetComponent<Rigidbody>().mass, ForceMode.Impulse);
            proj.Init(_battleIdentity);
            if (isplayer)
            {
                //proj.tag = "Player";
                //PoolManager.GetObject(proj.gameObject.name, proj.gameObject.transform.position,
                //    proj.gameObject.transform.rotation);
                //proj.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
        StartCoroutine(Reload(_cooldown));
    }
    public void TriggerLaser()
    {
        if (!_readyToFire)
            return;
        _projectile = projectiles[wpnum];
        var proj = Instantiate(_laser, _barrel.position, _barrel.rotation);
        proj.Init(_battleIdentity);
        if (isplayer) { proj.tag = "laser"; PoolManager.GetObject(proj.gameObject.name, proj.gameObject.transform.position, proj.gameObject.transform.rotation); proj.GetComponent<SpriteRenderer>().color = Color.green; }
        StartCoroutine(Reload(_cooldown));
    }

    private IEnumerator Reload(float cooldown)
    {
        _readyToFire = false;
        yield return new WaitForSeconds(cooldown);
        _readyToFire = true;
    }
}
