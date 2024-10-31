using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Linq;
using UnityEngine;

namespace Client
{
    sealed class CarUpgradeSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<CarUpgradeBuyEvent>> _filter = default;
        private EcsPoolInject<CarUpgradeBuyEvent> _upgradePool = default;
        private EcsPoolInject<UISoundEvent> _soundPool = default;
        private EcsPoolInject<MoneySpendEvent> _moneyPool = default;
        private EcsPoolInject<CarShopViewEvent> _carShopViewPool = default;
        private EcsPoolInject<ShopCarsComp> _shopPool = default;

        private EcsFilterInject<Inc<ShopCarComp>> _shopCarFilter = default;
        private EcsPoolInject<ShopCarComp> _shopCarPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var upgradeComp = ref _upgradePool.Value.Get(entity);

                ref var shopComp = ref _shopPool.Value.Get(_state.Value.InterfaceEntity);
                string currentCar = shopComp.CurrentCarSo.Id;
                int price = 0;

                PlayerCarSo carSo = shopComp.CurrentCarSo;
                SavedCar savedCar = _state.Value.PlayersSavedCars.FirstOrDefault(x => x.Id == currentCar);

                Debug.Log("CarUpgradeSystem currentCar = " + carSo.Id);
                Debug.Log("CarUpgradeSystem upgradeComp.UpgradeType = " + upgradeComp.UpgradeType);

                switch (upgradeComp.UpgradeType)
                {
                    case Enums.CarUpgradeType.MaxSpeed:
                        price = carSo.MaxSpeedLevels[savedCar.MaxSpeedLevel + 1].Price;
                        if (CheckPrice(price))
                        {
                            savedCar.MaxSpeedLevel++;
                            _carShopViewPool.Value.Add(_world.Value.NewEntity());
                            _state.Value.SaveCars();
                        }
                        break;

                    case Enums.CarUpgradeType.MotorPower:
                        price = carSo.HorcePowerLevels[savedCar.HorcePowerLevel + 1].Price;
                        if (CheckPrice(price))
                        {
                            savedCar.HorcePowerLevel++;
                            _carShopViewPool.Value.Add(_world.Value.NewEntity());
                            _state.Value.SaveCars();
                        }
                        break;

                    case Enums.CarUpgradeType.Hp:
                        price = carSo.HpLevels[savedCar.HpLevel + 1].Price;
                        if (CheckPrice(price))
                        {
                            savedCar.HpLevel++;
                            _carShopViewPool.Value.Add(_world.Value.NewEntity());
                            _state.Value.SaveCars();

                            if (savedCar.HpLevel == carSo.HpLevels.Length - 1)
                                HandleArmor(savedCar);
                        }
                        break;

                    case Enums.CarUpgradeType.Fuel:
                        price = carSo.FuelLevels[savedCar.FuelLevel + 1].Price;
                        if (CheckPrice(price))
                        {
                            savedCar.FuelLevel++;
                            _carShopViewPool.Value.Add(_world.Value.NewEntity());
                            _state.Value.SaveCars();
                        }
                        break;
                }
                _upgradePool.Value.Del(entity);
            }
        }

        private bool CheckPrice(int price)
        {
            if (price > _state.Value.PlayerMoneyScore)
            {
                ref var soundComp = ref _soundPool.Value.Add(_world.Value.NewEntity());
                soundComp.Sound = Enums.SoundEnum.WarningSound;
                return false;
            }
            else
            {
                ref var moneyComp = ref _moneyPool.Value.Add(_world.Value.NewEntity());
                moneyComp.MoneyValue = price;
                return true;
            }
        }

        private void HandleArmor(SavedCar savedCar)
        {
            foreach (var entity in _shopCarFilter.Value)
            {
                ref var shopCarComp = ref _shopCarPool.Value.Get(entity);
                if (shopCarComp.CarSo.Id == savedCar.Id)
                {
                    if (shopCarComp.CarMb.ArmorGameObject != null)
                        shopCarComp.CarMb.ArmorGameObject.SetActive(true);
                }
            }
        }
    }
}