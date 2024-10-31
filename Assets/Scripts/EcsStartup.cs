using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using UnityEngine;
using LevelController;
using static Enums;

namespace Client
{
    sealed class EcsStartup : MonoBehaviour
    {
        private EcsSystems _globalInitSystems, _globalRunSystems,
            _initLobbySystems, _initStartSceneSystems, _initPlaySystems,
            _runUpdateSystems, _playUpdateSystems,
            _pauseSystems, _runFixedSystems,
            _lobbySystems, _startSceneSystems, _npcSystems, _tutorialSystems;

        private EcsWorld _world;
        private GameState _state;

        [Header("Scene Type")]
        [SerializeField] private SceneType _sceneType;
        [SerializeField] private bool _getCarFromSceneScene;
        [SerializeField] private bool _showNpcCanvases;
        [SerializeField] private bool _showFpsCanvas;
        [SerializeField] private bool _enableDeath;
        [SerializeField] private bool _enableLowFuel;
        [SerializeField] private bool _tutorialScene;

        [Header("Configs")]
        [SerializeField] private SettingsConfig _settingsConfig;
        [SerializeField] private UIConfig _uiConfig;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private UIPanelConfig _uiPanelConfig;
        [SerializeField] private PlayerCarsConfig _playerCarsConfig;
        [SerializeField] private EnemyPrefabsConfig _enemyConfig;
        [SerializeField] private QuestsConfig _questsConfig;
        [SerializeField] private ParticlesConfig _particlesConfig;
        [SerializeField] private PickupsConfig _pickupsConfig;
        [SerializeField] private MissionsConfig _missionsConfig;
        [SerializeField] private SoundFMODConfig _soundFMODConfig;
        [SerializeField] private MinimapDataSo _minimapData;

        [SerializeField] private SceneIniterMb _sceneIniter;
        [SerializeField] private SessionContextHolder _sessionContextHolderPrefab;
        [SerializeField] private LocalizationConfig _localizationConfig;

        void Start()
        {
            _world = new EcsWorld();

            _globalInitSystems = new EcsSystems(_world);
            _globalRunSystems = new EcsSystems(_world);

            _initLobbySystems = new EcsSystems(_world);
            _lobbySystems = new EcsSystems(_world);

            _initStartSceneSystems = new EcsSystems(_world);
            _startSceneSystems = new EcsSystems(_world);

            _initPlaySystems = new EcsSystems(_world);

            _runUpdateSystems = new EcsSystems(_world);
            _playUpdateSystems = new EcsSystems(_world);
            _runFixedSystems = new EcsSystems(_world);
            _pauseSystems = new EcsSystems(_world);

            _npcSystems = new EcsSystems(_world);
            _tutorialSystems = new EcsSystems(_world);

            _state = new GameState(_world, _sceneType, _settingsConfig, _uiConfig,
                _playerConfig, _uiPanelConfig, _playerCarsConfig,
                _enemyConfig, _questsConfig, _particlesConfig, _pickupsConfig,
                _missionsConfig, _soundFMODConfig, _enableLowFuel, _enableDeath, _minimapData, _localizationConfig);

            SetupGlobalInitSystems();
            SetupGlobalSystems();

            if (_sceneType == SceneType.StartScene)
            {
                SetupInitStartSceneSystems();
                SetupStartSceneSystems();
            }
            else if (_sceneType == SceneType.LobbyScene)
            {
                SetupLobbyInitSystems();
                SetupLobbySystems();
            }
            else if (_sceneType == SceneType.PlayScene)
            {
                SetupPlayInitSystems();
                SetupRunUpdateSystems();
                SetupRunFixedSystems();
                SetupPlayUpdateSystems();
                SetupNpcSystems();
            }

            if (_tutorialScene)
                SetupTutorialSystems();

#if UNITY_EDITOR
            _globalInitSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
#endif

            InjectSystems();
            InitSystems();
        }

