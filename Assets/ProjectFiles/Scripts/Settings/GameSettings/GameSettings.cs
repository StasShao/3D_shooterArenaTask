using UnityEngine;

namespace ProjectFiles.Scripts.Settings.GameSettings
{
    [CreateAssetMenu(menuName = "Game settings")]
    public class GameSettings:ScriptableObject
    {
        [SerializeField] private KeyCode jumpKey,attackKey,selectView,grabKey;
        [SerializeField] private float cameraMoveSpeed, spawnTimeDelay;
        [SerializeField] private Vector3 cameraOffset;
        public KeyCode JumpKey { get { return jumpKey; } }
        public KeyCode AttackKey { get { return attackKey; } }
        public KeyCode SelectView { get { return selectView; } }
        public KeyCode GrabKey { get { return grabKey; } }
        public float CameraMoveSpeed { get { return cameraMoveSpeed; } }
        public float SpawnTimeDelay { get { return spawnTimeDelay; } }
        public Vector3 CameraOffset { get { return cameraOffset; } }
    }
}