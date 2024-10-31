using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;

namespace Client
{
    sealed class MissionFailedSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<MissionPartWithCheckpointsComp, MissionFailedEvent>> _checkpointsMissionFilter = default;
        private EcsFilterInject<Inc<MissionPartOnKillComp, MissionFailedEvent>> _killMissionFilter = default;
        private EcsFilterInject<Inc<MissionPartOnKillOnZoneComp, MissionFailedEvent>> _killOnZoneMissionFilter = default;
        private EcsFilterInject<Inc<MissionPartWithCollectablesComp, MissionFailedEvent>> _collectablesMissionFilter = default;

        private EcsPoolInject<MissionFailedEvent> _failedPool = default;
        private EcsPoolInject<MissionPartComp> _missionPartPool = default;

        private EcsPoolInject<MissionPartWithCheckpointsComp> _missionPartWithCheckpointsPool = default;
        private EcsPoolInject<MissionPartOnKillComp> _missionPartOnKillPool = default;
        private EcsPoolInject<MissionPartOnKillOnZoneComp> _killOnZonePool = default;

        private EcsPoolInject<MissionPartOnTimeComp> _missionPartOnTimePool = default;

        private EcsPoolInject<MinimapPanelComp> _minimapPool = default;
        private EcsPoolInject<MissionPartProgressUiEvent> _uiEventPool = default;
        private EcsPoolInject<DestroyCheckpointEvent> _destroyCheckpointPool = default;

        //TODO удалять энтитюхи миссий

        public void Run(EcsSystems systems)
        {
            HandleCheckpointsMission();
            HandleKillMission();
            HandleKillOnZoneMission();
            HandleCollectableMission();
        }

        private void HandleCheckpointsMission()
        {
            foreach (var entity in _checkpointsMissionFilter.Value)
            {
                //TODO надо проверять вся ли компоненты удаляются
                GetIdAndFailSave(entity, out int missionPartId);
                ShowUi(missionPartId);
                RemoveCheckpointsForMission(missionPartId);
                RemoveMainComponentsFromEntity(entity);
                _missionPartWithCheckpointsPool.Value.Del(entity);
            }
        }

        private void HandleKillMission()
        {
            foreach (var entity in _killMissionFilter.Value)
            {
                GetIdAndFailSave(entity, out int missionPartId);
                ShowUi(missionPartId);
                RemoveMainComponentsFromEntity(entity);
                _missionPartOnKillPool.Value.Del(entity);
            }
        }

        private void HandleKillOnZoneMission()
        {
            foreach (var entity in _killOnZoneMissionFilter.Value)
            {
                GetIdAndFailSave(entity, out int missionPartId);
                ShowUi(missionPartId);
                RemoveMainComponentsFromEntity(entity);
                
                ref var killOnZoneComp = ref _killOnZonePool.Value.Get(entity);
                RemoveItemFromMinimap(killOnZoneComp.ItemOnMinimapId);

                _killOnZonePool.Value.Del(entity);
            }
        }

        private void HandleCollectableMission()
        {
            foreach (var entity in _collectablesMissionFilter.Value)
            {


                _failedPool.Value.Del(entity);
            }
        }

        private void GetIdAndFailSave(int entity, out int id)
        {
            ref var missionComp = ref _missionPartPool.Value.Get(entity);
            missionComp.MissionPartSaveData.Failed = true;
            id = missionComp.MissionPart.Id;
        }

        private void ShowUi(int missionPartId)
        {
            ref var uiComp = ref _uiEventPool.Value.Add(_world.Value.NewEntity());
            uiComp.MissionPartId = missionPartId;
        }

        private void RemoveMainComponentsFromEntity(int entity)
        {
            _missionPartPool.Value.Del(entity);
            _missionPartOnTimePool.Value.Del(entity);
            _failedPool.Value.Del(entity);
        }

        private void RemoveItemFromMinimap(int minimapItemId)
        {
            ref var minimapComp = ref _minimapPool.Value.Get(_state.Value.InterfaceEntity);
            minimapComp.MinimapPanelMb.RemovePointFromMinimap(minimapItemId);
        }

        private EcsFilterInject<Inc<MissionCheckpointComp>> _checkpointFilter = default;
        private EcsPoolInject<MissionCheckpointComp> _checkpointsPool = default;

        private void RemoveCheckpointsForMission(int missionPartId)
        {
            List<int> checkpoints = GetCheckpointEntityByMissionPartId(missionPartId);
            foreach (int checkpointEntity in checkpoints)
                _destroyCheckpointPool.Value.Add(checkpointEntity);
        }

        private List<int> GetCheckpointEntityByMissionPartId(int missionPartId)
        {
            List<int> results = new List<int>();
            foreach (var entity in _checkpointFilter.Value)
            {
                ref var checkpointComp = ref _checkpointsPool.Value.Get(entity);
                if (checkpointComp.CheckpointMb.MissionPartId == missionPartId)
                    results.Add(entity);
            }
            return results;
        }

        private EcsFilterInject<Inc<MissionCollectableComp>> _collectablesFilter = default;
        private EcsPoolInject<MissionCollectableComp> _collectablesPool = default;

        private List<int> GetCollectablesEntityByMissionPartId(int missionPartId)
        {
            List<int> results = new List<int>();
            foreach (var entity in _collectablesFilter.Value)
            {
                ref var collectablesComp = ref _collectablesPool.Value.Get(entity);
                if (collectablesComp.CollectableMb.MissionPartId == missionPartId)
                    results.Add(entity);
            }
            return results;
        }
    }
}