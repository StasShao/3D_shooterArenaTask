using System;
using System.Collections.Generic;
using ProjectFiles.Scripts.Base;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace ProjectFiles.Scripts.Installers
{
    public class AiEnemySpawnerPoolInstaller:MonoInstaller
    {
        [SerializeField] private int poolCount,expandMaximumCount;
        [SerializeField] private bool isAutoExpand;
        [SerializeField] private CharacterBase[] prefabs;
        [SerializeField] private Transform spawnTransform;
        [HideInInspector] public List<CharacterBase> Characters = new();
         public int MaximumCharacters { get { return expandMaximumCount; } }

        private void InitializeSpawnCharacters()
        {
            for (int i = 0; i < poolCount; i++)
            {
                for (int j = 0; j < prefabs.Length; j++)
                {
                    var inst = Container.InstantiatePrefabForComponent<CharacterBase>(prefabs[j], spawnTransform.position,
                        spawnTransform.rotation, null);
                    inst.Init();
                    Characters.Add(inst);
                    Container.BindInstance(inst);
                    inst.gameObject.SetActive(false);
                }
            }
        }

        private void OnExpand(out CharacterBase character)
        {
            var randomCharacterIndex = Random.Range(0, prefabs.Length);
            var inst = Instantiate(prefabs[randomCharacterIndex], spawnTransform.position,
                spawnTransform.rotation, null);
            inst.Init();
            Characters.Add(inst);
            character = inst;
            inst.gameObject.SetActive(false);
        }
        public override void InstallBindings()
        {
            Container.Bind<AiEnemySpawnerPoolInstaller>().FromComponentInNewPrefab(this).AsSingle();
            InitializeSpawnCharacters();
        }

        public CharacterBase TryGetCharacter()
        {
            foreach (var character in Characters)
            {
                if (!character.gameObject.activeInHierarchy)
                {
                    return character;
                }
            }
            if (isAutoExpand&&Characters.Count < expandMaximumCount)
            {
                OnExpand(out CharacterBase character);
                return character;
            }
            throw new Exception("There is no more characters");
        }
    }
}