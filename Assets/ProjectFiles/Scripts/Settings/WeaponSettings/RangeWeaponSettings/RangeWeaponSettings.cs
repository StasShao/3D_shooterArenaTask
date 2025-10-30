using UnityEngine;

namespace ProjectFiles.Scripts.Settings.WeaponSettings.RangeWeaponSettings
{
    [CreateAssetMenu(menuName = "Range settings")]
    public class RangeWeaponSettings:ScriptableObject
    {
        [SerializeField] private float damage,shootForce;
        [SerializeField] private string attackAnimatorTriggerName;
        public float Damage { get { return damage; } }
        public float ShootForce { get { return shootForce; } }
        public string AttackAnimatorTriggerName { get { return attackAnimatorTriggerName; } }
    }
}