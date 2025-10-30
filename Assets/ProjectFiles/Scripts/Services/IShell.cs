using ProjectFiles.Scripts.Installers;
using UnityEngine;

namespace ProjectFiles.Scripts.Services
{
    public interface IShell
    {
        ICharacter Owner { get; }
        IColliderCacheData<ICharacterDamageable> CharacterCacheData { get; }
        VisualEffectSpawnerPoolInstaller VisualEffectPool { get; }
        float ShellDamage { get; }
        float LifeTime { get; }
        Rigidbody ShellBody { get; }
        void Init();
        void SetActive(bool active);
        void SetCharacterCacheData(IColliderCacheData<ICharacterDamageable> characterCacheData);
        void SetVisualEffectPool(VisualEffectSpawnerPoolInstaller effectPool);
        void SetOwner(ICharacter owner);
        void AddDamage(float damage);
    }
}