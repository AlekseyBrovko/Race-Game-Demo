using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public sealed class SpawnNpcSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<SpawnNpcEvent>> _filter = default;

        private EcsPoolInject<SpawnNpcEvent> _spawnPool = default;
        private EcsPoolInject<NpcSpawnerComp> _spawnerPool = default;
        private EcsPoolInject<SpawnerSpawnCoolDownComp> _coolDownPool = default;
        private EcsPoolInject<NpcInitPatrollPointsEvent> _initPatrollPointsPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var spawnComp = ref _spawnPool.Value.Get(entity);
                ref var spawnerComp = ref _spawnerPool.Value.Get(spawnComp.SpawnerEntity);

                Spawn(ref spawnerComp, spawnComp.SpawnerEntity);

                _spawnPool.Value.Del(entity);
            }
        }

        private void Spawn(ref NpcSpawnerComp spawnerComp, int entity)
        {
            if (GetSpawnPosition(ref spawnerComp, out Vector3 position))
            {
                GameObject zombie = GetRandomPool(spawnerComp).GetFromPool();
                zombie.transform.position = position;
                zombie.SetActive(true);

                ISpawned spawnedMb = zombie.GetComponent<ISpawned>();
                spawnedMb.SpawnerEntity = entity;
                spawnedMb.ZoneName = spawnerComp.ZoneName;
                spawnedMb.OnSpawn();

                spawnerComp.SpawnedMbs.Add(spawnedMb);
                _initPatrollPointsPool.Value.Add(spawnedMb.Entity);
            }
            else
            {
                ref var coolDownComp = ref _coolDownPool.Value.Get(entity);
                coolDownComp.Timers.Add(0f);
            }
        }

        private bool GetSpawnPosition(ref NpcSpawnerComp spawnerComp, out Vector3 position)
        {
            position = Vector3.zero;

            Vector3 spawnerPosition = spawnerComp.SpawnPointMb.transform.position;
            if (spawnerComp.SpawnPoints == null || spawnerComp.SpawnPoints.Length == 0)
            {
                if (!Physics.CheckSphere(spawnerPosition + Vector3.up * 0.5f, 1f))
                {
                    position = spawnerPosition;
                    return true;
                }
                return false;
            }
            else
            {
                int startIndex = Random.Range(0, spawnerComp.SpawnPoints.Length);
                Transform tempSpawnPoint = spawnerComp.SpawnPoints[Random.Range(0, spawnerComp.SpawnPoints.Length)];
                if (!Physics.CheckSphere(tempSpawnPoint.position + Vector3.up * 0.5f, 1f))
                {
                    position = tempSpawnPoint.position;
                    return true;
                }
                else
                {
                    startIndex = 0;
                    while (startIndex < spawnerComp.SpawnPoints.Length)
                    {
                        tempSpawnPoint = spawnerComp.SpawnPoints[startIndex];
                        if (!Physics.CheckSphere(tempSpawnPoint.position + Vector3.up * 0.5f, 1f))
                        {
                            position = tempSpawnPoint.position;
                            return true;
                        }
                        startIndex++;
                    }
                    return false;
                }
            }
        }

        private GameObjectsPool GetRandomPool(NpcSpawnerComp spawnerComp) =>
            spawnerComp.GameObjectsPools[Random.Range(0, spawnerComp.GameObjectsPools.Length)];
    }
}