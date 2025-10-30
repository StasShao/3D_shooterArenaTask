using System;
using System.Collections.Generic;
using ProjectFiles.Scripts.Base;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace ProjectFiles.Scripts.Installers
{
    public class WeaponSpawnerPoolInstaller:MonoInstaller
    {
        [SerializeField] private int poolCount,expandMaximumCount;
        [SerializeField] private bool isAutoExpand;
        [SerializeField] private WeaponBase[] prefabs;
        [SerializeField] private Transform spawnTransform;
        [HideInInspector] public List<WeaponBase> Weapons = new();
        private void InitializeSpawnWeapons()
        {
            for (int i = 0; i < poolCount; i++)
            {
                for (int j = 0; j < prefabs.Length; j++)
                {
                    var inst = Container.InstantiatePrefabForComponent<WeaponBase>(prefabs[j], spawnTransform.position,
                        spawnTransform.rotation, null);
                    inst.Init();
                    Weapons.Add(inst);
                    Container.BindInstance(inst);
                    inst.gameObject.SetActive(false);
                }
            }
        }
        private void OnExpand(out WeaponBase weapon)
        {
            var randomWeaponIndex = Random.Range(0, prefabs.Length);
            var inst = Instantiate(prefabs[randomWeaponIndex], spawnTransform.position,
                spawnTransform.rotation, null);
            inst.Init();
            Weapons.Add(inst);
            weapon = inst;
            inst.gameObject.SetActive(false);
        }
        public override void InstallBindings()
        {
            Container.Bind<WeaponSpawnerPoolInstaller>().FromComponentInNewPrefab(this).AsSingle();
            InitializeSpawnWeapons();
        }

        public WeaponBase TryGetWeapon()
        {
            foreach (var weapon in Weapons)
            {
                if (!weapon.gameObject.activeInHierarchy)
                {
                    return weapon;
                }
            }
            if (isAutoExpand&&Weapons.Count < expandMaximumCount)
            {
                OnExpand(out WeaponBase weapon);
                return weapon;
            }
            throw new Exception("There is no more characters");
        }
    }
}