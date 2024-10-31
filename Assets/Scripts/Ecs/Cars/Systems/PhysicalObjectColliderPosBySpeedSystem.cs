using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PhysicalObjectColliderPosBySpeedSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PhysicalObjectIsFlyingComp>> _filter = default;
        private EcsPoolInject<PhysicalObjectIsFlyingComp> _flyPool = default;

        private float _maxSpeed = 200f;
        private float _maxEncreaseOfCollider = 2f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var flyComp = ref _flyPool.Value.Get(entity);
                float speed = flyComp.SpeedKmH;
                float indexOfSpeed = speed / _maxSpeed;
                float index = _maxEncreaseOfCollider * indexOfSpeed;
                Vector3 directionOfRb = flyComp.Rigidbody.velocity.normalized;

                Vector3 direction = flyComp.ObjectTransform.InverseTransformDirection(directionOfRb);
                Vector3 newPosition = flyComp.ColliderStartPos + direction * index;
                flyComp.TriggerCollider.center = newPosition;
            }
        }
    }
}