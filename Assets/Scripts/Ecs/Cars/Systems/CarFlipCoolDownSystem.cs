using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarFlipCoolDownSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarFlipCoolDownComp>> _flipFilter = default;
        private EcsPoolInject<CarFlipCoolDownComp> _flipPool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _flipFilter.Value)
            {
                ref var coolDownComp = ref _flipPool.Value.Get(entity);
                coolDownComp.Timer -= Time.fixedDeltaTime;

                if (coolDownComp.Timer < 0)
                    _flipPool.Value.Del(entity);
            }
        }
    }
}