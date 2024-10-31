using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    public sealed class DeactivateNpcSpawnerSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<DeactivateSpawnerEvent>> _filter = default;

        private EcsPoolInject<DeactivateSpawnerEvent> _stopCoolDownPool = default;
        private EcsPoolInject<SpawnerSpawnCoolDownComp> _coolDownPool = default;
        private EcsPoolInject<NpcSpawnerComp> _spawnerPool = default;
        private EcsPoolInject<NpcStartInObjectPoolSystemsEvent> _goToPoolEvent = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                WorkWithCoolDowns(entity);
                WorkWithSpawnedEntities(entity);

                _stopCoolDownPool.Value.Del(entity);
            }
        }

        private void WorkWithCoolDowns(int entity)
        {
            ref var coolDownComp = ref _coolDownPool.Value.Get(entity);
            coolDownComp.Timers.Clear();
        }

        private void WorkWithSpawnedEntities(int entity)
        {
            ref var spawnerComp = ref _spawnerPool.Value.Get(entity);
            foreach (ISpawned spawned in spawnerComp.SpawnedMbs)
            {
                spawned.ReturnObjectInPool();
                _goToPoolEvent.Value.Add(spawned.Entity);
            }
            spawnerComp.SpawnedMbs.Clear();
        }
    }
}