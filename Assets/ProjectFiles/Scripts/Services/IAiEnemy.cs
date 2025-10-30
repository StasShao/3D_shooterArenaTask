using ProjectFiles.Scripts.Settings.AiEnemySettings;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

namespace ProjectFiles.Scripts.Services
{
    public interface IAiEnemy:ICharacter
    {
        Rig[] IkRigs { get; }
        float ModifiedSpeed { get; }
        NavMeshAgent Agent { get; }
        Animator CharacterAnimator { get; }
        Vector3 WayPoint { get; }
        AiEnemySettings CharacterSettings { get; }
        void FollowToPosition(Vector3 position);
        void Attack();
        void SetNextWayPoint(Vector3 nextWayPoint);
    }
}