using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client
{
    sealed class NpcMeleeAttackSystem : ChangeStateSystem
    {
        private EcsWorldInject _world = default;
        private EcsCustomInject<GameState> _state = default;

        private EcsFilterInject<Inc<NpcStartMeleeAttackSystemsEvent, NpcMeleeComp>> _startMeleeFilter = default;
        private EcsFilterInject<Inc<NpcStartMeleeAttackSystemsEvent, NpcRangeComp>> _startRangeFilter = default;
        private EcsFilterInject<Inc<NpcMeleeAttackComp>> _meleeFilter = default;

        private EcsFilterInject<Inc<NpcMeleeAttackComp,
            StartMeleeAttackMonitoringEvent>> _startMonitoringFilter = default;
        private EcsFilterInject<Inc<NpcMeleeAttackComp,
            StopMeleeAttackMonitoringEvent>> _stopMonitoringFilter = default;
        private EcsFilterInject<Inc<NpcMeleeAttackComp,
            NpcMeleeAttackAnimationStopEvent>> _stopAnimationFilter = default;

        private EcsPoolInject<NpcStartMeleeAttackSystemsEvent> _startPool = default;
        private EcsPoolInject<NpcMeleeAttackComp> _attackPool = default;
        private EcsPoolInject<NpcComp> _npcPool = default;
        private EcsPoolInject<NavmeshComp> _navmeshPool = default;
        private EcsPoolInject<StartMeleeAttackMonitoringEvent> _startMonitoringPool = default;
        private EcsPoolInject<LayerMaskComp> _layersPool = default;
        private EcsPoolInject<NpcStartAttackCoolDownSystemsEvent> _startCoolDownPool = default;
        private EcsPoolInject<NpcDamageEvent> _npcDamagePool = default;
        private EcsPoolInject<CarDamageEvent> _carDamagePool = default;
        private EcsPoolInject<NpcResetPositionBodyEvent> _resetPosPool = default;
        private EcsPoolInject<Sound3DEvent> _soundPool = default;

        protected override Enums.NpcStateType NpcStateType => Enums.NpcStateType.MeleeAttack;
        protected override Enums.NpcGlobalStateType NpcGlobalStateType => Enums.NpcGlobalStateType.FightState;

        private int _hitCollidersAmountByDefault = 10;
        private float _radiusOfAttackByDefault = 0.5f;

        public override void Run(EcsSystems systems)
        {
            HandleMeleeStart();
            HandleRangeStart();

            ListenStartMonitoring();
            ListenStopMonitoring();

            HandleMeleeAttackMonitoring();
            ListenStopAnimation();
        }

        private void HandleMeleeStart()
        {
            foreach (var entity in _startMeleeFilter.Value)
            {
                ResetState(entity);
                ChangeState(entity);

                ref var npcComp = ref _npcPool.Value.Get(entity);
                npcComp.NpcMb.Attack();

                ref var navmeshComp = ref _navmeshPool.Value.Get(entity);
                navmeshComp.Agent.enabled = false;

                _attackPool.Value.Add(entity);
                _startPool.Value.Del(entity);
            }
        }

        private void HandleRangeStart()
        {
            foreach (var entity in _startRangeFilter.Value)
            {
                ResetState(entity);
                ChangeState(entity);

                _attackPool.Value.Add(entity);
                _startPool.Value.Del(entity);
            }
        }

        private void HandleMeleeAttackMonitoring()
        {
            foreach (var entity in _meleeFilter.Value)
            {
                ref var attackComp = ref _attackPool.Value.Get(entity);
                if (!attackComp.IsAttacking)
                    continue;

                ref var npcComp = ref _npcPool.Value.Get(entity);
                ref var layersComp = ref _layersPool.Value.Get(_state.Value.LayerMaskEntity);

                float damageByDefault = _state.Value.SettingsConfig.MeleeDamage;
                Vector3 pos = attackComp.AttackPoint.position;

                int hitColliders = Physics.OverlapSphereNonAlloc(pos, _radiusOfAttackByDefault,
                    attackComp.HitColliders, npcComp.LayerMaskToSearchEnemies,
                    QueryTriggerInteraction.Collide);

                if (hitColliders != attackComp.HitCollidersAmount && hitColliders > 0)
                {
                    attackComp.HitCollidersAmount = hitColliders;
                    List<Transform> hitList =
                        GetTransformNotInList(attackComp.InjuredTransforms, attackComp.HitColliders);

                    if (hitList.Count > 0)
                    {
                        foreach (var transform in hitList)
                        {
                            if (transform.gameObject.TryGetComponent(out IEcsEntityMb entityMb))
                            {
                                switch (entityMb.EntityType)
                                {
                                    case Enums.EntityType.Car:
                                        ref var carDamageComp = ref _carDamagePool.Value.Add(_world.Value.NewEntity());
                                        carDamageComp.DamagedEntity = entityMb.Entity;
                                        carDamageComp.DamageValue = damageByDefault;
                                        carDamageComp.DamagerEntity = entity;
                                        carDamageComp.DamageType = Enums.DamageType.Melee;
                                        break;

                                    case Enums.EntityType.Npc:
                                        ref var npcDamageComp = ref _npcDamagePool.Value.Add(_world.Value.NewEntity());
                                        npcDamageComp.DamagedEntity = entityMb.Entity;
                                        npcDamageComp.DamageValue = damageByDefault;
                                        npcDamageComp.DamagerEntity = entity;
                                        npcDamageComp.DamageType = Enums.DamageType.Melee;
                                        break;
                                }
                                attackComp.InjuredTransforms.Add(transform);
                            }
                        }
                    }
                }
            }
        }

        private List<Transform> GetTransformNotInList(List<Transform> listToCheck, Collider[] colliders)
        {
            List<Transform> resultList = new List<Transform>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] == null)
                    continue;

                if (!HasMatch(colliders[i].transform, listToCheck))
                    resultList.Add(colliders[i].transform);
            }
            resultList = resultList.Distinct().ToList();
            return resultList;
        }

        private bool HasMatch(Transform transform, List<Transform> transforms)
        {
            for (int i = 0; i < transforms.Count; i++)
                if (transform == transforms[i])
                    return true;
            return false;
        }

        private void ListenStartMonitoring()
        {
            foreach (var entity in _startMonitoringFilter.Value)
            {
                ref var startMonitoringComp = ref _startMonitoringPool.Value.Get(entity);
                ref var attackComp = ref _attackPool.Value.Get(entity);
                attackComp.IsAttacking = true;
                attackComp.AttackPoint = startMonitoringComp.AttackPoint;
                attackComp.Radius = startMonitoringComp.Radius;
                attackComp.HitColliders = new Collider[_hitCollidersAmountByDefault];
                attackComp.InjuredTransforms = new List<Transform>();

                ref var npcComp = ref _npcPool.Value.Get(entity);
                ref var soundComp = ref _soundPool.Value.Add(_world.Value.NewEntity());
                soundComp.SoundType = Enums.SoundEnum.ZombieAttack;
                soundComp.Position = npcComp.TransformOfBody.position;
            }
        }

        private void ListenStopMonitoring()
        {
            foreach (var entity in _stopMonitoringFilter.Value)
            {
                ref var attackComp = ref _attackPool.Value.Get(entity);
                attackComp.IsAttacking = false;
            }
        }

        private void ListenStopAnimation()
        {
            foreach (var entity in _stopAnimationFilter.Value)
            {
                _attackPool.Value.Del(entity);

                ref var resetComp = ref _resetPosPool.Value.Add(_world.Value.NewEntity());
                resetComp.NpcEntity = entity;

                _startCoolDownPool.Value.Add(entity);
            }
        }
    }
}