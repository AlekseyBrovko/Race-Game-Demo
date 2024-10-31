using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class TestLogCatchSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<TestLogEvent>> _filter = default;
        private EcsPoolInject<TestLogEvent> _logPool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                Debug.Log("тестовый лог");
                _logPool.Value.Del(entity);
            }
        }
    }
}