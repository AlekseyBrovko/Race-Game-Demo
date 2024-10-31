using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class TimeScaleSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<TimeScaleEvent>> _filter = default;
        private EcsPoolInject<TimeScaleEvent> _timeScalePool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                ref var timeScaleComp = ref _timeScalePool.Value.Get(entity);
                Time.timeScale = timeScaleComp.TimeScale;
                _timeScalePool.Value.Del(entity);
            }
        }
    }
}