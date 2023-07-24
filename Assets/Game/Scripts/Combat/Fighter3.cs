using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using UnityEngine;

//uses:
//actionScheduler
//mover
//health
namespace RPG.Combat
{
    public class Fighter3 : MonoBehaviour, IAction, ISaveable
    {
        //these properties relate to weapons and need to be moved
        [Header("Fighter3 Stats")]
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float timeBetweenAttacks;
        [SerializeField] private float weaponDamage;

        [Header("Weapon")]
        [SerializeField] private Transform rightHandPosition = null;
        [SerializeField] private Transform leftHandPosition = null;
        [SerializeField] private Weapon defaultWeapon = null;
        [SerializeField] private Weapon equippedWeapon = null;
        [SerializeField] private UnityEngine.UI.Text SwordText;
        [Header("timer")]
        public float timer = 20;
        
        //references 
        public Health3 target; //serialized for debug
        
        //cached
        private Mover mover;
        private ActionScheduler actionScheduler;
        private Animator anim;
      
        void Awake()
        {
             mover = GetComponent<Mover>();
             actionScheduler = GetComponent<ActionScheduler>();
             anim = GetComponent<Animator>();
        }

        private void Start()
        {
            if (!equippedWeapon)
            {
                EquipWeapon(defaultWeapon);
            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            equippedWeapon = weapon;
            weapon.SpawnToPlayer(rightHandPosition, leftHandPosition, anim);
            if (tag == "Player" &&SwordText!=null && weapon.name == "Unarmed") SwordText.text = "Unarmed";
            else if (tag == "Player"&& SwordText!=null)
            { SwordText.text = weapon.name; }
        }

        public void UnequipWeapon()
        {
            if (equippedWeapon == defaultWeapon) return;
            equippedWeapon.DestroyWeaponOnPlayer(rightHandPosition, leftHandPosition, anim);
            EquipWeapon(defaultWeapon);
        }
        
        
        void Update()
        {
            
            
            timer += Time.deltaTime;

            AttackS();
            if (!target)
                return;



            if (!InRange())
            {
                //player has not reached enemy
                mover.MoveTo(target.transform.position);
            }
            else //witihn range of target, safe to attack
            {

                transform.LookAt(target.transform);
                mover.Cancel();

                //do attacking stuff here
                AttackBehavior();

            }

        }


        private bool InRange()
        {
            var distance = Mathf.Abs(Vector3.Distance(transform.position, target.transform.position));
            return distance < equippedWeapon.GetWeaponRange();
        }
        private void AttackS()
        {
            if(Input.GetMouseButtonDown(1))
            {
                anim.ResetTrigger("stopAttack");
                anim.SetTrigger("attack");
                print("attack");

                if (equippedWeapon.IsRanged())
                    equippedWeapon.SpawnNewProjectile(transform, rightHandPosition, leftHandPosition);


                    timer = 0;
            }
        }

        private void AttackBehavior()
        {
            if (target.IsDead())
            {
                Cancel();
                actionScheduler.CancelAction();
                
            }

            else if (timer > equippedWeapon.GetTimeBetweenAttacks())
            {
                anim.ResetTrigger("stopAttack");
                anim.SetTrigger("attack");

                if (equippedWeapon.IsRanged())
                    equippedWeapon.SpawnProjectile(target.transform, rightHandPosition, leftHandPosition);


                timer = 0;

            }
        }

        public void Cancel()
        {
           
            target = null;
            anim.SetTrigger("stopAttack");

        }
       
        public void Attack(GameObject combatTarget)
        {
            actionScheduler.StartAction(this);
            target = combatTarget.GetComponent<Health3>();
        }

        public bool CanAttack(GameObject target)
        {
            return target && !target.GetComponent<Health3>().IsDead();
        }
        
        //animation event
        void Hit()
        {
            if (!target) return;
            
            target.TakeDamage(equippedWeapon.GetWeaponDamage());
            
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if(equippedWeapon)
                Gizmos.DrawWireSphere(transform.position,equippedWeapon.GetWeaponRange());
        }

        public void SetTarget(Health3 other)
        {
            target = other;
        }

        public object CaptureState()
        {
            return equippedWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string) state;
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}