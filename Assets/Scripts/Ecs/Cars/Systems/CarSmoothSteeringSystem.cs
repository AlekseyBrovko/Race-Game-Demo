using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarSmoothSteeringSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarComp>> _filter = default;
        private EcsPoolInject<CarControllComp> _controllPool = default;
        private EcsPoolInject<CarComp> _carPool = default;

        private EcsFilterInject<Inc<CarComp, WheelSteeringComp>> _steeringFilter = default;
        private EcsPoolInject<WheelSteeringComp> _wheelSteeringPool = default;

        private float _indexOfReturnSpeed = 6f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var carComp = ref _carPool.Value.Get(entity);

                //Этот тип управления подойдёт через кнопки, но не подойдёт от джойстика

                if (controllComp.HorizontalInputPreviousFrame != controllComp.HorizontalInput)
                {
                    if (!_wheelSteeringPool.Value.Has(entity))
                        _wheelSteeringPool.Value.Add(entity);

                    ref var wheelSteeringComp = ref _wheelSteeringPool.Value.Get(entity);
                    float horInputDelta = Mathf.Abs(controllComp.HorizontalInputPreviousFrame - controllComp.HorizontalInput);

                    float timeToTurn;

                    if (controllComp.HorizontalInput == 0)
                        timeToTurn = horInputDelta * carComp.CarMb.SteeringSpeed / _indexOfReturnSpeed;
                    else
                        timeToTurn = horInputDelta * carComp.CarMb.SteeringSpeed;

                    //это надо переделать, разные машины будут с разной маневренностью
                    wheelSteeringComp.DurationTime = timeToTurn;
                    wheelSteeringComp.CurrentTime = timeToTurn;
                    wheelSteeringComp.NewValue = controllComp.HorizontalInput;
                    wheelSteeringComp.OldValue = controllComp.HorizontalInputPreviousFrame;
                }
                controllComp.HorizontalInputPreviousFrame = controllComp.HorizontalInput;
            }

            foreach (var entity in _steeringFilter.Value)
            {
                ref var controllComp = ref _controllPool.Value.Get(entity);
                ref var wheelSteeringComp = ref _wheelSteeringPool.Value.Get(entity);

                controllComp.HorizontalCurrent = Mathf.Lerp(controllComp.HorizontalCurrent, wheelSteeringComp.NewValue,
                    1 - wheelSteeringComp.CurrentTime / wheelSteeringComp.DurationTime);

                wheelSteeringComp.CurrentTime -= Time.fixedDeltaTime;

                if (wheelSteeringComp.CurrentTime <= 0)
                {
                    controllComp.HorizontalCurrent = wheelSteeringComp.NewValue;

                    _wheelSteeringPool.Value.Del(entity);
                }
            }
        }
    }
}