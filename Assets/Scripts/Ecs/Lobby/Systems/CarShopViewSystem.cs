using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Linq;

namespace Client
{
    sealed class CarShopViewSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<CarShopViewEvent>> _filter = default;
        private EcsPoolInject<CarShopViewEvent> _carShopViewEventPool = default;
        private EcsPoolInject<LobbyPanelComp> _lobbyPanelPool = default;
        private EcsPoolInject<ShopCarsComp> _shopPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var lobbyPanelComp = ref _lobbyPanelPool.Value.Get(_state.Value.InterfaceEntity);
                ref var shopComp = ref _shopPool.Value.Get(_state.Value.InterfaceEntity);

                string carId = shopComp.CurrentCarSo.Id;
                SavedCar savedCar = _state.Value.PlayersSavedCars.FirstOrDefault(x => x.Id == carId);

                lobbyPanelComp.LobbyPanelMb.ShowCarData(shopComp.CurrentCarSo, savedCar);

                _carShopViewEventPool.Value.Del(entity);
            }
        }
    }
}