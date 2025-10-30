using System;
using System.Collections.Generic;
using ProjectFiles.Scripts.Base;
using ProjectFiles.Scripts.Services;
using UnityEngine;
using Zenject;

namespace ProjectFiles.Scripts.Installers
{
    public class VisualEffectSpawnerPoolInstaller:MonoInstaller
    {
        [SerializeField] private VisualEffectBase[] prefabHitEffects;
        [SerializeField] private Transform spawnPlaceTransform;
        [SerializeField] private int poolCount;
        [SerializeField] private int maxPoolCount;
        [SerializeField] private bool isAutoExpand;
        [HideInInspector] public List<VisualEffectBase> effects = new();

        private void CreatePool()
        {
            for (int i = 0; i < poolCount; i++)
            {
                for (int j = 0; j < prefabHitEffects.Length; j++)
                {
                    var prefab = prefabHitEffects[j];
                   var inst = Container.InstantiatePrefabForComponent<VisualEffectBase>(prefab, spawnPlaceTransform.position,
                        spawnPlaceTransform.rotation, null);
                    inst.Init();
                    inst.gameObject.SetActive(false);
                    effects.Add(inst);
                    Container.BindInstance(inst);
                }
            }
        }
        public override void InstallBindings()
        {
            Container.Bind<VisualEffectSpawnerPoolInstaller>().FromComponentInNewPrefab(this).AsSingle();
            CreatePool();
        }

        private void CreateEffect(IVisualEffect.EffectType effectType,out VisualEffectBase createdEffect)
        {
            VisualEffectBase effect = null;
            for (int i = 0; i < prefabHitEffects.Length; i++)
            {
                var effectPrefab = prefabHitEffects[i];
                VisualEffectBase inst = Instantiate(effectPrefab, spawnPlaceTransform.position, spawnPlaceTransform.rotation, null);
                inst.Init();
                inst.gameObject.SetActive(false);
                effects.Add(inst);
                effect = inst;
            }
            if (effect!= null&&effect.VisualEffectType == effectType)
            {
                createdEffect = effect;
            }
            else
            {
                createdEffect = null;
            }
        }
        public IVisualEffect GetFreeEffect(IVisualEffect.EffectType effectType)
        {
            foreach (var effect in effects)
            {
                if (!effect.gameObject.activeInHierarchy&&effect.VisualEffectType == effectType)
                {
                    return effect;
                }
            }
            if (isAutoExpand && maxPoolCount > effects.Count)
            {
                CreateEffect(effectType, out VisualEffectBase newEffect);
                if (newEffect != null)
                {
                    return newEffect;
                }
            }
            throw new Exception("There is no effects");
        }
    }
}