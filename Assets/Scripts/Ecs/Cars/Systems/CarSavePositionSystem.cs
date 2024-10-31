using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarSavePositionSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarSavePositionComp>> _filter = default;
        private EcsPoolInject<CarSavePositionComp> _savePool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<CarWheelsControllComp> _wheelsPool = default;

        private float _timerDuration = 3f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var saveComp = ref _savePool.Value.Get(entity);
                saveComp.TimerForSave += Time.deltaTime;
                if (saveComp.TimerForSave > _timerDuration)
                {
                    saveComp.TimerForSave = 0f;
                    if (IsAllWheelsOnGround(entity))
                        SavePosition(entity);
                }
            }
        }

        private bool IsAllWheelsOnGround(int entity)
        {
            ref var wheelsComp = ref _wheelsPool.Value.Get(entity);
            foreach (var wheel in wheelsComp.Wheels)
                if (!wheel.WheelController.IsGrounded)
                    return false;
            return true;
        }

        private void SavePosition(int entity)
        {
            ref var saveComp = ref _savePool.Value.Get(entity);
            ref var viewComp = ref _viewPool.Value.Get(entity);

            saveComp.SavePosition = viewComp.Transform.rotation.eulerAngles;
            saveComp.SavePosition = viewComp.Transform.position;
        }
    }
}