using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CarTriggerSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsFilterInject<Inc<CarTriggerEvent>> _filter = default;
        private EcsPoolInject<CarTriggerEvent> _triggerPool = default;
        private EcsPoolInject<CarStatisticComp> _statisticPool = default;
        private EcsPoolInject<HitPedestrianByCarEvent> _hitPedestrianPool = default;
        private EcsPoolInject<PhysicalObjectComp> _physicalObjectPool = default;
        private EcsPoolInject<PhysicalObjectStartFlyEvent> _startFlyPool = default;
        private EcsPoolInject<PhysicalObjectIsMovingByCarComp> _movingByCarPool = default;

        private float _defaultSpeedThreshold = 20f;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var triggerComp = ref _triggerPool.Value.Get(entity);

                if (triggerComp.Interactable is ITriggerInteractableWithThreshold)
                {
                    var triggerMb = triggerComp.Interactable as ITriggerInteractableWithThreshold;
                    if (!triggerMb.HasFirstTrigger)
                    {
                        ref var carStatisticComp = ref _statisticPool.Value.Get(triggerComp.CarEntity);

                        float threshold = _defaultSpeedThreshold;
                        if (triggerMb.SpeedThreshold != null)
                            threshold = triggerMb.SpeedThreshold.Value;

                        if (carStatisticComp.SpeedKmpH >= threshold)
                        {
                            InitAndMakePhysical(triggerComp.Interactable as IPhysicalObject, triggerComp.CarEntity);
                            triggerMb.OnFirstTrigger();
                        }
                        _triggerPool.Value.Del(entity);
                        continue;
                    }
                }

                if (triggerComp.Interactable is ICollisionInteractableWithThreshold)
                {
                    var triggerMb = triggerComp.Interactable as ICollisionInteractableWithThreshold;
                    if (!triggerMb.HasFirstCollision)
                    {
                        _triggerPool.Value.Del(entity);
                        continue;
                    }
                }

                if (triggerComp.Interactable is IPhysicalObject)
                {
                    InitAndMakePhysical(triggerComp.Interactable as IPhysicalObject, triggerComp.CarEntity);
                    _triggerPool.Value.Del(entity);
                    continue;
                }

                if (triggerComp.Interactable is INpcMb)
                {
                    var npcMb = triggerComp.Interactable as INpcMb;
                    ref var hitPedestrianComp = ref _hitPedestrianPool.Value.Add(_world.Value.NewEntity());
                    hitPedestrianComp.PadastrianEntity = npcMb.Entity;
                    hitPedestrianComp.CarEntity = triggerComp.CarEntity;
                }
                _triggerPool.Value.Del(entity);
            }
        }

        private void InitAndMakePhysical(IPhysicalObject physicalMb, int carEntity)
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

            if (!_movingByCarPool.Value.Has(physicalMb.Entity))
            {
                ref var movingByCarComp = ref _movingByCarPool.Value.Add(physicalMb.Entity);
                movingByCarComp.CarEntity = carEntity;
            }
        }
    }
}