using System.Collections;
using ProjectFiles.Scripts.Installers;
using ProjectFiles.Scripts.Services;
using UnityEngine;

namespace ProjectFiles.Scripts.Base
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ShellBase:MonoBehaviour,IShell
    {
        [SerializeField] private float shellLifeTime;
        [SerializeField] private float damageValue;
        protected abstract void NextInit();
        protected abstract void OnShellEnable();
        public ICharacter Owner { get; private set; }
        public IColliderCacheData<ICharacterDamageable> CharacterCacheData { get;private set; }
        public VisualEffectSpawnerPoolInstaller VisualEffectPool { get;private set; }
        public float ShellDamage { get;private set; }
        public float LifeTime { get;private set; }
        public Rigidbody ShellBody { get; private set; }

        private void OnEnable()
        {
            ShellDamage = damageValue;
            StartCoroutine(LifeTimer());
            OnShellEnable();
        }
        public void Init()
        {
            ShellBody = GetComponent<Rigidbody>();
            LifeTime = shellLifeTime;
            ShellDamage = damageValue;
            NextInit();
        }
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void SetCharacterCacheData(IColliderCacheData<ICharacterDamageable> characterCacheData)
        {
            CharacterCacheData = characterCacheData;
        }

        public void SetVisualEffectPool(VisualEffectSpawnerPoolInstaller effectPool)
        {
            VisualEffectPool = effectPool;
        }

        public void SetOwner(ICharacter owner)
        {
            Owner = owner;
        }

        public void AddDamage(float damage)
        {
            ShellDamage += damage;
        }

        private IEnumerator LifeTimer()
        {
            yield return new WaitForSeconds(LifeTime);
            SetActive(false);
        }
        private void OnCollisionEnter(Collision other)
        {
            var cachedElements = CharacterCacheData.CachedElements;
            var cachedColliders = CharacterCacheData.CachedColliders;
            if (GameCoreLibrary.GameCoreLibrary.ColliderCache.TryGetElementCache<ICharacterDamageable>(
                    ref cachedElements, ref cachedColliders, other.collider,
                    out ICharacterDamageable character)&&Owner!= null&&character.Damageable != Owner.CharacterDamageableType)
            {
                character.Damage(ShellDamage);
                var effect = VisualEffectPool.GetFreeEffect(IVisualEffect.EffectType.HitEffect);
                if (effect != null)
                {
                    effect.EffectGameObject.transform.position = other.transform.position;
                    effect.EffectGameObject.transform.rotation = other.transform.rotation;
                    effect.EffectGameObject.SetActive(true);
                    effect.VisualEffect.Play();
                }
                SetActive(false);
            }
        }
    }
}