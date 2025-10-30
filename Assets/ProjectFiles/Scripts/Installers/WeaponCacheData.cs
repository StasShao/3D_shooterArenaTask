using System.Collections.Generic;
using ProjectFiles.Scripts.Base;
using ProjectFiles.Scripts.Services;
using UnityEngine;
using Zenject;

namespace ProjectFiles.Scripts.Installers
{
    public class WeaponCacheData:MonoInstaller,IColliderCacheData<IWeapon>
    {
        public List<IWeapon> CachedElements { get; private set; } = new();
        public List<Collider> CachedColliders { get;private set; }= new();
        public override void InstallBindings()
        {
            Container.Bind<IColliderCacheData<IWeapon>>().To<WeaponCacheData>().FromComponentInNewPrefab(this)
                .AsSingle();
        }
    }
}