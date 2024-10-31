using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Linq;
using UnityEngine;

namespace Client
{
    sealed class KillMissionSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<KillZombieEvent>> _killZombieFilter = default;

        private EcsFilterInject<Inc<MissionPartOnKillComp>, Exc<MissionPartOnKillOnZoneComp>> _killMissionFilter = default;
        private EcsFilterInject<Inc<MissionPartOnKillComp, MissionPartOnKillOnZoneComp>> _killOnZoneFilter = default;

        private EcsPoolInject<KillZombieEvent> _killZombiePool = default;
        private EcsPoolInject<MissionPartOnKillComp> _killPartPool = default;
        private EcsPoolInject<MissionPartOnKillOnZoneComp> _killOnZonePartPool = default;
        private EcsPoolInject<MissionPartProgressUiEvent> _progressUiPool = default;
        private EcsPoolInject<MinimapPanelComp> _minimapPool = default;
        private EcsPoolInject<MissionPartCompleteEvent> _missionPartCompletePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _killZombieFilter.Value)
            {
                ref var killZombieComp = ref _killZombiePool.Value.Get(entity);

                HandleKillMissions(killZombieComp);
                HandleKillOnZoneMissions(killZombieComp);
            }
        }

        private void HandleKillMissions(KillZombieEvent killZombieComp)
        {
            foreach (var missionPartEntity in _killMissionFilter.Value)
            {
                ref var missionPartComp = ref _killPartPool.Value.Get(missionPartEntity);
                if (missionPartComp.KillMissionPart.EnemyIds.Contains(killZombieComp.ZombieName))
                    HandleMissionProgress(ref missionPartComp, missionPartEntity, out bool isComplete);
            }
        }

        private void HandleKillOnZoneMissions(KillZombieEvent killZombieComp)
        {
            foreach (var missionPartEntity in _killOnZoneFilter.Value)
            {
                ref var missionPartComp = ref _killPartPool.Value.Get(missionPartEntity);
                ref var zoneComp = ref _killOnZonePartPool.Value.Get(missionPartEntity);

                if (missionPartComp.KillMissionPart.EnemyIds.Contains(killZombieComp.ZombieName) &&
                    killZombieComp.ZoneName == zoneComp.ZoneName)
                {
                    int itemOnMinimapId = zoneComp.ItemOnMinimapId;
                    HandleMissionProgress(ref missionPartComp, missionPartEntity, out bool isComplete);
                    if (isComplete)
                    {
                        Debug.Log("HandleKillOnZoneMissions isComplete");
                        ref var minimapComp = ref _minimapPool.Value.Get(_state.Value.InterfaceEntity);
                        minimapComp.MinimapPanelMb.RemovePointFromMinimap(itemOnMinimapId);
                    }
                }
            }
        }

        private void HandleMissionProgress(ref MissionPartOnKillComp missionPartComp, int missionEntity, out bool isComplete)
        {
            missionPartComp.KillMissionPartSaveData.Progress++;
            isComplete = false;
            int missionId = missionPartComp.KillMissionPart.Id;

            if (missionPartComp.KillMissionPart.SaveMissionProgress)
                _state.Value.SaveMissionData();

            if (missionPartComp.KillMissionPartSaveData.Progress >= missionPartComp.KillMissionPart.EnemyAmount)
            {
                missionPartComp.KillMissionPartSaveData.Progress = missionPartComp.KillMissionPart.EnemyAmount;
                missionPartComp.KillMissionPartSaveData.Complete = true;
                isComplete = true;
                HandleMissionComplete(missionEntity);
            }
            ShowMissionProgress(missionId);
        }

        private void ShowMissionProgress(int missionPartId)
        {
            ref var progressUiComp = ref _progressUiPool.Value.Add(_world.Value.NewEntity());
            progressUiComp.MissionPartId = missionPartId;
        }

        private void HandleMissionComplete(int missionEntity)
        {
            if (_killPartPool.Value.Has(missionEntity))
                _killPartPool.Value.Del(missionEntity);

            if (_killOnZonePartPool.Value.Has(missionEntity))
                _killOnZonePartPool.Value.Del(missionEntity);

            _missionPartCompletePool.Value.Add(_world.Value.NewEntity());
        }
    }
}