using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class NpcRangeSetRunSpeedSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<NpcSetRunSpeedEvent>> _filter = default;
        private EcsPoolInject<NpcSetRunSpeedEvent> _eventPool = default;
        private EcsPoolInject<NavmeshComp> _navmeshPool = default;
        private EcsPoolInject<NpcComp> _npcPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var npcComp = ref _npcPool.Value.Get(entity);
                ref var navmeshComp = ref _navmeshPool.Value.Get(entity);
                navmeshComp.Agent.speed = npcComp.NpcMb.RunSpeed;

                _eventPool.Value.Del(entity);
            }
        }
    }
}