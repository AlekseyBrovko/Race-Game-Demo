using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PlayerSpawnPointsInit : IEcsInitSystem
    {
        public PlayerSpawnPointsInit(bool isTestScene, SceneIniterMb sceneIniter = null)
        {
            _isTestScene = isTestScene;
            _sceneIniter = sceneIniter;
        }

        private SceneIniterMb _sceneIniter;
        private bool _isTestScene;

        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<PlayerSpawnPointsComp> _spawnPointsPool = default;

        public void Init(EcsSystems systems)
        {
            if (!_isTestScene)
            {
                int entity = _world.Value.NewEntity();
                _state.Value.PlayerSpawnPointsEntity = entity;

                GameObject point = default;
                if (_sceneIniter == null)
                    point = Object.FindObjectOfType<PlayerSpawnPointMb>().gameObject;
                else
                    point = _sceneIniter.PlayerSpawnPointMb.gameObject;

                if (point != null)
                {
                    ref var spawnPointsComp = ref _spawnPointsPool.Value.Add(entity);
                    spawnPointsComp.StartSpawnTransform = point.transform;
                }
            }
        }
    }
}