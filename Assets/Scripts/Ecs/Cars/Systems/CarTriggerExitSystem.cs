using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarTriggerExitSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CarTriggerExitEvent>> _filter = default;
        private EcsPoolInject<CarTriggerExitEvent> _eventPool = default;
        private EcsPoolInject<PhysicalObjectIsMovingByCarComp> _movingByCarPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var triggerExitComp = ref _eventPool.Value.Get(entity);
                if (triggerExitComp.Interactable is IPhysicalObject)
                {
                    var physicalObject = triggerExitComp.Interactable as IPhysicalObject;
                    if (_movingByCarPool.Value.Has(physicalObject.Entity))
                        _movingByCarPool.Value.Del(physicalObject.Entity);
                    //Debug.Log("CarTriggerExitSystem");
                }
                _eventPool.Value.Del(entity);
            }
        }
    }
}