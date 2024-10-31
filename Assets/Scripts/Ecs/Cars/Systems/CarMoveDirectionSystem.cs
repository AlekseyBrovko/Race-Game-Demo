using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarMoveDirectionSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarMoveDirectionComp>> _filter = default;
        private EcsPoolInject<CarMoveDirectionComp> _directionPool = default;
        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var viewComp = ref _viewPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);
                ref var directionComp = ref _directionPool.Value.Get(entity);

                Vector3 rbDirection = carComp.Rigidbody.velocity.normalized;
                Vector3 carForwardDirection = viewComp.Transform.forward;
                Vector3 carRightDirection = viewComp.Transform.right;

                //directionComp.DirectionByRb = new Vector3(rbDirection.x, 0, rbDirection.z).normalized;
                directionComp.DirectionByRb = rbDirection;
                directionComp.AngleBetweenRbAndForwardDir = Vector3.Angle(rbDirection, carForwardDirection);
                directionComp.DotProductBetweenRbAndForwardDir = Vector3.Dot(rbDirection, carRightDirection);
            }
        }
    }
}