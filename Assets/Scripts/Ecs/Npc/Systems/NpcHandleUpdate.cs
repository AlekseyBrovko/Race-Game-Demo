using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class NpcHandleUpdate : IEcsRunSystem
    {
        private EcsFilterInject<Inc<NpcComp>, Exc<NpcInactiveComp>> _filter = default;
        private EcsPoolInject<NpcComp> _npcPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var npcComp = ref _npcPool.Value.Get(entity);
                npcComp.NpcMb.Update();
            }
        }
    }
}