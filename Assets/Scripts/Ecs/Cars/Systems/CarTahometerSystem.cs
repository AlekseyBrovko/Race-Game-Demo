using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarTahometerSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarTahometerComp>, Exc<CarIsFlyComp>> _filter = default;
        private EcsFilterInject<Inc<CarTahometerComp, CarIsFlyComp>> _flyFilter = default;

        private EcsPoolInject<CarTahometerComp> _tahoPool = default;
        private EcsPoolInject<CarStatisticComp> _statisticPool = default;
        private EcsPoolInject<CarControllComp> _controllPool = default;
        private EcsPoolInject<CarWheelsControllComp> _wheelsPool = default;
        private EcsPoolInject<CarComp> _carPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var tahoComp = ref _tahoPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var wheelsComp = ref _wheelsPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);

                float currentSpeed = statisticComp.SpeedKmpH;
                Wheel fastestWheel = GetFastestWheel(wheelsComp.DrivingWheels);

                float indexOfSpeed = (Mathf.PI * 2 * fastestWheel.WheelController.Radius *
                    fastestWheel.WheelController.RPM) / 60f * 3.6f;

                if (Tools.ComparisonWithThreshold(indexOfSpeed, currentSpeed, currentSpeed * 0.1f))
                {
                    //вписывается в индекс движения
                    tahoComp.NextValueRpm = tahoComp.MaxRpm * (currentSpeed / carComp.CarCharacteristic.MaxSpeed);
                }
                else
                {
                    //дрифтит и вращается быстрее чем движется машина
                    tahoComp.NextValueRpm = tahoComp.MaxRpm * (indexOfSpeed / carComp.CarCharacteristic.MaxSpeed);
                }

                ClampAndSmoothTahometer(ref tahoComp);
            }

            foreach (var entity in _flyFilter.Value)
            {
                ref var tahoComp = ref _tahoPool.Value.Get(entity);
                ref var controllComp = ref _controllPool.Value.Get(entity);
                
                float speedOfTahoChangeInFly = 1000f;
                if(controllComp.VerticalInput == 0)
                    tahoComp.NextValueRpm = tahoComp.CurrentValueRpm - speedOfTahoChangeInFly/* / 2f*/;
                else
                    tahoComp.NextValueRpm = tahoComp.CurrentValueRpm + (speedOfTahoChangeInFly * controllComp.VerticalInput);

                ClampAndSmoothTahometer(ref tahoComp);
            }
        }

        private Wheel GetFastestWheel(Wheel[] wheels)
        {
            float rpm = float.NegativeInfinity;
            Wheel result = null;
            foreach (var wheel in wheels)
            {
                if (Mathf.Abs(wheel.WheelController.RPM) > rpm)
                {
                    rpm = wheel.WheelController.RPM;
                    result = wheel;
                }
            }
            return result;
        }

        private void ClampAndSmoothTahometer(ref CarTahometerComp tahoComp)
        {
            float smoothTime = 0.1f;
            float velocity = 0.0f;

            tahoComp.CurrentValueRpm =
                Mathf.SmoothDamp(tahoComp.CurrentValueRpm, tahoComp.NextValueRpm, ref velocity, smoothTime);
            if (tahoComp.CurrentValueRpm < tahoComp.MinRpm)
                tahoComp.CurrentValueRpm = tahoComp.MinRpm;

            if (tahoComp.CurrentValueRpm > tahoComp.MaxRpm)
                tahoComp.CurrentValueRpm = tahoComp.MaxRpm;
        }
    }
}