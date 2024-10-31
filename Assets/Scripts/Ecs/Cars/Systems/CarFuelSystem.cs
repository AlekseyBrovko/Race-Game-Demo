using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarFuelSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<InGamePanelComp>> _uiFilter = default;
        private EcsFilterInject<Inc<CarFuelComp>, Exc<CarOutOfFuelComp>> _carFuelFilter = default;

        private EcsPoolInject<InGamePanelComp> _fuelPanelPool = default;
        private EcsPoolInject<CarFuelComp> _carFuelPool = default;
        private EcsPoolInject<CarControllComp> _controllPool = default;
        private EcsPoolInject<CarThrottleComp> _throttlePool = default;
        private EcsPoolInject<CarOutOfFuelComp> _outOfFuelPool = default;

        private float _timerForShowFuel = 0f;
        private float _showFuelDuration = 0.5f;
        private float _animationThreshold = 0.2f;

        public void Run(EcsSystems systems)
        {
            foreach (var carEntity in _carFuelFilter.Value)
            {
                ref var carControllComp = ref _controllPool.Value.Get(carEntity);
                ref var carFuelComp = ref _carFuelPool.Value.Get(carEntity);

                float fuelIndex = 0.1f;
                if (_throttlePool.Value.Has(carEntity))
                    fuelIndex = Mathf.Abs(carControllComp.VerticalInput) * 0.9f + 0.1f;

                carFuelComp.CurrentValue -= Time.fixedDeltaTime * fuelIndex;

                float indexOfFuel = carFuelComp.CurrentValue / carFuelComp.FullTankOfGasValue;

                _timerForShowFuel -= Time.fixedDeltaTime;
                if (_timerForShowFuel < 0)
                {
                    bool showAnimation = false;
                    if (indexOfFuel < _animationThreshold && !carFuelComp.ShowLowFuel)
                    {
                        showAnimation = true;
                        carFuelComp.ShowLowFuel = true;
                    }

                    _timerForShowFuel = _showFuelDuration;
                    ShowFuelValue(indexOfFuel, showAnimation);
                }

                if (carFuelComp.CurrentValue < 0)
                {
                    if (_state.Value.EnableFuel)
                        _outOfFuelPool.Value.Add(carEntity);
                }
            }
        }

        private void ShowFuelValue(float sliderValue, bool showAnimation)
        {
            foreach (var entity in _uiFilter.Value)
            {
                ref var fuelPanelComp = ref _fuelPanelPool.Value.Get(entity);
                fuelPanelComp.InGamePanelMb.ShowFuelValue(sliderValue, showAnimation);
            }
        }
    }
}