using UnityEngine;
using UnityEngine.UI;


namespace ProjectFiles.Scripts.Services
{
    public interface ICharacter
    {
        bool IsAlive { get; }
        Slider CharacterHealthHud { get; }
        ICharacterDamageable[] CharacterDamageableParts { get; }
        ICharacterDamageable.DamageableType CharacterDamageableType { get; }
        Rigidbody[] CharacterRagdollParts { get; }
        IWeapon CharacterWeapon { get; }
        Transform CharacterWeaponPlaceTransform { get; }
        Transform CharacterTransform { get; }
        Transform WeaponHoldIkTransformLeft { get; }
        Transform WeaponHoldIkTransformRight { get; }
        float CharacterHealth { get; }
        float ModifiedHealth { get; }
        void Death();
        void TakeWeapon(IWeapon weapon);
        void SetWeapon(IWeapon weapon);
        void DropWeapon();
        void TakeDamage(float damage);
        void AddHealth(float health);
        void Init();
        void Tickable();
        void FixedTickable();
        void SetModifyHealthValue(float health);
        void AddModifiedHealth(float health);
    }
}