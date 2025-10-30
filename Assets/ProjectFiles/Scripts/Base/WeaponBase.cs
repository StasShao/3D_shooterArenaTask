using System.Collections;
using ProjectFiles.Scripts.Services;
using UnityEngine;

namespace ProjectFiles.Scripts.Base
{
    [RequireComponent(typeof(Rigidbody),typeof(Animator))]
    public abstract class WeaponBase:MonoBehaviour,IWeapon
    {
        [SerializeField] private Transform weaponTransform, forLeftHandIkTransform, forRightHandIkTransform;
        [SerializeField] private float attackDelay;
        public IWeapon.WeaponState WeaponStates { get;private set; }
        public Animator WeaponAnimator { get;private set; }
        public Rigidbody WeaponBody { get;private set; }
        public Collider WeaponCollider { get;private set; }
        public ICharacter WeaponOwner { get; private set; }
        public Transform WeaponTransform { get;private set; }
        public Transform ForLeftHandIkTransform { get;private set; }
        public Transform ForRightHandIkTransform { get;private set; }
        protected abstract void Initialize();
        protected abstract void AttackNext();
        public void Init()
        {
            WeaponTransform = weaponTransform;
            ForLeftHandIkTransform = forLeftHandIkTransform;
            ForRightHandIkTransform = forRightHandIkTransform;
            WeaponCollider = GetComponent<Collider>();
            WeaponBody = GetComponent<Rigidbody>();
            WeaponAnimator = GetComponent<Animator>();
            Initialize();
        }
        private void OnEnable()
        {
            StartCoroutine(AttackDelayStart());
        }

        public void AttackWeapon(bool canAttack)
        {
            if (canAttack)
            {
                WeaponStates = IWeapon.WeaponState.Attack;
            }
            else
            {
                WeaponStates = IWeapon.WeaponState.NotAttack;
            }
        }
        public void SetOwner(ICharacter character)
        {
            WeaponOwner = character;
        }
        public void SetColliderActive(bool active)
        {
            if(WeaponCollider == null)return;
            WeaponCollider.enabled = active;
        }
        public void SetKinematic(bool isKinematic)
        {
            WeaponBody.isKinematic = isKinematic;
        }
        private IEnumerator AttackDelay()
        {
            AttackNext();
            WeaponStates = IWeapon.WeaponState.NotAttack;
            yield return new WaitForSeconds(attackDelay);
            StartCoroutine(AttackDelayStart());
        }
        private IEnumerator AttackDelayStart()
        {
            yield return new WaitUntil(() => WeaponOwner != null);
            if (WeaponStates == IWeapon.WeaponState.NotAttack)
            {
                yield return new WaitUntil(() => WeaponStates == IWeapon.WeaponState.Attack);
                AttackNext();
                WeaponStates = IWeapon.WeaponState.NotAttack;
                yield return new WaitForSeconds(attackDelay);
                StartCoroutine(AttackDelayStart());
                yield break;
            }
            yield return new WaitUntil(() => WeaponStates == IWeapon.WeaponState.Attack);
            StartCoroutine(AttackDelay());
        }
    }
}