using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class NpcRagdollInitSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<RagdollInitEvent>> _filter = default;
        private EcsPoolInject<RagdollInitEvent> _initPool = default;
        private EcsPoolInject<RagdollComp> _ragdollPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var initComp = ref _initPool.Value.Get(entity);
                ref var ragdollComp = ref _ragdollPool.Value.Add(entity);
                ragdollComp.RagdollMb = initComp.RagdollMb;
                ragdollComp.Transform = ragdollComp.RagdollMb.transform;
                ragdollComp.MainRigidbody = ragdollComp.RagdollMb.MainRigidbody;

                _initPool.Value.Del(entity);
            }
        }
    }
}