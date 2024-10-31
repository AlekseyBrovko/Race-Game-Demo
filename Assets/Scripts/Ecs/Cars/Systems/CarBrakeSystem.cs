using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class CarBrakeSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarBrakeComp>> _brakeFilter = default;
        private EcsPoolInject<CarBrakeComp> _brakePool = default;
        private EcsPoolInject<CarWheelsControllComp> _wheelsPool = default;
        private EcsPoolInject<CarStopBrakeEvent> _stopBreakEventPool = default;
        private EcsPoolInject<CarComp> _carPool = default;

        private EcsFilterInject<Inc<StartHandBrakeEvent>> _startBrakeFilter = default;
        private EcsFilterInject<Inc<EndHandBrakeEvent>> _endBrakeFilter = default;
        private EcsFilterInject<Inc<HandBrakeComp>> _handBrakeFilter = default;

        private EcsPoolInject<StartHandBrakeEvent> _startBrakeEvent = default;
        private EcsPoolInject<EndHandBrakeEvent> _endBrakeEvent = default;

        private EcsFilterInject<Inc<EngineBrakingComp>> _engineBrakingPool = default;
        private EcsFilterInject<Inc<StopEngineBrakingEvent>> _stopEngineBrakingFilter = default;
        private EcsPoolInject<StopEngineBrakingEvent> _stopBrakingEngineEventPool = default;

        private float _handBrakeForce = 4000f;
        private float _engineBrakingForce = 200f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _engineBrakingPool.Value)
            {
                ref var wheelsComp = ref _wheelsPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);
                _engineBrakingForce = carComp.CarMb.BreakPower / 20f;
                foreach (var wheel in wheelsComp.DrivingWheels)
                    wheel.SetBrake(_engineBrakingForce / wheelsComp.DrivingWheels.Length);
            }

            foreach (var entity in _stopEngineBrakingFilter.Value)
            {
                ref var wheelsComp = ref _wheelsPool.Value.Get(entity);

                foreach (var wheel in wheelsComp.DrivingWheels)
                    wheel.SetBrake(0);

                _stopBrakingEngineEventPool.Value.Del(entity);
            }

            foreach (var entity in _brakeFilter.Value)
            {
                ref var brakeComp = ref _brakePool.Value.Get(entity);
                ref var wheelsComp = ref _wheelsPool.Value.Get(entity);

                if (wheelsComp.Wheels.Length == 4)
                {
                    foreach (var wheel in wheelsComp.Wheels)
                        if (wheel.Orientation == Enums.OrientationEnum.Front)
                            wheel.SetBrake(brakeComp.BrakeTorqueValue / 2f * 0.6f);
                        else
                            wheel.SetBrake(brakeComp.BrakeTorqueValue / 2f * 0.4f);
                }
                else
                {
                    foreach (var wheel in wheelsComp.Wheels)
                        wheel.SetBrake(brakeComp.BrakeTorqueValue / wheelsComp.Wheels.Length);
                }

                if (brakeComp.BrakeTorqueValue == 0)
                {
                    if (!_stopBreakEventPool.Value.Has(entity))
                        _stopBreakEventPool.Value.Add(entity);

                    _brakePool.Value.Del(entity);
                }
            }

            foreach (var entity in _handBrakeFilter.Value)
            {
                ref var wheelsComp = ref _wheelsPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);

                if (carComp.CarMb.HandBreakPower == 0)
                    _handBrakeForce = carComp.CarMb.BreakPower;
                else
                    _handBrakeForce = carComp.CarMb.HandBreakPower;

                foreach (var wheel in wheelsComp.HandBrakeWheels)
                    wheel.SetBrake(_handBrakeForce);
            }

            foreach (var entity in _startBrakeFilter.Value)
            {
                ref var wheelsComp = ref _wheelsPool.Value.Get(entity);
                foreach (var wheel in wheelsComp.HandBrakeWheels)
                    wheel.SetHandBrakeFrictionSettings(0.6f);
                _startBrakeEvent.Value.Del(entity);
            }

            foreach (var entity in _endBrakeFilter.Value)
            {
                ref var wheelsComp = ref _wheelsPool.Value.Get(entity);
                foreach (var wheel in wheelsComp.HandBrakeWheels)
                {
                    wheel.SetStartFrictionSettings();
                    wheel.SetBrake(0);
                }

                _endBrakeEvent.Value.Del(entity);
            }
        }
    }
}