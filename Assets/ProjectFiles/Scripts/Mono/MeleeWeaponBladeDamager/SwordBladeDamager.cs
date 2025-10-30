using System;
using ProjectFiles.Scripts.Installers;
using ProjectFiles.Scripts.Services;
using UnityEngine;

namespace ProjectFiles.Scripts.Mono.MeleeWeaponBladeDamager
{
    public class SwordBladeDamager:MonoBehaviour,IMeleeDamageMakeble
    {
        [SerializeField]private float swordDamage;
        public VisualEffectSpawnerPoolInstaller VisualEffectPool { get;private set; }
        public IColliderCacheData<ICharacterDamageable> DamageableData { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            var cachedElements = DamageableData.CachedElements;
            var cachedColliders = DamageableData.CachedColliders;
            if (GameCoreLibrary.GameCoreLibrary.ColliderCache.TryGetElementCache<ICharacterDamageable>(
                    ref cachedElements, ref cachedColliders, other,
                    out ICharacterDamageable character)&&Owner!= null&&character.Damageable != Owner.CharacterDamageableType)
            {
                character.Damage(swordDamage);
                var effect = VisualEffectPool.GetFreeEffect(IVisualEffect.EffectType.HitEffect);
                if (effect != null)
                {
                    effect.EffectGameObject.transform.position = other.transform.position;
                    effect.EffectGameObject.transform.rotation = other.transform.rotation;
                    effect.EffectGameObject.SetActive(true);
                    effect.VisualEffect.Play();
                }
            }
        }
        public ICharacter Owner { get; private set; }
        public Collider DamageMakeCollider { get;private set; }
        public void Init()
        {
            DamageMakeCollider = GetComponent<Collider>();
            if (DamageMakeCollider == null)
            {
                throw new Exception("Make sure add collider trigger to" + this);
            }
            Debug.Log(Owner);
        }
        public void SetOwner(ICharacter owner)
        {
            Owner = owner;
        }

        public void SetDamageColliderActive(bool active)
        {
            DamageMakeCollider.enabled = active;
        }
        public void SetVisualEffectPool(VisualEffectSpawnerPoolInstaller vfxPool)
        {
            VisualEffectPool = vfxPool;
        }
        public void SetDamageableData(IColliderCacheData<ICharacterDamageable> damageableData)
        {
            DamageableData = damageableData;
        }
    }
}