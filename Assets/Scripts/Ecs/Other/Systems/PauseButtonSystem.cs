using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class PauseButtonSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<PauseButtonEvent>> _filter = default;
        private EcsPoolInject<PauseButtonEvent> _eventPool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.PausePanelId);

                _state.Value.StartPauseSystems();
                _eventPool.Value.Del(entity);
            }
        }
    }
}