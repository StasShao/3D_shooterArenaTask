using System.Collections.Generic;
using ProjectFiles.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;


namespace ProjectFiles.Scripts.Base
{
    public abstract class CharacterBase:MonoBehaviour,ICharacter
    {
        [SerializeField] private ICharacterDamageable.DamageableType characterDamageableType;
        [SerializeField] private Transform characterWeaponPlaceTransform;
        [SerializeField] private Transform weaponHoldIkTransformLeft, weaponHoldIkTransformRight;
        [SerializeField] private Rigidbody[] characterRagdoll; 
        [SerializeField]private Slider healthHud;
        protected abstract void Initialize();
        protected abstract void Tick();
        protected abstract void FixedTick();
        protected abstract void CharacterDeath();
        protected abstract void OnCharacterEnable();
        protected abstract void OnCharacterDisable();
        protected abstract void Upgrade();
        public bool IsAlive { get; private set; }
        public Slider CharacterHealthHud { get;private set; }
        public ICharacterDamageable[] CharacterDamageableParts { get;private set; }
        public ICharacterDamageable.DamageableType CharacterDamageableType { get;private set; }
        public Rigidbody[] CharacterRagdollParts { get;private set; }
        public IWeapon CharacterWeapon { get;protected set; }
        public Transform CharacterWeaponPlaceTransform { get; private set; }
        public Transform CharacterTransform { get; private set; }
        public Transform WeaponHoldIkTransformLeft { get;private set; }
        public Transform WeaponHoldIkTransformRight { get;private set; }
        public float CharacterHealth { get; protected set; }
        public float ModifiedHealth { get;private set; }

        public void Death()
        {
            IsAlive = false;
            foreach (var ragdollPart in CharacterRagdollParts)
            {
                ragdollPart.isKinematic = false;
            }
            CharacterDeath();
        }
        private void OnEnable()
        {
            IsAlive = true;
            OnCharacterEnable();
        }
        private void OnDisable()
        {
            OnCharacterDisable();
        }
        public void TakeWeapon(IWeapon weapon)
        {
            if (CharacterWeapon != null)
            {
                CharacterWeapon.SetColliderActive(true);
                CharacterWeapon.SetKinematic(false);
                DropWeapon();
            }
            WeaponHoldIkTransformRight.position = weapon.ForRightHandIkTransform.position;
            WeaponHoldIkTransformLeft.position = weapon.ForLeftHandIkTransform.position;
            CharacterWeapon = weapon;
            CharacterWeapon.WeaponTransform.parent = CharacterWeaponPlaceTransform;
            CharacterWeapon.WeaponTransform.localPosition = Vector3.zero;
            CharacterWeapon.WeaponAnimator.enabled = true;
            CharacterWeapon.SetKinematic(true);
            CharacterWeapon.SetColliderActive(false);
            CharacterWeapon.SetOwner(this);
        }
        public void SetWeapon(IWeapon weapon)
        {
            CharacterWeapon = weapon;
        }
        public void DropWeapon()
        {
            if (CharacterWeapon == null)
            {
                return;
            }
            List<Transform> weaponHoldInTransforms = new();
            weaponHoldInTransforms.Add(WeaponHoldIkTransformLeft);
            weaponHoldInTransforms.Add(WeaponHoldIkTransformRight);
            foreach (var ikHold in weaponHoldInTransforms)
            {
                ikHold.parent = transform;
            }
            CharacterWeapon.WeaponAnimator.enabled = false;
            CharacterWeapon.WeaponTransform.parent = null;
            CharacterWeapon.SetKinematic(false);
            CharacterWeapon.SetColliderActive(true);
            CharacterWeapon.SetOwner(null);
            CharacterWeapon = null;
        }
        public void TakeDamage(float damage)
        {
            if (CharacterHealth == 0.0f)
            {
                return;
            }
            if (damage >= CharacterHealth)
            {
                CharacterHealth = 0.0f;
                Death();
                return;
            }
            CharacterHealth -= damage;
        }
        public void AddHealth(float health)
        {
            ModifiedHealth += health;
            Upgrade();
        }
        public void Init()
        {
            CharacterTransform = transform;
            CharacterWeaponPlaceTransform = characterWeaponPlaceTransform;
            CharacterDamageableType = characterDamageableType;
            WeaponHoldIkTransformLeft = weaponHoldIkTransformLeft;
            WeaponHoldIkTransformRight = weaponHoldIkTransformRight;
            CharacterRagdollParts = characterRagdoll;
            CharacterHealthHud = healthHud;
            CharacterHealthHud.maxValue = CharacterHealth;
            var damageables =  GetComponentsInChildren<ICharacterDamageable>();
            foreach (var damageable in damageables)
            {
                if(damageable == null)continue;
                damageable.InitCharacter(this);
            }
            CharacterDamageableParts = damageables;
            Initialize();
        }
        public void Tickable()
        {
            CharacterHealthHud.value = CharacterHealth;
            if(!IsAlive)return;
            Tick();
        }
        public void FixedTickable()
        {
            if(!IsAlive)return;
            FixedTick();
        }

        public void SetModifyHealthValue(float health)
        {
            ModifiedHealth = health;
        }

        public void AddModifiedHealth(float health)
        {
            ModifiedHealth += health;
        }
    }
}