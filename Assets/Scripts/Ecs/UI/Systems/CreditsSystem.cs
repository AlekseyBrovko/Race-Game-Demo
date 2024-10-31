using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CreditsSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<CreditsEvent>> _creditsFilter = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;
        private EcsFilterInject<Inc<TouchComponent>> _touchFilter = default;

        private bool _isActive = false;

        public void Run(EcsSystems systems)
        {
            ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
            foreach (var entity in _creditsFilter.Value)
            {
                _isActive = true;
                //interfaceComp.SettingsPanelBehaviour.OpenFromSystem();
                _creditsFilter.Pools.Inc1.Del(entity);
            }

            if (_isActive)
            {
                foreach (var entity in _touchFilter.Value)
                {
                    ref var touchComp = ref _touchFilter.Pools.Inc1.Get(entity);
                    if (touchComp.Phase == TouchPhase.Began)
                    {
                        //interfaceComp.SettingsPanelBehaviour.CloseCredits();
                        _isActive = false;
                    }
                }
            }
        }
    }
}