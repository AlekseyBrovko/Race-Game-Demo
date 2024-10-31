using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using UnityEngine;

namespace Client
{
    public class NpcChangeStateSystem : IEcsRunSystem
    {
        //protected EcsWorldInject _world = default;
        //protected EcsPoolInject<NpcChangeStateEvent> _changeStatePool = default;

        public virtual void Run(EcsSystems systems) { }

        //public bool HasChangeStatePool(int entity, out Type stateInEvent)
        //{
        //    stateInEvent = null;
        //    if (_changeStatePool.Value.Has(entity))
        //    {
        //        ref var changeComp = ref _changeStatePool.Value.Get(entity);
        //        stateInEvent = changeComp.StateType;
        //        return true;
        //    }
        //    return false;
        //}

        public void CheckOnDeathCompAndChangeState(int entity, Type newStateType, Action OnSuccessCallback = null, IDataForSetState newStateData = null)
        {
            //if (HasChangeStatePool(entity, out Type stateInEvent))
            //{
            //    if (stateInEvent != typeof(DeadState))
            //    {
            //        ref var changeComp = ref _changeStatePool.Value.Get(entity);
            //        changeComp.StateType = newStateType;
            //        changeComp.ClassOfEvent = this.GetType();
            //        changeComp.NewStateData = newStateData;
            //        OnSuccessCallback?.Invoke();
            //    }
            //}
            //else
            //{
            //    ref var changeComp = ref _changeStatePool.Value.Add(entity);
            //    changeComp.StateType = newStateType;
            //    changeComp.ClassOfEvent = this.GetType();
            //    changeComp.NewStateData = newStateData;
            //    OnSuccessCallback?.Invoke();
            //}
        }

        public void CheckOnDeathOrHurtCompandChangeState(int entity, Type newStateType, Action OnSuccessCallback = null, IDataForSetState newStateData = null)
        {
            //if (HasChangeStatePool(entity, out Type stateInEvent))
            //{
            //    if (stateInEvent != typeof(DeadState) && stateInEvent != typeof(HurtByCarState))
            //    {
            //        ref var changeComp = ref _changeStatePool.Value.Get(entity);
            //        changeComp.StateType = newStateType;
            //        changeComp.ClassOfEvent = this.GetType();
            //        changeComp.NewStateData = newStateData;
            //        OnSuccessCallback?.Invoke();
            //    }
            //}
            //else
            //{
            //    ref var changeComp = ref _changeStatePool.Value.Add(entity);
            //    changeComp.StateType = newStateType;
            //    changeComp.ClassOfEvent = this.GetType();
            //    changeComp.NewStateData = newStateData;
            //    OnSuccessCallback?.Invoke();
            //}
        }
    }
}