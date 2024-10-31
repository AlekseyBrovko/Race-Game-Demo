using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class CarBrakeLightsSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarStartBrakeEvent>> _startFilter = default;
        private EcsFilterInject<Inc<CarStopBrakeEvent>> _stopFilter = default;
        private EcsPoolInject<CarComp> _carPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _startFilter.Value)
            {
                ref var carComp = ref _carPool.Value.Get(entity);
                foreach (var particle in carComp.CarMb.StopFlareParticles)
                    particle.Play();
            }

            foreach (var entity in _stopFilter.Value)
            {
                ref var carComp = ref _carPool.Value.Get(entity);
                foreach (var particle in carComp.CarMb.StopFlareParticles)
                    particle.Stop();
            }
        }
    }
}