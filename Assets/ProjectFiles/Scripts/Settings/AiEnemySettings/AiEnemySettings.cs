using UnityEngine;

namespace ProjectFiles.Scripts.Settings.AiEnemySettings
{
    [CreateAssetMenu(menuName = "AiEnemy settings")]
    public class AiEnemySettings:ScriptableObject
    {
        [SerializeField] private float moveSpeed,maximumMoveSpeed,lookSpeed,startHealth,searchRange,meleeAttackDistance,rangeAttackDistance;
        [SerializeField] private string forwardMovementAnimatorName,sideMovementAnimatorName;
        [SerializeField] private int maximumEnemyDifficult;
        [SerializeField] private Vector3 moveZoneClamp;
        private int _enemyDifficult;
        public float MoveSpeed { get { return moveSpeed; } }
        public float MaximumMoveSpeed { get { return maximumMoveSpeed; } }
        public float LookSpeed { get { return lookSpeed; } }
        public float StartHealth { get { return startHealth; } }
        public float SearchRange { get { return searchRange; } }
        public float MeleeAttackDistance { get { return meleeAttackDistance; } }
        public float RangeAttackDistance { get { return rangeAttackDistance; } }
        public string ForwardMovementAnimatorName { get { return forwardMovementAnimatorName; } }
        public string FideMovementAnimatorName { get { return sideMovementAnimatorName; } }
        public int EnemyDifficult { get { return _enemyDifficult; } }
        public Vector3 MoveZoneClamp { get { return moveZoneClamp; } }

        public void SetEnemyDifficult(int difficult)
        {
            _enemyDifficult = difficult;
            if (_enemyDifficult >= maximumEnemyDifficult)
            {
                _enemyDifficult = maximumEnemyDifficult;
            }
            if (_enemyDifficult < 0)
            {
                _enemyDifficult = 0;
            }
        }
        public void PlusEnemyDifficult()
        {
            if (_enemyDifficult >= maximumEnemyDifficult)
            {
                _enemyDifficult = maximumEnemyDifficult;
                return;
            }
            _enemyDifficult++;
        }
        public void MinusEnemyDifficult()
        {
            if (_enemyDifficult <= 0)
            {
                _enemyDifficult = 0;
                return;
            }
            _enemyDifficult--;
        }
    }
}