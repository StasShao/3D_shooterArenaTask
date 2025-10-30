using UnityEngine;

namespace ProjectFiles.Scripts.Services
{
    public interface ICharacterDamageable
    {
        Collider DamageableCollider { get; }
        bool IsActive { get; }
        DamageableType Damageable { get; }
        ICharacter Character { get; }
        void InitCharacter(ICharacter character);
        void Damage(float damage);
        void SatActive(bool active);
        enum DamageableType
        {
            Enemy,
            Player
        }
    }
}