        private void Update()
        {
            _globalInitSystems?.Run();
            _globalRunSystems?.Run();

            if (_sceneType == SceneType.PlayScene)
            {
                _initPlaySystems?.Run();
                _npcSystems?.Run();
                _runUpdateSystems?.Run();
                if (_state.PlaySystems) _playUpdateSystems?.Run();
                if (_state.PauseSystems) _pauseSystems?.Run();
            }
            else if (_sceneType == SceneType.StartScene)
            {
                _initStartSceneSystems?.Run();
                _startSceneSystems?.Run();
            }
            else if (_sceneType == SceneType.LobbyScene)
            {
                _initLobbySystems?.Run();
                _lobbySystems?.Run();
            }

            if (_tutorialScene)
                _tutorialSystems?.Run();
        }

        private void FixedUpdate()
        {
            _runFixedSystems?.Run();
        }

        #region GlobalSystems
        private void SetupGlobalInitSystems()
        {
            _globalInitSystems

                .Add(new PlatformInit())
                .Add(new LevelControllerInit())

                .Add(new InitInterface(_showFpsCanvas))
                .Add(new InputInit())
                .Add(new InitSounds())
                .Add(new InitMusic())

                .Add(new InitSessionContext(_sessionContextHolderPrefab))

                .Add(new TestersInit())

                .Add(new LocalizationInit())

                //.Add(new TutorialInit(_sceneType))
                .Add(new UnfadeCanvasInit())
            ;
        }

        private void SetupGlobalSystems()
        {
            _globalRunSystems

                .Add(new EnablePauseSystems())
                .Add(new EnablePlaySystems())
                
                .Add(new InputFromInputActionSystem())
                .Add(new TouchInputSystem())

                .Add(new StartGameButtonSystem())
                .Add(new EscButtonSystem())
                .Add(new LoadSceneSystem())

                .Add(new SimpleMoveTransformSystem())

                .Add(new TestLogStartSystem())
                //.Add(new TestLogCatchSystem())

                .Add(new MoneySystem())
                .Add(new UiMoneySystem())

                .Add(new CarCollisionSoundSystem())
                .Add(new SoundSystem())
                .Add(new VolumeChangeSystem())              //in work

                .Add(new TimeScaleSystem())

                .Add(new LocalizationChangeSystem())

                //PROMO IN GAME
                .Add(new StartPlayPromoVideoSystem())               //in work
                .Add(new PlayPromoVideoTestSystem())                //in work
                .Add(new OnStopPlayPromoVideoSystem())              //in work


                .Add(new TutorialSystem())
                .Add(new QuitSystem())
            ;

            _globalRunSystems

                .DelHere<EscButtonEvent>()
                .DelHere<TestLogEvent>()
                .DelHere<ChangeGlobalStateEvent>()
                ;
        }
        #endregion

        #region PlaySystems
        private void SetupPlayInitSystems()
        {
            _initPlaySystems
                .Add(new InitPlaySceneInterface())

                .Add(new CameraInit(_sceneIniter))
                .Add(new LayerMaskInit())
                .Add(new PatrollPointsInit(_sceneIniter))

                .Add(new PlayerSpawnPointsInit(_getCarFromSceneScene, _sceneIniter))     //для тестов просто выключать, и будет искать машину на сцене

                .Add(new NpcTestInit())

                .Add(new ObjectPoolInit())
                .Add(new PickupsPointsInit(_sceneIniter))
                .Add(new PointsOfInterestInit(_sceneIniter))

                .Add(new SpawnersInit(_sceneIniter))
                //.Add(new QuestsInit())

                .Add(new MissionInit())                  
                .Add(new RespawnPointsInit(_sceneIniter))
                .Add(new MissionSpawnPointsInit(_sceneIniter))
                .Add(new ZonesSeparatorsInit(_sceneIniter))
                
                .Add(new PlayerCarInit())
            ;
        }

