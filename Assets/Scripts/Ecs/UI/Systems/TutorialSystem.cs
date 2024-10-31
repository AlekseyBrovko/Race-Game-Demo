using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class TutorialSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<ShowShopTutorialComp>> _shopTutorialFilter = default;
        private EcsFilterInject<Inc<ShowInGameTutorialComp>> _inGameTutorialFilter = default;

        private EcsPoolInject<ShowShopTutorialComp> _shopTutorialPool = default;
        private EcsPoolInject<ShowInGameTutorialComp> _inGameTutorialPool = default;

        private EcsPoolInject<InterfaceComp> _interfacePool = default;

        private float _timer = 0f;
        private float _durationToShowInGameTutorial = 1f;
        private float _durationToShowShowTutorial = 1f;

        public void Run(EcsSystems systems)
        {
            HandleShopTutorial();
            HandleInGameTutorial();
        }

        private void HandleShopTutorial()
        {
            foreach (var entity in _shopTutorialFilter.Value)
            {
                _timer += Time.deltaTime;
                if (_timer > _durationToShowShowTutorial)
                {
                    ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                    interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.TutorialShopPanelId);
                    _state.Value.ShopTutorial = false;
                    _state.Value.SaveTutorial();
                    _shopTutorialPool.Value.Del(entity);
                }
            }
        }

        private void HandleInGameTutorial()
        {
            foreach (var entity in _inGameTutorialFilter.Value)
            {
                _timer += Time.deltaTime;
                if (_timer > _durationToShowInGameTutorial)
                {
                    ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                    if (_state.Value.IsMobilePlatform)
                        interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.TutorialInGameAndroidPanelId);
                    else
                        interfaceComp.CanvasBehaviour.ShowPanelById(PanelsIdHolder.TutorialInGameDesktopPanelId);

                    _state.Value.InGameTutorial = false;
                    _state.Value.SaveTutorial();
                    _state.Value.StartPauseSystems();
                    _inGameTutorialPool.Value.Del(entity);
                }
            }
        }
    }
}