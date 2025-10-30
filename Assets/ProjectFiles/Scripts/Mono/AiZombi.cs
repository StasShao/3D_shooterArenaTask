using System.Collections;
using ProjectFiles.Scripts.Base;
using ProjectFiles.Scripts.Services;
using ProjectFiles.Scripts.Settings.AiEnemySettings;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using Zenject;

namespace ProjectFiles.Scripts.Mono
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AiZombi:CharacterBase,IAiEnemy
    {
        private GameCoreLibrary.GameCoreLibrary.AiCharacterBehaviourManager _aiCharacterBehaviour;
        [Inject] private IColliderCacheData<ICharacterDamageable> _characterDamageableCacheData;
        [Inject] private IColliderCacheData<IWeapon> _weaponCacheData;
        [SerializeField] private AiEnemySettings aiEnemySettings;
        [SerializeField] private Rig[] characterRigs;
        public Rig[] IkRigs { get; private set; }
        public float ModifiedSpeed { get;private set; }
        public NavMeshAgent Agent { get; private set; }
        public Animator CharacterAnimator { get;private set; }
        public Vector3 WayPoint { get;private set; }
        public AiEnemySettings CharacterSettings { get;private set; }
        private bool _isInit;
        private float _startAgentSpeed;
        protected override void Initialize()
        {
            Agent = GetComponent<NavMeshAgent>();
            CharacterAnimator = GetComponentInChildren<Animator>();
            CharacterSettings = aiEnemySettings;
            CharacterHealth = CharacterSettings.StartHealth;
            SetModifyHealthValue(200);
            IkRigs = characterRigs;
            _aiCharacterBehaviour = new GameCoreLibrary.GameCoreLibrary.AiCharacterBehaviourManager(this,_characterDamageableCacheData,_weaponCacheData);
            _startAgentSpeed = Agent.speed;
            _isInit = true;
        }
        protected override void Tick()
        {
            if(!gameObject.activeInHierarchy)return;
            WeaponHold();
            CharacterAnimator.SetFloat(CharacterSettings.ForwardMovementAnimatorName,Agent.velocity.magnitude);
            _aiCharacterBehaviour.OnAiBehaviourTick();
        }
        protected override void FixedTick()
        {
            if(!gameObject.activeInHierarchy)return;
        }
        protected override void CharacterDeath()
        {
            CharacterAnimator.enabled = false;
            Agent.speed = 0;
            DropWeapon();
            foreach (var damageablePart in CharacterDamageableParts)
            {
                damageablePart.SatActive(false);
            }
            StartCoroutine(DeathDelay());
        }
        protected override void OnCharacterEnable()
        {
            StartCoroutine(EnableDelay());
        }
        protected override void OnCharacterDisable()
        {
            
        }
        protected override void Upgrade()
        {
            if(Agent.speed >= CharacterSettings.MaximumMoveSpeed)return;
            ModifiedSpeed += CharacterSettings.MoveSpeed;
            Debug.Log(Agent.speed);
        }

        public void FollowToPosition(Vector3 position)
        {
            Agent.SetDestination(position);
        }

        public void Attack()
        {
            if(CharacterWeapon == null)return;
        }
        public void SetNextWayPoint(Vector3 nextWayPoint)
        {
            WayPoint = nextWayPoint;
        }
        private void WeaponHold()
        {
            if (CharacterWeapon == null)
            {
                foreach (var rig in IkRigs)
                {
                    rig.weight = 0;
                }
                return;
            }
            CharacterWeapon.WeaponTransform.position = CharacterWeaponPlaceTransform.position;
            CharacterWeapon.WeaponTransform.rotation = CharacterWeaponPlaceTransform.rotation;
            WeaponHoldIkTransformRight.position = CharacterWeapon.ForRightHandIkTransform.position;
            WeaponHoldIkTransformLeft.position = CharacterWeapon.ForLeftHandIkTransform.position;
            foreach (var rig in IkRigs)
            {
                rig.weight = 1;
            }
        }
        private IEnumerator EnableDelay()
        {
            yield return new WaitUntil(() => _isInit);
            CharacterHealthHud.minValue = 0;
            CharacterHealthHud.maxValue = CharacterSettings.StartHealth + ModifiedHealth;
            CharacterHealthHud.value = CharacterHealthHud.maxValue ;
            foreach (var damageablePart in CharacterDamageableParts)
            {
                damageablePart.SatActive(true);
            }
            Agent.speed = _startAgentSpeed + ModifiedSpeed;
            CharacterHealth = CharacterSettings.StartHealth + ModifiedHealth;
            foreach (var ragdollPart in CharacterRagdollParts)
            {
                ragdollPart.isKinematic = true;
            }
            CharacterAnimator.enabled = true;
        }

        private IEnumerator DeathDelay()
        {
            yield return new WaitForSeconds(5);
            gameObject.SetActive(false);
        }
    }
}