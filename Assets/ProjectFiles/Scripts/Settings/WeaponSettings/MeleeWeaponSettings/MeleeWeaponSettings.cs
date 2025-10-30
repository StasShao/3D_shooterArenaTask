using UnityEngine;

namespace ProjectFiles.Scripts.Settings.WeaponSettings.MeleeWeaponSettings
{
    [CreateAssetMenu(menuName = "MeleeWeapon settings")]
    public class MeleeWeaponSettings:ScriptableObject
    {
        [SerializeField] private float damage;
        [SerializeField] private string attackAnimatorTriggerName;
        public float Damage { get { return damage; } }
        public string AttackAnimatorTriggerName { get { return attackAnimatorTriggerName; } }
    }
}