        private void SetupPlayUpdateSystems()
        {
            _playUpdateSystems

                //INPUT
                .Add(new JoystickOnOffSystem())
                .Add(new PauseButtonSystem())
                .Add(new PlayerCarInputSystem())

                //SPAWNERS
                .Add(new NearestSpawnPointsSystem())
                .Add(new SpawnerSpawnFullPointsNpcSystem())
                .Add(new DeactivateNpcSpawnerSystem())
                .Add(new SpawnerSpawnCoolDownSystem())
                .Add(new SpawnNpcSystem())

                .Add(new PickupsSpawnSystem())

                //CAR
                .Add(new CarPickupsSystem())
                .Add(new ZoneChangeSystem())

                .Add(new RestoreCarHpSystem())
                .Add(new RestoreCarFuelSystem())

                .Add(new CarDamageSystem())
                .Add(new CarDeathSystem())                          //in work

                .Add(new CarBrakeLightsSystem())
                .Add(new CarBackDriveLightsSystem())

                .Add(new CarMotorSoundSystem())
                .Add(new CarWheelsSoundSystem())
                .Add(new CarFuelSystem())                           //in work
                .Add(new CarOutOfFuelSystem())                      //in work

                //UI
                //.Add(new TimerOnQuestUISystem())

                //QUESTS
                //.Add(new KillQuestChapterSystem())

                //MISSIONS
                .Add(new KillMissionSystem())
                .Add(new RideByCheckpointsMissionSystem())
                .Add(new RideByCollectablesMissionSystem())
                .Add(new TimerOnMissionSystem())
                .Add(new MissionFailedSystem())
                .Add(new CheckMissionCompleteSystem())                      //in work need test
                .Add(new MissionCompleteSystem())                           //in work need test
                .Add(new MissionPauseOnCompleteSystem())                    //in work need test

                .Add(new SpawnMissionCheckpointSystem())
                .Add(new DestroyCheckpointSystem())

                .Add(new SpawnMissionCollectablesSystem())
                .Add(new DestroyMissionCollectablesSystem())

                .Add(new MissionUISystem())

                //CAMERA
                .Add(new CameraChangePositionSystem())
                .Add(new CameraRotationAroundCarSystem())

                .Add(new LoseSystem())                              //in work

            ;

            _playUpdateSystems
                .DelHere<CarStartBrakeEvent>()
                .DelHere<CarStopBrakeEvent>()
                .DelHere<StartBackDriveEvent>()
                .DelHere<StopBackDriveEvent>()
                .DelHere<StartSkidmarksEvent>()
                .DelHere<EndSkidmarksEvent>()
                .DelHere<KillZombieEvent>()

            ;
        }
        #endregion

        #region RunSystems
        private void SetupRunUpdateSystems()
        {
            _runUpdateSystems

                .Add(new ContinueButtonSystem())

                .Add(new UiSpeedSystem())
                .Add(new MinimapPlayerPositioningSystem())

                .Add(new CarSpawnSystem())
                .Add(new CarInitSystem())
                .Add(new PlayerCarInitSystem())
                
                .Add(new PlayerCarVisualHpSystem())
                
                //TODO подумать может в другое место воткнуть
                .Add(new ShowBrifSystem())

                //QUESTS
                //.Add(new RefreshQuestsSystem())                 //in work
                //.Add(new StartQuestSystem())                    //in work
                //.Add(new QuestCheckpointSystem())               //in work
                //.Add(new FinishQuestSystem())                   //in work

                //.Add(new StartChapterForTimeSystem())           //in work
                //.Add(new QuestChapterForTimeSystem())           //in work
                //.Add(new StopChapterForTimeSystem())            //in work

                //.Add(new FailedQuestSystem())                   //in work

                //.Add(new StartKillQuestChapterSystem())
                //.Add(new StopKillQuestChapterSystem())

                //.Add(new TimeScaleSystem())

                ;

            _runUpdateSystems

                .DelHere<CreditsEvent>()
            ;
        }
        #endregion

