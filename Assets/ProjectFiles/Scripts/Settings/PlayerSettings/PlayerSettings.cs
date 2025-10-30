using UnityEngine;

namespace ProjectFiles.Scripts.Settings.PlayerSettings
{
    [CreateAssetMenu(menuName = "Player settings")]
    public class PlayerSettings:ScriptableObject
    {
        [SerializeField] private float moveSpeed,lookSpeed,jumpForce,startHealth,gravityForce,grabRange,damageUpgradeStep,healthUpgradeStep;
        [SerializeField] private string forwardMovementAnimatorName,sideMovementAnimatorName;
        public float MoveSpeed { get { return moveSpeed; } }
        public float GravityForce { get { return gravityForce; } }
        public float LookSpeed { get { return lookSpeed; } }
        public float JumpForce { get { return jumpForce; } }
        public float StartHealth { get { return startHealth; } }
        public float GrabRange { get { return grabRange; } }
        public float DamageUpgradeStep { get { return damageUpgradeStep; } }
        public float HealthUpgradeStep { get { return healthUpgradeStep; } }
        public string ForwardMovementAnimatorName { get { return forwardMovementAnimatorName; } }
        public string FideMovementAnimatorName { get { return sideMovementAnimatorName; } }
    }
}