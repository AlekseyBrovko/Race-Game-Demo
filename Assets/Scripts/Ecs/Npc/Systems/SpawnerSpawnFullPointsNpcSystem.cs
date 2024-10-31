using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class SpawnerSpawnFullPointsNpcSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<SpawnNpcFullPointsEvent, NpcSpawnerComp>> _filter = default;

        private EcsPoolInject<SpawnNpcFullPointsEvent> _eventPool = default;
        private EcsPoolInject<NpcSpawnerComp> _spawnerPool = default;
        private EcsPoolInject<NpcInitPatrollPointsEvent> _initPatrollPointsPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var spawnerComp = ref _spawnerPool.Value.Get(entity);
                Spawn(spawnerComp, entity);

                _eventPool.Value.Del(entity);
            }
        }

        private void Spawn(NpcSpawnerComp spawnerComp, int spawnerEntity)
        {
            for (int i = 0; i < spawnerComp.SpawnPoints.Length; i++)
            {
                GameObject zombie = GetRandomPool(spawnerComp).GetFromPool();
                zombie.transform.position = spawnerComp.SpawnPoints[i].position;

                ISpawned spawnedMb = zombie.GetComponent<ISpawned>();
                spawnedMb.SpawnerEntity = spawnerEntity;
                spawnedMb.ZoneName = spawnerComp.ZoneName;
                spawnedMb.OnSpawn();
                spawnerComp.SpawnedMbs.Add(spawnedMb);
                _initPatrollPointsPool.Value.Add(spawnedMb.Entity);
            }
        }

        private GameObjectsPool GetRandomPool(NpcSpawnerComp spawnerComp) =>
            spawnerComp.GameObjectsPools[Random.Range(0, spawnerComp.GameObjectsPools.Length)];
    }
}