        #region FixedSystems
        private void SetupRunFixedSystems()
        {
            _runFixedSystems

                .Add(new CarFlipCoolDownSystem())

                .Add(new CarStatisticSystem())
                .Add(new CarStateSystem())
                .Add(new CarMoveDirectionSystem())

                .Add(new CarColliderPositionFromSpeedSystem())

                .Add(new CarTriggerSystem())
                .Add(new CarTriggerExitSystem())
                .Add(new CarCollisionSystem())

                .Add(new PhysicalObjectStartFlySystem())
                .Add(new PhysicalObjectSpeedSystem())
                .Add(new PhysicalObjectColliderPosBySpeedSystem())
                .Add(new PhysicalObjectsTriggerSystem())
                .Add(new PhysicalObjectCollisionSystem())
                .Add(new PhysicalObjectsCheckFlySystem())

                .Add(new HitNpcByCarSystem())
                .Add(new HitNpcByPhysicalObjectSystem())

                .Add(new CarControllInFlySystem())
                .Add(new PlayerCarFlyTimer())                           //in work
                .Add(new CarDriftSystem())                              //in work

                .Add(new CarRadiusOfTurnLimitSystem())
                .Add(new CarSmoothSteeringSystem())
                .Add(new CarSteeringSystem())

                .Add(new CarMoveSystem())
                .Add(new CarTorqueSystem())
                .Add(new CarWheelRpmLimiterSystem())

                .Add(new CarHandBrakeMonitoringSystem())
                .Add(new CarEngineBrakingSystem())
                .Add(new CarBrakeSystem())

                //.Add(new CarSavePositionSystem())

                .Add(new CarFlipSystem())                           //in work

                .Add(new CarWheelEffectsPositionSystem())
                .Add(new CarWheelSlipEffectsSystem())

                //.Add(new CarTahometerSystem())
                .Add(new NewCarTahometerSystem())

                //CAMERA
                .Add(new CameraChangeTargetSystem())
                .Add(new CameraStartRotationByInputSystem())
                .Add(new CameraListenBackwardMoving())
                .Add(new CameraFollowCarSystem())
                //.Add(new CameraRotationAroundCarSystem())           

                .Add(new NpcDamageSystem())
                .Add(new NpcShowRagdollSystem())

                .Add(new ThrowObjectsInitSystem())
                .Add(new ThrowObjectCollisionSystem())
                .Add(new ThrowObjectExplosionSystem())
                .Add(new ThrowObjectDeleteSystem())

                .Add(new ResetPhysicOnObjectsSystem())
            ;

            _runFixedSystems
                .DelHere<CarFlipEvent>()
                ;
        }
        #endregion

        #region LobbySystems
        private void SetupLobbyInitSystems()
        {
            _initStartSceneSystems
                .Add(new InitLobbySceneInterface())
                .Add(new ShopCarsInit())

            ;
        }

        private void SetupLobbySystems()
        {
            _lobbySystems
                .Add(new StartMoveShopCarsSystem())
                .Add(new MoveShopCarSystem())
                .Add(new MoveCarsCoolDownSystem())

                .Add(new CarRotationInputSystem())

                .Add(new CarShopViewSystem())

                .Add(new CarBuySystem())
                .Add(new CarChooseSystem())
                .Add(new CarUpgradeSystem())

            ;
        }
        #endregion

        #region StartSceneSystems
        private void SetupInitStartSceneSystems()
        {
            _initLobbySystems
                .Add(new InitStartSceneInterface())
            ;

            _initLobbySystems
                .DelHere<CreditsEvent>()
            ;
        }

        private void SetupStartSceneSystems()
        {

        }
        #endregion

        #region NpcSystems
        private void SetupNpcSystems()
        {
            _npcSystems

                .Add(new NpcInitSystem(_showNpcCanvases))

                //state systems
                .Add(new NpcSpawnStateSystem())
                .Add(new NpcIdleSystem())
                .Add(new NpcPatrollSystem())
                .Add(new NpcChasingSystem())
                .Add(new NpcAimSystem())
                .Add(new NpcMeleeAttackSystem())
                .Add(new NpcRangeAttackSystem())
                .Add(new NpcAttackCoolDownSystem())

                .Add(new NpcHurtByCarSystem())
                .Add(new NpcDeadSystem())
                .Add(new NpcInObjectPoolSystem())

                .Add(new NpcInitPatrollPointsSystem())
                .Add(new NpcLookAroundInPeaceSystem())
                .Add(new RestoreNpcHpSystem())
                .Add(new ResetPositionAndRotationOfBodySystem())
                .Add(new NpcRagdollInitSystem())
                .Add(new NpcWalkAnimationSystem())

                .Add(new RagdollCoolDownSystem())
                .Add(new HideRagdollSystem())

                .Add(new NpcDebugCanvasRotationSystem())

                .Add(new NpcRangeSetRunSpeedSystem())
                .Add(new NpcHandleUpdate())

                ;


            _npcSystems

                .DelHere<StartMeleeAttackMonitoringEvent>()         //начало мониторинга атаки
                .DelHere<StopMeleeAttackMonitoringEvent>()          //конец мониторинга атаки
                .DelHere<NpcCarHitAnimationStopEvent>()             //конец анимации урона
                .DelHere<NpcMeleeAttackAnimationStopEvent>()        //ивент с аниматора об окончании анимации

                .DelHere<NpcHasArrivedEvent>()                      //это под снос скорее всего
                .DelHere<NpcHitFromCarEvent>()                      //может останется

                ;
        }
        #endregion

