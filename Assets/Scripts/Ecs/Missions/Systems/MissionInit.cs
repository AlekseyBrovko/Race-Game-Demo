using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using UnityEngine;
using static Enums;

namespace Client
{
    sealed class MissionInit : IEcsInitSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsPoolInject<BuildAtStartMissionPanelEvent> _buildPanelPool = default;
        private EcsPoolInject<SpawnCheckpointByMissionPoint> _spawnCheckpointPool = default;

        private EcsPoolInject<MissionPartComp> _missionPartPool = default;
        private EcsPoolInject<MissionPartOnTimeComp> _missionPartOnTimePool = default;
        private EcsPoolInject<MissionPartWithCheckpointsComp> _missionPartWithCheckpointsPool = default;
        private EcsPoolInject<MissionPartOnKillComp> _missionPartOnKillPool = default;
        private EcsPoolInject<MissionPartOnKillOnZoneComp> _killOnZonePool = default;
        private EcsPoolInject<MissionPartWithCollectablesComp> _missionPartWithCollectablesPool = default;

        private EcsPoolInject<SpawnMissionCollectableEvent> _spawnCollectablePool = default;

        private EcsPoolInject<MissionPartOneByOneComp> _oneByOnePool = default;
        private EcsPoolInject<MissionPartAllOnMapComp> _allOneMapPool = default;

        private EcsPoolInject<MinimapPanelComp> _minimapPool = default;
        private EcsPoolInject<PointsOfInterestComp> _pointsPool = default;

        private EcsPoolInject<ShowMissionBrifEvent> _brifPool = default;

        private MissionBase _currentMission;

        public void Init(EcsSystems systems)
        {
            if (SessionContextHolder.Instance.GameModeType == GameModType.Story)
            {
                _buildPanelPool.Value.Add(_world.Value.NewEntity());

                if (_state.Value.MissionSaveData == null)
                    StartNewGame();
                else
                    LoadMission();
            }
        }

        private void StartNewGame()
        {
            Debug.Log("NewMissionInit StartNewGame()");
            MissionsConfig missionsConfig = _state.Value.MissionsConfig;
            _currentMission = missionsConfig.GetFirstMission();
            _state.Value.CurrentMission = _currentMission;
            _state.Value.MissionSaveData = new MissionSaveData(_currentMission, false);
            _state.Value.SaveMissionData();
            ShowBrif();
            BuildMissionComponents();
        }

        private void LoadMission()
        {
            Debug.Log("NewMissionInit LoadMission()");
            MissionsConfig missionsConfig = _state.Value.MissionsConfig;
            _currentMission = missionsConfig.GetMissionById(_state.Value.MissionSaveData.MissionId);

            if (IsMissionComplete())
            {
                //выяснить все ли мисси пройдены,
                //если нет, то узнать последняя миссия или нет
                if (!_state.Value.MissionSaveData.AllMissionComplete)
                {
                    if (_state.Value.MissionsConfig.IsLastMission(_currentMission.Id))
                    {
                        _currentMission = _state.Value.MissionsConfig.GetMissionOnAllComplete(_currentMission.Id);
                        _state.Value.MissionSaveData = new MissionSaveData(_currentMission, true);
                    }
                    else
                    {
                        _currentMission = _state.Value.MissionsConfig.GetNextMission(_currentMission.Id);
                        _state.Value.MissionSaveData = new MissionSaveData(_currentMission, false);
                    }
                }
                else
                {
                    _currentMission = _state.Value.MissionsConfig.GetMissionOnAllComplete(_currentMission.Id);
                    _state.Value.MissionSaveData = new MissionSaveData(_currentMission, true);
                }

                _state.Value.SaveMissionData();
            }
            _state.Value.CurrentMission = _currentMission;

            ShowBrif();
            BuildMissionComponents();
        }

        private void ShowBrif()
        {
            if (_state.Value.CurrentMission.Brif.ShowBrif)
                _brifPool.Value.Add(_world.Value.NewEntity());
        }

