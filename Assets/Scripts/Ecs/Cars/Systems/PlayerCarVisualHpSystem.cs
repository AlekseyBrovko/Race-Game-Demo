using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class PlayerCarVisualHpSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<VisualHpEvent>> _hpFilter = default;
        //private EcsFilterInject<Inc<HealthPanelComp>> _uiFilter = default;
        private EcsFilterInject<Inc<InGamePanelComp>> _uiFilter = default;

        private EcsPoolInject<VisualHpEvent> _eventPool = default;
        //private EcsPoolInject<HealthPanelComp> _panelPool = default;
        private EcsPoolInject<InGamePanelComp> _panelPool = default;
        private EcsPoolInject<HpComp> _hpPool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _hpFilter.Value)
            {
                foreach(var uiEntity in _uiFilter.Value)
                {
                    ref var hpComp = ref _hpPool.Value.Get(_state.Value.PlayerCarEntity);
                    ref var panelComp = ref _panelPool.Value.Get(uiEntity);
                    panelComp.InGamePanelMb.ShowHealth(hpComp.HpValue / hpComp.FullHpValue, true);
                }
                _eventPool.Value.Del(entity);
            }
        }
    }
}