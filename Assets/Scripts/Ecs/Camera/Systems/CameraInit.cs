using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CameraInit : IEcsInitSystem
    {
        public CameraInit(SceneIniterMb sceneIniter = null)
        {
            _sceneIniter = sceneIniter;
        }

        private SceneIniterMb _sceneIniter;

        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;
        private EcsPoolInject<CameraComp> _cameraPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<VirtualCameraComp> _virtualCameraPool = default;

        public void Init(EcsSystems systems)
        {
            int cameraEntity = _world.Value.NewEntity();
            _state.Value.CameraEntity = cameraEntity;

            GameObject cameraGo = default;
            GameObject vcGo = default;
            GameObject freeLookCamera = default;

            if (_sceneIniter == null)
            {
                cameraGo = GameObject.FindGameObjectWithTag("MainCamera");
                vcGo = GameObject.FindGameObjectWithTag("Virtual Camera");
                freeLookCamera = GameObject.FindGameObjectWithTag("FreeLook Camera");
            }
            else
            {
                cameraGo = _sceneIniter.MainCamera;
                vcGo = _sceneIniter.VirtualCamera.gameObject;
                freeLookCamera = _sceneIniter.FreeLookCamera.gameObject;
            }

            ref var cameraComp = ref _cameraPool.Value.Add(cameraEntity);
            cameraComp.Camera = cameraGo.GetComponent<Camera>();

            ref var viewComp = ref _viewPool.Value.Add(cameraEntity);
            viewComp.Transform = cameraGo.transform;

            ref var virtualCameraComp = ref _virtualCameraPool.Value.Add(cameraEntity);
            virtualCameraComp.CinemachineBrain = cameraGo.GetComponent<CinemachineBrain>();

            if (vcGo != null)
                virtualCameraComp.VirtualCamera = vcGo.GetComponent<CinemachineVirtualCamera>();

            if (freeLookCamera != null)
            {
                virtualCameraComp.FreelookCamera = freeLookCamera.GetComponent<CinemachineFreeLook>();
                virtualCameraComp.FreelookCamera.enabled = false;
            }
        }
    }
}