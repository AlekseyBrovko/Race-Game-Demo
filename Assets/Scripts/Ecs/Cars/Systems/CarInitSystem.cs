using FMODUnity;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client
{
    sealed class CarInitSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<CarInitEvent>> _filter = default;

        private EcsPoolInject<CarInitEvent> _initPool = default;
        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<CarWheelsControllComp> _wheelControllPool = default;
        private EcsPoolInject<CarStatisticComp> _carStatisticPool = default;
        private EcsPoolInject<PlayerCarInitEvent> _playerCarInitPool = default;
        private EcsPoolInject<CarControllComp> _carControllPool = default;

        private EcsPoolInject<CarWcForwardSlipStatisticComp> _forwardSlipPool = default;
        private EcsPoolInject<CarWcSideSlipStaticticComp> _sideSlipPool = default;
        private EcsPoolInject<CarMotorAndBrakeTorqueStatisticComp> _torquePool = default;
        private EcsPoolInject<CarWcRpmStaticticComp> _rpmPool = default;
        private EcsPoolInject<IndexOfSpeedStatisticComp> _speedIndexPool = default;

        private EcsPoolInject<CarFsmControllComp> _fsmPool = default;
        private EcsPoolInject<WheelComp> _wheelPool = default;
        private EcsPoolInject<CarSavePositionComp> _savePool = default;
        private EcsPoolInject<CarTahometerComp> _tahometorPool = default;
        private EcsPoolInject<CarMoveDirectionComp> _directionPool = default;

        private EcsPoolInject<CarColliderComp> _carColliderPool = default;

        private EcsPoolInject<LayerMaskComp> _layersPool = default;
        private EcsPoolInject<HpComp> _hpPool = default;

        private EcsPoolInject<CarSoundComp> _soundPool = default;

        //TODO максимальные и минимальные значения тахометра лучше пока не трогать,
        //придётся перерабатывать формулы рассчёта тахометра от скорости
        private float _maxTahoRpm = 6000f;
        private float _minTahoRpm = 1000f;

        private int _transmissionGearsAmountByDefault = 3;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var carInitComp = ref _initPool.Value.Get(entity);
                GameObject carGo = carInitComp.CarGo;
                ICar carMb = carGo.GetComponent<ICar>();
                carMb.Init(_state.Value, entity);

                ref var viewComp = ref _viewPool.Value.Add(entity);
                viewComp.Transform = carGo.transform;

                ref var carComp = ref _carPool.Value.Add(entity);
                carComp.CarMb = carMb;
                carComp.Wheels = carMb.Wheels;
                carComp.Rigidbody = carMb.Rigidbody;
                carComp.CameraOffsetMb = carGo.GetComponent<ICameraOffset>();
                carComp.DriftHelpIndex = carMb.DriftHelpIndex;
                carComp.CarTransform = carGo.transform;
                carComp.DeathSmokeParticles = carMb.DeathSmokeParticles;
                carComp.ArmorGameObject = carMb.ArmorGameObject;

                //если расширять, то надо править
                PlayerCarSo carSo = _state.Value.PlayerCarsPrefabsConfig.GetCarSoById(_state.Value.CurrentPlayerCar);

                if (_state.Value.StartGameTutorial)
                    carSo = _state.Value.PlayerCarsPrefabsConfig.TutorialCar;

                if (carSo == null)
                    Debug.LogWarning("carSo == null");

                SavedCar carSaveData = _state.Value.PlayersSavedCars
                    .FirstOrDefault(x => x.Id == _state.Value.CurrentPlayerCar);

                int maxSpeed = carSo.MaxSpeedLevels[carSaveData.MaxSpeedLevel].Value;
                int horcePower = carSo.HorcePowerLevels[carSaveData.HorcePowerLevel].Value;
                int hpLevel = carSo.HpLevels[carSaveData.HpLevel].Value;
                int fuelLevel = carSo.FuelLevels[carSaveData.FuelLevel].Value;

                if (_state.Value.StartGameTutorial)
                {
                    maxSpeed = carSo.MaxSpeedLevels[carSo.MaxSpeedLevels.Length - 1].Value;
                    horcePower = carSo.HorcePowerLevels[carSo.HorcePowerLevels.Length - 1].Value;
                    hpLevel = carSo.HpLevels[carSo.HpLevels.Length - 1].Value;
                    fuelLevel = carSo.FuelLevels[carSo.FuelLevels.Length - 1].Value;
                }

                CarCharacteristics carStats = new CarCharacteristics(
                    maxSpeed, horcePower, hpLevel, fuelLevel);

                carComp.CarCharacteristic = carStats;

                if (carSaveData.HpLevel == carSo.HpLevels.Length - 1)
                {
                    if (carMb.ArmorGameObject != null)
                        carMb.ArmorGameObject.SetActive(true);
                }
                else
                {
                    if (carMb.ArmorGameObject != null)
                        carMb.ArmorGameObject.SetActive(false);
                }

                if (carMb.TransmissionGearsAmount == 0)
                    carComp.TransmissionGearsAmount = _transmissionGearsAmountByDefault;
                else
                    carComp.TransmissionGearsAmount = carMb.TransmissionGearsAmount;

                var playerConfig = _state.Value.PlayerConfig;
                switch (carMb.CarType)
                {
                    case Enums.CarType.Simple:
                        carComp.ThrottleCurve = playerConfig.SimpleThrottleCurve;
                        break;

                    case Enums.CarType.Race:
                        carComp.ThrottleCurve = playerConfig.RaceThrottleCurve;
                        break;

                    case Enums.CarType.Drift:
                        carComp.ThrottleCurve = playerConfig.DriftThrottleCurve;
                        break;

                    case Enums.CarType.Offroad:
                        carComp.ThrottleCurve = playerConfig.OffroadThrottleCurve;
                        break;
                }

                ref var saveComp = ref _savePool.Value.Add(entity);
                saveComp.SavePosition = carGo.transform.position;

                _carStatisticPool.Value.Add(entity);
                _carControllPool.Value.Add(entity);

                _forwardSlipPool.Value.Add(entity);
                _sideSlipPool.Value.Add(entity);
                _torquePool.Value.Add(entity);
                _rpmPool.Value.Add(entity);
                _speedIndexPool.Value.Add(entity);
                _directionPool.Value.Add(entity);

                ref var tahoComp = ref _tahometorPool.Value.Add(entity);
                tahoComp.MaxRpm = _maxTahoRpm;
                tahoComp.MinRpm = _minTahoRpm;

                ref var carColliderComp = ref _carColliderPool.Value.Add(entity);
                carColliderComp.MainTriggerCollider = carMb.MainTriggerCollider;
                carColliderComp.ColliderStartSize = carMb.MainTriggerCollider.size;
                carColliderComp.ColliderStartPos = carMb.MainTriggerCollider.center;

                ref var carHpComp = ref _hpPool.Value.Add(entity);
                carHpComp.HpValue = carStats.HpLevel;
                carHpComp.FullHpValue = carStats.HpLevel;

                InitWheels(entity);
                InitCenterOfMass(entity);
                InitFsm(entity);
                InitMotorSound(carMb, entity);

                //for tests
                if (carGo.TryGetComponent(out PlayerCar playerCarMb))
                    _playerCarInitPool.Value.Add(entity);

                _initPool.Value.Del(entity);
            }
        }

        private void InitMotorSound(ICar carMb, int entity)
        {
            var soundFmodConfig = _state.Value.SoundFMODConfig;
            ref var soundComp = ref _soundPool.Value.Add(entity);
            soundComp.CarTransform = carMb.Transform;
            soundComp.MainRigidbody = carMb.Rigidbody;

            SoundFMODConfig config = _state.Value.SoundFMODConfig;
            EventReference motorSoundReference = default;

            switch (carMb.CarMotorType)
            {
                case Enums.CarMotorType.OldDiesel:
                    motorSoundReference = config.OldCarMotorSound;
                    break;

                case Enums.CarMotorType.Truck:
                    motorSoundReference = config.TruckMotorSound;
                    break;

                case Enums.CarMotorType.V8:
                    motorSoundReference = config.MuscleCarMotorSound;
                    break;

                case Enums.CarMotorType.FamilySedan:

                    break;
            }

            soundComp.EventInstance =
                RuntimeManager.CreateInstance(motorSoundReference);

            RuntimeManager.AttachInstanceToGameObject(
                soundComp.EventInstance, carMb.Transform, carMb.Rigidbody);

            soundComp.EventInstance.start();
            soundComp.EventInstance.release();
        }

        private void InitWheels(int entity)
        {
            ref var carComp = ref _carPool.Value.Get(entity);
            ref var wheelControllComp = ref _wheelControllPool.Value.Add(entity);

            wheelControllComp.Wheels = carComp.Wheels;

            ref var layersComp = ref _layersPool.Value.Get(_state.Value.LayerMaskEntity);

            foreach (var wheel in carComp.Wheels)
                wheel.WheelController.layerMask = layersComp.DefaultLayer;

            Vector3 leftFrontWheelPos = new Vector3();
            Vector3 leftRearWheelPos = new Vector3();
            Vector3 rightFrontWheelPos = new Vector3();

            List<Wheel> handBrakeWheels = new List<Wheel>();

            WheelAxisPair frontWheels = new WheelAxisPair();
            WheelAxisPair rearWheels = new WheelAxisPair();

            foreach (var wheel in carComp.Wheels)
            {
                if (wheel.Orientation == Enums.OrientationEnum.Front)
                {
                    if (wheel.Side == Enums.SideEnum.Left)
                    {
                        leftFrontWheelPos = wheel.WheelController.gameObject.transform.position;
                        wheelControllComp.FrontLeftWheel = wheel;
                        frontWheels.LeftWheel = wheel;
                    }
                    else
                    {
                        rightFrontWheelPos = wheel.WheelController.gameObject.transform.position;
                        wheelControllComp.FrontRightWheel = wheel;
                        frontWheels.RightWheel = wheel;
                    }
                }
                else
                {
                    if (wheel.Side == Enums.SideEnum.Left)
                    {
                        leftRearWheelPos = wheel.WheelController.gameObject.transform.position;
                        wheelControllComp.RearLeftWheel = wheel;
                        handBrakeWheels.Add(wheel);
                        rearWheels.LeftWheel = wheel;
                    }
                    else
                    {
                        wheelControllComp.RearRightWheel = wheel;
                        handBrakeWheels.Add(wheel);
                        rearWheels.RightWheel = wheel;
                    }
                }
            }
            carComp.WidthWheelBase = Vector3.Distance(leftFrontWheelPos, rightFrontWheelPos);
            carComp.LengthWheelBase = Vector3.Distance(leftFrontWheelPos, leftRearWheelPos);

            List<Wheel> drivingWheels = new List<Wheel>();

            switch (carComp.CarMb.WheelDriveType)
            {
                case Enums.WheelDriveType.Fwd:
                    wheelControllComp.DrivingWheelAxisPairs = new WheelAxisPair[] { frontWheels };

                    foreach (var wheel in carComp.Wheels)
                        if (wheel.Orientation == Enums.OrientationEnum.Front)
                            drivingWheels.Add(wheel);
                    break;

                case Enums.WheelDriveType.Rwd:
                    wheelControllComp.DrivingWheelAxisPairs = new WheelAxisPair[] { rearWheels };

                    foreach (var wheel in carComp.Wheels)
                        if (wheel.Orientation == Enums.OrientationEnum.Rear)
                            drivingWheels.Add(wheel);
                    break;

                case Enums.WheelDriveType.Awd:
                    wheelControllComp.DrivingWheelAxisPairs = new WheelAxisPair[] { frontWheels, rearWheels };

                    foreach (var wheel in carComp.Wheels)
                        drivingWheels.Add(wheel);
                    break;

                case Enums.WheelDriveType.Custom:
                    foreach (var wheel in carComp.Wheels)
                        if (wheel.IsCustomDrivingWheel)
                            drivingWheels.Add(wheel);
                    break;
            }
            wheelControllComp.DrivingWheels = drivingWheels.ToArray();
            wheelControllComp.HandBrakeWheels = handBrakeWheels.ToArray();

            wheelControllComp.WheelAxisPairs = new WheelAxisPair[] { frontWheels, rearWheels };

            foreach (var wheel in handBrakeWheels)
                wheel.SaveStartFrictionSettings();

            List<int> wheelEntities = new List<int>();
            foreach (var wheel in wheelControllComp.Wheels)
            {
                int wheelEntity = _world.Value.NewEntity();
                ref var wheelComp = ref _wheelPool.Value.Add(wheelEntity);
                wheelComp.Wheel = wheel;

                wheelComp.EventInstance = RuntimeManager.CreateInstance(
                    _state.Value.SoundFMODConfig.WheelSkidSound);
                Rigidbody rb = null;
                RuntimeManager.AttachInstanceToGameObject
                    (
                    wheelComp.EventInstance,
                    wheelComp.Wheel.WheelController.transform,
                    rb);
                wheelComp.EventInstance.start();
                wheelComp.EventInstance.release();

                wheelEntities.Add(wheelEntity);
            }
            wheelControllComp.WheelsEntities = wheelEntities.ToArray();

            if (IsCustomWheels(wheelControllComp.Wheels) || wheelControllComp.Wheels.Length > 4)
            {
                //если колёс больше 4
            }
        }

        private void InitCenterOfMass(int entity)
        {
            ref var carComp = ref _carPool.Value.Get(entity);
            Transform centerOfMass = carComp.CarMb.CenterOfMass;
            var rb = carComp.CarMb.Rigidbody;
            rb.centerOfMass = centerOfMass.transform.localPosition;
        }

        private void InitFsm(int entity)
        {
            ref var fsmComp = ref _fsmPool.Value.Add(entity);
            fsmComp.CarControllFsm = new CarControllFsm(_state.Value, entity);
        }

        private bool IsCustomWheels(Wheel[] wheels)
        {
            foreach (var wheel in wheels)
                if (wheel.IsCustomDrivingWheel)
                    return true;
            return false;
        }
    }
}