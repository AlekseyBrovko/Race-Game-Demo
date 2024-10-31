using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class ContinueTutorialSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<ContinueTutorialEvent>> _filter = default;
        private EcsPoolInject<ContinueTutorialEvent> _continuePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {

                _continuePool.Value.Del(entity);
            }
        }
    }
}