using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.VisualScripting;

namespace Client
{
    sealed class PhysicalObjectSpeedSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PhysicalObjectIsFlyingComp>> _filter = default;
        private EcsPoolInject<PhysicalObjectIsFlyingComp> _flyPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var flyComp = ref _flyPool.Value.Get(entity);
                float speedMpS = flyComp.Rigidbody.velocity.magnitude;
                flyComp.PreviousFrameSpeedKmH = flyComp.SpeedKmH;
                flyComp.SpeedKmH = speedMpS * 3.6f;
            }
        }
    }
}