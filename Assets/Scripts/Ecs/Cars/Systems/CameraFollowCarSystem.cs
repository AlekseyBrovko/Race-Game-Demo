using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CameraFollowCarSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CameraFollowComp, CarComp>> _followFilter = default;
        private EcsFilterInject<Inc<CameraFollowComp, CarComp, CarIsMovingForwardComp>, Exc<CameraFlyAroundCarComp, CameraInputRotationComp>> _forwardFilter = default;

        private EcsFilterInject<Inc<CameraFollowComp, CarComp, CarIsMovingBackwardComp>, Exc<CameraListenCameraBackwardDriveComp, CameraInputRotationComp>> _backwardFilter = default;

        private EcsFilterInject<Inc<CameraFollowComp, CarComp, CarIsIdleComp>, Exc<CameraFlyAroundCarComp, CameraInputRotationComp>> _idleFilter = default;
        private EcsFilterInject<Inc<CameraFollowComp, CarComp, CarIsDriftComp>, Exc<CameraFlyAroundCarComp, CameraInputRotationComp>> _driftFilter = default;
        private EcsFilterInject<Inc<CameraFollowComp, CarComp, CarIsFlyComp>, Exc<CameraFlyAroundCarComp, CameraInputRotationComp>> _flyFilter = default;

        private EcsPoolInject<CameraFollowComp> _followPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;

        private float _smoothDamp = 0.05f;
        private float _velocityLookAt;
        private float _velocityFollow;

        //TODO вот эту штуку брать из компонента
        private float _lookAtPointOffset = 3f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _followFilter.Value)
            {
                ref var followComp = ref _followPool.Value.Get(entity);
                ref var viewComp = ref _viewPool.Value.Get(entity);
                Vector3 carPos = viewComp.Transform.position;
                Vector3 directionToLook = followComp.LookAtTransform.position - followComp.FollowTransform.position;
                followComp.FollowTransform.rotation = Quaternion.LookRotation(directionToLook);
                followComp.FollowTransform.position = GetSmoothYpos(followComp.FollowTransform.position, carPos, ref _velocityFollow);
            }

            foreach (var entity in _idleFilter.Value)
                SetPositionForCameraInSimpleMoveForward(entity);

            foreach (var entity in _forwardFilter.Value)
                SetPositionForCameraInSimpleMoveForward(entity);

            foreach (var entity in _driftFilter.Value) 
                SetPositionForCameraInSimpleMoveForward(entity);

            foreach (var entity in _flyFilter.Value)
                SetPositionForCameraInSimpleMoveForward(entity);

            foreach (var entity in _backwardFilter.Value)
            {
                ref var viewComp = ref _viewPool.Value.Get(entity);
                ref var followComp = ref _followPool.Value.Get(entity);

                Vector3 carPos = viewComp.Transform.position;
                Vector3 newLookAtPos = carPos - viewComp.Transform.forward * _lookAtPointOffset;
                followComp.LookAtTransform.position = GetSmoothYpos(followComp.LookAtTransform.position, newLookAtPos, ref _velocityLookAt);
            }
        }

        private void SetPositionForCameraInSimpleMoveForward(int entity)
        {
            ref var followComp = ref _followPool.Value.Get(entity);
            ref var viewComp = ref _viewPool.Value.Get(entity);

            Vector3 carPos = viewComp.Transform.position;
            Vector3 tempPos = carPos + viewComp.Transform.forward * _lookAtPointOffset;
            Vector3 newLookAtPos = new Vector3(tempPos.x, carPos.y, tempPos.z);
            followComp.LookAtTransform.position = GetSmoothYpos(followComp.LookAtTransform.position, newLookAtPos, ref _velocityLookAt);
        }

        private Vector3 GetSmoothYpos(Vector3 current, Vector3 next, ref float velocity)
        {
            float yCurrent = current.y;
            float yNext = next.y;
            float yResult = Mathf.SmoothDamp(yCurrent, yNext, ref velocity, _smoothDamp);
            Vector3 result = new Vector3(next.x, yResult, next.z);
            return result;
        }
    }
}