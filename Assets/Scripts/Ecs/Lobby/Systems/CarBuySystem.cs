using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarBuySystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<CarBuyEvent>> _filter = default;
        private EcsPoolInject<CarBuyEvent> _carBuyPool = default;
        private EcsPoolInject<UISoundEvent> _soundPool = default;
        private EcsPoolInject<MoneySpendEvent> _moneyPool = default;
        private EcsPoolInject<CarShopViewEvent> _carShopViewEventPool = default;

        private EcsFilterInject<Inc<ShopCarComp>> _shopCarFilter = default;
        private EcsPoolInject<ShopCarComp> _shopCarPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var carBuyComp = ref _carBuyPool.Value.Get(entity);
                PlayerCarSo carSo = _state.Value.PlayerCarsPrefabsConfig.GetCarSoById(carBuyComp.CarId);
                if (carSo.Price <= _state.Value.PlayerMoneyScore)
                {
                    ref var moneyComp = ref _moneyPool.Value.Add(_world.Value.NewEntity());
                    moneyComp.MoneyValue = carSo.Price;

                    _state.Value.CurrentPlayerCar = carBuyComp.CarId;

                    SavedCar newCar = new SavedCar();
                    newCar.Id = carBuyComp.CarId;
                    _state.Value.PlayersSavedCars.Add(newCar);
                    _state.Value.SaveCars();

                    HandleBuyedCar(newCar);
                    _carShopViewEventPool.Value.Add(_world.Value.NewEntity());
                }
                else
                {
                    ref var soundComp = ref _soundPool.Value.Add(_world.Value.NewEntity());
                    soundComp.Sound = Enums.SoundEnum.WarningSound;
                }

                _carBuyPool.Value.Del(entity);
            }
        }

        private void HandleBuyedCar(SavedCar savedCar)
        {
            foreach (var entity in _shopCarFilter.Value)
            {
                ref var shopCarComp = ref _shopCarPool.Value.Get(entity);
                if (shopCarComp.CarSo.Id == savedCar.Id)
                    shopCarComp.SavedCar = savedCar;
            }
        }
    }
}