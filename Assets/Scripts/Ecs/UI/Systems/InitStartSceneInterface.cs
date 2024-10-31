using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class InitStartSceneInterface : IEcsInitSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        public void Init(EcsSystems systems)
        {
            int interfaceEntity = _state.Value.InterfaceEntity;
            ref var interfaceComp = ref _interfacePool.Value.Get(interfaceEntity);
            interfaceComp.CanvasBehaviour.ShowStartSceneStartPanel();
        }
    }
}