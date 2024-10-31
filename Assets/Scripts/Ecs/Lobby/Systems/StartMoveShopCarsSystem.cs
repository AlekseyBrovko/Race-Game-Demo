using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Linq;
using UnityEngine;

namespace Client
{
    sealed class StartMoveShopCarsSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<NextShopItemEvent>> _nextFilter = default;
        private EcsFilterInject<Inc<PreviousShopItemEvent>> _prevFilter = default;
        private EcsFilterInject<Inc<ShopCarComp>> _shopCarFilter = default;

        private EcsPoolInject<NextShopItemEvent> _nextEventPool = default;
        private EcsPoolInject<PreviousShopItemEvent> _prevEventPool = default;

        private EcsPoolInject<LobbyPanelComp> _lobbyPanelPool = default;
        private EcsPoolInject<ShopCarsComp> _carsPool = default;
        private EcsPoolInject<MoveShopCarComp> _movePool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<MoveCarsCoolDownComp> _coolDownPool = default;

        private float _stepOfModels = 15f;
        private float _moveTime = 1f;

        public void Run(EcsSystems systems)
        {
            HandleNextEvent();
            HandlePreviousEvent();
        }

        private void HandleNextEvent()
        {
            foreach(var entity in _nextFilter.Value)
            {
                ref var shopCarsComp = ref _carsPool.Value.Get(_state.Value.InterfaceEntity);
                if (shopCarsComp.Cars.Last() != shopCarsComp.CurrentCar)
                {
                    shopCarsComp.CurrentCar = shopCarsComp.Cars[shopCarsComp.Cars.IndexOf(shopCarsComp.CurrentCar) + 1];
                    shopCarsComp.CurrentCarSo = shopCarsComp.CarsDictionary[shopCarsComp.CurrentCar];
                    MoveAllShopCars(Enums.SideEnum.Right);
                }   
                
                _nextEventPool.Value.Del(entity);
            }
        }

        private void HandlePreviousEvent() 
        {
            foreach(var entity in _prevFilter.Value)
            {
                ref var shopCarsComp = ref _carsPool.Value.Get(_state.Value.InterfaceEntity);
                if (shopCarsComp.Cars[0] != shopCarsComp.CurrentCar)
                {
                    shopCarsComp.CurrentCar = shopCarsComp.Cars[shopCarsComp.Cars.IndexOf(shopCarsComp.CurrentCar) - 1];
                    shopCarsComp.CurrentCarSo = shopCarsComp.CarsDictionary[shopCarsComp.CurrentCar];
                    MoveAllShopCars(Enums.SideEnum.Left);
                }

                _prevEventPool.Value.Del(entity);
            }
        }

        private void MoveAllShopCars(Enums.SideEnum side)
        {
            ref var lobbyPanelComp = ref _lobbyPanelPool.Value.Get(_state.Value.InterfaceEntity);
            lobbyPanelComp.LobbyPanelMb.OnStartMoveAnimation();
            foreach (var entity in _shopCarFilter.Value)
            {
                ref var viewComp = ref _viewPool.Value.Get(entity);
                ref var moveComp = ref _movePool.Value.Add(entity);
                moveComp.Transform = viewComp.Transform;
                moveComp.Duration = _moveTime;
                moveComp.Timer = _moveTime;
                Vector3 pos = viewComp.Transform.position;
                Vector3 endPos = new Vector3();
                moveComp.StartPosition = pos;
                if (side == Enums.SideEnum.Right)
                    endPos = new Vector3(pos.x - _stepOfModels, pos.y, pos.z);
                else
                    endPos = new Vector3(pos.x + _stepOfModels, pos.y, pos.z);
                moveComp.EndPosition = endPos;
            }

            ref var coolDownComp = ref _coolDownPool.Value.Add(_state.Value.InterfaceEntity);
            coolDownComp.Timer = _moveTime;
        }
    }
}