        #region Tutorial
        private void SetupTutorialSystems()
        {
            _tutorialSystems
                .Add(new TutorialLevelInit())

                .Add(new TutorialTimerSystem())
                .Add(new ContinueTutorialSystem())          //in work
                .Add(new TutorialShowHowToRideSystem())
                .Add(new SkipTutorialSystem());
        }
        #endregion

        #region InjectInitDestroy
        private void InjectSystems()
        {
            _globalInitSystems.Inject(_state);
            _globalRunSystems.Inject(_state);
            _initLobbySystems.Inject(_state);
            _initStartSceneSystems.Inject(_state);
            _startSceneSystems.Inject(_state);
            _lobbySystems.Inject(_state);
            _initPlaySystems.Inject(_state);
            _runUpdateSystems.Inject(_state);
            _playUpdateSystems.Inject(_state);
            _pauseSystems.Inject(_state);
            _runFixedSystems.Inject(_state);
            _npcSystems.Inject(_state);
            _tutorialSystems.Inject(_state);
        }

        private void InitSystems()
        {
            _globalInitSystems.Init();
            _globalRunSystems.Init();
            _initLobbySystems.Init();
            _initStartSceneSystems.Init();
            _startSceneSystems.Init();
            _lobbySystems.Init();
            _initPlaySystems.Init();
            _runUpdateSystems.Init();
            _playUpdateSystems.Init();
            _pauseSystems.Init();
            _runFixedSystems.Init();
            _npcSystems.Init();
            _tutorialSystems.Init();
        }

        void OnDestroy()
        {
            _state.SaveMoneyScore();

            if (_globalInitSystems != null)
            {
                _globalInitSystems.Destroy();
                //_globalInitSystems.GetWorld().Destroy();
                _globalInitSystems = null;
            }

            if (_initLobbySystems != null)
            {
                _initLobbySystems.Destroy();
                //_initLobbySystems.GetWorld().Destroy();
                _initLobbySystems = null;
            }

            if (_initStartSceneSystems != null)
            {
                _initStartSceneSystems.Destroy();
                //_initStartSceneSystems.GetWorld().Destroy();
                _initStartSceneSystems = null;
            }

            if (_initPlaySystems != null)
            {
                _initPlaySystems.Destroy();
                //_initPlaySystems.GetWorld().Destroy();
                _initPlaySystems = null;
            }

            if (_globalRunSystems != null)
            {
                _globalRunSystems.Destroy();
                _globalRunSystems = null;
            }

            if (_startSceneSystems != null)
            {
                _startSceneSystems.Destroy();
                _startSceneSystems = null;
            }

            if (_lobbySystems != null)
            {
                _lobbySystems.Destroy();
                _lobbySystems = null;
            }

            if (_runUpdateSystems != null)
            {
                _runUpdateSystems.Destroy();
                _runUpdateSystems = null;
            }

            if (_playUpdateSystems != null)
            {
                _playUpdateSystems.Destroy();
                _playUpdateSystems = null;
            }

            if (_pauseSystems != null)
            {
                _pauseSystems.Destroy();
                _pauseSystems = null;
            }

            if (_runFixedSystems != null)
            {
                _runFixedSystems.Destroy();
                _runFixedSystems = null;
            }

            if (_npcSystems != null)
            {
                _npcSystems.Destroy();
                _npcSystems = null;
            }

            if (_tutorialSystems != null)
            {
                _tutorialSystems.Destroy();
                _tutorialSystems = null;
            }
        }
        #endregion
    }
}