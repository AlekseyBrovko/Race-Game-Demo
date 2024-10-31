using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarWheelSlipEffectsSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<WheelComp>> _filter = default;
        private EcsPoolInject<WheelComp> _wheelPool = default;

        private EcsFilterInject<Inc<StartSkidmarksEvent>> _startSkidmarksFilter = default;
        private EcsFilterInject<Inc<EndSkidmarksEvent>> _endSkidmarksFilter = default;

        private EcsPoolInject<SkidmarksComp> _skidmarksPool = default;
        private EcsPoolInject<StartSkidmarksEvent> _startSkidMarksPool = default;
        private EcsPoolInject<EndSkidmarksEvent> _endSkidmarksPool = default;

        private EcsFilterInject<Inc<SmokeComp>> _smokeFilter = default;
        private EcsPoolInject<SmokeComp> _smokePool = default;

        private float _particleSlipThreshold = 0.4f;
        private float _skidmarksSlipThreshold = 0.2f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var wheelComp = ref _wheelPool.Value.Get(entity);
                Wheel wheel = wheelComp.Wheel;
                if (wheel.WheelController.IsGrounded)
                {
                    float indexOfSlip = Mathf.Max(Mathf.Abs(wheel.WheelController.forwardFriction.slip),
                        Mathf.Abs(wheel.WheelController.sideFriction.slip));

                    //skidmarks
                    if (indexOfSlip > _skidmarksSlipThreshold && !_skidmarksPool.Value.Has(entity))
                    {
                        _skidmarksPool.Value.Add(entity);
                        if (!_startSkidMarksPool.Value.Has(entity))
                            _startSkidMarksPool.Value.Add(entity);
                    }

                    if (_skidmarksPool.Value.Has(entity))
                    {
                        ref var skidComp = ref _skidmarksPool.Value.Get(entity);
                        skidComp.IndexOfSlip = indexOfSlip;
                    }

                    if (indexOfSlip < _skidmarksSlipThreshold && _skidmarksPool.Value.Has(entity))
                    {
                        _skidmarksPool.Value.Del(entity);

                        if (!_endSkidmarksPool.Value.Has(entity))
                            _endSkidmarksPool.Value.Add(entity);
                    }

                    //smoke
                    if (indexOfSlip > _particleSlipThreshold && !_smokePool.Value.Has(entity))
                        _smokePool.Value.Add(entity);

                    if (indexOfSlip < _particleSlipThreshold && _smokePool.Value.Has(entity))
                        _smokePool.Value.Del(entity);
                }
                else
                {
                    if (_skidmarksPool.Value.Has(entity))
                    {
                        _skidmarksPool.Value.Del(entity);
                        if (!_endSkidmarksPool.Value.Has(entity))
                            _endSkidmarksPool.Value.Add(entity);
                    }
                    if (_smokePool.Value.Has(entity))
                        _smokePool.Value.Del(entity);
                }
            }

            foreach (var entity in _startSkidmarksFilter.Value)
            {
                ref var wheelComp = ref _wheelPool.Value.Get(entity);
                wheelComp.Wheel.EffectsMb.SkidmarksTrail.emitting = true;
                //_startSkidMarksPool.Value.Del(entity);
            }

            foreach (var entity in _endSkidmarksFilter.Value)
            {
                ref var wheelComp = ref _wheelPool.Value.Get(entity);
                wheelComp.Wheel.EffectsMb.SkidmarksTrail.emitting = false;
            }

            foreach (var entity in _smokeFilter.Value)
            {
                ref var wheelComp = ref _wheelPool.Value.Get(entity);
                foreach (var patricle in wheelComp.Wheel.EffectsMb.SmokeParticles)
                    patricle.Emit(1);
            }
        }
    }
}