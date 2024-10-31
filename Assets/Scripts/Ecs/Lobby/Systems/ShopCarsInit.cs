using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Client
{
    sealed class ShopCarsInit : IEcsInitSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<ShopCarsComp> _shopPool = default;
        private EcsPoolInject<ShopCarComp> _shopCarPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<CarShopViewEvent> _carShopViewEventPool = default;

        private Vector3 _startPos = new Vector3(0f, 0f, 0f);
        private Vector3 _startRotation = new Vector3(0f, 225f, 0f);

        private float _stepForModels = 15f;

        public void Init(EcsSystems systems)
        {
            ref var shopComp = ref _shopPool.Value.Add(_state.Value.InterfaceEntity);
            shopComp.CarsDictionary = new Dictionary<ICar, PlayerCarSo>();
            shopComp.Cars = new List<ICar>();

            PlayerCarsConfig carShopConfig = _state.Value.PlayerCarsPrefabsConfig;
            List<Transform> cars = new List<Transform>();

            Vector3 currentPos = _startPos;
            for (int i = 0; i < carShopConfig.Cars.Length; i++)
            {
                PlayerCarSo carSo = carShopConfig.Cars[i];
                GameObject carGo = GameObject.Instantiate(carSo.CarPrefab);
                carGo.transform.position = currentPos;
                carGo.transform.rotation = Quaternion.Euler(_startRotation);

                ICar carMb = carGo.GetComponent<ICar>();
                carMb.SetShopState();
                shopComp.CarsDictionary.Add(carMb, carSo);
                shopComp.Cars.Add(carMb);

                int carEntity = _world.Value.NewEntity();

                ref var shopCarComp = ref _shopCarPool.Value.Add(carEntity);
                shopCarComp.CarMb = carMb;
                shopCarComp.CarSo = carSo;
                shopCarComp.SavedCar = _state.Value.PlayersSavedCars.FirstOrDefault(x => x.Id == carSo.Id);

                if (shopCarComp.SavedCar != null)
                {
                    if (carMb.ArmorGameObject != null)
                    {
                        if (shopCarComp.SavedCar.HpLevel == carSo.HpLevels.Length - 1)
                            carMb.ArmorGameObject?.SetActive(true);
                        else
                            carMb.ArmorGameObject?.SetActive(false);
                    }
                }
                else
                {
                    if (carMb.ArmorGameObject != null)
                        carMb.ArmorGameObject.SetActive(false);
                }

                ref var viewComp = ref _viewPool.Value.Add(carEntity);
                viewComp.Transform = carGo.transform;
                cars.Add(carGo.transform);

                currentPos = new Vector3(currentPos.x + _stepForModels, currentPos.y, currentPos.z);
            }

            string currentPlayerCar = _state.Value.CurrentPlayerCar;
            PlayerCarSo currentPlayerCarSo = carShopConfig.GetCarSoById(currentPlayerCar);
            int indexOfPlayerCar = Array.IndexOf(carShopConfig.Cars, currentPlayerCarSo);
            ICar currentCar = shopComp.Cars[indexOfPlayerCar];

            foreach (var car in cars)
            {
                car.transform.position = new Vector3(
                    car.transform.position.x - _stepForModels * indexOfPlayerCar,
                    car.transform.position.y,
                    car.transform.position.z);
            }

            shopComp.CurrentCar = currentCar;
            shopComp.CurrentCarSo = currentPlayerCarSo;

            _carShopViewEventPool.Value.Add(_world.Value.NewEntity());
        }
    }
}