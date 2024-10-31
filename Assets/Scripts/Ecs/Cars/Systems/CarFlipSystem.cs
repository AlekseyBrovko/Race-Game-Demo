using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    sealed class CarFlipSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<CarFlipEvent>, Exc<CarFlipCoolDownComp>> _filter = default;
        private EcsPoolInject<CarFlipEvent> _flipPool = default;
        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<RespawnPointsComp> _respawnPool = default;
        private EcsPoolInject<PlayerCarZoneComp> _playerCarZoneComp = default;
        private EcsPoolInject<CarFlipCoolDownComp> _coolDownPool = default;

        private float _coolDown = 3f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                int carEntity = _state.Value.PlayerCarEntity;
                ref var carComp = ref _carPool.Value.Get(carEntity);

                var nearestPoint = GetNearestRespawnPoint(carEntity, carComp);

                foreach (var wheel in carComp.Wheels)
                {
                    wheel.WheelController.enabled = false;

                    //TODO не работает
                    //wheel.EffectsMb.SkidmarksTrail.emitting = false;
                    //wheel.EffectsMb.SkidmarksTrail.enabled = false;
                    //wheel.EffectsMb.SkidmarksTrail.gameObject.SetActive(false);
                    //wheel.EffectsMb.SkidmarksTrail.forceRenderingOff = true;
                    //wheel.EffectsMb.SkidmarksTrail.time = 0;
                }   

                Rigidbody rb = carComp.CarMb.Rigidbody;
                rb.angularVelocity = Vector3.zero;
                rb.velocity = Vector3.zero;

                rb.position = nearestPoint.transform.position + Vector3.up;
                rb.rotation = nearestPoint.transform.rotation;

                ref var coolDownComp = ref _coolDownPool.Value.Add(entity);
                coolDownComp.Timer = _coolDown;

                foreach (var wheel in carComp.Wheels)
                {
                    wheel.WheelController.enabled = true;

                    //wheel.EffectsMb.SkidmarksTrail.enabled = true;
                    //wheel.EffectsMb.SkidmarksTrail.gameObject.SetActive(true);
                    //wheel.EffectsMb.SkidmarksTrail.forceRenderingOff = false;
                    //wheel.EffectsMb.SkidmarksTrail.time = 5;
                }   

                _flipPool.Value.Del(entity);
            }
        }

        private RespawnPointMb GetNearestRespawnPoint(int carEntity, CarComp carComp)
        {
            ref var respawnPointComp = ref _respawnPool.Value.Get(_state.Value.RespawnPointsEntity);
            ref var playerZoneComp = ref _playerCarZoneComp.Value.Get(carEntity);

            List<RespawnPointMb> respawnPoints = new List<RespawnPointMb>();
            foreach (var point in respawnPointComp.RespawnPoints)
            {
                if (point.IslandName == playerZoneComp.IslandName)
                    respawnPoints.Add(point);
            }

            RespawnPointMb nearestPoint = null;
            float distance = float.MaxValue;
            Vector3 carPosition = carComp.CarTransform.position;

            foreach (var point in respawnPoints)
            {
                Vector3 direction = point.transform.position - carPosition;
                float tempDistance = direction.sqrMagnitude;
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    nearestPoint = point;
                }
            }

            return nearestPoint;
        }
    }
}