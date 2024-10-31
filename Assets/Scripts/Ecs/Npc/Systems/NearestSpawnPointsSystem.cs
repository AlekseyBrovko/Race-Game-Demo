using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Linq;
using UnityEngine;

namespace Client
{
    sealed class NearestSpawnPointsSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<SpawnersHolderComp> _spawnersPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;

        private EcsPoolInject<SpawnNpcFullPointsEvent> _spawnFullPointsPool = default;
        private EcsPoolInject<NpcSpawnerIsActiveComp> _spawnerIsActivePool = default;
        private EcsPoolInject<DeactivateSpawnerEvent> _deactivatePool = default;

        private const int _activeSpawnCount = 15;
        private Transform[] _tempActiveSpawners = new Transform[_activeSpawnCount];
        private SpawnerMb[] _spawnerMBs = new SpawnerMb[_activeSpawnCount];

        private float _duration = 1f;
        private float _timer = 0f;

        public void Run(EcsSystems systems)
        {
            //TODO подумать как лучше
            if (!_state.Value.StartGameTutorial)
            {
                _timer -= Time.deltaTime;
                if (_timer < 0)
                {
                    _timer = _duration;
                    HandleNearestSpawnPoints();
                }
            }
        }

        private void HandleNearestSpawnPoints()
        {
            ref var spawnersComp = ref _spawnersPool.Value.Get(_state.Value.EnemySpawnersEntity);
            ref var viewComp = ref _viewPool.Value.Get(_state.Value.PlayerCarEntity);
            Vector3 playerPos = viewComp.Transform.position;

            //TODO нужно делать сортировку бинарным деревом скорее всего, но это не точно

            foreach (SpawnerPointCalculation data in spawnersComp.SpawnersCalculations)
            {
                Vector3 direction = data.Spawner.position - playerPos;
                data.SqrDistance = direction.sqrMagnitude;
            }

            float previousDistance = 0;
            for (int i = 0; i < _activeSpawnCount; i++)
            {
                float tempDistance = float.MaxValue;
                
                foreach (var spawner in spawnersComp.SpawnersCalculations)
                {
                    if (i == 0)
                    {
                        if (spawner.SqrDistance < tempDistance)
                        {
                            tempDistance = spawner.SqrDistance;
                            _tempActiveSpawners[i] = spawner.Spawner;
                        }
                    }
                    else
                    {
                        if (spawner.SqrDistance < tempDistance && spawner.SqrDistance > previousDistance)
                        {
                            tempDistance = spawner.SqrDistance;
                            _tempActiveSpawners[i] = spawner.Spawner;
                        }
                    }
                }
                previousDistance = tempDistance;
            }

            for (int i = 0; i < _tempActiveSpawners.Length; i++)
            {
                _spawnerMBs[i] = spawnersComp.Spawners[_tempActiveSpawners[i]];

                if (!_spawnerIsActivePool.Value.Has(_spawnerMBs[i].Entity))
                {
                    _spawnerIsActivePool.Value.Add(_spawnerMBs[i].Entity);
                    _spawnFullPointsPool.Value.Add(_spawnerMBs[i].Entity);
                }

                //_spawnerMBs[i].ActivateSpawnPoint();
                //TODO работает, добить
                //for log
                //float distance = Vector3.SqrMagnitude(_spawnerMBs[i].transform.position - playerPos);
                //Debug.Log("distance = " + distance);
            }

            foreach (SpawnerMb spawner in spawnersComp.ActiveSpawners)
            {
                if (!_spawnerMBs.Contains(spawner))
                {
                    //spawner.DeactivateSpawnPoint();
                    if (_spawnerIsActivePool.Value.Has(spawner.Entity))
                    {
                        _spawnerIsActivePool.Value.Del(spawner.Entity);
                        _deactivatePool.Value.Add(spawner.Entity);
                    }
                }
            }

            spawnersComp.ActiveSpawners = _spawnerMBs.ToList();
        }
    }
}