using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    sealed class RespawnPointsInit : IEcsInitSystem
    {
        public RespawnPointsInit(SceneIniterMb sceneIniter) =>
            _sceneIniter = sceneIniter;

        private SceneIniterMb _sceneIniter;

        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<RespawnPointsComp> _respawnPool = default;

        public void Init(EcsSystems systems)
        {
            int respawnEntity = _world.Value.NewEntity();
            _state.Value.RespawnPointsEntity = respawnEntity;

            List<RespawnPointMb> respawnPoints = new List<RespawnPointMb>();
            foreach (Transform child in _sceneIniter.RespawnPoints)
                if (child.TryGetComponent(out RespawnPointMb respawnMb))
                    respawnPoints.Add(respawnMb);

            ref var respawnComp = ref _respawnPool.Value.Add(respawnEntity);
            respawnComp.RespawnPoints = respawnPoints.ToArray();
        }
    }
}