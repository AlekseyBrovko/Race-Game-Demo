using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PlayerCarFlyTimer : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarIsFlyComp, PlayerCarComp>> _filter = default;
        private EcsPoolInject<CarIsFlyComp> _flyPool = default;

        private float _thresholdBeforeHelp = 5f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var flyComp = ref _flyPool.Value.Get(entity);
                flyComp.Timer += Time.fixedDeltaTime;
                if (flyComp.Timer > _thresholdBeforeHelp)
                {
                    Debug.Log("помогайте");
                }
            }    
        }
    }
}