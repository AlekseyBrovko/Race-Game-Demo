using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    sealed class PointsOfInterestInit : IEcsInitSystem
    {
        public PointsOfInterestInit(SceneIniterMb sceneIniter)
        {
            _sceneIniter = sceneIniter;
        }

        private SceneIniterMb _sceneIniter;

        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<PointsOfInterestComp> _pointsPool = default;

        public void Init(EcsSystems systems)
        {
            int pointsEntity = _world.Value.NewEntity();
            _state.Value.PointsOfInterestEntity = pointsEntity;


            ref var pointsComp = ref _pointsPool.Value.Add(pointsEntity);

            pointsComp.NamesPosPointsDictionary = new Dictionary<string, Vector3>();
            foreach (Transform point in _sceneIniter.PointsOfInterestHolder.transform)
            {
                PointOfInterestMb pointMb = point.GetComponent<PointOfInterestMb>();
                pointsComp.NamesPosPointsDictionary.Add(pointMb.PointName, point.position);
            }

            pointsComp.NamesPosZonesDictionary = new Dictionary<string, Vector3>();
            foreach (CityZoneMb zoneMb in _sceneIniter.SpawnersHolder.CityZones)
                pointsComp.NamesPosZonesDictionary.Add(zoneMb.ZoneName, zoneMb.transform.position);
        }
    }
}