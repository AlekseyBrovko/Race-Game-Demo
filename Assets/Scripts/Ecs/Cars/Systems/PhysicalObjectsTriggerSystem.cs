using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PhysicalObjectsTriggerSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsFilterInject<Inc<PhysicalObjectTriggerEvent>> _filter = default;
        private EcsPoolInject<PhysicalObjectTriggerEvent> _eventPool = default;
        private EcsPoolInject<PhysicalObjectComp> _physicalObjectPool = default;
        private EcsPoolInject<PhysicalObjectStartFlyEvent> _startFlyPool = default;
        //private EcsPoolInject<PhysicalObjectIsMovingByCarComp> _movingByCarPool = default;
        private EcsPoolInject<PhysicalObjectIsFlyingComp> _flyPool = default;
        private EcsPoolInject<HitPedestrianByPhysicalObjectEvent> _hitNpcByObjectPool = default;

        private float _defaultSpeedThreshold = 0f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var triggerComp = ref _eventPool.Value.Get(entity);

                //пока в сторонку
                //bool isMovingByCar = false;
                //if (_movingByCarPool.Value.Has(triggerComp.FlyingObject.Entity))
                //    isMovingByCar = true;
                //else
                //    isMovingByCar = false;

                //этот баг вроде пофикшен нормальным способом, нет не пофикшен
                if (!_flyPool.Value.Has(triggerComp.FlyingObject.Entity))
                {
                    _eventPool.Value.Del(entity);
                    continue;
                }

                ref var flyObjectComp = ref _flyPool.Value.Get(triggerComp.FlyingObject.Entity);
                
                if (triggerComp.Interactable is ITriggerInteractableWithThreshold)
                {
                    var triggerMb = triggerComp.Interactable as ITriggerInteractableWithThreshold;
                    if (!triggerMb.HasFirstTrigger)
                    {
                        //ref var carStatisticComp = ref _statisticPool.Value.Get(triggerComp.CarEntity);
                        ref var flyComp = ref _flyPool.Value.Get(triggerComp.FlyingObject.Entity);

                        float threshold = _defaultSpeedThreshold;
                        if (triggerMb.SpeedThreshold != null)
                            threshold = triggerMb.SpeedThreshold.Value;

                        if (flyComp.SpeedKmH >= threshold)
                        {
                            InitAndMakePhysical(triggerComp.Interactable as IPhysicalObject);
                            triggerMb.OnFirstTrigger();
                        }
                        _eventPool.Value.Del(entity);
                        continue;
                    }
                }

                if (triggerComp.Interactable is ICollisionInteractableWithThreshold)
                {
                    var collisionMb = triggerComp.Interactable as ICollisionInteractableWithThreshold;
                    if (!collisionMb.HasFirstCollision)
                    {
                        _eventPool.Value.Del(entity);
                        continue;
                    }   
                }

                if (triggerComp.Interactable is IPhysicalObject)
                {
                    InitAndMakePhysical(triggerComp.Interactable as IPhysicalObject);
                    _eventPool.Value.Del(entity);
                    continue;
                }

                if (triggerComp.Interactable is INpcMb)
                {
                    INpcMb npcMb = triggerComp.Interactable as INpcMb;
                    ref var hitComp = ref _hitNpcByObjectPool.Value.Add(_world.Value.NewEntity());
                    hitComp.PadastrianEntity = npcMb.Entity;
                    hitComp.PhysicalObject = triggerComp.FlyingObject;
                }

                _eventPool.Value.Del(entity);
            }
        }

        private void InitAndMakePhysical(IPhysicalObject physicalMb)
        {
            if (!physicalMb.HasInited)
            {
                int physicalEntity = _world.Value.NewEntity();
                physicalMb.InitOnFirstTriggerWithCar(_world.Value, physicalEntity);

                ref var physicalObjectComp = ref _physicalObjectPool.Value.Add(physicalEntity);
                physicalObjectComp.PhysicalMb = physicalMb;
            }

            if (!physicalMb.IsPhysical)
            {
                if (physicalMb.OnBeganPhysical())
                {
                    ref var startFlyComp = ref _startFlyPool.Value.Add(physicalMb.Entity);
                    startFlyComp.PhysicalMb = physicalMb;
                }
            }

            //TODO тут походу по цепочке надо
            //if (!_movingByCarPool.Value.Has(physicalMb.Entity))
            //    _movingByCarPool.Value.Add(physicalMb.Entity);
        }
    }
}