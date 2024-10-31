using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class RideByCheckpointsMissionSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;

        private EcsFilterInject<Inc<RideMissionCheckpointEvent>> _checkpointFilter = default;

        private EcsFilterInject<Inc<MissionPartWithCheckpointsComp, MissionPartOneByOneComp>>
            _missionOneByOnePartFilter = default;

        private EcsFilterInject<Inc<MissionPartWithCheckpointsComp, MissionPartAllOnMapComp>>
            _missionAllOnMapFilter = default;

        private EcsPoolInject<RideMissionCheckpointEvent> _checkpointPool = default;
        private EcsPoolInject<MissionPartWithCheckpointsComp> _missionPartPool = default;
        private EcsPoolInject<SpawnCheckpointByMissionPoint> _spawnCheckpointByPointPool = default;
        private EcsPoolInject<DestroyCheckpointEvent> _destroyCheckpointPool = default;
        private EcsPoolInject<MissionPartProgressUiEvent> _uiEventPool = default;
        private EcsPoolInject<MissionPartCompleteEvent> _missionPartCompletePool = default;
        private EcsPoolInject<UISoundEvent> _soundPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var checkpointEventEntity in _checkpointFilter.Value)
            {
                ref var checkpointComp = ref _checkpointPool.Value.Get(checkpointEventEntity);

                PlayCheckpointSound();

                HandleRideOnebyOneMissions(checkpointComp);
                HandleRideAllOnMapMissions(checkpointComp);


                _destroyCheckpointPool.Value.Add(checkpointComp.CheckpointEntity);
                _checkpointPool.Value.Del(checkpointEventEntity);
            }
        }

        private void HandleRideOnebyOneMissions(RideMissionCheckpointEvent checkpointComp)
        {
            foreach (var missionPartEntity in _missionOneByOnePartFilter.Value)
            {
                ref var missionComp = ref _missionPartPool.Value.Get(missionPartEntity);
                if (missionComp.RideMissionPart.Id == checkpointComp.MissionPartId)
                    HandleRideMissionOneByOneProgress(ref missionComp);
            }
        }

        private void HandleRideAllOnMapMissions(RideMissionCheckpointEvent checkpointComp)
        {
            foreach (var missionPartEntity in _missionAllOnMapFilter.Value)
            {
                ref var missionComp = ref _missionPartPool.Value.Get(missionPartEntity);
                if (missionComp.RideMissionPart.Id == checkpointComp.MissionPartId)
                    HandleRideMissionAllOnMapProgress(ref missionComp);
            }
        }

        private void HandleRideMissionOneByOneProgress(ref MissionPartWithCheckpointsComp missionComp)
        {
            //если последний чекпоинт
            if (missionComp.RideMissionPartSaveData.CheckpointsCounter ==
                missionComp.RideMissionPart.InterestPointsIds.Length - 1)
            {
                missionComp.RideMissionPartSaveData.Complete = true;
                _missionPartCompletePool.Value.Add(_world.Value.NewEntity());
                ShowProgress(missionComp.RideMissionPart.Id);
            }
            else
            {
                missionComp.RideMissionPartSaveData.CheckpointsCounter++;
                SpawnNextCheckpoint(missionComp);
            }
        }

        private void HandleRideMissionAllOnMapProgress(ref MissionPartWithCheckpointsComp missionComp)
        {
            if (missionComp.RideMissionPartSaveData.CheckpointsCounter ==
                missionComp.RideMissionPart.InterestPointsIds.Length - 1)
            {
                missionComp.RideMissionPartSaveData.Complete = true;
                _missionPartCompletePool.Value.Add(_world.Value.NewEntity());
                ShowProgress(missionComp.RideMissionPart.Id);
            }
            else
            {
                missionComp.RideMissionPartSaveData.CheckpointsCounter++;
            }
        }

        private void SpawnNextCheckpoint(MissionPartWithCheckpointsComp missionComp)
        {
            int checkpointsCounter = missionComp.RideMissionPartSaveData.CheckpointsCounter;
            int checkpointsAmount = missionComp.RideMissionPart.InterestPointsIds.Length;

            MissionPoint point = missionComp.RideMissionPart.InterestPointsIds[checkpointsCounter];

            ref var spawnCheckpointByIdPointComp = ref _spawnCheckpointByPointPool.Value.Add(_world.Value.NewEntity());
            spawnCheckpointByIdPointComp.MissionPartId = missionComp.RideMissionPart.Id;
            spawnCheckpointByIdPointComp.FinishCheckpoint = checkpointsCounter == checkpointsAmount - 1;
            spawnCheckpointByIdPointComp.MissionPoint = point;
        }

        private void ShowProgress(int missionPartId)
        {
            ref var uiComp = ref _uiEventPool.Value.Add(_world.Value.NewEntity());
            uiComp.MissionPartId = missionPartId;
        }

        private void PlayCheckpointSound()
        {
            ref var soundComp = ref _soundPool.Value.Add(_world.Value.NewEntity());
            soundComp.Sound = Enums.SoundEnum.CheckpointMissionSound;
        }
    }
}