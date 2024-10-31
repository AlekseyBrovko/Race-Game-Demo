using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    sealed class NpcChasingSystem : ChangeStateSystem
    {
        private EcsCustomInject<GameState> _state = default;
        private EcsFilterInject<Inc<NpcStartChasingSystemsEvent>> _startFilter = default;
        private EcsPoolInject<NpcStartChasingSystemsEvent> _startChasingPool = default;
        private EcsPoolInject<NpcChasingComp> _chasingPool = default;
        private EcsPoolInject<NpcLookAroundComp> _lookPool = default;
        private EcsPoolInject<NpcFightComp> _fightPool = default;
        private EcsPoolInject<NavmeshComp> _navmeshPool = default;
        private EcsPoolInject<NpcComp> _npcPool = default;

        private EcsFilterInject<Inc<NpcChasingComp, NpcMeleeComp>, 
            Exc<NpcStartInObjectPoolSystemsEvent, NpcStartDeadSystemsEvent>> _chasingMeleeFilter = default;
        private EcsFilterInject<Inc<NpcChasingComp, NpcRangeComp>, 
            Exc<NpcStartInObjectPoolSystemsEvent, NpcStartDeadSystemsEvent>> _chasingRangeFilter = default;

        private EcsPoolInject<NpcStartMeleeAttackSystemsEvent> _startAttackPool = default;
        private EcsPoolInject<NpcStartPatrollSystemsEvent> _startPatrollPool = default;
        private EcsPoolInject<NpcStartAimSystemsEvent> _startAimPool = default;
        private EcsPoolInject<LayerMaskComp> _layersPool = default;
        private EcsPoolInject<NpcRangeComp> _rangePool = default;

        protected override Enums.NpcStateType NpcStateType => Enums.NpcStateType.Chasing;
        protected override Enums.NpcGlobalStateType NpcGlobalStateType => Enums.NpcGlobalStateType.FightState;

        private float _durationOfCheckDistanceOfMelee = 0.1f;
        private float _durationOfCheckDistanceOfRange = 0.2f;
        private float _durationOfChasing = 5f;
        private float _durationOfSetDestination = 0.5f;

        private float _radiusOfMeleeCheckEnemie = 1f;

        public override void Run(EcsSystems systems)
        {
            HandleStart();
            HandleMeleeChasing();
            HandleRangeChasing();
        }

        private void HandleStart()
        {
            foreach (var entity in _startFilter.Value)
            {
                ResetState(entity);
                ChangeState(entity);

                if (CheckOnInObjectsPoolTransition(entity))
                {
                    _startChasingPool.Value.Del(entity);
                    continue;
                }

                ref var navmeshComp = ref _navmeshPool.Value.Get(entity);
                ref var npcComp = ref _npcPool.Value.Get(entity);
                navmeshComp.Agent.enabled = true;
                navmeshComp.Agent.speed = npcComp.NpcMb.RunSpeed;

                ref var lookComp = ref _lookPool.Value.Get(entity);

                if (lookComp.NearestTarget == null)
                {
                    Debug.LogWarning("lookComp.NearestTarget == null");
                    ref var layersComp = ref _layersPool.Value.Get(_state.Value.LayerMaskEntity);
                    Collider[] colliders = Physics.OverlapSphere(npcComp.MainTransform.position,
                        npcComp.RadiusOfLook, npcComp.LayerMaskToSearchEnemies, QueryTriggerInteraction.Ignore);

                    List<Transform> targetTransforms = new List<Transform>();
                    foreach (var col in colliders)
                        targetTransforms.Add(col.transform);

                    lookComp.Targets = targetTransforms;
                    lookComp.NearestTarget = Tools.GetNearestTransform(npcComp.MainTransform.position, lookComp.Targets);

                }

                //TODO тут ошибка вылетела NearestTarget == null
                IVisible visibleMb = lookComp.NearestTarget.GetComponent<IVisible>();

                ref var fightComp = ref _fightPool.Value.Get(entity);
                fightComp.Target = lookComp.NearestTarget;
                fightComp.LookPointOfTarget = visibleMb.LookPoint;

                _chasingPool.Value.Add(entity);
                _startChasingPool.Value.Del(entity);
            }
        }

        private void HandleMeleeChasing()
        {
            foreach (var entity in _chasingMeleeFilter.Value)
            {
                ref var chasingComp = ref _chasingPool.Value.Get(entity);
                ref var fightComp = ref _fightPool.Value.Get(entity);
                ref var npcComp = ref _npcPool.Value.Get(entity);

                if (IsReadyToCheckDistance(ref chasingComp))
                {
                    chasingComp.TimerOfCheckDistance = _durationOfCheckDistanceOfMelee;

                    if (Physics.CheckSphere(
                        npcComp.MainTransform.position,
                        _radiusOfMeleeCheckEnemie,
                        npcComp.LayerMaskToSearchEnemies,
                        QueryTriggerInteraction.Collide))
                    {
                        _startAttackPool.Value.Add(entity);
                        _chasingPool.Value.Del(entity);
                        continue;
                    }
                }

                ref var layersComp = ref _layersPool.Value.Get(_state.Value.LayerMaskEntity);
                if (IsReadyToSkipChasing(ref chasingComp))
                {
                    //нужно проверить расстояние и видимость
                    float sqrDistance = Vector3.SqrMagnitude(fightComp.Target.position - npcComp.MainTransform.position);
                    if (sqrDistance > npcComp.RadiusOfLook * npcComp.RadiusOfLook)
                    {
                        if (!_startPatrollPool.Value.Has(entity))
                            _startPatrollPool.Value.Add(entity);
                        _chasingPool.Value.Del(entity);
                        continue;
                    }
                    else if (!Tools.IsInVisible(npcComp.MainTransform, fightComp.Target, layersComp.DefaultLayer))
                    {
                        if (!_startPatrollPool.Value.Has(entity))
                            _startPatrollPool.Value.Add(entity);
                        _chasingPool.Value.Del(entity);
                        continue;
                    }
                    else
                    {
                        chasingComp.TimerOfCheckStopChasing = _durationOfChasing;
                    }
                }

                if (IsReadyToSetDestination(ref chasingComp))
                {
                    chasingComp.TimerOfSetAgentSestination = _durationOfSetDestination;
                    ref var navmeshComp = ref _navmeshPool.Value.Get(entity);
                    if (navmeshComp.Agent.isOnNavMesh)
                        navmeshComp.Agent.SetDestination(fightComp.Target.position);
                    else
                        Debug.LogWarning($"!navmeshComp.Agent.isOnNavMesh ; npc.name = {npcComp.MainTransform.name}; " +
                            $"position = {npcComp.MainTransform.position};");
                }
            }
        }

        private void HandleRangeChasing()
        {
            foreach (var entity in _chasingRangeFilter.Value)
            {
                ref var chasingComp = ref _chasingPool.Value.Get(entity);
                ref var fightComp = ref _fightPool.Value.Get(entity);
                ref var npcComp = ref _npcPool.Value.Get(entity);
                ref var rangeComp = ref _rangePool.Value.Get(entity);
                ref var layersComp = ref _layersPool.Value.Get(_state.Value.LayerMaskEntity);

                if (IsReadyToCheckDistance(ref chasingComp))
                {
                    chasingComp.TimerOfCheckDistance = _durationOfCheckDistanceOfRange;

                    float sqrDistance = Vector3.SqrMagnitude(fightComp.Target.position - npcComp.MainTransform.position);

                    if (sqrDistance < rangeComp.RangeMb.RangeOfAttack * rangeComp.RangeMb.RangeOfAttack &&
                        Tools.IsInVisible(npcComp.MainTransform, fightComp.LookPointOfTarget, layersComp.DefaultLayer))
                    {
                        if (rangeComp.RangeMb is IRangeThrowingNpc)
                        {
                            IRangeThrowingNpc throwingNpcMb = rangeComp.RangeMb as IRangeThrowingNpc;
                            if (throwingNpcMb.CanThrowObject)
                            {
                                _startAimPool.Value.Add(entity);
                                _chasingPool.Value.Del(entity);
                            }
                        }
                        else
                        {
                            _startAimPool.Value.Add(entity);
                            _chasingPool.Value.Del(entity);
                        }
                        continue;
                    }
                }

                if (IsReadyToSkipChasing(ref chasingComp))
                {
                    //TODO логично было бы через lookdistance
                    float sqrDistance = Vector3.SqrMagnitude(fightComp.Target.position - npcComp.MainTransform.position);
                    if (sqrDistance > rangeComp.RangeMb.RangeOfAttack ||
                        !Tools.IsInVisible(npcComp.MainTransform, fightComp.Target, layersComp.DefaultLayer))
                    {
                        if (!_startPatrollPool.Value.Has(entity))
                            _startPatrollPool.Value.Add(entity);
                        _chasingPool.Value.Del(entity);
                        continue;
                    }
                }

                if (IsReadyToSetDestination(ref chasingComp))
                {
                    chasingComp.TimerOfSetAgentSestination = _durationOfSetDestination;
                    ref var navmeshComp = ref _navmeshPool.Value.Get(entity);

                    if (navmeshComp.Agent.isOnNavMesh)
                        navmeshComp.Agent.SetDestination(fightComp.Target.position);
                    else
                        Debug.LogWarning($"!navmeshComp.Agent.isOnNavMesh name = {npcComp.MainTransform.name}; " +
                        $"position = {npcComp.MainTransform.position};");
                }
            }
        }

        private bool IsReadyToCheckDistance(ref NpcChasingComp chasingComp)
        {
            chasingComp.TimerOfCheckDistance -= Time.deltaTime;
            if (chasingComp.TimerOfCheckDistance <= 0)
                return true;
            return false;
        }

        private bool IsReadyToSkipChasing(ref NpcChasingComp chasingComp)
        {
            chasingComp.TimerOfCheckStopChasing -= Time.deltaTime;
            if (chasingComp.TimerOfCheckStopChasing <= 0)
                return true;
            return false;
        }

        private bool IsReadyToSetDestination(ref NpcChasingComp chasingComp)
        {
            chasingComp.TimerOfSetAgentSestination -= Time.deltaTime;
            if (chasingComp.TimerOfSetAgentSestination <= 0)
                return true;
            return false;
        }
    }
}