        private void BuildMissionComponents()
        {
            _state.Value.MissionSaveData.LogSaveDatas();

            Debug.Log("_currentMission.MissionParts.Length = " + _currentMission.MissionParts.Length);

            for (int i = 0; i < _currentMission.MissionParts.Length; i++)
            {
                MissionPartBase missionPart = _currentMission.MissionParts[i];
                MissionPartSaveDataBase missionPartSaveData =
                    _state.Value.MissionSaveData.GetMissionPartById(missionPart.Id);

                int missionPartEntity = _world.Value.NewEntity();
                ref var missionPartComp = ref _missionPartPool.Value.Add(missionPartEntity);
                missionPartComp.MissionPart = missionPart;
                missionPartComp.MissionPartSaveData = missionPartSaveData;

                if (missionPartComp.MissionPartSaveData == null)
                    Debug.Log("NewMissionInit MissionPartSaveData == null");
                else
                    Debug.Log("NewMissionInit MissionPartSaveData != null");

                if (missionPartComp.MissionPartSaveData.Complete)
                    continue;

                if (missionPart.MissionOnTime)
                    BuildOnTimeMission(missionPart, missionPartSaveData, missionPartEntity);

                switch (missionPart.MissionType)
                {
                    case MissionType.RideByCheckpointsOneByOne:
                        BuildRideMission(missionPart, missionPartSaveData, missionPartEntity);
                        break;

                    case MissionType.RideByCheckpointsAllOnMap:
                        BuildRideAllCheckpointsOnMapMission(missionPart, missionPartSaveData, missionPartEntity);
                        break;

                    case MissionType.Kill:
                        BuildKillMission(missionPart, missionPartSaveData, missionPartEntity);
                        break;

                    case MissionType.KillOnZone:
                        BuildKillMission(missionPart, missionPartSaveData, missionPartEntity);
                        BuildKillOnZoneMission(missionPart, missionPartSaveData, missionPartEntity);
                        break;

                    case MissionType.CollectOneByOne:
                        BuildCollectMission(missionPart, missionPartSaveData, missionPartEntity);
                        break;

                    case MissionType.CollectAllOnMap:
                        BuildCollectMissionAllOnMap(missionPart, missionPartSaveData, missionPartEntity);
                        break;
                }
            }
        }

        private bool IsMissionComplete()
        {
            foreach (var part in _state.Value.MissionSaveData.AllMissionsParts)
                if (!part.Complete)
                {
                    Debug.Log("IsMissionComplete = false");
                    return false;
                }

            Debug.Log("IsMissionComplete = true");
            return true;
        }

        private void BuildRideMission(MissionPartBase missionPart,
            MissionPartSaveDataBase missionPartSaveData, int missionPartEntity)
        {
            _oneByOnePool.Value.Add(missionPartEntity);

            ref var rideMissionComp = ref _missionPartWithCheckpointsPool.Value.Add(missionPartEntity);
            rideMissionComp.RideMissionPartSaveData = (RidePartSaveData)missionPartSaveData;
            rideMissionComp.RideMissionPart = (RideOneByOneMissionPart)missionPart;

            int checkpointsAmount = rideMissionComp.RideMissionPart.InterestPointsIds.Length;
            MissionPoint point = rideMissionComp.RideMissionPart.InterestPointsIds[rideMissionComp.Counter];

            if (missionPart.SaveMissionProgress)
                rideMissionComp.Counter = rideMissionComp.RideMissionPartSaveData.CheckpointsCounter;

            ref var spawnCheckpointComp = ref _spawnCheckpointPool.Value.Add(_world.Value.NewEntity());
            spawnCheckpointComp.MissionPartId = missionPart.Id;
            spawnCheckpointComp.MissionPoint = point;
            spawnCheckpointComp.FinishCheckpoint = checkpointsAmount - 1 == rideMissionComp.Counter;
        }

        private void BuildRideAllCheckpointsOnMapMission(MissionPartBase missionPart,
            MissionPartSaveDataBase missionPartSaveData, int missionPartEntity)
        {
            _allOneMapPool.Value.Add(missionPartEntity);

            ref var rideMissionComp = ref _missionPartWithCheckpointsPool.Value.Add(missionPartEntity);
            rideMissionComp.RideMissionPartSaveData = (RidePartSaveData)missionPartSaveData;
            rideMissionComp.RideMissionPart = (RideAllOnMapMissionPart)missionPart;

            foreach (var point in rideMissionComp.RideMissionPart.InterestPointsIds)
            {
                ref var spawnCheckpointComp = ref _spawnCheckpointPool.Value.Add(_world.Value.NewEntity());
                spawnCheckpointComp.MissionPartId = missionPart.Id;
                spawnCheckpointComp.MissionPoint = point;
            }
        }

