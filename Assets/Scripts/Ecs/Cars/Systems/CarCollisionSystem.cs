using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarCollisionSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsFilterInject<Inc<CarCollisionEvent>> _filter = default;
        private EcsPoolInject<CarCollisionEvent> _collisionPool = default;
        private EcsPoolInject<ColisionPedestrianByCarEvent> _collisionPedestrianPool = default;
        private EcsPoolInject<PhysicalObjectComp> _physicalObjectPool = default;
        private EcsPoolInject<CarStatisticComp> _statisticPool = default;
        private EcsPoolInject<PhysicalObjectStartFlyEvent> _startFlyPool = default;

        private float _defaultSpeedThreshold = 20f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var collisionComp = ref _collisionPool.Value.Get(entity);

                if (collisionComp.Interactable is ICollisionInteractableWithThreshold)
                {
                    var collisionMb = collisionComp.Interactable as ICollisionInteractableWithThreshold;
                    ref var carStatisticComp = ref _statisticPool.Value.Get(collisionComp.CarEntity);

                    float threshold = _defaultSpeedThreshold;
                    if (collisionMb.SpeedThreshold != null)
                        threshold = collisionMb.SpeedThreshold.Value;

                    if (!collisionMb.HasFirstCollision)
                    {
                        if (carStatisticComp.PreviousFrameSpeedKmpH >= threshold)
                        {
                            collisionMb.OnFirstCollision();
                            InitAndMakePhysical(collisionComp.Interactable as IPhysicalObject);
                        }
                        _collisionPool.Value.Del(entity);
                        continue;
                    }
                }

                if (collisionComp.Interactable is ICollisionInterractable)
                {
                    var collisionMb = collisionComp.Interactable as ICollisionInterractable;
                    collisionMb.OnFirstCollision();
                    _collisionPool.Value.Del(entity);
                    continue;
                }

                if (collisionComp.Interactable is INpcMb)
                {
                    var collisionMb = collisionComp.Interactable as INpcMb;
                    ref var collisionPedestrianComp = ref _collisionPedestrianPool.Value.Add(_world.Value.NewEntity());
                    collisionPedestrianComp.PadastrianEntity = collisionMb.Entity;
                    collisionPedestrianComp.CarEntity = collisionComp.CarEntity;
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