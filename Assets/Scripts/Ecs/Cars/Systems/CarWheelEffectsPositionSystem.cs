using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using WheelHit = NWH.WheelController3D.WheelHit;

namespace Client
{
    sealed class CarWheelEffectsPositionSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<WheelComp>> _filter = default;
        private EcsPoolInject<WheelComp> _wheelPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var wheelComp = ref _wheelPool.Value.Get(entity);
                if (wheelComp.Wheel.WheelController.IsGrounded)
                {
                    var hitPoint = wheelComp.Wheel.WheelController.HitPoint;
                    wheelComp.Wheel.EffectsMb.gameObject.transform.position = hitPoint + Vector3.up * 0.1f;
                }
            }
        }
    }
}