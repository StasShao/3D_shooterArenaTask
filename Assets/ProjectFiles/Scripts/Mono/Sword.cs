using ProjectFiles.Scripts.Base;
using ProjectFiles.Scripts.Installers;
using ProjectFiles.Scripts.Services;
using ProjectFiles.Scripts.Settings.WeaponSettings.MeleeWeaponSettings;
using UnityEngine;
using Zenject;

namespace ProjectFiles.Scripts.Mono
{
    public class Sword:WeaponBase,IMeleeWeapon
    {
        [SerializeField] private MeleeWeaponSettings meleeWeaponSettings;
        [SerializeField] private Collider damageTrigger;
        [Inject] private VisualEffectSpawnerPoolInstaller _visualEffectSpawnerPool;
        [Inject] private IColliderCacheData<ICharacterDamageable> _colliderCacheData;
        private IMeleeDamageMakeble _meleeDamageMakeble;
        public MeleeWeaponSettings WeaponSettings { get; private set; }
        protected override void Initialize()
        {
            WeaponSettings = meleeWeaponSettings;
            _meleeDamageMakeble = GetComponentInChildren<IMeleeDamageMakeble>();
            _meleeDamageMakeble.Init();
            _meleeDamageMakeble.SetDamageableData(_colliderCacheData);
            _meleeDamageMakeble.SetVisualEffectPool(_visualEffectSpawnerPool);
            DeactivateDamageTrigger();
        }
        protected override void AttackNext()
        {
            _meleeDamageMakeble.SetOwner(WeaponOwner);
            WeaponAnimator.SetTrigger(WeaponSettings.AttackAnimatorTriggerName);
        }
        public void ActivateDamageTrigger()
        {
            damageTrigger.enabled = true;
        }

        public void DeactivateDamageTrigger()
        {
            damageTrigger.enabled = false;
        }
    }
}