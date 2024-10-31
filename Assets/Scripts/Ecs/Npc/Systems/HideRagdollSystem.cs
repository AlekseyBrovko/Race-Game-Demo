using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class HideRagdollSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<NpcHideRagdollEvent>> _filter = default;
        private EcsPoolInject<NpcHideRagdollEvent> _hidePool = default;
        private EcsPoolInject<RagdollComp> _ragdollPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var ragdollComp = ref _ragdollPool.Value.Get(entity);
                ragdollComp.RagdollMb.ReturnObjectInPool();

                _hidePool.Value.Del(entity);
            }
        }
    }
}