using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System;

namespace Client
{
    sealed class SpawnMissionCheckpointSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<SpawnCheckpointByMissionPoint>> _filter = default;
        private EcsPoolInject<SpawnCheckpointByMissionPoint> _spawnPool = default;

        private EcsPoolInject<MinimapPanelComp> _minimapPool = default;
        private EcsPoolInject<MissionCheckpointComp> _checkpointPool = default;
        private EcsPoolInject<PointsOfInterestComp> _pointsPool = default;

        private Vector3 _bottomOffset = new Vector3(0f, 0.05f, 0f);

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var spawnComp = ref _spawnPool.Value.Get(entity);
                
                Vector3 position = Vector3.zero;
                if (string.IsNullOrEmpty(spawnComp.MissionPoint.InterestPointId))
                    position = spawnComp.MissionPoint.PointPosition;
                else
                    position = GetPositionOfPointOfInterest(spawnComp.MissionPoint.InterestPointId);

                SpawnCheckpoint(position, spawnComp.MissionPartId, spawnComp.FinishCheckpoint);

                _spawnPool.Value.Del(entity);
            }
        }

        private void SpawnCheckpoint(Vector3 position, int missionPartId, bool finishCheckpoint)
        {
            ref var minimapComp = ref _minimapPool.Value.Get(_state.Value.InterfaceEntity);
            int pointId = Guid.NewGuid().GetHashCode();

            GameObject checkpointPrefab = null;
            if (finishCheckpoint)
            {
                checkpointPrefab = _state.Value.MissionsConfig.FinishMissionPoint;
                minimapComp.MinimapPanelMb.ShowLastPoint(position, pointId);
            }
            else
            {
                checkpointPrefab = _state.Value.MissionsConfig.MissionPoint;
                minimapComp.MinimapPanelMb.ShowCheckpoint(position, pointId);
            }

            int checkpointEntity = _world.Value.NewEntity();
            ref var checkpointComp = ref _checkpointPool.Value.Add(checkpointEntity);

            GameObject checkpointGo = GameObject.Instantiate(
                checkpointPrefab, position + _bottomOffset, Quaternion.identity);

            MissionCheckpointMb checkpointMb = checkpointGo.GetComponent<MissionCheckpointMb>();
            checkpointMb.Init(_state.Value, missionPartId, pointId, checkpointEntity);
            checkpointComp.CheckpointMb = checkpointMb;
        }

        private Vector3 GetPositionOfPointOfInterest(string pointId)
        {
            ref var pointsComp = ref _pointsPool.Value.Get(_state.Value.PointsOfInterestEntity);
            return pointsComp.NamesPosPointsDictionary[pointId];
        }
    }
}