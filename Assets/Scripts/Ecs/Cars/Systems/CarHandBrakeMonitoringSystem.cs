using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class CarHandBrakeMonitoringSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarControllComp>, Exc<CarIsFlyComp>> _filter = default;
        private EcsPoolInject<CarControllComp> _controllPool = default;
        private EcsPoolInject<HandBrakeComp> _brakePool = default;
        private EcsPoolInject<StartHandBrakeEvent> _startBrakeEvent = default;
        private EcsPoolInject<EndHandBrakeEvent> _endBrakeEvent = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var controllComp = ref _controllPool.Value.Get(entity);
                if (controllComp.IsHandBrake && !_brakePool.Value.Has(entity))
                {
                    _brakePool.Value.Add(entity);
                    _startBrakeEvent.Value.Add(entity);
                }

                if (!controllComp.IsHandBrake && _brakePool.Value.Has(entity))
                {
                    _brakePool.Value.Del(entity);
                    _endBrakeEvent.Value.Add(entity);
                }
            }
        }
    }
}