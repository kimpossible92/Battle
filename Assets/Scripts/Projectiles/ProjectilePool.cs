using System;
//using Gameplay.Helpers;
using UnityEngine;

namespace Gameplay.Weapons.Projectiles
{
    public abstract class ProjectilePool : MonoBehaviour, IDamageDealer
    {
        #region Interface
        [SerializeField]
        private bool isDontDestroy;
        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }
        #endregion
        [SerializeField]
        private float _speed;

        [SerializeField] 
        private float _damage;

        public bool _dontDestroyEnemy => isDontDestroy;
        private UnitBattleIdentity _battleIdentity;
        private Vector3 _Direction = Vector3.zero;
        private bool ImpactToPlayer = false;
        public void SetDirection(Vector3 vector,float speed1,bool _impactToPl)
        {
            _speed =  speed1;
            _Direction = vector;
            ImpactToPlayer = _impactToPl;
        }


        public UnitBattleIdentity BattleIdentity => _battleIdentity;
        public float Damage => _damage;
        public float Speed=>_speed;

        
        public void SetDestroy(bool isdestroy)
        {
            isDontDestroy = isdestroy;
        }
        public void Init(UnitBattleIdentity battleIdentity)
        {
            _battleIdentity = battleIdentity;
        }
        private void Start()
        {
            Invoke("isDestroy", 5.1f);
        }
        private void isDestroy()
        {
            Destroy(gameObject);
        }
        private void Update()
        {
            //Move(_speed);
            transform.position += _Direction * Time.deltaTime * _speed;
        }

        
        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.GetComponent<ICh>()) other.gameObject.GetComponent<ICh>()._healthMinus();
            var damagableObject = other.gameObject.GetComponent<IDamagable>();
            
            if (damagableObject != null 
                && damagableObject.BattleIdentity != BattleIdentity && other.gameObject.tag !="bonus")
            {
                damagableObject.ApplyDamage(this);
            }
            if (other.gameObject.tag == "Metal") { if (ImpactToPlayer) { _Direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position; } else if (_Direction.z > 0) { _Direction = new Vector3(-_Direction.x, _Direction.y, _Direction.z); } else { _Direction = new Vector3(_Direction.x, _Direction.y, -_Direction.z); } }
            if (other.gameObject.tag == "Player") { Destroy(gameObject); }
            //if (other.gameObject.tag == "enemy") { Destroy(gameObject); }
        }
        


        protected abstract void Move(float speed);
    }
}
