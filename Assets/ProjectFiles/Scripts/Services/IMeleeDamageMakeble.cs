using ProjectFiles.Scripts.Installers;
using UnityEngine;

namespace ProjectFiles.Scripts.Services
{
    public interface IMeleeDamageMakeble
    {
        VisualEffectSpawnerPoolInstaller VisualEffectPool { get; }
        IColliderCacheData<ICharacterDamageable> DamageableData { get; }
        ICharacter Owner { get; }
        Collider DamageMakeCollider { get; }
        void Init();
        void SetOwner(ICharacter owner);
        void SetDamageColliderActive(bool active);
        void SetVisualEffectPool(VisualEffectSpawnerPoolInstaller vfxPool);
        void SetDamageableData(IColliderCacheData<ICharacterDamageable> damageableData);
    }
}