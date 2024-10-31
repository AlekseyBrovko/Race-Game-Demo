using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using UnityEngine;

namespace Client
{
    sealed class SpawnMissionCollectablesSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<SpawnMissionCollectableEvent>> _spawnByPointFilter = default;
        private EcsPoolInject<SpawnMissionCollectableEvent> _spawnCollectablePool = default;

        private EcsPoolInject<MissionCollectableComp> _collectablePool = default;
        private EcsPoolInject<MinimapPanelComp> _minimapPool = default;
        private EcsPoolInject<PointsOfInterestComp> _pointsPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _spawnByPointFilter.Value)
            {
                ref var spawnComp = ref _spawnCollectablePool.Value.Get(entity);

                Vector3 position = Vector3.zero;
                if (string.IsNullOrEmpty(spawnComp.CollectPoint.InterestPointId))
                    position = spawnComp.CollectPoint.PointPosition;
                else
                    position = GetPositionOfPointOfInterest(spawnComp.CollectPoint.InterestPointId);

                Spawn(position, spawnComp.MissionPartId, spawnComp.CollectablePrefab);
                _spawnCollectablePool.Value.Del(entity);
            }
        }

        private void Spawn(Vector3 position, int missionPartId, MissionCollectableMb prefab)
        {
            MissionCollectableMb collectableMb = GameObject.Instantiate(prefab);
            collectableMb.transform.position = position;

            int collectableEntity = _world.Value.NewEntity();
            int pointId = Guid.NewGuid().GetHashCode();

            ref var collectableComp = ref _collectablePool.Value.Add(collectableEntity);
            collectableComp.CollectableMb = collectableMb;
            collectableMb.Init(_state.Value, missionPartId, pointId, collectableEntity);

            ref var minimapComp = ref _minimapPool.Value.Get(_state.Value.InterfaceEntity);
            minimapComp.MinimapPanelMb.ShowCollectable(position, pointId);
        }

        private Vector3 GetPositionOfPointOfInterest(string pointId)
        {
            ref var pointsComp = ref _pointsPool.Value.Get(_state.Value.PointsOfInterestEntity);
            return pointsComp.NamesPosPointsDictionary[pointId];
        }
    }
}