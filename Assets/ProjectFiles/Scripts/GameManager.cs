using System.Collections;
using ProjectFiles.Scripts.Base;
using ProjectFiles.Scripts.Installers;
using ProjectFiles.Scripts.Services;
using ProjectFiles.Scripts.Settings.GameSettings;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ProjectFiles.Scripts
{
    public class GameManager:MonoBehaviour
    {
        private GameCoreLibrary.GameCoreLibrary.CharacterBehaviourManager _characterBehaviourManager;
        private GameCoreLibrary.GameCoreLibrary.InputManager _inputManager;
        private GameCoreLibrary.GameCoreLibrary.WeaponSpawner _weaponSpawner;
        private GameCoreLibrary.GameCoreLibrary.EnemySpawner _enemySpawner;
        private GameCoreLibrary.GameCoreLibrary.FreeCameraLook _freeCameraLook;
        private GameCoreLibrary.GameCoreLibrary.GameStatsBehaviour _gameStatsBehaviour;
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private Transform[] randomTransformSpawns;
        [SerializeField] private CharacterBase player;
        [SerializeField] private Text waveCurrent,currentAliveEnemies;
        [SerializeField] private GameObject[] aliveStatElements, deathStatElements;
        [Inject] private AiEnemySpawnerPoolInstaller _enemySpawnerPool;
        [Inject] private WeaponSpawnerPoolInstaller _weaponPool;
        [Inject] private IInputControllable _inputControllable;
        private bool _canWeaponReset;
        private bool _canEnemySpawn;
        private void Awake()
        {
            _inputManager = new GameCoreLibrary.GameCoreLibrary.InputManager(_inputControllable,gameSettings);
            _characterBehaviourManager =
                new GameCoreLibrary.GameCoreLibrary.CharacterBehaviourManager(_enemySpawnerPool.Characters);
            _weaponSpawner = new GameCoreLibrary.GameCoreLibrary.WeaponSpawner(_weaponPool, randomTransformSpawns, 30);
            player.Init();
            var pl = (IPlayer)player;
            _enemySpawner = new GameCoreLibrary.GameCoreLibrary.EnemySpawner(_enemySpawnerPool,pl,waveCurrent,currentAliveEnemies);
            _freeCameraLook = new GameCoreLibrary.GameCoreLibrary.FreeCameraLook(gameSettings, pl.CameraLookTransform,pl.CameraMoveTransform);
            _gameStatsBehaviour =
                new GameCoreLibrary.GameCoreLibrary.GameStatsBehaviour(pl,aliveStatElements, deathStatElements);
            StartCoroutine(StartWeaponReset());
            StartCoroutine(EnemySpawnStart());
        }
        private void Update()
        {
            var data = _enemySpawner.GetData();
            _inputManager.OnPcInputTick();
            _characterBehaviourManager.OnCharactersTick();
            _canWeaponReset = _weaponSpawner.TrySpawnedWeaponResetPosition(out WeaponBase weapon);
            player.Tickable();
            _enemySpawner.SpawnerTick();
            _canEnemySpawn = data.CanEnemySpawn;
            _gameStatsBehaviour.Tick();
        }
        private void FixedUpdate()
        {
            _characterBehaviourManager.OnCharactersFixedTick();
            player.FixedTickable();
            _freeCameraLook.FixedTick();
        }
        private IEnumerator ResetWeaponDelay()
        {
            yield return new WaitForSeconds(5);
            StartCoroutine(StartWeaponReset());
        }
        private IEnumerator StartWeaponReset()
        {
            yield return new WaitUntil(() => _canWeaponReset);
            if (_weaponSpawner.TrySpawnedWeaponResetPosition(out WeaponBase weapon))
            {
                _weaponSpawner.WeaponSpawn(weapon);
            }
            StartCoroutine(ResetWeaponDelay());
        }
        private IEnumerator EnemySpawnTimer()
        {
            yield return new WaitForSeconds(gameSettings.SpawnTimeDelay *0.1f);
            StartCoroutine(EnemySpawnStart());
        }
        private IEnumerator EnemySpawnStart()
        {
            yield return new WaitUntil(() => _canEnemySpawn);
            yield return new WaitForSeconds(gameSettings.SpawnTimeDelay);
            _enemySpawner.SpawnWave();
            StartCoroutine(EnemySpawnTimer());
        }
    }
}