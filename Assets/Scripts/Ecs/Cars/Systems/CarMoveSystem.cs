using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarMoveSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<CarIsMovingForwardComp>> _moveForwardFilter = default;
        private EcsFilterInject<Inc<CarIsMovingBackwardComp>> _moveBackFilter = default;
        private EcsFilterInject<Inc<CarIsDriftComp>> _driftFilter = default;
        private EcsFilterInject<Inc<CarIsIdleComp>> _idleFilter = default;

        private EcsPoolInject<CarControllComp> _controllPool = default;
        private EcsPoolInject<CarThrottleComp> _motorTorquePool = default;
        private EcsPoolInject<CarBrakeComp> _brakeTorquePool = default;
        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<CarStatisticComp> _statisticPool = default;
        private EcsPoolInject<CarStartBrakeEvent> _startBrakeEventPool = default;
        private EcsPoolInject<CarMoveDirectionComp> _directionPool = default;

        private EcsPoolInject<CarOutOfFuelComp> _outOfFuelPool = default;
        private EcsPoolInject<CarDeathComp> _carDeathPool = default;

        private float _transmissionIndex;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _moveForwardFilter.Value)
            {
                var playerSettingsConfig = _state.Value.PlayerConfig;
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);
                ref var directionComp = ref _directionPool.Value.Get(entity);

                _transmissionIndex = playerSettingsConfig.TransmissionIndex;
                float motorIndex = carComp.ThrottleCurve.Evaluate(statisticComp.SpeedKmpH / carComp.CarCharacteristic.MaxSpeed);

                if (controllComp.VerticalInput > 0)
                {
                    //спорная штука
                    float cruchIndexOfBackMove = 1f;
                    if (directionComp.AngleBetweenRbAndForwardDir > 150f)
                        cruchIndexOfBackMove = 5f;

                    SetMotorTorque(entity, carComp.CarCharacteristic.HorcePower 
                        * _transmissionIndex * controllComp.VerticalInput * motorIndex * cruchIndexOfBackMove);
                    SetBreakTorque(entity, 0);
                }
                else if (controllComp.VerticalInput == 0)
                {
                    SetMotorTorque(entity, 0);
                    SetBreakTorque(entity, 0);
                }
                else if (controllComp.VerticalInput < 0)
                {
                    SetMotorTorque(entity, 0);
                    SetBreakTorque(entity, Mathf.Abs(carComp.CarMb.BreakPower * controllComp.VerticalInput));
                }
            }

            foreach (var entity in _moveBackFilter.Value)
            {
                var settingsConfig = _state.Value.PlayerConfig;
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);

                _transmissionIndex = settingsConfig.TransmissionIndex;
                float motorIndex = carComp.ThrottleCurve.Evaluate(statisticComp.SpeedKmpH / settingsConfig.BackMaxSpeedKmh);

                if (controllComp.VerticalInput > 0)
                {
                    SetMotorTorque(entity, 0);
                    SetBreakTorque(entity, Mathf.Abs(carComp.CarMb.BreakPower * controllComp.VerticalInput));
                }
                else if (controllComp.VerticalInput == 0)
                {
                    SetMotorTorque(entity, 0);
                    SetBreakTorque(entity, 0);
                }
                else if (controllComp.VerticalInput < 0)
                {
                    SetMotorTorque(entity, carComp.CarCharacteristic.HorcePower 
                        * _transmissionIndex * controllComp.VerticalInput * motorIndex);
                    SetBreakTorque(entity, 0);
                }
            }

            foreach (var entity in _driftFilter.Value)
            {
                //TODO наверняка можно сюда бахнуть другие подстройки для дрифта
                var settingsConfig = _state.Value.PlayerConfig;
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);
                ref var directionComp = ref _directionPool.Value.Get(entity);

                _transmissionIndex = settingsConfig.TransmissionIndex;

                //TODO эту штуку оставить только для дрифт автомобилей
                //float motorIndex = carComp.ThrottleCurve.Evaluate(statisticComp.SpeedKmpH / carComp.CarMb.MaxSpeed);
                float motorIndex = 1f;

                if (controllComp.VerticalInput > 0)
                {
                    //спорная штука
                    float cruchIndexOfBackMove = 1f;
                    if (directionComp.AngleBetweenRbAndForwardDir > 150f)
                        cruchIndexOfBackMove = 2f;

                    SetMotorTorque(entity, carComp.CarCharacteristic.HorcePower * _transmissionIndex * controllComp.VerticalInput * motorIndex * cruchIndexOfBackMove);
                    SetBreakTorque(entity, 0);
                }
                else if (controllComp.VerticalInput == 0)
                {
                    SetMotorTorque(entity, 0);
                    SetBreakTorque(entity, 0);
                }
                else if (controllComp.VerticalInput < 0)
                {
                    SetMotorTorque(entity, 0);
                    SetBreakTorque(entity, Mathf.Abs(carComp.CarMb.BreakPower * controllComp.VerticalInput));
                }
            }

            foreach (var entity in _idleFilter.Value)
            {
                var settingsConfig = _state.Value.PlayerConfig;
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);

                _transmissionIndex = settingsConfig.TransmissionIndex;
                float motorIndex = carComp.ThrottleCurve.Evaluate(statisticComp.SpeedKmpH / carComp.CarCharacteristic.MaxSpeed);

                if (controllComp.VerticalInput > 0)
                {
                    SetMotorTorque(entity, carComp.CarCharacteristic.HorcePower 
                        * _transmissionIndex * controllComp.VerticalInput * motorIndex);
                    SetBreakTorque(entity, 0);
                }
                else if (controllComp.VerticalInput == 0)
                {
                    SetMotorTorque(entity, 0);
                    SetBreakTorque(entity, 0);
                }
                else if (controllComp.VerticalInput < 0)
                {
                    SetMotorTorque(entity, carComp.CarCharacteristic.HorcePower 
                        * _transmissionIndex * controllComp.VerticalInput * motorIndex);
                    SetBreakTorque(entity, 0);
                }
            }
        }

        private void SetMotorTorque(int entity, float value)
        {
            if (!_motorTorquePool.Value.Has(entity) && value == 0)
            { }
            else
            {
                if (!_motorTorquePool.Value.Has(entity))
                    _motorTorquePool.Value.Add(entity);

                ref var motorTorqueComp = ref _motorTorquePool.Value.Get(entity);

                if (!_outOfFuelPool.Value.Has(entity) && !_carDeathPool.Value.Has(entity))
                    motorTorqueComp.MotorTorqueValue = value;
                else
                    motorTorqueComp.MotorTorqueValue = 0;
            }
        }

        private void SetBreakTorque(int entity, float value)
        {
            if (!_brakeTorquePool.Value.Has(entity) && value == 0)
            { }
            else
            {
                if (!_brakeTorquePool.Value.Has(entity))
                {
                    _brakeTorquePool.Value.Add(entity);
                    _startBrakeEventPool.Value.Add(entity);
                }

                ref var brakeComp = ref _brakeTorquePool.Value.Get(entity);
                brakeComp.BrakeTorqueValue = value;
            }
        }
    }
}