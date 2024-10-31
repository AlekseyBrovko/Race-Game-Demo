using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.VisualScripting;
using UnityEngine;

namespace Client
{
    sealed class PhysicalObjectStartFlySystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PhysicalObjectStartFlyEvent>> _filter = default;
        private EcsPoolInject<PhysicalObjectStartFlyEvent> _eventPool = default;
        private EcsPoolInject<PhysicalObjectIsFlyingComp> _flyPool = default;

        //TODO тут так и лезет
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var eventComp = ref _eventPool.Value.Get(entity);

                if (_flyPool.Value.Has(entity))
                    continue;

                ref var flyComp = ref _flyPool.Value.Add(entity);

                //TODO тут нул вылетает периодически
                flyComp.ObjectTransform = eventComp.PhysicalMb.Transform;

                flyComp.Rigidbody = eventComp.PhysicalMb.Rigidbody;
                flyComp.TriggerCollider = eventComp.PhysicalMb.TriggerCollider;
                flyComp.ColliderStartPos = eventComp.PhysicalMb.TriggerCollider.center;

                _eventPool.Value.Del(entity);
            }
        }
    }
}