using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CameraListenBackwardMoving : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CameraFollowComp, StartBackDriveEvent>> _startBackwardfilter = default;
        private EcsFilterInject<Inc<CameraFollowComp, StopBackDriveEvent>> _stopBackwardfilter = default;
        private EcsFilterInject<Inc<CameraListenCameraBackwardDriveComp>> _backdriveFilter = default;
        private EcsPoolInject<CameraListenCameraBackwardDriveComp> _backdrivePool = default;

        private float _durationOfTransition = 2f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _backdriveFilter.Value)
            {
                ref var backdriveComp = ref _backdrivePool.Value.Get(entity);
                backdriveComp.Timer += Time.fixedDeltaTime;
                if (backdriveComp.Timer > _durationOfTransition)
                    _backdrivePool.Value.Del(entity);
            }

            foreach (var entity in _startBackwardfilter.Value)
            {
                if (!_backdrivePool.Value.Has(entity))
                    _backdrivePool.Value.Add(entity);
            }

            foreach (var entity in _stopBackwardfilter.Value)
            {
                if (_backdrivePool.Value.Has(entity))
                    _backdrivePool.Value.Del(entity);
            }
        }
    }
}