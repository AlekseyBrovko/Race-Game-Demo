using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class CarMotorSoundSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarTahometerComp>> _filter = default;
        private EcsPoolInject<CarTahometerComp> _tahoPool = default;
        private EcsPoolInject<CarSoundComp> _soundPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var tahoComp = ref _tahoPool.Value.Get(entity);
                ref var soundComp = ref _soundPool.Value.Get(entity);
                soundComp.EventInstance.setParameterByName("RPM", tahoComp.CurrentValueRpm);
            }
        }
    }
}