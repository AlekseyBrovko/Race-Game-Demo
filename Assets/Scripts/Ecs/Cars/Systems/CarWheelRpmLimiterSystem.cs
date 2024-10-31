using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarWheelRpmLimiterSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarThrottleComp>> _filter = default;
        private EcsPoolInject<CarStatisticComp> _statisticPool = default;
        private EcsPoolInject<CarWheelsControllComp> _wheelsPool = default;
        private EcsPoolInject<IndexOfSpeedStatisticComp> _speedIndexPool = default;

        private float _maxDeltaOfSpeed = 3f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var wheelsComp = ref _wheelsPool.Value.Get(entity);
                ref var indexComp = ref _speedIndexPool.Value.Get(entity);

                foreach (var wheel in wheelsComp.DrivingWheels)
                {
                    float speedIndexOfWheel = 3.6f * (Mathf.PI * 2 * wheel.WheelController.Radius *
                        wheel.WheelController.RPM) / 60f;

                    if (statisticComp.SpeedKmpH == 0)
                        continue;

                    if (speedIndexOfWheel / statisticComp.SpeedKmpH > _maxDeltaOfSpeed)
                        wheel.SetTorque(0);
                }
            }
        }
    }
}