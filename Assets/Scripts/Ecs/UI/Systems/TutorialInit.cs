using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using static Enums;

namespace Client
{
    sealed class TutorialInit : IEcsInitSystem
    {
        public TutorialInit(SceneType sceneType) =>
            _sceneType = sceneType;

        private SceneType _sceneType;
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<ShowShopTutorialComp> _showShopTutorialPool = default;
        private EcsPoolInject<ShowInGameTutorialComp> _showInGameTutorialPool = default;

        public void Init(EcsSystems systems)
        {
            if (_sceneType == SceneType.LobbyScene && _state.Value.ShopTutorial)
                _showShopTutorialPool.Value.Add(_world.Value.NewEntity());

            if (_sceneType == SceneType.PlayScene && _state.Value.InGameTutorial)
                _showInGameTutorialPool.Value.Add(_world.Value.NewEntity());
        }
    }
}