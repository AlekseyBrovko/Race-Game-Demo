using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PhysicalObjectCollisionSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsFilterInject<Inc<PhysicalObjectCollisionEvent>> _filter = default;
        private EcsPoolInject<PhysicalObjectCollisionEvent> _collisionPool = default;
        private EcsPoolInject<CollisionPedestrianByPhysicalObjectEvent> _collisionInNpcPool = default;
        private EcsPoolInject<PhysicalObjectIsMovingByCarComp> _movingByCarPool = default;
        private EcsPoolInject<PhysicalObjectIsFlyingComp> _flyPool = default;
        private EcsPoolInject<PhysicalObjectComp> _physicalObjectPool = default;
        private EcsPoolInject<PhysicalObjectStartFlyEvent> _startFlyPool = default;

        private float _defaultSpeedThreshold = 20f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                bool isMovingByCar = false;

                ref var collisionComp = ref _collisionPool.Value.Get(entity);

                if (_movingByCarPool.Value.Has(collisionComp.FlyingObject.Entity))
                    isMovingByCar = true;
                else
                    isMovingByCar = false;

                //if (!_flyPool.Value.Has(collisionComp.FlyingObject.Entity))
                //{
                //    _collisionPool.Value.Del(entity);
                //    continue;
                //}

                if (collisionComp.Interactable is IPhysicalObject)
                {
                    var physicalMb = collisionComp.Interactable as IPhysicalObject;
                    if (physicalMb.IsPhysical)
                    {
                        _collisionPool.Value.Del(entity);
                        continue;
                    }

                    if (collisionComp.Interactable is ICollisionInteractableWithThreshold)
                    { 
                        if (isMovingByCar)
                        {
                            var collisionMb = collisionComp.Interactable as ICollisionInteractableWithThreshold;

                            if (!collisionMb.HasFirstCollision)
                            {
                                ref var flyComp = ref _flyPool.Value.Get(collisionComp.FlyingObject.Entity);

                                float threshold = _defaultSpeedThreshold;
                                if (collisionMb.SpeedThreshold != null)
                                    threshold = collisionMb.SpeedThreshold.Value;

                                if (flyComp.PreviousFrameSpeedKmH > threshold)
                                {
                                    collisionMb.OnFirstCollision();
                                    InitAndMakePhysical(collisionComp.Interactable as IPhysicalObject);

                                    _collisionPool.Value.Del(entity);
                                    continue;
                                }
                            }
                        }
                    }
                }
                else if (collisionComp.Interactable is INpcMb)
                {
                    INpcMb npcMb = collisionComp.Interactable as INpcMb;
                    ref var collisionToNpcComp = ref _collisionInNpcPool.Value.Add(_world.Value.NewEntity());
                    collisionToNpcComp.PadastrianEntity = npcMb.Entity;
                    collisionToNpcComp.PhysicalObject = collisionComp.FlyingObject;
                }

                _collisionPool.Value.Del(entity);
            }
        }

        private void InitAndMakePhysical(IPhysicalObject physicalMb)
        {
            if (!physicalMb.HasInited)
            {
                int physicalEntity = _world.Value.NewEntity();
                physicalMb.InitOnFirstTriggerWithCar(_world.Value, physicalEntity);

                //TODO вот здесь на боочке и лезут баги
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
        }
    }
}