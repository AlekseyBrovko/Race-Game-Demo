using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class ResetPhysicOnObjectsSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarComp>> _carFilter = default;
        private EcsFilterInject<Inc<PhysicalObjectComp>> _objectFilter = default;

        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<PhysicalObjectComp> _objectPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _carFilter.Value)
            {
                ref var carComp = ref _carPool.Value.Get(entity);
                carComp.CarMb.ResetCollisionsTemp();
            }

            foreach (var entity in _objectFilter.Value)
            {
                ref var objectComp = ref _objectPool.Value.Get(entity);
                objectComp.PhysicalMb.ResetCollisionsTemp();
            }
        }
    }
}