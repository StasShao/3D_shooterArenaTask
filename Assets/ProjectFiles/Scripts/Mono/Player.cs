using System.Collections;
using ProjectFiles.Scripts.Base;
using ProjectFiles.Scripts.Services;
using ProjectFiles.Scripts.Settings.PlayerSettings;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Zenject;

namespace ProjectFiles.Scripts.Mono
{
    [RequireComponent (typeof(CharacterController))]
    public class Player:CharacterBase,IPlayer
    {
        private GameCoreLibrary.GameCoreLibrary.PlayerCharacterMotionController _playerCharacterMotionController;
        private GameCoreLibrary.GameCoreLibrary.WeaponGrabber _weaponGrabber;
        [SerializeField] private Rig[] characterRigs;
        [SerializeField] private Transform cameraLookTransform,cameraMoveTransform;
        [Inject] private IColliderCacheData<IWeapon> _colliderCacheData;
        [Inject] private IInputControllable _inputControllable;
        [SerializeField] private PlayerSettings playerSettings;
        private bool _isInit;
        public Rig[] IkRigs { get;private set; }
        public float DamageUpgradeValue { get;private set; }
        public Transform CameraLookTransform { get;private set; }
        public Transform CameraMoveTransform { get;private set; }
        public Animator PlayerAnimator { get;private set; }
        public CharacterController PlayerCharacterController { get;private set; }
        public PlayerSettings PlayerCharacterSettings { get;private set; }
        public void AddUpgrade()
        {
            DamageUpgradeValue += PlayerCharacterSettings.DamageUpgradeStep;
            AddModifiedHealth(playerSettings.HealthUpgradeStep);
            CharacterHealth = playerSettings.StartHealth;
            CharacterHealth += ModifiedHealth;
        }
        public void Attack()
        {
            if (_inputControllable.Attack)
            {
                if (CharacterWeapon != null)
                {
                    CharacterWeapon.AttackWeapon(true);
                }
            }
            else
            {
                if (CharacterWeapon != null)
                {
                    CharacterWeapon.AttackWeapon(false);
                }
            }
        }
        protected override void Initialize()//Init character
        {
            PlayerCharacterSettings = playerSettings;
            CharacterHealth = PlayerCharacterSettings.StartHealth;
            IkRigs = characterRigs;
            CameraLookTransform = cameraLookTransform;
            CameraMoveTransform = cameraMoveTransform;
            PlayerCharacterController = GetComponent<CharacterController>();
            PlayerAnimator = GetComponentInChildren<Animator>();
            _playerCharacterMotionController =
                new GameCoreLibrary.GameCoreLibrary.PlayerCharacterMotionController(this, _inputControllable);
            _weaponGrabber = new GameCoreLibrary.GameCoreLibrary.WeaponGrabber(this, _colliderCacheData,_inputControllable);
            _isInit = true;
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
        protected override void Tick()
        {
            _playerCharacterMotionController.OnTick();
            _weaponGrabber.WeaponGrabInRange(PlayerCharacterSettings.GrabRange);
            PlayerAnimator.SetFloat(PlayerCharacterSettings.ForwardMovementAnimatorName,_inputControllable.ForwardDirection);
            PlayerAnimator.SetFloat(PlayerCharacterSettings.FideMovementAnimatorName,_inputControllable.SideDirection);
            WeaponHold();
            Attack();
        }
        protected override void FixedTick()
        {
            _playerCharacterMotionController.OnMotion();
        }
        protected override void CharacterDeath()
        {
            PlayerAnimator.enabled = false;
            DropWeapon();
            foreach (var damageablePart in CharacterDamageableParts)
            {
                damageablePart.SatActive(false);
            }
        }
        protected override void OnCharacterEnable()
        {
            CharacterHealth = PlayerCharacterSettings.StartHealth;
            StartCoroutine(EnableDelay());
        }
        protected override void OnCharacterDisable() { }
        protected override void Upgrade() { }
        private IEnumerator EnableDelay()
        {
            yield return new WaitUntil(() => _isInit);
            CharacterHealthHud.minValue = 0;
            CharacterHealthHud.maxValue = CharacterHealth;
            CharacterHealthHud.value = CharacterHealth;
            foreach (var damageablePart in CharacterDamageableParts)
            {
                damageablePart.SatActive(true);
            }
            PlayerAnimator.enabled = true;
            foreach (var ragdollPart in CharacterRagdollParts)
            {
                ragdollPart.isKinematic = true;
            }
        }
    }
}