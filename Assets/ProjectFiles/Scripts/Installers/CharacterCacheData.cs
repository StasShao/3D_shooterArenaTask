using System.Collections.Generic;
using ProjectFiles.Scripts.Services;
using UnityEngine;
using Zenject;

namespace ProjectFiles.Scripts.Installers
{
    public class CharacterCacheData:MonoInstaller,IColliderCacheData<ICharacterDamageable>
    {
        public List<ICharacterDamageable> CachedElements { get; private set; } = new();
        public List<Collider> CachedColliders { get;private set; }= new();
        public override void InstallBindings()
        {
            Container.Bind<IColliderCacheData<ICharacterDamageable>>().FromComponentInNewPrefab(this).AsSingle();
        }
    }
}