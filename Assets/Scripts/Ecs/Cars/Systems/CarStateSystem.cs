using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class CarStateSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarComp>, Exc<CarIsFlyComp>> _carIsGroundFilter = default;
        private EcsFilterInject<Inc<CarIsIdleComp>> _idleFilter = default;
        private EcsFilterInject<Inc<CarIsMovingForwardComp>> _forwardFilter = default;
        private EcsFilterInject<Inc<CarIsMovingBackwardComp>> _backFilter = default;
        private EcsFilterInject<Inc<CarIsFlyComp>> _flyFilter = default;
        private EcsFilterInject<Inc<CarIsDriftComp>> _driftFilter = default;

        private EcsPoolInject<CarStatisticComp> _statisticPool = default;
        private EcsPoolInject<CarControllComp> _controllPool = default;
        private EcsPoolInject<CarFsmControllComp> _fsmPool = default;

        private EcsPoolInject<CarWheelsControllComp> _wheelsPool = default;
        private EcsPoolInject<CarMoveDirectionComp> _directionPool = default;

        private float _idleThreshold = 3f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _carIsGroundFilter.Value)
            {
                ref var wheelsComp = ref _wheelsPool.Value.Get(entity);
                if (IsTotalFly(wheelsComp.Wheels))
                {
                    ref var fsmComp = ref _fsmPool.Value.Get(entity);
                    fsmComp.CarControllFsm.SetFlyState();
                }
            }

            foreach (var entity in _flyFilter.Value)
            {
                ref var wheelsComp = ref _wheelsPool.Value.Get(entity);
                if (!IsTotalFly(wheelsComp.Wheels))
                {
                    ref var directionComp = ref _directionPool.Value.Get(entity);
                    ref var fsmComp = ref _fsmPool.Value.Get(entity);
                    ref var statisticComp = ref _statisticPool.Value.Get(entity);

                    if (statisticComp.SpeedKmpH < _idleThreshold)
                        fsmComp.CarControllFsm.SetIdleState();
                    else if (directionComp.AngleBetweenRbAndForwardDir > 90f)
                        fsmComp.CarControllFsm.SetMoveBackState();
                    else
                        fsmComp.CarControllFsm.SetMoveForwardState();
                }
            }

            foreach (var entity in _idleFilter.Value)
            {
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var fsmComp = ref _fsmPool.Value.Get(entity);

                if (controllComp.VerticalInput > 0)
                    fsmComp.CarControllFsm.SetMoveForwardState();

                if (controllComp.VerticalInput < 0)
                    fsmComp.CarControllFsm.SetMoveBackState();

                if (statisticComp.SpeedKmpH > _idleThreshold)
                    fsmComp.CarControllFsm.SetMoveForwardState();
            }

            foreach (var entity in _forwardFilter.Value)
            {
                ref var directionComp = ref _directionPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var fsmComp = ref _fsmPool.Value.Get(entity);

                if (statisticComp.SpeedKmpH < _idleThreshold)
                {
                    if (controllComp.VerticalInput == 0)
                        fsmComp.CarControllFsm.SetIdleState();

                    if (controllComp.VerticalInput < 0)
                        fsmComp.CarControllFsm.SetMoveBackState();
                }
                else if (directionComp.AngleBetweenRbAndForwardDir > 120f 
                    && controllComp.VerticalInput < 0)
                {
                    fsmComp.CarControllFsm.SetMoveBackState();
                }
                else if (statisticComp.SpeedKmpH > 20f && 
                    directionComp.AngleBetweenRbAndForwardDir > 20f &&
                    directionComp.AngleBetweenRbAndForwardDir < 170f &&
                    controllComp.VerticalInput > 0 &&
                    controllComp.HorizontalInput != 0)
                {
                    fsmComp.CarControllFsm.SetDriftState();
                }
            }

            foreach (var entity in _backFilter.Value)
            {
                ref var directionComp = ref _directionPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var fsmComp = ref _fsmPool.Value.Get(entity);

                if (statisticComp.SpeedKmpH < _idleThreshold)
                {
                    if (controllComp.VerticalInput == 0)
                        fsmComp.CarControllFsm.SetIdleState();

                    if (controllComp.VerticalInput > 0)
                        fsmComp.CarControllFsm.SetMoveForwardState();
                }
                else if (directionComp.AngleBetweenRbAndForwardDir < 90f && controllComp.VerticalInput > 0)
                {
                    fsmComp.CarControllFsm.SetMoveForwardState();
                }
            }

            foreach(var entity in _driftFilter.Value)
            {
                ref var directionComp = ref _directionPool.Value.Get(entity);
                ref var statisticComp = ref _statisticPool.Value.Get(entity);
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var fsmComp = ref _fsmPool.Value.Get(entity);

                if (controllComp.VerticalInput < 0)
                    fsmComp.CarControllFsm.SetMoveForwardState();

                if (controllComp.VerticalInput == 0 && statisticComp.SpeedKmpH < _idleThreshold)
                    fsmComp.CarControllFsm.SetIdleState();

                if (directionComp.AngleBetweenRbAndForwardDir < 10f)
                    fsmComp.CarControllFsm.SetMoveForwardState();
            }
        }

        private bool IsTotalFly(Wheel[] wheels)
        {
            foreach (var wheel in wheels)
                if (wheel.WheelController.IsGrounded)
                    return false;
            return true;
        }
    }
}