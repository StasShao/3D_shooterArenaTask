using System.Collections.Generic;
using ProjectFiles.Scripts.Base;
using ProjectFiles.Scripts.Installers;
using ProjectFiles.Scripts.Services;
using ProjectFiles.Scripts.Settings.GameSettings;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectFiles.Scripts.GameCoreLibrary
{
    public class GameCoreLibrary
    {
        #region Input
        public class InputManager
        {
            private readonly IInputControllable _inputControllable;
            private readonly GameSettings _gameSettings;
            public InputManager(IInputControllable inputControllable,GameSettings gameSettings)
            {
                _inputControllable = inputControllable;
                _gameSettings = gameSettings;
            }
            public void OnPcInputTick()
            {
                _inputControllable.SetAttack(Input.GetKeyDown(_gameSettings.AttackKey));
                _inputControllable.SetJump(Input.GetKeyDown(_gameSettings.JumpKey));
                _inputControllable.SetMouseAxis(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));
                _inputControllable.SetMousePosition(Input.mousePosition);
                _inputControllable.SetDirection(Input.GetAxis("Vertical"),Input.GetAxis("Horizontal"));
                _inputControllable.SetGrab(Input.GetKeyDown(_gameSettings.GrabKey));
            }
        }
        #endregion
        
        #region Behaviours
        
        #region Stats
         public class GameStatsBehaviour
        {
            private IPlayer _player;
            private GameObject[] _aliveStatsElements, _deathStatsElement;

            public GameStatsBehaviour(IPlayer player,GameObject[] aliveStatsElements,GameObject[] deathStatsElements)
            {
                _player = player;
                _aliveStatsElements = aliveStatsElements;
                _deathStatsElement = deathStatsElements;
            }

            public void Tick()
            {
                if (_player.IsAlive)
                {
                    foreach (var aliveElement in _aliveStatsElements)
                    {
                        aliveElement.SetActive(true);
                    }
                    foreach (var deathElement in _deathStatsElement)
                    {
                        deathElement.SetActive(false);
                    }
                }
                else
                {
                    foreach (var aliveElement in _aliveStatsElements)
                    {
                        aliveElement.SetActive(false);
                    }
                    foreach (var deathElement in _deathStatsElement)
                    {
                        deathElement.SetActive(true);
                    }
                }
            }
        }
         #endregion
         
        #region Character
        
        public class CharacterBehaviourManager
        {
            private readonly List<ICharacter> _characters = new();

            public CharacterBehaviourManager(List<CharacterBase> characters)
            {
                for (int i = 0; i < characters.Count; i++)
                {
                    _characters.Add(characters[i]);
                }
            }
            public void OnCharactersTick()
            {
                foreach (var character in _characters)
                {
                    character.Tickable();
                }
            }
            public void OnCharactersFixedTick()
            {
                foreach (var character in _characters)
                {
                    character.FixedTickable();
                }
            }
        }
        public class PlayerBehaviourManger
        {
            private readonly ICharacter _character;

            public PlayerBehaviourManger(CharacterBase playerCharacter)
            {
                _character = playerCharacter;
            }

            public void OnPlayerTick()
            {
                _character.Tickable();
            }
            public void OnPlayerFixedTick()
            {
                _character.FixedTickable();
            }
        }
        public class AiCharacterBehaviourManager
        {
            private readonly CharacterBase _character;
            private CharacterState _characterState;
            private IColliderCacheData<ICharacterDamageable> _characterCacheData;
            private IColliderCacheData<IWeapon> _weaponCachData;
            private bool _canSetNextWayOnPatrolState;
            private enum CharacterState
            {
                searchingWeapon,
                followToTarget,
                patrol
            }
            public AiCharacterBehaviourManager(CharacterBase character,IColliderCacheData<ICharacterDamageable> characterCacheData,IColliderCacheData<IWeapon> weaponCachData)
            {
                _character = character;
                _characterCacheData = characterCacheData;
                _weaponCachData = weaponCachData;
            }
            private void OnSearchingWeapon()
            {
                if(_characterState != CharacterState.searchingWeapon)return;
                var aiCharacter = (IAiEnemy)_character;
                if (aiCharacter != null)
                {
                    var others = Physics.OverlapSphere(aiCharacter.CharacterTransform.position,
                        aiCharacter.CharacterSettings.SearchRange);
                    IWeapon nearestWeapon = null;
                    var minNearestWeaponDistance = 999.9f;
                    for (int i = 0; i < others.Length; i++)
                    {
                        var other = others[i];
                        var cachedColliders = _weaponCachData.CachedColliders;
                        var cachedElements = _weaponCachData.CachedElements;
                        if (ColliderCache.TryGetElementCache<IWeapon>(ref cachedElements,
                                ref cachedColliders, other, out IWeapon weapon)&&weapon.WeaponOwner == null)
                        {
                            var distance = Vector3.Distance(aiCharacter.CharacterTransform.position,
                                weapon.WeaponTransform.position);
                            if (distance < minNearestWeaponDistance)
                            {
                                minNearestWeaponDistance = distance;
                                nearestWeapon = weapon;
                            }
                        }
                    }
                    if (nearestWeapon != null)
                    {
                        aiCharacter.SetNextWayPoint(nearestWeapon.WeaponTransform.position);
                        if (_characterState == CharacterState.searchingWeapon&&minNearestWeaponDistance <= 1.0f&&aiCharacter.CharacterWeapon == null)
                        {
                            aiCharacter.TakeWeapon(nearestWeapon);
                            _characterState = CharacterState.followToTarget;
                        }
                    }
                }
            }
            private bool OnStateCheck()
            {
                var aiCharacter = (IAiEnemy)_character;
                if (aiCharacter != null)
                {
                    if (aiCharacter.CharacterWeapon == null)
                    {
                        _characterState = CharacterState.searchingWeapon;
                        OnSearchingWeapon();
                    }
                    bool isFollow = false;
                    var others = Physics.OverlapSphere(_character.CharacterTransform.position,
                        aiCharacter.CharacterSettings.SearchRange);
                    for (int i = 0; i < others.Length; i++)
                    {
                        var cachedColliders = _characterCacheData.CachedColliders;
                        var cachedElements = _characterCacheData.CachedElements;
                        var other = others[i];
                        if (ColliderCache.TryGetElementCache<ICharacterDamageable>(ref cachedElements, ref cachedColliders, other,
                                out ICharacterDamageable character)&&character.Damageable == ICharacterDamageable.DamageableType.Player)
                        {
                            if (aiCharacter.CharacterWeapon == null)
                            {
                                _characterState = CharacterState.searchingWeapon;
                                return true;
                            }
                            aiCharacter.SetNextWayPoint(character.Character.CharacterTransform.position);
                            isFollow = true;
                            _canSetNextWayOnPatrolState = true;
                            return true;
                        }
                    }
                    if (isFollow)
                    {
                        _canSetNextWayOnPatrolState = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            private bool TryStateBehaviourState()
            {
                var aiCharacter = (IAiEnemy)_character;
                if (_characterState == CharacterState.followToTarget)
                {
                    if (aiCharacter != null)
                    {
                        aiCharacter.Agent.SetDestination(aiCharacter.WayPoint);
                        var attackDistance =
                            Vector3.Distance(aiCharacter.CharacterTransform.position, aiCharacter.WayPoint);
                        if (aiCharacter.CharacterWeapon != null)
                        {
                            var typeWeaponRange = aiCharacter.CharacterWeapon as IRangeWeaponType;
                            var typeWeaponMelee = aiCharacter.CharacterWeapon as IMeleeWeapon;
                            if (typeWeaponMelee != null)
                            {
                                if (attackDistance <= aiCharacter.CharacterSettings.MeleeAttackDistance)
                                {
                                    if (aiCharacter.CharacterWeapon != null)
                                    {
                                        aiCharacter.CharacterWeapon.AttackWeapon(true);
                                    }
                                }
                                else
                                {
                                    if (aiCharacter.CharacterWeapon != null)
                                    {
                                        aiCharacter.CharacterWeapon.AttackWeapon(false);
                                    }
                                }
                                return true;
                            }
                            if (typeWeaponRange != null)
                            {
                                if (attackDistance <= aiCharacter.CharacterSettings.RangeAttackDistance)
                                {
                                    if (aiCharacter.CharacterWeapon != null)
                                    {
                                        aiCharacter.CharacterWeapon.AttackWeapon(true);
                                    }
                                }
                                else
                                {
                                    if (aiCharacter.CharacterWeapon != null)
                                    {
                                        aiCharacter.CharacterWeapon.AttackWeapon(false);
                                    }
                                }
                                return true;
                            }
                        }
                    }
                }
                if (_characterState == CharacterState.patrol)
                {
                    if (aiCharacter != null)
                    {
                        aiCharacter.Agent.SetDestination(aiCharacter.WayPoint);
                        if (aiCharacter.CharacterWeapon == null)
                        {
                            _characterState = CharacterState.searchingWeapon;
                            return true;
                        }
                        if (_canSetNextWayOnPatrolState)
                        {
                            aiCharacter.SetNextWayPoint(new Vector3(Random.Range(-10.0f,10.0f),0,Random.Range(-10.0f,10.0f)));
                            _canSetNextWayOnPatrolState = false;
                        }
                        aiCharacter.Agent.SetDestination(aiCharacter.WayPoint);
                        var distanceToChangeWayPoint = Vector3.Distance(aiCharacter.CharacterTransform.position,
                            aiCharacter.WayPoint);
                        if (distanceToChangeWayPoint <= 2)
                        {
                            aiCharacter.SetNextWayPoint(new Vector3(
                                Random.Range(-aiCharacter.CharacterSettings.MoveZoneClamp.x,aiCharacter.CharacterSettings.MoveZoneClamp.x),
                                0,
                                Random.Range(-aiCharacter.CharacterSettings.MoveZoneClamp.z,aiCharacter.CharacterSettings.MoveZoneClamp.z)));
                        }
                        return true;
                    }
                }
                return false;
            }

            public void OnAiBehaviourTick()
            {
                if (OnStateCheck())
                {
                    _characterState = CharacterState.followToTarget;
                }
                else
                {
                    _characterState = CharacterState.patrol;
                }
                TryStateBehaviourState();
            }
        }
        #endregion
        #endregion
        
        #region Interact
        
        #region Grab
        public class WeaponGrabber
        {
            private readonly ICharacter _character;
            private readonly IColliderCacheData<IWeapon> _weaponCacheData;
            private readonly IInputControllable _inputControllable;
            public WeaponGrabber(ICharacter character,IColliderCacheData<IWeapon> weaponCacheData,IInputControllable inputControllable)
            {
                _character = character;
                _weaponCacheData = weaponCacheData;
                _inputControllable = inputControllable;
            }

            public void WeaponGrabInRange(float range)
            {
                if (_inputControllable.Grab&&TryGrabWeapon(range, out IWeapon weapon))
                {
                    _character.TakeWeapon(weapon);
                    if (_character.CharacterWeapon != null)
                    {
                        
                    }
                }
            }
            private bool TryGrabWeapon(float searchRange,out IWeapon weapon)
            {
                var others = Physics.OverlapSphere(_character.CharacterTransform.position, searchRange);
                for (int i = 0; i < others.Length; i++)
                {
                    var other = others[i];
                    var cachedColliders = _weaponCacheData.CachedColliders;
                    var cachedElements = _weaponCacheData.CachedElements;
                    if (ColliderCache.TryGetElementCache<IWeapon>(ref cachedElements,
                            ref cachedColliders, other, out weapon))
                    {
                        return true;
                    }
                }
                weapon = default;
                return false;
            }
        }
        #endregion
        #endregion
        
        #region Spawners
        
        #region Weapon
        public class WeaponSpawner
        {
            private WeaponSpawnerPoolInstaller _weaponSpawnerPoolInstaller;
            private Transform[] _positionSpawnTransforms;
            private int _maxCount;
            private int _spawnCount;
            private int _spawns;
            private List<Transform> _usedSpawnTransforms = new();
            private List<WeaponBase> _usedWeapons = new();
            public WeaponSpawner(WeaponSpawnerPoolInstaller weaponPoolInstaller,Transform[] spawnPositionTransforms,int spawnCount)
            {
                _weaponSpawnerPoolInstaller = weaponPoolInstaller;
                _positionSpawnTransforms = spawnPositionTransforms;
                _maxCount = _weaponSpawnerPoolInstaller.Weapons.Count;
                _spawnCount = spawnCount;
                if (_spawnCount > _maxCount)
                {
                    _spawnCount = _maxCount;
                }
                if (_spawnCount < 0)
                {
                    _spawnCount = 0;
                }

                SpawnWeaponRandomPositionsAtAwake();
            }
            private void SpawnWeaponRandomPositionsAtAwake()
            {
                if(_spawns == _maxCount)return;
                for (int i = 0; i < _spawnCount; i++)
                {
                    var randomPositionTransform =
                        _positionSpawnTransforms[Random.Range(0, _positionSpawnTransforms.Length)];
                    if (_usedSpawnTransforms.Contains(randomPositionTransform))
                    {
                        continue;
                    }
                    var weapon = _weaponSpawnerPoolInstaller.TryGetWeapon();
                    if (weapon != null)
                    {
                        _usedWeapons.Add(weapon);
                        _usedSpawnTransforms.Add(randomPositionTransform);
                        weapon.WeaponTransform.parent = randomPositionTransform;
                        weapon.WeaponTransform.localPosition = Vector3.zero;
                        weapon.WeaponTransform.rotation = randomPositionTransform.rotation;
                        weapon.gameObject.SetActive(true);
                        _spawns++;
                    }
                    if (_spawnCount > _usedSpawnTransforms.Count)
                    {
                        SpawnWeaponRandomPositionsAtAwake();
                    }
                }
            }

            public bool TrySpawnedWeaponResetPosition(out WeaponBase weaponToReset)
            {
                bool canReset = false;
                WeaponBase checkedWeapon = null;
                for (int i = 0; i < _usedWeapons.Count; i++)
                {
                    var weapon = _usedWeapons[i];
                    var spawnTransform = _usedSpawnTransforms[i];
                    if (weapon.gameObject.activeInHierarchy&&weapon.WeaponTransform.position != spawnTransform.position&&weapon.WeaponOwner == null)
                    {
                        checkedWeapon = weapon;
                        canReset = true;
                    }
                }
                if (canReset)
                {
                    weaponToReset = checkedWeapon;
                    return true;
                }
                else
                {
                    weaponToReset = checkedWeapon;
                    return false;
                }
            }
            public void WeaponSpawn(WeaponBase weapon)
            {
                for (int i = 0; i < _usedWeapons.Count; i++)
                {
                    var checkWeapon = _usedWeapons[i];
                    if (weapon == checkWeapon)
                    {
                        weapon.gameObject.SetActive(false);
                        var transformSpawn = _usedSpawnTransforms[i];
                        weapon.WeaponTransform.parent = transformSpawn;
                        weapon.SetKinematic(true);
                        weapon.WeaponTransform.localPosition = Vector3.zero;
                        weapon.WeaponTransform.rotation = transformSpawn.rotation;
                        weapon.gameObject.SetActive(true);
                    }
                }
            }
        }
        #endregion
        
        #region Enemy
        public class EnemySpawner
        {
            private EnemySpawnerData _enemySpawnerData;
            private readonly AiEnemySpawnerPoolInstaller _enemySpawnerPool;
            private IPlayer _player;
            private Text _waveCurrent,_currentAliveEnemies;
            public EnemySpawner(AiEnemySpawnerPoolInstaller enemySpawnerPool,IPlayer player,Text waveCurrent,Text currentAliveEnemies)
            {
                _enemySpawnerData = new EnemySpawnerData();
                _waveCurrent = waveCurrent;
                _enemySpawnerPool = enemySpawnerPool;
                _currentAliveEnemies = currentAliveEnemies;
                _player = player;
                var enemyWaves = _enemySpawnerData.EnemyWaves;
                _enemySpawnerData.EnemyWaves = enemyWaves;
            }
            public EnemySpawnerData GetData()
            {
                return _enemySpawnerData;
            }
            public void SpawnerTick()
            {
                var enemySpawns = _enemySpawnerData.EnemySpawns;
                var enemyWaves = _enemySpawnerData.EnemyWaves;
                var canEnemySpawn = _enemySpawnerData.CanEnemySpawn;
                var maxSpawns = _enemySpawnerPool.MaximumCharacters;
                bool isMaxSpawns = false;
                int aliveEnemiesCount = 0;
                if (enemySpawns == enemyWaves)
                {
                    enemySpawns = 0;
                    canEnemySpawn = false;
                }
                if (enemyWaves >= maxSpawns)
                {
                    enemyWaves = maxSpawns - 1;
                    isMaxSpawns = true;
                }
                bool isAllEnemyDead = true;
                for (int i = 0; i < _enemySpawnerPool.Characters.Count; i++)
                {
                    var enemy = _enemySpawnerPool.Characters[i];
                    if (enemy.gameObject.activeInHierarchy)
                    {
                        if (enemy.CharacterHealth > 0)
                        {
                            aliveEnemiesCount++;
                        }
                        isAllEnemyDead = false;
                    }
                }
                _currentAliveEnemies.text = aliveEnemiesCount.ToString();
                if (!canEnemySpawn&&isAllEnemyDead)
                {
                    if (!isMaxSpawns)
                    {
                        enemyWaves++;
                        _waveCurrent.text = enemyWaves.ToString();
                        _player.AddUpgrade();
                    }
                    canEnemySpawn = true;
                }
                _enemySpawnerData.EnemySpawns = enemySpawns;
                _enemySpawnerData.EnemyWaves = enemyWaves;
                _enemySpawnerData.CanEnemySpawn = canEnemySpawn;
            }
            private float HealthModify(CharacterBase character)
            {
                var h = character.ModifiedHealth * 0.5f;
                return h;
            }
            public void SpawnWave()
            {
                var enemyWaves = _enemySpawnerData.EnemyWaves;
                var enemySpawns = _enemySpawnerData.EnemySpawns;
                var maxSpawnsEnemies = _enemySpawnerPool.Characters.Count - 2;
                if (enemySpawns >= maxSpawnsEnemies)
                {
                    enemyWaves = maxSpawnsEnemies;
                    _enemySpawnerData.EnemyWaves = enemyWaves;
                }
                for (int i = 0; i < enemyWaves; i++)
                {
                    var enemy = _enemySpawnerPool.TryGetCharacter();
                    if (enemy != null)
                    {
                        enemy.AddHealth(HealthModify(enemy));
                        enemy.transform.position = Vector3.zero;
                        enemy.gameObject.SetActive(true);
                        enemySpawns++;
                    }
                }
                _enemySpawnerData.EnemySpawns = enemySpawns;
            }

            public struct EnemySpawnerData
            {
                public int EnemySpawns;
                public int EnemyWaves;
                public bool CanEnemySpawn;
            }
        }
        #endregion
        
        #endregion
        
        #region Camera
        
        #region FreeCamera
        public class FreeCameraLook
        {
            private Transform _lookTransform,_followTransform;
            private GameSettings _gameSettings;
            private Camera _camera;

            public FreeCameraLook(GameSettings gameSettings,Transform lookTransform,Transform followTransform)
            {
                _camera = Camera.main;
                _gameSettings = gameSettings;
                _lookTransform = lookTransform;
                _followTransform = followTransform;
            }

            
            public void FixedTick()
            {
                _camera.transform.position =
                    Vector3.Lerp(_camera.transform.position, _followTransform.position,
                        _gameSettings.CameraMoveSpeed * Time.deltaTime) + _gameSettings.CameraOffset;
                _camera.transform.LookAt(_lookTransform);
            }
        }
        #endregion
        
        #endregion
        
        #region Cache
        
        #region Colluder cache
        public static class ColliderCache
        {
            public static bool TryGetElementCache<T>(ref List<T> cachedElements,ref List<Collider> cachedColliders,Collider other,out T element)
            {
                for (int i = 0; i < cachedColliders.Count; i++)
                {
                    var cachedCollider = cachedColliders[i];
                    var cachedElement = cachedElements[i];
                    if (cachedCollider == other)
                    {
                        element = cachedElement;
                        return true;
                    }
                }
                if (!cachedColliders.Contains(other) && other.TryGetComponent<T>(out element))
                {
                    cachedColliders.Add(other);
                    cachedElements.Add(element);
                    return true;
                }
                element = default;
                return false;
            }
        }
        #endregion
        #endregion

        #region Motions

        #region PlayerMotion
        public class PlayerCharacterMotionController
        {
            private readonly IPlayer _player;
            private readonly IInputControllable _inputControllable;
            public PlayerCharacterMotionController(IPlayer player,IInputControllable inputControllable)
            {
                _player = player;
                _inputControllable = inputControllable;
            }

            public void OnTick()
            {
                var controller = _player.PlayerCharacterController;
                if (_inputControllable.Jump&&controller.isGrounded)
                {
                    controller.Move(Vector3.up * _player.PlayerCharacterSettings.JumpForce);
                }
            }
            public void OnMotion()
            {
                var controller = _player.PlayerCharacterController;
                var animator = _player.PlayerAnimator;
                controller.Move(controller.transform.forward * _inputControllable.ForwardDirection *
                                _player.PlayerCharacterSettings.MoveSpeed);
                controller.Move(controller.transform.right * _inputControllable.SideDirection *
                                _player.PlayerCharacterSettings.MoveSpeed);
                controller.transform.Rotate(0,_inputControllable.MouseAxis.x * _player.PlayerCharacterSettings.LookSpeed,0);
                if (!controller.isGrounded)
                {
                    controller.Move(Vector3.down * _player.PlayerCharacterSettings.GravityForce * Time.deltaTime);
                }
                animator.SetFloat(_player.PlayerCharacterSettings.ForwardMovementAnimatorName,controller.velocity.magnitude);
            }
        }
        #endregion
        
        #endregion
        
    }
}