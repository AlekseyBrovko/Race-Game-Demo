using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    sealed class ObjectPoolInit : IEcsInitSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<ObjectsPoolComp> _objectsPool = default;

        public void Init(EcsSystems systems)
        {
            EnemyPrefabsConfig zombieConfig = _state.Value.EnemyPrefabsConfig;

            int objectsPoolEntity = _world.Value.NewEntity();
            _state.Value.ObjectsPoolEntity = objectsPoolEntity;

            ref var objectsPoolComp = ref _objectsPool.Value.Add(objectsPoolEntity);

            objectsPoolComp.EnemyPoolsDictionary = new Dictionary<string, GameObjectsPool>();
            foreach (var item in zombieConfig.Prefabs)
            {
                GameObjectsPool pool = new GameObjectsPool(_state.Value, item.Id, item.Id, 2, item.Prefab);
                objectsPoolComp.EnemyPoolsDictionary.Add(item.Id, pool);
            }

            objectsPoolComp.EnemyRagdollsDictionary = new Dictionary<string, GameObjectsPool>();
            foreach (var item in zombieConfig.RagdollPrefabs)
            {
                GameObjectsPool pool = new GameObjectsPool(_state.Value, item.Id, item.Id, 2, item.Prefab);
                objectsPoolComp.EnemyRagdollsDictionary.Add(item.Id, pool);
            }

            objectsPoolComp.ThrowingObjectsDictionary = new Dictionary<string, GameObjectsPool>();
            foreach (var item in zombieConfig.ThrowingObjectsPrefabs)
            {
                GameObjectsPool pool = new GameObjectsPool(_state.Value, item.Id, item.Id, 2, item.Prefab);
                objectsPoolComp.ThrowingObjectsDictionary.Add(item.Id, pool);
            }
        }
    }
}