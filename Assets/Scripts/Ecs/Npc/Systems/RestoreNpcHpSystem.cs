using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class RestoreNpcHpSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<NpcComp, RestoreFullHpEvent>> _filter = default;
        private EcsPoolInject<RestoreFullHpEvent> _restorePool = default;
        private EcsPoolInject<HpComp> _hpPool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                ref var hpComp = ref _hpPool.Value.Get(entity);
                hpComp.HpValue = hpComp.FullHpValue;

                _restorePool.Value.Del(entity);
            }
        }
    }
}