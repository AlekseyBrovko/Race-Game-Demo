using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PhysicalObjectsCheckFlySystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PhysicalObjectIsFlyingComp>> _filter = default;
        private EcsPoolInject<PhysicalObjectIsFlyingComp> _flyingPool = default;
        private EcsPoolInject<PhysicalObjectComp> _physicalObjectPool = default;

        private float _defaultDurationOfCheck = 1f;
        private float _speedThreshold = 1f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var flyComp = ref _flyingPool.Value.Get(entity);
                flyComp.Timer += Time.fixedDeltaTime;
                if (flyComp.Timer > _defaultDurationOfCheck)
                {
                    flyComp.Timer = 0;
                    if (flyComp.SpeedKmH < _speedThreshold)
                    {
                        ref var physicalObjectComp = ref _physicalObjectPool.Value.Get(entity);
                        physicalObjectComp.PhysicalMb.OnEndPhysical();
                        _flyingPool.Value.Del(entity);
                    }
                }
            }
        }
    }
}