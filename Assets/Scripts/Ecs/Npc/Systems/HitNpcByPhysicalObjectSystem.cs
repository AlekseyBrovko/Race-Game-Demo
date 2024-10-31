using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class HitNpcByPhysicalObjectSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;

        private EcsFilterInject<Inc<HitPedestrianByPhysicalObjectEvent>> _triggerByObjectFilter = default;
        private EcsFilterInject<Inc<CollisionPedestrianByPhysicalObjectEvent>> _collisionByObjectFilter = default;

        private EcsPoolInject<HitPedestrianByPhysicalObjectEvent> _triggerByObjectPool = default;
        private EcsPoolInject<CollisionPedestrianByPhysicalObjectEvent> _collisionByObjectPool = default;
        private EcsPoolInject<NpcDamageEvent> _damagePool = default;
        private EcsPoolInject<NpcComp> _npcPool = default;
        private EcsPoolInject<PhysicalObjectIsFlyingComp> _flyPool = default;
        private EcsPoolInject<NpcHurtByCarComp> _hurtPool = default;

        private float _damageValue = 20f;
        private float _noDamageThreshold = 3f;
        private float _hitToRagdollSpeedThreshold = 20f;

        public void Run(EcsSystems systems)
        {
            HandlePhysicalObjectTriggerEnter();
            HandlePhysicalObjectCollisionEnter();
        }

        private void HandlePhysicalObjectTriggerEnter()
        {
            foreach (var entity in _triggerByObjectFilter.Value)
            {
                ref var triggerComp = ref _triggerByObjectPool.Value.Get(entity);

                //TODO костыль для ухода от баги
                if (!_flyPool.Value.Has(triggerComp.PhysicalObject.Entity))
                {
                    _triggerByObjectPool.Value.Del(entity);
                    continue;
                }
                ref var flyComp = ref _flyPool.Value.Get(triggerComp.PhysicalObject.Entity);
                if (flyComp.SpeedKmH > _hitToRagdollSpeedThreshold)
                {
                    ref var npcComp = ref _npcPool.Value.Get(triggerComp.PadastrianEntity);
                    SetDamageToEntity(triggerComp.PadastrianEntity, triggerComp.PhysicalObject.Entity, float.PositiveInfinity);
                }
                _triggerByObjectPool.Value.Del(entity);
            }
        }

        private void HandlePhysicalObjectCollisionEnter()
        {
            foreach (var entity in _collisionByObjectFilter.Value)
            {
                ref var collisionComp = ref _collisionByObjectPool.Value.Get(entity);

                //TODO обратить внимание, вывалилась ошибка, окружил костылём
                if (!_flyPool.Value.Has(collisionComp.PhysicalObject.Entity))
                {
                    _collisionByObjectPool.Value.Del(entity);
                    continue;
                }
                ref var flyComp = ref _flyPool.Value.Get(collisionComp.PhysicalObject.Entity);
                if (flyComp.PreviousFrameSpeedKmH > _noDamageThreshold)
                {
                    if (!_hurtPool.Value.Has(collisionComp.PadastrianEntity))
                    {
                        SetDamageToEntity(collisionComp.PadastrianEntity,
                            collisionComp.PhysicalObject.Entity, _damageValue);
                    }
                }
                _collisionByObjectPool.Value.Del(entity);
            }
        }

        private void SetDamageToEntity(int entity, int damagerEntity, float value)
        {
            ref var damageComp = ref _damagePool.Value.Add(_world.Value.NewEntity());
            damageComp.DamageValue = value;
            damageComp.DamagerEntity = damagerEntity;
            damageComp.DamagedEntity = entity;
            damageComp.DamageType = Enums.DamageType.ObjectHit;
        }
    }
}