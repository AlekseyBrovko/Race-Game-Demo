using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarColliderPositionFromSpeedSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarColliderComp>> _filter = default;
        private EcsPoolInject<CarColliderComp> _colliderPool = default;
        private EcsPoolInject<CarStatisticComp> _statisticPool = default;
        private EcsPoolInject<CarMoveDirectionComp> _directionPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;

        private float _maxSpeed = 200f;
        private float _maxEncreaseOfCollider = 2f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var colliderComp = ref _colliderPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var directionComp = ref _directionPool.Value.Get(entity);
                ref var viewComp = ref _viewPool.Value.Get(entity);

                float speed = statisticComp.SpeedKmpH;
                float indexOfSpeed = speed / _maxSpeed;
                float index = _maxEncreaseOfCollider * indexOfSpeed;

                Vector3 direction = viewComp.Transform.InverseTransformDirection(directionComp.DirectionByRb);
                Vector3 newPosition = colliderComp.ColliderStartPos + direction * index;
                colliderComp.MainTriggerCollider.center = newPosition;
            }
        }
    }
}