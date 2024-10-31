using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Linq;

namespace Client
{
    sealed class CarTorqueSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarThrottleComp>> _filter = default;
        private EcsPoolInject<CarThrottleComp> _torquePool = default;
        private EcsPoolInject<CarWheelsControllComp> _wheelsPool = default;
        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<CarStatisticComp> _statisticPool = default;

        private float _maxSpeedOfProportionalTorque = 20f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var torqueComp = ref _torquePool.Value.Get(entity);
                ref var wheelsComp = ref _wheelsPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);

                if (carComp.CarMb.WheelDriveType == Enums.WheelDriveType.Awd)
                {
                    WheelAxisPair frontPair = wheelsComp.DrivingWheelAxisPairs.FirstOrDefault
                        (x => x.LeftWheel.Orientation == Enums.OrientationEnum.Front);

                    WheelAxisPair rearPair = wheelsComp.DrivingWheelAxisPairs.FirstOrDefault
                        (x => x.LeftWheel.Orientation == Enums.OrientationEnum.Rear);

                    SetSimpleTorqueOnAxis(frontPair, 0.4f * torqueComp.MotorTorqueValue);
                    SetSimpleTorqueOnAxis(rearPair, 0.6f * torqueComp.MotorTorqueValue);
                }
                else if (carComp.CarMb.WheelDriveType == Enums.WheelDriveType.Custom)
                {
                    foreach (var wheel in wheelsComp.DrivingWheels)
                        wheel.SetTorque(torqueComp.MotorTorqueValue / wheelsComp.DrivingWheels.Length);
                }
                else
                {
                    if (statisticComp.SpeedKmpH < _maxSpeedOfProportionalTorque)
                        SetProportionalTorqueOnAxis(wheelsComp.DrivingWheelAxisPairs[0], torqueComp.MotorTorqueValue);
                    else
                        SetSimpleTorqueOnAxis(wheelsComp.DrivingWheelAxisPairs[0], torqueComp.MotorTorqueValue);
                }

                if (torqueComp.MotorTorqueValue == 0)
                    _torquePool.Value.Del(entity);
            }
        }

        private void SetSimpleTorqueOnAxis(WheelAxisPair wheelPair, float value)
        {
            wheelPair.LeftWheel.SetTorque(value / 2f);
            wheelPair.RightWheel.SetTorque(value / 2f);
        }

        private void SetProportionalTorqueOnAxis(WheelAxisPair wheelPair, float value)
        {
            float leftWheelRpm = wheelPair.LeftWheel.WheelController.RPM;
            float rightWheelRpm = wheelPair.RightWheel.WheelController.RPM;
            float allRpm = leftWheelRpm + rightWheelRpm;
            float leftWheelIndex = leftWheelRpm / allRpm;
            float rightWheelIndex = rightWheelRpm / allRpm;
            wheelPair.LeftWheel.SetTorque(value * rightWheelIndex);
            wheelPair.RightWheel.SetTorque(value * leftWheelIndex);
        }
    }
}