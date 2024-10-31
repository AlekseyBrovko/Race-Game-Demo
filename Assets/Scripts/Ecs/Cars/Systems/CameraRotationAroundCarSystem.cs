using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CameraRotationAroundCarSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<CameraInputRotationComp>> _filter = default;
        private EcsPoolInject<CameraInputRotationComp> _cameraRotationPool = default;
        private EcsPoolInject<InputComp> _inputPool = default;
        private EcsPoolInject<VirtualCameraComp> _cameraPool = default;

        private float _durationToDefaultTransition = 2f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var inputComp = ref _inputPool.Value.Get(_state.Value.InputEntity);
                ref var cameraRotationComp = ref _cameraRotationPool.Value.Get(entity);
                ref var cameraComp = ref _cameraPool.Value.Get(_state.Value.CameraEntity);

                if (inputComp.TouchOrMouseInput != Vector2.zero)
                {
                    cameraRotationComp.TimerToDefaultTransition = 0;
                    cameraComp.FreelookCamera.m_XAxis.Value += inputComp.TouchOrMouseInput.x * Time.deltaTime * 10f;
                    cameraComp.FreelookCamera.m_YAxis.Value += inputComp.TouchOrMouseInput.y * Time.deltaTime / 5f;
                }
                    

                cameraRotationComp.TimerToDefaultTransition += Time.deltaTime;

                if (cameraRotationComp.TimerToDefaultTransition > _durationToDefaultTransition)
                {
                    

                    cameraComp.FreelookCamera.enabled = false;
                    cameraComp.VirtualCamera.enabled = true;

                    _cameraRotationPool.Value.Del(entity);
                }
            }
        }
    }
}