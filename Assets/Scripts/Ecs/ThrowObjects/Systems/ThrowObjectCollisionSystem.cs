using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ThrowObjectCollisionSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        //TODO тут тоже могут быть проблемы с событиями
        private EcsFilterInject<Inc<CollisionWithThrowingObjectEvent, CarComp>> _carFilter = default;
        private EcsFilterInject<Inc<CollisionWithThrowingObjectEvent, NpcComp>> _npcFilter = default;

        private EcsPoolInject<CollisionWithThrowingObjectEvent> _collisionEventPool = default;
        private EcsPoolInject<NpcDamageEvent> _npcDamagePool = default;
        private EcsPoolInject<CarDamageEvent> _carDamagePool = default;

        private EcsPoolInject<CarComp> _carPool = default;
        private EcsPoolInject<ViewComp> _viewPool = default;

        //если вводить дружелюбных npc, нужно будет перерабатывать
        public void Run(EcsSystems systems)
        {
            HandlCars();
            HandlNpc();
        }

        private void HandlCars()
        {
            foreach (var entity in _carFilter.Value)
            {
                ref var carComp = ref _carPool.Value.Get(entity);
                ref var carViewComp = ref _viewPool.Value.Get(entity);
                ref var collisionComp = ref _collisionEventPool.Value.Get(entity);
                
                ref var damageComp = ref _carDamagePool.Value.Add(_world.Value.NewEntity());
                damageComp.DamageValue = _state.Value.SettingsConfig.RangeDamage;
                damageComp.DamageType = Enums.DamageType.TrowObject;
                damageComp.DamagedEntity = entity;
                damageComp.PointOfForce = collisionComp.ThrowObject.Transform.position;

                _collisionEventPool.Value.Del(entity);
            }
        }

        private void HandlNpc()
        {
            foreach (var entity in _npcFilter.Value)
            {
                if (!_npcDamagePool.Value.Has(entity))
                    _npcDamagePool.Value.Add(entity);
                ref var damageComp = ref _npcDamagePool.Value.Get(entity);
                damageComp.DamageValue = float.PositiveInfinity;

                _collisionEventPool.Value.Del(entity);
            }
        }
    }
}