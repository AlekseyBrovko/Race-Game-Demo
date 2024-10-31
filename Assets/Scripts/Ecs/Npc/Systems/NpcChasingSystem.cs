using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    //public class NpcOldChasingSystem : NpcChangeStateSystem
    //{
        //private EcsCustomInject<GameState> _state = default;

        //private EcsFilterInject<Inc<NpcDestinationComp, NpcIsChasingComp, NpcRangeComp>> _chasingRangeFilter = default;
        //private EcsFilterInject<Inc<NpcDestinationComp, NpcIsChasingComp, NpcMeleeComp>> _chasingMeleeFilter = default;

        //private EcsPoolInject<NpcDestinationComp> _destinationPool = default;
        //private EcsPoolInject<ViewComp> _viewPool = default;
        //private EcsPoolInject<NavmeshComp> _navmeshPool = default;
        //private EcsPoolInject<NpcHasArrivedEvent> _arrivedPool = default;
        //private EcsPoolInject<NpcRangeComp> _rangePool = default;
        //private EcsPoolInject<NpcRangeAttackCoolDownComp> _rangeAttackCoolDownPool = default;
        //private EcsPoolInject<NpcFightComp> _fightPool = default;
        //private EcsPoolInject<LayerMaskComp> _layersPool = default;

        //private EcsPoolInject<NpcPatrollPointsComp> _patrollPointsPool = default;

        //private float _sqrAttackRange = 1f;
        //private float _checkDurationInFight = 0.2f;

        //public override void Run(EcsSystems systems)
        //{
        //    HandleRangeNpc();
        //    HandleMeleeNpc();
        //}

        //private void HandleRangeNpc()
        //{
        //    foreach (var entity in _chasingRangeFilter.Value)
        //    {
        //        ref var rangeComp = ref _rangePool.Value.Get(entity);
        //        float sqrDistance = rangeComp.RangeMb.RangeOfAttack * rangeComp.RangeMb.RangeOfAttack;

        //        if (_rangeAttackCoolDownPool.Value.Has(entity))
        //            continue;

        //        ref var destinationComp = ref _destinationPool.Value.Get(entity);
        //        destinationComp.Timer -= Time.deltaTime;
        //        if (destinationComp.Timer < 0)
        //        {
        //            //проверить видно ли противника
        //            ref var viewComp = ref _viewPool.Value.Get(entity);
        //            ref var layersComp = ref _layersPool.Value.Get(_state.Value.LayerMaskEntity);

        //            ref var fightComp = ref _fightPool.Value.Get(entity);
        //            if (!Tools.IsInVisible(viewComp.Transform, fightComp.LookPointOfTarget, layersComp.DefaultLayer))
        //            {
        //                destinationComp.Timer = _checkDurationInFight;
        //                continue;
        //            }

        //            if (CheckArrivedDistance(sqrDistance, entity))
        //                CheckOnDeathOrHurtCompandChangeState(entity, typeof(AimingState));
        //            else
        //                destinationComp.Timer = _checkDurationInFight;
        //        }
        //    }
        //}

        //private void HandleMeleeNpc()
        //{
        //    foreach (var entity in _chasingMeleeFilter.Value)
        //    {
        //        ref var destinationComp = ref _destinationPool.Value.Get(entity);
        //        destinationComp.Timer -= Time.deltaTime;
        //        if (destinationComp.Timer < 0)
        //        {
        //            destinationComp.Timer = _checkDurationInFight;

        //            if (CheckArrivedDistance(_sqrAttackRange, entity))
        //            {
        //                destinationComp.Timer = 0;
        //                _arrivedPool.Value.Add(entity);
        //            }
        //        }   
        //    }
        //}

        //private bool CheckArrivedDistance(float sqrDistance, int entity)
        //{
        //    ref var destinationComp = ref _destinationPool.Value.Get(entity);
        //    ref var navmeshComp = ref _navmeshPool.Value.Get(entity);
        //    ref var viewComp = ref _viewPool.Value.Get(entity);

        //    if (destinationComp.DestinationTransform == null)
        //    {
        //        ref var patrollPointsComp = ref _patrollPointsPool.Value.Get(entity);
        //        destinationComp.DestinationTransform = patrollPointsComp.CurrentPoint;
        //    }

        //    if (destinationComp.DestinationTransform == null)
        //        destinationComp.DestinationTransform = viewComp.Transform;

        //    Vector3 destinationPos = destinationComp.DestinationTransform.position;
        //    navmeshComp.Agent.destination = destinationPos;
        //    destinationComp.Distance = Vector3.SqrMagnitude(destinationPos - viewComp.Transform.position);

        //    if (sqrDistance > destinationComp.Distance)
        //        return true;
        //    return false;
        //}
    //}
}