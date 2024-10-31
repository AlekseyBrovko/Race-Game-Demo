using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class InitLobbySceneInterface : IEcsInitSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;
        private EcsPoolInject<TimeScaleEvent> _timeScalePool = default;

        public void Init(EcsSystems systems)
        {
            int interfaceEntity = _state.Value.InterfaceEntity;
            ref var interfaceComp = ref _interfacePool.Value.Get(interfaceEntity);
            interfaceComp.CanvasBehaviour.ShowLobbySceneStartPanel();

            ref var timeScaleComp = ref _timeScalePool.Value.Add(_world.Value.NewEntity());
            timeScaleComp.TimeScale = 1f;
        }
    }
}