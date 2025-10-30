using UnityEngine;

namespace ProjectFiles.Scripts.Services
{
    public interface IWeapon
    {
        WeaponState WeaponStates { get; }
        Animator WeaponAnimator { get; }
        Rigidbody WeaponBody { get; }
        Collider WeaponCollider { get; }
        ICharacter WeaponOwner { get; }
        Transform WeaponTransform { get; }
        Transform ForLeftHandIkTransform { get; }
        Transform ForRightHandIkTransform { get; }
        void Init();
        void AttackWeapon(bool canAttack);
        void SetOwner(ICharacter character);
        void SetColliderActive(bool active);
        void SetKinematic(bool isKinematic);

        enum WeaponState
        {
            Attack,
            NotAttack
        }
    }
}