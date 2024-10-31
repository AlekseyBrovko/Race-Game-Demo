using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    sealed class MissionSpawnPointsInit : IEcsInitSystem
    {
        public MissionSpawnPointsInit(SceneIniterMb sceneIniter) =>
            _sceneIniter = sceneIniter;

        private SceneIniterMb _sceneIniter;

        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<MissionSpawnPointsComp> _missionSpawnPointsPool = default;

        public void Init(EcsSystems systems)
        {
            ref var missionSpawnComp = ref _missionSpawnPointsPool.Value.Add(_state.Value.RespawnPointsEntity);
            missionSpawnComp.MissionSpawnPoints = new Dictionary<string, Transform>();

            foreach (Transform child in _sceneIniter.MissionSpawnPoints)
            {
                MissionSpawnPointMb spawnPointMb = child.GetComponent<MissionSpawnPointMb>();
                missionSpawnComp.MissionSpawnPoints.Add(spawnPointMb.MissionSpawnPointId, spawnPointMb.transform);
            }
            Debug.Log("MissionSpawnPointsInit missionSpawnComp.MissionSpawnPoints.Count = " + missionSpawnComp.MissionSpawnPoints.Count);
        }
    }
}