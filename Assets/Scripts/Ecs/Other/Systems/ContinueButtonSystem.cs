using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class ContinueButtonSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<ContinueButtonEvent>> _filter = default;
        private EcsPoolInject<ContinueButtonEvent> _eventPool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                _state.Value.StartPlaySystems();

                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.InGamePanelId);

                _eventPool.Value.Del(entity);
            }
        }
    }
}