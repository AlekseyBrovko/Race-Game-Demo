using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class CarDriftSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarDriftComp>> _filter = default;

        private EcsPoolInject<CarDriftComp> _driftPool = default;
        //private EcsPoolInject<CarMoveDirectionComp> _directionPool = default;
        //private EcsPoolInject<CarStatisticComp> _statisticPool = default;
        //private EcsPoolInject<CarControllComp> _controllPool = default;
        //private EcsPoolInject<CarFsmControllComp> _fsmPool = default;

        //private EcsPoolInject<CarWheelsControllComp> _wheelsPool = default;
        //private EcsPoolInject<CarComp> _carPool = default;
        //private EcsPoolInject<View> _viewPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var driftComp = ref _driftPool.Value.Get(entity);


            }
        }
    }
}