using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    sealed class SpawnersInit : IEcsInitSystem
    {
        public SpawnersInit(SceneIniterMb sceneIniter = null) =>
            _sceneIniter = sceneIniter;

        private SceneIniterMb _sceneIniter;

        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsPoolInject<NpcSpawnerComp> _spawnPointPool = default;
        private EcsPoolInject<ObjectsPoolComp> _objectsPool = default;
        private EcsPoolInject<SpawnersHolderComp> _spawnersPool = default;
        private EcsPoolInject<SpawnerSpawnCoolDownComp> _coolDownPool = default;

        private List<GameObjectsPool> _tempPools = new List<GameObjectsPool>();
        private List<Transform> _tempPoints = new List<Transform>();

        public void Init(EcsSystems systems)
        {
            SpawnerMb[] points = default;
            if (_sceneIniter == null)
            {
                points = Object.FindObjectsOfType<SpawnerMb>();
            }
            else
            {
                SpawnersHolder holder = _sceneIniter.SpawnersHolder;
                points = holder.Spawners;
            }

            int spawnersEntity = _world.Value.NewEntity();
            _state.Value.EnemySpawnersEntity = spawnersEntity;

            ref var spawnersComp = ref _spawnersPool.Value.Add(spawnersEntity);
            spawnersComp.ActiveSpawners = new List<SpawnerMb>();
            spawnersComp.Spawners = new Dictionary<Transform, SpawnerMb>();

            List<SpawnerPointCalculation> tempTransformsSpawners = new List<SpawnerPointCalculation>();
            ref var objectsPoolsComp = ref _objectsPool.Value.Get(_state.Value.ObjectsPoolEntity);

            foreach (SpawnerMb pointMb in points)
            {
                int spawnEntity = _world.Value.NewEntity();
                ref var spawnPointComp = ref _spawnPointPool.Value.Add(spawnEntity);
                spawnPointComp.SpawnPointMb = pointMb;
                spawnPointComp.Spawner = pointMb.transform;
                spawnPointComp.ZoneName = pointMb.ZoneName;
                spawnPointComp.SpawnedMbs = new List<ISpawned>();

                ref var coolDownComp = ref _coolDownPool.Value.Add(spawnEntity);
                coolDownComp.Timers = new List<float>();

                InitPoolsForSpawner(ref spawnPointComp, ref objectsPoolsComp);
                InitSpawnPointsForSpawner(ref spawnPointComp);

                //для системы просчета удаленности спаунера от игрока
                spawnersComp.Spawners.Add(pointMb.transform, pointMb);
                tempTransformsSpawners.Add(new SpawnerPointCalculation(pointMb.transform));

                pointMb.Init(spawnEntity);
            }

            spawnersComp.SpawnersCalculations = tempTransformsSpawners;
        }

        private void InitPoolsForSpawner(ref NpcSpawnerComp spawnPointComp, ref ObjectsPoolComp objectsPoolsComp)
        {
            foreach (string npcName in spawnPointComp.SpawnPointMb.ObjectsPoolNames)
            {
                if (!objectsPoolsComp.EnemyPoolsDictionary.ContainsKey(npcName))
                {
                    Debug.Log($"Нет по имени - {npcName} - пула объектов");
                    continue;
                }
                _tempPools.Add(objectsPoolsComp.EnemyPoolsDictionary[npcName]);
            }
            spawnPointComp.GameObjectsPools = _tempPools.ToArray();
            _tempPools.Clear();
        }

        private void InitSpawnPointsForSpawner(ref NpcSpawnerComp spawnPointComp)
        {
            foreach (Transform child in spawnPointComp.SpawnPointMb.transform)
                _tempPoints.Add(child);

            spawnPointComp.SpawnPoints = _tempPoints.ToArray();
            _tempPoints.Clear();
        }
    }
}