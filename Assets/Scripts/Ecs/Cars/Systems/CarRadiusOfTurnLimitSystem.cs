using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarRadiusOfTurnLimitSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<CarControllComp, CarIsMovingForwardComp>> _moveForwardFilter = default;
        private EcsFilterInject<Inc<CarControllComp, CarIsMovingBackwardComp>> _moveBackFilter = default;
        private EcsFilterInject<Inc<CarControllComp, CarIsDriftComp>> _driftFilter = default;

        private EcsPoolInject<CarControllComp> _controllPool = default;
        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<CarStatisticComp> _statisticPool = default;
        private EcsPoolInject<CarMoveDirectionComp> _directionPool = default;

        private float _minRadius = 1.5f;

        //коэффициент до * 6 на максимальной скорости
        private float maxIndexOfIncreaseTurnRadius = 6;

        public void Run(EcsSystems systems)
        {
            foreach(var entity in _moveForwardFilter.Value)
            {
                var settingsConfig = _state.Value.PlayerConfig;
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);    

                controllComp.TurningRadius = carComp.CarMb.TurningRadius;

                float speedIndex = settingsConfig.SteeringAnimationCurve.Evaluate(statisticComp.SpeedKmpH / 200f);
                controllComp.TurningRadiusByLimiter = (1 + maxIndexOfIncreaseTurnRadius * speedIndex) * controllComp.TurningRadius;
            }

            foreach (var entity in _moveBackFilter.Value)
            {
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);

                controllComp.TurningRadius = carComp.CarMb.TurningRadius;

                controllComp.TurningRadiusByLimiter = controllComp.TurningRadius * 0.7f;
            }

            foreach(var entity in _driftFilter.Value)
            {
                ref var directionComp = ref _directionPool.Value.Get(entity);
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);

                float angle = directionComp.AngleBetweenRbAndForwardDir;

                controllComp.TurningRadiusByLimiter = carComp.LengthWheelBase / Mathf.Sin(angle * Mathf.Deg2Rad);

                //слишком лихо выходит, надо убавить

                //TODO попробовать от скорости уменьшать, т.к. на небольших скоростях вполне неплохо рулится,
                //а на больших выворота не хватает

                //TODO нужно прокинуть из каждой машины, нужна подгонка по месту
                //controllComp.TurningRadiusByLimiter *= 1.25f;
                //controllComp.TurningRadiusByLimiter *= 1.1f;

                controllComp.TurningRadiusByLimiter *= carComp.DriftHelpIndex;

                if (controllComp.TurningRadiusByLimiter < _minRadius)
                    controllComp.TurningRadiusByLimiter = _minRadius;
            }
        }
    }
}