using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitInterface : IEcsInitSystem
    {
        public InitInterface(bool isTestScene)
        {
            _showCanvas = isTestScene;
        }

        private bool _showCanvas;
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        public void Init(EcsSystems systems)
        {
            var interfaceEntity = _world.Value.NewEntity();
            _state.Value.InterfaceEntity = interfaceEntity;
            ref var interfaceComp = ref _interfacePool.Value.Add(interfaceEntity);

            CanvasBehaviour canvasMB = GameObject.Instantiate(_state.Value.UiConfig.CanvasBehaviourPrefab);

            interfaceComp.CanvasBehaviour = canvasMB;
            interfaceComp.CanvasBehaviour.Init(_state.Value);

            if (_showCanvas)
            {
                GameObject.Instantiate(_state.Value.UiConfig.FpsCanvas);
            }

            if (_state.Value.SceneType == Enums.SceneType.StartScene)
                GameObject.Instantiate(_state.Value.UiConfig.LoadingCanvas);
        }
    }
}