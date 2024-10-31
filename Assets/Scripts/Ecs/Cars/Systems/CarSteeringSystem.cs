using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarSteeringSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarControllComp>, Exc<CarIsDriftComp>> _filter = default;
        private EcsFilterInject<Inc<CarIsDriftComp>> _driftFilter = default;

        private EcsPoolInject<CarControllComp> _controllPool = default;
        private EcsPoolInject<CarWheelsControllComp> _wheelsPool = default;
        private EcsPoolInject<CarComp> _carPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);
                ref var wheelComp = ref _wheelsPool.Value.Get(entity);

                if (controllComp.HorizontalCurrent > 0)
                {
                    wheelComp.FrontLeftWheel.WheelController.SteerAngle = Mathf.Rad2Deg * Mathf.Atan(carComp.LengthWheelBase /
                            (controllComp.TurningRadiusByLimiter + (carComp.WidthWheelBase / 2))) * controllComp.HorizontalCurrent;

                    wheelComp.FrontRightWheel.WheelController.SteerAngle = Mathf.Rad2Deg * Mathf.Atan(carComp.LengthWheelBase /
                            (controllComp.TurningRadiusByLimiter - (carComp.WidthWheelBase / 2))) * controllComp.HorizontalCurrent;
                }
                else if (controllComp.HorizontalCurrent < 0)
                {
                    wheelComp.FrontLeftWheel.WheelController.SteerAngle = Mathf.Rad2Deg * Mathf.Atan(carComp.LengthWheelBase /
                        (controllComp.TurningRadiusByLimiter - (carComp.WidthWheelBase / 2))) * controllComp.HorizontalCurrent;

                    wheelComp.FrontRightWheel.WheelController.SteerAngle = Mathf.Rad2Deg * Mathf.Atan(carComp.LengthWheelBase /
                        (controllComp.TurningRadiusByLimiter + (carComp.WidthWheelBase / 2))) * controllComp.HorizontalCurrent;
                }
                else if (controllComp.HorizontalCurrent == 0)
                {
                    wheelComp.FrontLeftWheel.WheelController.SteerAngle = Mathf.Rad2Deg * 0;
                    wheelComp.FrontRightWheel.WheelController.SteerAngle = Mathf.Rad2Deg * 0;
                }
            }

            foreach (var entity in _driftFilter.Value)
            {
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);
                ref var wheelComp = ref _wheelsPool.Value.Get(entity);
                
                if (controllComp.HorizontalCurrent == 0)
                {
                    wheelComp.FrontLeftWheel.WheelController.SteerAngle = Mathf.Rad2Deg * 0;
                    wheelComp.FrontRightWheel.WheelController.SteerAngle = Mathf.Rad2Deg * 0;
                }
                else
                {
                    wheelComp.FrontRightWheel.WheelController.SteerAngle = Mathf.Rad2Deg * Mathf.Atan(carComp.LengthWheelBase /
                            (controllComp.TurningRadiusByLimiter - (carComp.WidthWheelBase / 2))) * controllComp.HorizontalCurrent;
                    wheelComp.FrontLeftWheel.WheelController.SteerAngle = Mathf.Rad2Deg * Mathf.Atan(carComp.LengthWheelBase /
                            (controllComp.TurningRadiusByLimiter - (carComp.WidthWheelBase / 2))) * controllComp.HorizontalCurrent;
                }
            }
        }
    }
}