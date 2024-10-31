using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class CarBackDriveLightsSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<StartBackDriveEvent>> _startFilter = default;
        private EcsFilterInject<Inc<StopBackDriveEvent>> _stopFilter = default;
        private EcsPoolInject<CarComp> _carPool = default;
            
        public void Run(EcsSystems systems)
        {
            foreach(var entity in _startFilter.Value)
            {
                ref var carComp = ref _carPool.Value.Get(entity);
                foreach(var particle in carComp.CarMb.BackDriveFlareParticles)
                    particle.Play();
            }

            foreach(var entity in _stopFilter.Value)
            {
                ref var carComp = ref _carPool.Value.Get(entity);
                foreach (var particle in carComp.CarMb.BackDriveFlareParticles)
                    particle.Stop();
            }
        }
    }
}