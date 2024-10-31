using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarWheelsSoundSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<StartSkidmarksEvent>> _startFilter = default;
        private EcsFilterInject<Inc<EndSkidmarksEvent>> _stopFilter = default;
        private EcsFilterInject<Inc<SkidmarksComp>> _skidmarksFilter = default;

        private EcsPoolInject<WheelComp> _wheelPool = default;
        private EcsPoolInject<SkidmarksComp> _skidmarksPool = default;

        private float _smooth = 0.1f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _startFilter.Value)
            {
                ref var wheelComp = ref _wheelPool.Value.Get(entity);
                wheelComp.EventInstance.setParameterByName("Slip", 0.3f);
            }

            foreach (var entity in _stopFilter.Value)
            {
                ref var wheelComp = ref _wheelPool.Value.Get(entity);
                wheelComp.EventInstance.setParameterByName("Slip", 0);
            }

            foreach (var entity in _skidmarksFilter.Value)
            {
                ref var wheelComp = ref _wheelPool.Value.Get(entity);
                ref var skidComp = ref _skidmarksPool.Value.Get(entity);

                float volumeTemp = skidComp.IndexOfSlip;
                volumeTemp = Mathf.Clamp(volumeTemp, 0, 1);

                skidComp.Volume = Mathf.SmoothDamp(skidComp.Volume, volumeTemp, ref skidComp.ForSmooth, _smooth);
                wheelComp.EventInstance.setParameterByName("Slip", skidComp.Volume);
            }
        }
    }
}