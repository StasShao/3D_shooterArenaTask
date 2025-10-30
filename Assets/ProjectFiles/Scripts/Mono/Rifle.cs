using System;
using System.Collections.Generic;
using ProjectFiles.Scripts.Base;
using ProjectFiles.Scripts.Installers;
using ProjectFiles.Scripts.Services;
using ProjectFiles.Scripts.Settings.WeaponSettings.RangeWeaponSettings;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace ProjectFiles.Scripts.Mono
{
    [RequireComponent(typeof(AudioSource))]
    public class Rifle:WeaponBase,IRangeWeaponType
    {
        [SerializeField] private RangeWeaponSettings weaponSettings;
        [SerializeField] private AudioClip[] audioClips;
        [SerializeField] private ShellBase shellPrefab;
        [SerializeField] private int shellsCount,maxShells;
        [SerializeField] private bool isAutoExpand;
        [SerializeField] private Transform shellSpawnTransform;
        [Inject] private IColliderCacheData<ICharacterDamageable> _characterCacheData;
        [Inject] private VisualEffectSpawnerPoolInstaller _visualEffectSpawnerPool;
        private List<ShellBase> _shells = new();
        private AudioSource _audioSource;
        public RangeWeaponSettings WeaponSettings { get; private set; }
        protected override void Initialize()
        {
            WeaponSettings = weaponSettings;
            _audioSource = GetComponent<AudioSource>();
            CreateShellPool();
        }
        protected override void AttackNext()
        {
            WeaponAnimator.SetTrigger(WeaponSettings.AttackAnimatorTriggerName);
        }
        private void CreateShellPool()
        {
            for (int i = 0; i < shellsCount; i++)
            {
                var shell = Instantiate(shellPrefab, shellSpawnTransform.position, shellSpawnTransform.rotation, null);
                shell.SetCharacterCacheData(_characterCacheData);
                shell.SetVisualEffectPool(_visualEffectSpawnerPool);
                shell.SetOwner(WeaponOwner);
                shell.Init();
                shell.SetActive(false);
                _shells.Add(shell);
            }
        }

        private AudioClip RandomClip()
        {
            var randomClipIndex = Random.Range(0, audioClips.Length);
            return audioClips[randomClipIndex];
        }
        private void CreateShell(out ShellBase createdShell)
        {
            var shell = Instantiate(shellPrefab, shellSpawnTransform.position, shellSpawnTransform.rotation, null);
            shell.SetCharacterCacheData(_characterCacheData);
            shell.SetVisualEffectPool(_visualEffectSpawnerPool);
            shell.Init();
            shell.SetOwner(WeaponOwner);
            shell.SetActive(false);
            _shells.Add(shell);
            createdShell = shell;
        }
        private bool TryGetFreeShell(out ShellBase freeShell)
        {
            foreach (var shell in _shells)
            {
                if (!shell.gameObject.activeInHierarchy)
                {
                    freeShell = shell;
                    return true;
                }
            }
            freeShell = null;
            return false;
        }
        private ShellBase GetFreeShell()
        {
            if (TryGetFreeShell(out ShellBase freeShell))
            {
                return freeShell;
            }
            if (isAutoExpand && maxShells > _shells.Count)
            {
                CreateShell(out ShellBase createdShell);
                return createdShell;
            }
            throw new Exception("There is no shells to expand");
        }
        public void Shoot()
        {
            var shell = GetFreeShell();
            if (shell != null)
            {
                var clip = RandomClip();
                _audioSource.clip = clip;
                _audioSource.PlayOneShot(clip);
                shell.SetOwner(WeaponOwner);
                var shootEffect = _visualEffectSpawnerPool.GetFreeEffect(IVisualEffect.EffectType.FireEffect);
                if (shootEffect != null)
                {
                    shootEffect.EffectGameObject.transform.position = shellSpawnTransform.position;
                    shootEffect.EffectGameObject.transform.rotation = shellSpawnTransform.rotation;
                    shootEffect.EffectGameObject.SetActive(true);
                    shootEffect.VisualEffect.Play();
                }
                var shellTransform = shell.transform;
                var shellBody = shell.ShellBody;
                shellTransform.position = shellSpawnTransform.position;
                shellTransform.rotation = shellSpawnTransform.rotation;
                shellBody.velocity = Vector3.zero;
                shellBody.angularVelocity = Vector3.zero;
                shell.gameObject.SetActive(true);
                if (WeaponOwner.CharacterDamageableType == ICharacterDamageable.DamageableType.Player)
                {
                    var owner = (IPlayer)WeaponOwner;
                    Debug.Log(owner.DamageUpgradeValue);
                    shell.AddDamage(owner.DamageUpgradeValue);
                }
                shellBody.AddForce(shellSpawnTransform.forward * weaponSettings.ShootForce,ForceMode.Impulse);
            }
        }
    }
}