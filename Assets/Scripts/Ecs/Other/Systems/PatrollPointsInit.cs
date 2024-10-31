using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System.Collections.Generic;

namespace Client
{
    sealed class PatrollPointsInit : IEcsInitSystem
    {
        public PatrollPointsInit(SceneIniterMb sceneIniter = null)
        {
            _sceneIniter = sceneIniter;
        }

        private SceneIniterMb _sceneIniter;

        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<PatrollPointsComp> _pointsPool = default;

        //TODO подумать, может и нафик не нужна эта система
        public void Init(EcsSystems systems)
        {
            int patrollPointsEntity = _world.Value.NewEntity();
            _state.Value.PatrollPointsEntity = patrollPointsEntity;
            ref var patrollPointsComp = ref _pointsPool.Value.Add(patrollPointsEntity);
            patrollPointsComp.PatrollPoints = new List<Transform>();

            if (_sceneIniter == null)
            {
                PatrollPointMb[] points = Object.FindObjectsOfType<PatrollPointMb>();
                foreach (var point in points)
                    patrollPointsComp.PatrollPoints.Add(point.gameObject.transform);
            }
            else
            {
                GameObject pointsHolder = _sceneIniter.PatrollPointsHolder.gameObject;
                foreach (Transform child in pointsHolder.transform)
                    patrollPointsComp.PatrollPoints.Add(child);
            }
        }
    }
}