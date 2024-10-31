using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarRotationInputSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<CarRotationLobbyInputComp>> _filter = default;
        private EcsPoolInject<InputComp> _inputPool = default;
        private EcsPoolInject<ShopCarsComp> _shopCarsPool = default;

        private float _rotationSpeed = 80f;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                ref var inputComp = ref _inputPool.Value.Get(_state.Value.InputEntity);
                ref var carsComp = ref _shopCarsPool.Value.Get(_state.Value.InterfaceEntity);

                Transform currentCar = carsComp.CurrentCar.Transform;
                currentCar.Rotate(currentCar.up, - _rotationSpeed * inputComp.TouchOrMouseInput.x * Time.deltaTime);
            }
        }
    }
}