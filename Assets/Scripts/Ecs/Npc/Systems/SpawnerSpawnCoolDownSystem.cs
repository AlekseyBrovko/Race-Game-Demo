using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public sealed class SpawnerSpawnCoolDownSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsFilterInject<Inc<SpawnerSpawnCoolDownComp, NpcSpawnerIsActiveComp>> _filter = default;

        private EcsPoolInject<SpawnerSpawnCoolDownComp> _spawnCoolDownPool = default;
        private EcsPoolInject<SpawnNpcEvent> _spawnEvent = default;

        private float _spawnCoolDownByDefault = 3f;
        private List<float> _tempTimers = new List<float>();

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var coolDownComp = ref _spawnCoolDownPool.Value.Get(entity);
                for (int i = 0; i < coolDownComp.Timers.Count; i++)
                {
                    coolDownComp.Timers[i] += Time.deltaTime;
                    if (coolDownComp.Timers[i] > _spawnCoolDownByDefault)
                    {
                        ref var spawnComp = ref _spawnEvent.Value.Add(_world.Value.NewEntity());
                        spawnComp.SpawnerEntity = entity;

                        _tempTimers.Add(coolDownComp.Timers[i]);
                    }
                }

                if (_tempTimers.Count > 0)
                {
                    foreach (var timer in _tempTimers)
                        coolDownComp.Timers.Remove(timer);

                    _tempTimers.Clear();
                }
            }
        }
    }
}