        private void BuildOnTimeMission(MissionPartBase missionPart,
            MissionPartSaveDataBase missionPartSaveData, int missionPartEntity)
        {
            ref var onTimeComp = ref _missionPartOnTimePool.Value.Add(missionPartEntity);
            onTimeComp.Timer = missionPart.TimeOnMission;
            onTimeComp.MissionPart = missionPart;
            onTimeComp.MissionPartSaveData = missionPartSaveData;
        }

        private void BuildKillMission(MissionPartBase missionPart,
            MissionPartSaveDataBase missionPartSaveData, int missionPartEntity)
        {
            ref var killMissionPartComp = ref _missionPartOnKillPool.Value.Add(missionPartEntity);
            killMissionPartComp.KillMissionPart = (KillByNameMissionPart)missionPart;
            killMissionPartComp.KillMissionPartSaveData = (KillPartSaveData)missionPartSaveData;

            //if (missionPart.SaveMissionProgress)
            //    killMissionPartComp.Counter = killMissionPartComp.KillMissionPartSaveData.Progress;
        }

        private void BuildKillOnZoneMission(MissionPartBase missionPart,
            MissionPartSaveDataBase missionPartSaveData, int missionPartEntity)
        {
            KillOnZoneMissionPart killOnZoneMissionPart = (KillOnZoneMissionPart)missionPart;
            ref var killOnZoneComp = ref _killOnZonePool.Value.Add(missionPartEntity);
            killOnZoneComp.ZoneName = killOnZoneMissionPart.ZoneName;
            killOnZoneComp.ItemOnMinimapId = Guid.NewGuid().GetHashCode();
            Debug.Log("killOnZoneComp.ItemOnMinimapId = " + killOnZoneComp.ItemOnMinimapId);

            //TODO создание значков на миникарте нужно рефакторить на отдельную систему
            ref var minimapComp = ref _minimapPool.Value.Get(_state.Value.InterfaceEntity);
            ref var pointsComp = ref _pointsPool.Value.Get(_state.Value.PointsOfInterestEntity);
            Vector3 position = pointsComp.NamesPosZonesDictionary[killOnZoneComp.ZoneName];
            int id = killOnZoneComp.ItemOnMinimapId;

            minimapComp.MinimapPanelMb.ShowZone(position, id);
        }

        private void BuildCollectMission(MissionPartBase missionPart,
            MissionPartSaveDataBase missionPartSaveData, int missionPartEntity)
        {
            ref var collectMissionComp = ref _missionPartWithCollectablesPool.Value.Add(missionPartEntity);
            collectMissionComp.CollectMissionPart = (CollectMissionPart)missionPart;
            collectMissionComp.CollectMissionPartSaveData = (CollectPartSaveData)missionPartSaveData;
            _oneByOnePool.Value.Add(missionPartEntity);

            SpawnCollectable(collectMissionComp.CollectMissionPart, collectMissionComp.CollectMissionPart.CollectPoints[0]);
        }

        private void BuildCollectMissionAllOnMap(MissionPartBase missionPart,
            MissionPartSaveDataBase missionPartSaveData, int missionPartEntity)
        {
            ref var collectMissionComp = ref _missionPartWithCollectablesPool.Value.Add(missionPartEntity);
            collectMissionComp.CollectMissionPart = (CollectMissionPart)missionPart;
            collectMissionComp.CollectMissionPartSaveData = (CollectPartSaveData)missionPartSaveData;
            _allOneMapPool.Value.Add(missionPartEntity);

            foreach (MissionPoint collectPoint in collectMissionComp.CollectMissionPart.CollectPoints)
                SpawnCollectable(collectMissionComp.CollectMissionPart, collectPoint);
        }

        private void SpawnCollectable(CollectMissionPart missionPart, MissionPoint collectPoint)
        {
            ref var spawnCollectableComp = ref _spawnCollectablePool.Value.Add(_world.Value.NewEntity());
            spawnCollectableComp.MissionPartId = missionPart.Id;
            spawnCollectableComp.CollectablePrefab = missionPart.CollectPrefab;
            spawnCollectableComp.CollectPoint = collectPoint;
        }
    }
}