using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class NewCarTahometerSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<CarTahometerComp, PlayerCarComp, CarIsIdleComp>, Exc<CarIsFlyComp>> _playerCarIsIdleFilter = default;
        private EcsFilterInject<Inc<CarTahometerComp, PlayerCarComp, CarIsMovingForwardComp>, Exc<CarIsFlyComp>> _playerCarMoveForwardFilter = default;
        private EcsFilterInject<Inc<CarTahometerComp, PlayerCarComp, CarIsMovingBackwardComp>, Exc<CarIsFlyComp>> _playerCarMoveBackwardFilter = default;
        private EcsFilterInject<Inc<CarTahometerComp, PlayerCarComp, CarIsFlyComp>> _playerFlyCarFilter = default;
        private EcsFilterInject<Inc<CarTahometerComp>, Exc<PlayerCarComp>> _notPlayerCarFilter = default;

        private EcsPoolInject<CarTahometerComp> _tahoPool = default;
        private EcsPoolInject<CarStatisticComp> _statisticPool = default;
        private EcsPoolInject<CarControllComp> _controllPool = default;
        private EcsPoolInject<CarComp> _carPool = default;

        public void Run(EcsSystems systems)
        {
            HandlePlayerCarTahometer();
            HandlePlayerFlyCar();
            HandleNotPlayerCar();
        }

        private void HandlePlayerCarTahometer()
        {
            foreach (var entity in _playerCarIsIdleFilter.Value)
            {
                ref var tahometerComp = ref _tahoPool.Value.Get(entity);
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);

                float currentSpeed = statisticComp.SpeedKmpH;
                float maxSpeed = carComp.CarCharacteristic.MaxSpeed;

                if (controllComp.VerticalInput > 0)
                {
                    int gears = carComp.TransmissionGearsAmount;
                    float speedOnGearsIndex = maxSpeed / (float)(gears + 1); //делим скорость на кусочки, на первой передаче два кусочка
                    bool firstGear = false;
                    float startGearSpeedValue = 0;
                    float topGearSpeedValue = 0;

                    //проверяем первая передача или нет
                    if (currentSpeed <= speedOnGearsIndex * 2f)
                    {
                        //первая передача
                        startGearSpeedValue = 0;
                        topGearSpeedValue = speedOnGearsIndex * 2f;
                        firstGear = true;
                    }
                    else
                    {
                        for (int gear = 2; gear <= gears; gear++)
                        {
                            float currentGearStartSpeed = speedOnGearsIndex * gear;
                            float currentGearMaxSpeed = speedOnGearsIndex * (gear + 1);
                            if (currentSpeed >= currentGearStartSpeed && currentSpeed < currentGearMaxSpeed)
                            {
                                startGearSpeedValue = currentGearStartSpeed;
                                topGearSpeedValue = currentGearMaxSpeed;
                            }
                        }
                    }

                    //высчитали нижнюю и верхнюю скорость на текущей передаче, и от 0 до 1 приводим это всё
                    float indexOfSpeedT = currentSpeed - startGearSpeedValue;
                    float maxT = topGearSpeedValue - startGearSpeedValue;
                    float indexT = indexOfSpeedT / maxT;

                    if (firstGear)
                        tahometerComp.NextValueRpm = 1000f * (5f * indexT + 1f);
                    else
                        tahometerComp.NextValueRpm = 500f * (5f * indexT + 7f);
                }
                else
                {
                    float speedOfTahoChangeInFly = 1000f;
                    tahometerComp.NextValueRpm = tahometerComp.CurrentValueRpm - speedOfTahoChangeInFly/* / 2f*/;
                }

                ClampAndSmoothTahometer(ref tahometerComp);
            }

            foreach (var entity in _playerCarMoveForwardFilter.Value)
            {
                ref var tahometerComp = ref _tahoPool.Value.Get(entity);
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);

                float currentSpeed = statisticComp.SpeedKmpH;
                float maxSpeed = carComp.CarCharacteristic.MaxSpeed;

                if (controllComp.VerticalInput > 0)
                {
                    int gears = carComp.TransmissionGearsAmount;
                    float speedOnGearsIndex = maxSpeed / (float)(gears + 1); //делим скорость на кусочки, на первой передаче два кусочка
                    bool firstGear = false;
                    float startGearSpeedValue = 0;
                    float topGearSpeedValue = 0;
                    
                    //проверяем первая передача или нет
                    if (currentSpeed <= speedOnGearsIndex * 2f)
                    {
                        //первая передача
                        startGearSpeedValue = 0;
                        topGearSpeedValue = speedOnGearsIndex * 2f;
                        firstGear = true;
                    }
                    else
                    {
                        for (int gear = 2; gear <= gears; gear++)
                        {
                            float currentGearStartSpeed = speedOnGearsIndex * gear;
                            float currentGearMaxSpeed = speedOnGearsIndex * (gear + 1);
                            if (currentSpeed >= currentGearStartSpeed && currentSpeed < currentGearMaxSpeed)
                            {
                                startGearSpeedValue = currentGearStartSpeed;
                                topGearSpeedValue = currentGearMaxSpeed;
                            }
                        }
                    }

                    //высчитали нижнюю и верхнюю скорость на текущей передаче, и от 0 до 1 приводим это всё
                    float indexOfSpeedT = currentSpeed - startGearSpeedValue;
                    float maxT = topGearSpeedValue - startGearSpeedValue;
                    float indexT = indexOfSpeedT / maxT;

                    if (firstGear)
                        tahometerComp.NextValueRpm = 1000f * (5f * indexT + 1f);
                    else
                        tahometerComp.NextValueRpm = 500f * (5f * indexT + 7f);
                }
                else
                {
                    float speedOfTahoChangeInFly = 1000f;
                    tahometerComp.NextValueRpm = tahometerComp.CurrentValueRpm - speedOfTahoChangeInFly/* / 2f*/;
                }

                ClampAndSmoothTahometer(ref tahometerComp);
            }

            foreach (var entity in _playerCarMoveBackwardFilter.Value)
            {
                ref var tahometerComp = ref _tahoPool.Value.Get(entity);
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);

                float maxSpeed = _state.Value.PlayerConfig.BackMaxSpeedKmh;
                float indexT = statisticComp.SpeedKmpH / (float)maxSpeed;
                tahometerComp.NextValueRpm = 1000f * (5f * indexT + 1f);

                ClampAndSmoothTahometer(ref tahometerComp);
            }
        }

        private void HandlePlayerFlyCar()
        {
            foreach (var entity in _playerFlyCarFilter.Value)
            {
                ref var tahoComp = ref _tahoPool.Value.Get(entity);
                ref var controllComp = ref _controllPool.Value.Get(entity);

                float speedOfTahoChangeInFly = 1000f;
                if (controllComp.VerticalInput == 0)
                    tahoComp.NextValueRpm = tahoComp.CurrentValueRpm - speedOfTahoChangeInFly/* / 2f*/;
                else
                    tahoComp.NextValueRpm = tahoComp.CurrentValueRpm + (speedOfTahoChangeInFly * controllComp.VerticalInput);

                ClampAndSmoothTahometer(ref tahoComp);
            }
        }

        private void HandleNotPlayerCar()
        {
            foreach (var entity in _notPlayerCarFilter.Value)
            {

            }
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