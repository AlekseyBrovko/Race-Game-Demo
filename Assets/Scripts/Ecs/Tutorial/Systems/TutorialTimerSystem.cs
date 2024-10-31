using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class TutorialTimerSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<TutorialTimerComp>> _filter = default;
        private EcsPoolInject<TutorialTimerComp> _timerPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var timerComp = ref _timerPool.Value.Get(entity);
                timerComp.Timer -= Time.deltaTime;
                if (timerComp.Timer < 0)
                    _timerPool.Value.Del(entity);
            }
        }
    }
}