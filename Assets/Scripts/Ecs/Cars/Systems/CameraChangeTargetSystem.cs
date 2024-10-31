using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CameraChangeTargetSystem : IEcsRunSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<ChangeCameraTargetEvent>> _filter = default;
        private EcsFilterInject<Inc<CameraFollowComp>> _cameraFollowFilter = default;

        private EcsPoolInject<VirtualCameraComp> _cameraPool = default;
        private EcsPoolInject<ChangeCameraTargetEvent> _changeEventPool = default;
        private EcsPoolInject<CameraFollowComp> _cameraFollowPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;
        private EcsPoolInject<CarComp> _carPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                foreach (var oldFollowEntity in _cameraFollowFilter.Value)
                {
                    ref var oldFollowComp = ref _cameraFollowPool.Value.Get(oldFollowEntity);
                    GameObject.Destroy(oldFollowComp.FollowTransform.gameObject);
                    GameObject.Destroy(oldFollowComp.LookAtTransform.gameObject);
                    _cameraFollowPool.Value.Del(entity);
                }

                ref var changeTargetComp = ref _changeEventPool.Value.Get(entity);
                if (changeTargetComp.TargetGo.TryGetComponent(out ICar carMb))
                {
                    GameObject lookAtGo = new GameObject("LookAtTransform");
                    GameObject followGo = new GameObject("FollowTransform");

                    ref var carViewComp = ref _viewPool.Value.Get(carMb.Entity);

                    lookAtGo.transform.rotation = carViewComp.Transform.rotation;
                    followGo.transform.rotation = carViewComp.Transform.rotation;

                    ref var followComp = ref _cameraFollowPool.Value.Add(carMb.Entity);
                    followComp.LookAtTransform = lookAtGo.transform;
                    followComp.FollowTransform = followGo.transform;

                    ref var cameraComp = ref _cameraPool.Value.Get(_state.Value.CameraEntity);
                    cameraComp.VirtualCamera.m_Follow = followGo.transform;
                    cameraComp.VirtualCamera.m_LookAt = lookAtGo.transform;

                    cameraComp.FreelookCamera.Follow = carViewComp.Transform;
                    cameraComp.FreelookCamera.LookAt = carViewComp.Transform;

                    CinemachineVirtualCamera rig_0 = cameraComp.FreelookCamera.GetRig(0);
                    CinemachineVirtualCamera rig_1 = cameraComp.FreelookCamera.GetRig(1);
                    CinemachineVirtualCamera rig_2 = cameraComp.FreelookCamera.GetRig(2);

                    rig_0.LookAt = carViewComp.Transform;
                    rig_1.LookAt = carViewComp.Transform;
                    rig_2.LookAt = carViewComp.Transform;

                    ref var carComp = ref _carPool.Value.Get(carMb.Entity);

                    float yOffset = 2.5f;
                    float zOffset = 5.33f;

                    if (carComp.CameraOffsetMb != null)
                    {
                        yOffset = carComp.CameraOffsetMb.CameraYOffset;
                        zOffset = carComp.CameraOffsetMb.CameraZOffset;
                    }

                    var transposerBodyBase = cameraComp.VirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
                    var bodyTransposer = transposerBodyBase as CinemachineTransposer;
                    bodyTransposer.m_FollowOffset = new Vector3(0f, yOffset, -zOffset);

                    Vector2 pos1 = new Vector2(zOffset, yOffset);
                    Vector2 zeroVector = Vector2.zero;
                    Vector3 direction = pos1 - zeroVector;
                    float distance = direction.magnitude;
                    float downHeight = distance * Mathf.Sin(10f * Mathf.Deg2Rad);
                    float downRadius = distance * Mathf.Cos(10f * Mathf.Deg2Rad);
                    float upHight = distance * Mathf.Sin(60f * Mathf.Deg2Rad);
                    float upRadius = distance * Mathf.Cos(60f * Mathf.Deg2Rad);

                    cameraComp.FreelookCamera.m_Orbits[0].m_Radius = downRadius;
                    cameraComp.FreelookCamera.m_Orbits[0].m_Height = downHeight;

                    cameraComp.FreelookCamera.m_Orbits[1].m_Radius = zOffset;
                    cameraComp.FreelookCamera.m_Orbits[1].m_Height = yOffset;

                    cameraComp.FreelookCamera.m_Orbits[2].m_Radius = upRadius;
                    cameraComp.FreelookCamera.m_Orbits[2].m_Height = upHight;

                    Vector3 offsetForRig = new Vector3(0, yOffset / 2f, 0);

                    var composerBase0 = rig_0.GetCinemachineComponent<CinemachineComposer>();
                    composerBase0.m_TrackedObjectOffset = offsetForRig;

                    var composerBase1 = rig_1.GetCinemachineComponent<CinemachineComposer>();
                    composerBase1.m_TrackedObjectOffset = offsetForRig;

                    var composerBase2 = rig_2.GetCinemachineComponent<CinemachineComposer>();
                    composerBase2.m_TrackedObjectOffset = offsetForRig;

                    var composerBase = cameraComp.VirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Aim);
                    var aimComposer = composerBase as CinemachineComposer;
                    aimComposer.m_TrackedObjectOffset = 
                        new Vector3(0f, carComp.CameraOffsetMb.CameraLookAtYOffset, 0f);
                }
                _changeEventPool.Value.Del(entity);
            }
        }
    }
}