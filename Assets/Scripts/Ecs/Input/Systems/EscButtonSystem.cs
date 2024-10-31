using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{   
    sealed class EscButtonSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<EscButtonEvent>> _filter = default;
        private EcsPoolInject<PauseButtonEvent> _pauseButtonPool = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                if (_state.Value.PlaySystems)
                {
                    _pauseButtonPool.Value.Add(_world.Value.NewEntity());
                }
                else
                {
                    ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                    interfaceComp.CanvasBehaviour.OnBackButtonClick();
                }
            }
        }
    }
}