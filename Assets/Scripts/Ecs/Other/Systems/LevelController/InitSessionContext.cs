using Leopotam.EcsLite;
using UnityEngine;

namespace Client
{
    sealed class InitSessionContext : IEcsInitSystem
    {
        public InitSessionContext(SessionContextHolder contextPrefab) =>
            _contextPrefab = contextPrefab;

        private SessionContextHolder _contextPrefab;

        public void Init(EcsSystems systems)
        {
            if (SessionContextHolder.Instance == null)
            {
                var contextHolder = GameObject.Instantiate(_contextPrefab);
                contextHolder.Init();
            }
        }
    }
}