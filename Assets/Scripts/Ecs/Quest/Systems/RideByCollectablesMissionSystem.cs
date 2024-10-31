using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class RideByCollectablesMissionSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;

        private EcsFilterInject<Inc<MissionCollectEvent>> _collectFilter = default;
        private EcsPoolInject<MissionCollectEvent> _collectPool = default;

        private EcsFilterInject<Inc<MissionPartWithCollectablesComp, MissionPartOneByOneComp>>
            _missionOneByOneFilter = default;

        private EcsFilterInject<Inc<MissionPartWithCollectablesComp, MissionPartAllOnMapComp>>
            _missionAllOnMapFilter = default;

        private EcsPoolInject<MissionPartWithCollectablesComp> _missionPool = default;
        private EcsPoolInject<MissionPartProgressUiEvent> _uiEventPool = default;
        private EcsPoolInject<SpawnMissionCollectableEvent> _spawnCollectablePool = default;
        private EcsPoolInject<DestroyMissionCollectableEvent> _destroyPool = default;
        private EcsPoolInject<MissionPartCompleteEvent> _missionPartCompletePool = default;
        private EcsPoolInject<UISoundEvent> _soundPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var collectEntity in _collectFilter.Value)
            {
                ref var collectComp = ref _collectPool.Value.Get(collectEntity);

                HandleOneByOne(collectComp);
                HandleAllOnMap(collectComp);
                PlaySound();

                _destroyPool.Value.Add(collectComp.CollectableEntity);
                _collectPool.Value.Del(collectEntity);
            }
        }

        private void HandleOneByOne(MissionCollectEvent collectComp)
        {
            foreach (var entity in _missionOneByOneFilter.Value)
            {
                ref var missionComp = ref _missionPool.Value.Get(entity);
                if (missionComp.CollectMissionPart.Id == collectComp.MissionPartId)
                    HandleOneByOneProgress(ref missionComp);
            }
        }

        private void HandleAllOnMap(MissionCollectEvent collectComp)
        {
            foreach (var entity in _missionAllOnMapFilter.Value)
            {
                ref var missionComp = ref _missionPool.Value.Get(entity);
                if (missionComp.CollectMissionPart.Id == collectComp.MissionPartId)
                    HandleAllOnMapProgress(ref missionComp);
            }
        }

        private void HandleOneByOneProgress(ref MissionPartWithCollectablesComp missionComp)
        {
            if (missionComp.CollectMissionPartSaveData.Progress ==
                missionComp.CollectMissionPart.CollectPoints.Length - 1)
            {
                missionComp.CollectMissionPartSaveData.Complete = true;
                _missionPartCompletePool.Value.Add(_world.Value.NewEntity());
                ShowProgress(missionComp.CollectMissionPart.Id);
            }
            else
            {
                missionComp.CollectMissionPartSaveData.Progress++;
                SpawnNextCollectable(missionComp);
            }
        }

        private void HandleAllOnMapProgress(ref MissionPartWithCollectablesComp missionComp)
        {
            if (missionComp.CollectMissionPartSaveData.Progress ==
                missionComp.CollectMissionPart.CollectPoints.Length - 1)
            {
                missionComp.CollectMissionPartSaveData.Complete = true;
                _missionPartCompletePool.Value.Add(_world.Value.NewEntity());
                ShowProgress(missionComp.CollectMissionPart.Id);
            }
            else
            {
                missionComp.CollectMissionPartSaveData.Progress++;
            }
        }

        private void SpawnNextCollectable(MissionPartWithCollectablesComp missionComp)
        {
            int collectableIndex = missionComp.CollectMissionPartSaveData.Progress;

            ref var spawnCollectableComp = ref _spawnCollectablePool.Value.Add(_world.Value.NewEntity());
            spawnCollectableComp.CollectPoint = missionComp.CollectMissionPart.CollectPoints[collectableIndex];
            spawnCollectableComp.CollectablePrefab = missionComp.CollectMissionPart.CollectPrefab;
            spawnCollectableComp.MissionPartId = missionComp.CollectMissionPart.Id;
        }

        private void ShowProgress(int missionPartId)
        {
            ref var uiComp = ref _uiEventPool.Value.Add(_world.Value.NewEntity());
            uiComp.MissionPartId = missionPartId;
        }

        private void PlaySound()
        {
            ref var soundComp = ref _soundPool.Value.Add(_world.Value.NewEntity());
            soundComp.Sound = Enums.SoundEnum.CollectMissionSound;
        }
    }
}