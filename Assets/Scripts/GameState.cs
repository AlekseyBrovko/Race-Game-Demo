using Leopotam.EcsLite;
using Saver;
using System.Collections.Generic;
using SaverPlayerPrefs;

namespace Client
{
    public class GameState
    {
        public EcsWorld EcsWorld;
        public Enums.SceneType SceneType;
        public SettingsConfig SettingsConfig;
        public UIConfig UiConfig;
        public PlayerConfig PlayerConfig;
        public PlayerCarsConfig PlayerCarsPrefabsConfig;
        public EnemyPrefabsConfig EnemyPrefabsConfig;
        public UIPanelConfig UiPanelConfig;
        public QuestsConfig QuestsConfig;
        public ParticlesConfig ParticlesConfig;
        public PickupsConfig PickupsConfig;
        public MissionsConfig MissionsConfig;
        public SoundFMODConfig SoundFMODConfig;
        public MinimapDataSo MinimapData;
        public LocalizationConfig LocalizationConfig;

        public float MasterVolumeValue = 1f;
        public float SoundsVolumeValue = 1f;
        public float MusicVolumeValue = 1f;

        public bool IsMobilePlatform;
        public bool IsWebGl;

        public int CameraEntity;
        public int PlayerEntity;
        public int PlayerCarEntity;
        public int InputEntity;
        public int SoundsEntity;
        public int VibrationEntity;
        public int LevelEntity;
        public int InterfaceEntity;
        public int LayerMaskEntity;
        public int PatrollPointsEntity;
        public int ObjectsPoolEntity;
        public int PlayerSpawnPointsEntity;
        public int EnemySpawnersEntity;
        public int PointsOfInterestEntity;
        public int QuestEntity;
        public int RespawnPointsEntity;
        public int TutorialEntity;

        public MissionSaveData MissionSaveData { get; set; }
        public MissionBase CurrentMission { get; set; }

        public bool PlaySystems;
        public bool PauseSystems;

        //TODO Saver переписать по человечески, вынести это в отдельный класс
        public bool IsOnMasterVolume = true;
        public bool IsOnSounds = true;
        public bool IsOnMusic = true;
        public bool IsOnVibration = true;

        public bool ShopTutorial;
        public bool InGameTutorial;
        public bool StartGameTutorial;

        //это на режим фрирайд пойдёт
        public bool WithFuel = true;

        //only for develop
        public bool EnableFuel;
        public bool EnableDeath;

        public int PlayerMoneyScore;
        public string CurrentPlayerCar;
        public List<SavedCar> PlayersSavedCars;

        private INewSaver _saver;

        public GameState(EcsWorld world, Enums.SceneType sceneType, SettingsConfig settingsConfig,
            UIConfig uiConfig, PlayerConfig playerConfig, UIPanelConfig uiPanelConfig,
            PlayerCarsConfig playerCarsConfig, EnemyPrefabsConfig enemyConfig, QuestsConfig questsConfig,
            ParticlesConfig _particlesConfig, PickupsConfig pickupsConfig, MissionsConfig missionConfig,
            SoundFMODConfig soundFMODConfig, bool enableFuel, bool enableDeath, MinimapDataSo minimapData, 
            LocalizationConfig localizationConfig)
        {
            EcsWorld = world;
            SceneType = sceneType;
            SettingsConfig = settingsConfig;
            UiConfig = uiConfig;
            PlayerConfig = playerConfig;
            UiPanelConfig = uiPanelConfig;
            PlayerCarsPrefabsConfig = playerCarsConfig;
            EnemyPrefabsConfig = enemyConfig;
            QuestsConfig = questsConfig;
            ParticlesConfig = _particlesConfig;
            PickupsConfig = pickupsConfig;
            MissionsConfig = missionConfig;
            SoundFMODConfig = soundFMODConfig;
            MinimapData = minimapData;
            LocalizationConfig = localizationConfig;

            EnableFuel = enableFuel;
            EnableDeath = enableDeath;

            _saver = new PlayerPrefsSaver(this);

            _saver.LoadSettings();
            _saver.LoadCarsData();
            _saver.LoadMoneyScore();
            _saver.LoadMissionData();
            _saver.LoadTutorial();

            //Load();
        }

        #region LevelGroups
        public void StartPlaySystems() =>
            EcsWorld.GetPool<EnablePlaySystemsEvent>().Add(LevelEntity);

        public void StartPauseSystems() =>
            EcsWorld.GetPool<EnablePauseSystemsEvent>().Add(LevelEntity);
        #endregion

        #region Save
        public void SaveSettings() =>
            _saver.SaveSettings();

        public void SaveCars() =>
            _saver.SaveCarsData();

        public void SaveMoneyScore() =>
            _saver.SaveMoneyScore();

        public void SaveMissionData() =>
            _saver.SaveMissionData();

        public void SaveTutorial() =>
            _saver.SaveTutorial();
        #endregion Save
    }
}