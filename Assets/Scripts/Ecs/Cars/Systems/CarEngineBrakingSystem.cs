using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class CarEngineBrakingSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarComp>> _filter = default;
        private EcsPoolInject<CarControllComp> _controllPool = default;
        private EcsPoolInject<EngineBrakingComp> _brakingPool = default;
        private EcsPoolInject<StopEngineBrakingEvent> _stopBrakeEventPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var controllComp = ref _controllPool.Value.Get(entity);
                if (controllComp.VerticalInput == 0 && !_brakingPool.Value.Has(entity))
                    _brakingPool.Value.Add(entity);

                if (controllComp.VerticalInput != 0 && _brakingPool.Value.Has(entity))
                {
                    _stopBrakeEventPool.Value.Add(entity);
                    _brakingPool.Value.Del(entity);
                }
            }
        }
    }
}