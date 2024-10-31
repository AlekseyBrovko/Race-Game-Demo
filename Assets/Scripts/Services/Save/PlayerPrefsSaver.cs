using Client;
using Saver;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaverPlayerPrefs
{
    public class PlayerPrefsSaver : INewSaver
    {
        //TODO прокинуть сюда версионность
        private const int _settingsSaveVersion = 2;
        private const int _moneySaveVersion = 2;
        private const int _carsSaveVersion = 2;
        private const int _missionSaveVersion = 5;

        public PlayerPrefsSaver() { }
        public PlayerPrefsSaver(GameState state) =>
            _state = state;

        private GameState _state;

        #region Money
        private const string _moneyTag = "Money";
        private int _startMoneyValue = 20000;
        public void SaveMoneyScore()
        {
            MoneyData moneyData = new MoneyData();
            moneyData.Version = _moneySaveVersion;
            moneyData.Money = _state.PlayerMoneyScore;
            SaveData(moneyData, _moneyTag);
        }

        public void LoadMoneyScore()
        {
            if (PlayerPrefs.HasKey(_moneyTag))
            {
                string loadedValue = PlayerPrefs.GetString(_moneyTag);
                MoneyData carData = JsonUtility.FromJson<MoneyData>(loadedValue);
                _state.PlayerMoneyScore = carData.Money;
            }
            else
            {
                LoadDefaultMoneyScore();
            }
        }

        private void LoadDefaultMoneyScore()
        {
            MoneyData moneyData = new MoneyData();
            moneyData.Money = _state.PlayerMoneyScore = _startMoneyValue;
            SaveData(moneyData, _moneyTag);
        }
        #endregion

        #region Position
        private const string _positionTag = "Position";
        public void SavePlayerPosition()
        {

        }

        public void LoadPlayerPosition()
        {

        }
        #endregion

        #region Settings
        private const string _settingsTag = "Settings";
        public void SaveSettings()
        {
            SettingsData settingsData = new SettingsData();
            settingsData.Version = _settingsSaveVersion;
            settingsData.IsOnSounds = _state.IsOnMasterVolume;
            settingsData.IsOnMusic = _state.IsOnMusic;
            settingsData.IsOnVibration = _state.IsOnVibration;
            settingsData.SoundsVolumeValue = _state.MasterVolumeValue;
            settingsData.MusicVolumeValue = _state.MusicVolumeValue;

            SaveData(settingsData, _settingsTag);
        }

        public void LoadSettings()
        {
            if (PlayerPrefs.HasKey(_settingsTag))
            {
                string loadedValue = PlayerPrefs.GetString(_settingsTag);
                SettingsData settingsData = JsonUtility.FromJson<SettingsData>(loadedValue);
                if (settingsData.Version != _settingsSaveVersion)
                {
                    LoadStandartSettings();
                }
                else
                {
                    _state.IsOnMasterVolume = settingsData.IsOnSounds;
                    _state.IsOnMusic = settingsData.IsOnMusic;
                    _state.IsOnVibration = settingsData.IsOnVibration;
                    _state.MasterVolumeValue = settingsData.SoundsVolumeValue;
                    _state.MusicVolumeValue = settingsData.MusicVolumeValue;
                }
            }
            else
            {
                LoadStandartSettings();
            }
        }

        private void LoadStandartSettings()
        {
            SettingsData settingsData = new SettingsData();

            settingsData.IsOnSounds = _state.IsOnMasterVolume = true;
            settingsData.IsOnMusic = _state.IsOnMusic = true;
            settingsData.IsOnVibration = _state.IsOnVibration = true;
            settingsData.SoundsVolumeValue = _state.MasterVolumeValue = 1f;
            settingsData.MusicVolumeValue = _state.MusicVolumeValue = 1f;

            SaveData(settingsData, _settingsTag);
        }
        #endregion

        #region Cars
        private const string _carsTag = "Cars";
        public void SaveCarsData()
        {
            CarsData carsData = new CarsData();
            carsData.Version = _carsSaveVersion;
            carsData.CurrentCar = _state.CurrentPlayerCar;
            carsData.SavedCars = _state.PlayersSavedCars;
            SaveData(carsData, _carsTag);
        }

        public void LoadCarsData()
        {
            if (PlayerPrefs.HasKey(_carsTag))
            {
                string loadedValue = PlayerPrefs.GetString(_carsTag);
                CarsData carData = JsonUtility.FromJson<CarsData>(loadedValue);
                if (carData.Version != _carsSaveVersion)
                {
                    LoadDefaultCar();
                }
                else
                {
                    _state.CurrentPlayerCar = carData.CurrentCar;
                    _state.PlayersSavedCars = carData.SavedCars;
                }
            }
            else
            {
                LoadDefaultCar();
            }
        }

        private void LoadDefaultCar()
        {
            CarsData carsData = new CarsData();
            carsData.Version = _carsSaveVersion;
            carsData.CurrentCar = _state.CurrentPlayerCar = _state.PlayerCarsPrefabsConfig.DefaultCar.Id;
            SavedCar defaultCar = new SavedCar();
            defaultCar.Id = _state.PlayerCarsPrefabsConfig.DefaultCar.Id;
            carsData.SavedCars.Add(defaultCar);

            _state.PlayersSavedCars = carsData.SavedCars;

            SaveData(carsData, _carsTag);
        }
        #endregion

        #region Mission
        private const string _missionTag = "Mission";
        public void SaveMissionData()
        {
            MissionSaveData saveData = _state.MissionSaveData;
            saveData.Version = _missionSaveVersion;
            SaveData(saveData, _missionTag);
        }

        public void LoadMissionData()
        {
            if (PlayerPrefs.HasKey(_missionTag))
            {
                MissionSaveData missionData = LoadData<MissionSaveData>(_missionTag);
                if (missionData.Version == _missionSaveVersion)
                {
                    _state.MissionSaveData = missionData;
                    _state.MissionSaveData.PrepareAfterLoad();
                }   
            }
        }
        #endregion

        private void SaveData<T>(T data, string saveTag) where T : class
        {
            string dataForSave = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(saveTag, dataForSave);
            PlayerPrefs.Save();
        }

        private T LoadData<T>(string saveTag) where T : class
        {
            if (PlayerPrefs.HasKey(saveTag))
            {
                string loadedValue = PlayerPrefs.GetString(saveTag);
                T result = JsonUtility.FromJson<T>(loadedValue);
                return result;
            }

            return null;
        }

        private const string _tutorialTag = "Tutorial";
        public void SaveTutorial()
        {
            TutorialData data = new TutorialData();
            data.InGameTutorial = _state.InGameTutorial;
            data.ShopTutorial = _state.ShopTutorial;
            data.StartGameTutorial = _state.StartGameTutorial;
            Debug.Log("_state.StartGameTutorial = " + _state.StartGameTutorial);
            SaveData(data, _tutorialTag);
        }

        public void LoadTutorial()
        {
            if (PlayerPrefs.HasKey(_tutorialTag))
            {
                string loadedValue = PlayerPrefs.GetString(_tutorialTag);
                TutorialData tutorialData = JsonUtility.FromJson<TutorialData>(loadedValue);
                _state.InGameTutorial = tutorialData.InGameTutorial;
                _state.ShopTutorial = tutorialData.ShopTutorial;
                _state.StartGameTutorial = tutorialData.StartGameTutorial;
            }
            else
            {
                LoadDefaultTutorial();
            }
        }

        public TutorialData LoadTutorialData()
        {
            TutorialData data = new TutorialData();
            if (!PlayerPrefs.HasKey(_tutorialTag))
            {
                data = new TutorialData();
                data.InGameTutorial = true;
                data.ShopTutorial = true;
                data.StartGameTutorial = true;
                SaveData(data, _tutorialTag);
            }
            else
            {
                data = LoadData<TutorialData>(_tutorialTag);
            }
            
            return data;
        }

        private void LoadDefaultTutorial()
        {
            TutorialData data = new TutorialData();
            data.InGameTutorial = _state.InGameTutorial = true;
            data.ShopTutorial = _state.ShopTutorial = true;
            data.StartGameTutorial = _state.StartGameTutorial = true;
            SaveData(data, _tutorialTag);
        }

        [Serializable]
        private class SettingsData
        {
            public int Version;

            public bool IsOnSounds;
            public bool IsOnMusic;
            public bool IsOnVibration;

            public float SoundsVolumeValue;
            public float MusicVolumeValue;
        }

        [Serializable]
        private class PositionData
        {
            public int Version;
            public float X_pos;
            public float Y_pos;
            public float Z_pos;
        }

        [Serializable]
        private class MoneyData
        {
            public int Version;
            public int Money;
        }

        [Serializable]
        private class CarsData
        {
            public int Version;
            public string CurrentCar;
            public List<SavedCar> SavedCars = new List<SavedCar>();
        }

        [Serializable]
        public class TutorialData
        {
            public bool ShopTutorial;
            public bool InGameTutorial;

            public bool StartGameTutorial;
        }
    }
}