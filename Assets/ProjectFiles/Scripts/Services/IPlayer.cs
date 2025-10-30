using ProjectFiles.Scripts.Settings.PlayerSettings;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ProjectFiles.Scripts.Services
{
    public interface IPlayer:ICharacter
    {
        Rig[] IkRigs { get; }
        float DamageUpgradeValue { get; }
        Transform CameraLookTransform { get; }
        Transform CameraMoveTransform { get; }
        Animator PlayerAnimator { get; }
        CharacterController PlayerCharacterController { get; }
        PlayerSettings PlayerCharacterSettings { get; }
        void AddUpgrade();
        void Attack();
    }
}