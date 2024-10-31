using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CameraStartRotationByInputSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<CameraFollowComp, CarComp>> _filter = default;
        private EcsPoolInject<CameraInputRotationComp> _rotationPool = default;
        private EcsPoolInject<InputComp> _inputPool = default;
        private EcsPoolInject<VirtualCameraComp> _cameraComp = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var inputComp = ref _inputPool.Value.Get(_state.Value.InputEntity);
                ref var cameraComp = ref _cameraComp.Value.Get(_state.Value.CameraEntity);

                if (inputComp.TouchOrMouseInput != Vector2.zero && !_rotationPool.Value.Has(entity))
                {
                    _rotationPool.Value.Add(entity);
                    cameraComp.VirtualCamera.enabled = false;
                    cameraComp.FreelookCamera.enabled = true;
                }

                //if (inputComp.TouchOrMouseInput != Vector2.zero && !_rotationPool.Value.Has(entity))
                //{
                //    _rotationPool.Value.Add(entity);

                //    var componentAimBase = cameraComp.VirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Aim);
                //    var aimComposer = componentAimBase as CinemachineComposer;
                //    aimComposer.m_SoftZoneHeight = 0;
                //    aimComposer.m_SoftZoneWidth = 0;
                //    aimComposer.m_LookaheadTime = 0;
                //    aimComposer.m_LookaheadSmoothing = 0;

                //    var transposerBodyBase = cameraComp.VirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
                //    var bodyTransposer = transposerBodyBase as CinemachineTransposer;
                //    bodyTransposer.m_XDamping = 0;
                //    bodyTransposer.m_YDamping = 0;
                //    bodyTransposer.m_ZDamping = 0;
                //    bodyTransposer.m_YawDamping = 0;
                //}
            }
        }
    }
}