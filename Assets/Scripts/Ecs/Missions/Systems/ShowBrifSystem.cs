using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class ShowBrifSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<ShowMissionBrifEvent>> _showBrifFilter = default;
        private EcsPoolInject<ShowMissionBrifEvent> _showBrifPool = default;

        private EcsFilterInject<Inc<ContinueAfterBrifEvent>> _continueFilter = default;
        private EcsPoolInject<ContinueAfterBrifEvent> _continuePool = default;

        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        public void Run(EcsSystems systems)
        {
            HandleShowBrif();
            HandleContinue();
        }

        private void HandleShowBrif()
        {
            foreach (var entity in _showBrifFilter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                var brifData = _state.Value.CurrentMission.Brif;

                interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.BrifPanelId, brifData);
                _state.Value.StartPauseSystems();

                _showBrifPool.Value.Del(entity);
            }
        }

        private void HandleContinue()
        {
            foreach (var entity in _continueFilter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.InGamePanelId);
                _state.Value.StartPlaySystems();

                _continuePool.Value.Del(entity);
            }
        }
    }
}