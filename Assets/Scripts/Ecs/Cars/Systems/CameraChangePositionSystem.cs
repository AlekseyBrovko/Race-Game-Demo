using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CameraChangePositionSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<CameraChangePositionEvent>> _filter = default;
        private EcsPoolInject<CameraChangePositionEvent> _eventPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;

        private float _xOffset = 5f;
        private float _yOffset = 5f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var viewCameraComp = ref _viewPool.Value.Get(_state.Value.CameraEntity);
                ref var viewPlayerComp = ref _viewPool.Value.Get(_state.Value.PlayerCarEntity);

                viewCameraComp.Transform.position = viewPlayerComp.Transform.position - 
                    viewPlayerComp.Transform.forward * _xOffset +
                    Vector3.up * _yOffset;

                viewCameraComp.Transform.rotation = Quaternion.LookRotation(
                    viewPlayerComp.Transform.position - viewCameraComp.Transform.position);

                _eventPool.Value.Del(entity);
            }
        }
    }
}