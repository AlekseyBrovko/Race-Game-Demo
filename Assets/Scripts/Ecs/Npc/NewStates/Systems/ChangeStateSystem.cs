using Client;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class ChangeStateSystem : IEcsRunSystem
{
    protected EcsPoolInject<NpcIdleComp> _idleParentPool = default;
    protected EcsPoolInject<NpcAimComp> _aimParentPool = default;
    protected EcsPoolInject<NpcMeleeAttackComp> _attackParentPool = default;
    protected EcsPoolInject<NpcChasingComp> _chasingParentPool = default;
    protected EcsPoolInject<NpcDeadComp> _deadParentPool = default;
    protected EcsPoolInject<NpcHurtByCarComp> _hurtByCarParentPool = default;
    protected EcsPoolInject<NpcInObjectPoolComp> _poolParentPool = default;
    protected EcsPoolInject<NpcPatrollComp> _patrollParentPool = default;
    protected EcsPoolInject<NpcRangeAttackComp> _rangeParentPool = default;
    protected EcsPoolInject<NpcAttackCoolDownComp> _coolDownParentPool = default;

    protected EcsPoolInject<NpcInPeaceComp> _peaceParentPool = default;
    protected EcsPoolInject<NpcInFightComp> _fightParentPool = default;
    protected EcsPoolInject<NpcInactiveComp> _inactiveParentPool = default;

    protected EcsPoolInject<NpcStartInObjectPoolSystemsEvent> _startInObjectPool = default;
    protected EcsPoolInject<NpcStartDeadSystemsEvent> _startDeadPool = default;

    protected EcsPoolInject<NpcStateComp> _stateParentPool = default;

    protected EcsPoolInject<NpcDebugCanvasComp> _debugPool = default;

    protected virtual Enums.NpcStateType NpcStateType { get; }
    protected virtual Enums.NpcGlobalStateType NpcGlobalStateType { get; }

    public bool CheckOnInObjectsPoolTransition(int entity)
    {
        if (_startInObjectPool.Value.Has(entity) || _startDeadPool.Value.Has(entity))
        {
            Debug.Log("CheckOnInObjectsPoolTransition");
            return true;
        }

        return false;
    }

    public virtual void ChangeState(int entity)
    {
        ref var stateComp = ref _stateParentPool.Value.Get(entity);
        stateComp.PreviousState = stateComp.State;
        stateComp.State = NpcStateType;
        stateComp.States.Add(NpcStateType.ToString());

        if (_debugPool.Value.Has(entity))
        {
            ref var debugCanvasComp = ref _debugPool.Value.Get(entity);
            debugCanvasComp.CanvasMb.ShowState(stateComp.State.ToString());
        }

        ResetGlobalState(entity);

        switch (NpcGlobalStateType)
        {
            case Enums.NpcGlobalStateType.PeaceState:
                _peaceParentPool.Value.Add(entity);
                break;

            case Enums.NpcGlobalStateType.FightState:
                _fightParentPool.Value.Add(entity);
                break;

            case Enums.NpcGlobalStateType.InactiveState:
                _inactiveParentPool.Value.Add(entity);
                break;
        }
    }

    public virtual void ResetGlobalState(int entity)
    {
        if (_peaceParentPool.Value.Has(entity))
            _peaceParentPool.Value.Del(entity);

        if (_fightParentPool.Value.Has(entity))
            _fightParentPool.Value.Del(entity);

        if (_inactiveParentPool.Value.Has(entity))
            _inactiveParentPool.Value.Del(entity);
    }

    public virtual void ResetState(int entity)
    {
        if (_idleParentPool.Value.Has(entity))
            _idleParentPool.Value.Del(entity);

        if (_aimParentPool.Value.Has(entity))
            _aimParentPool.Value.Del(entity);

        if (_attackParentPool.Value.Has(entity))
            _attackParentPool.Value.Del(entity);

        if (_chasingParentPool.Value.Has(entity))
            _chasingParentPool.Value.Del(entity);

        if (_deadParentPool.Value.Has(entity))
            _deadParentPool.Value.Del(entity);

        if (_hurtByCarParentPool.Value.Has(entity))
            _hurtByCarParentPool.Value.Del(entity);

        if (_poolParentPool.Value.Has(entity))
            _poolParentPool.Value.Del(entity);

        if (_patrollParentPool.Value.Has(entity))
            _patrollParentPool.Value.Del(entity);

        if (_rangeParentPool.Value.Has(entity))
            _rangeParentPool.Value.Del(entity);

        if (_coolDownParentPool.Value.Has(entity))
            _coolDownParentPool.Value.Del(entity);
    }

    public virtual void Run(EcsSystems systems) { }
}