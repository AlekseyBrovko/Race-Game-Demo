using Client;
using Leopotam.EcsLite;
using UnityEngine;

public class CarIdleState : CarControllState
{
    protected EcsPool<CameraFlyAroundCarComp> _cameraFlyPool;
    public CarIdleState(CarControllFsm fsm, int entity, GameState state) : base(fsm, entity, state) { }

    public override void OnInit()
    {
        _cameraFlyPool = _world.GetPool<CameraFlyAroundCarComp>();
    }

    public override void EnterState(CarControllState previousState = null)
    {
        if (previousState != null)
        {
            var type = previousState.GetType();
            if (type == typeof(CarMoveForwardState))
            {
                //Debug.Log("ехали вперед");
            }   
            else if (type == typeof(CarMoveBackwardState))
            {
                //Debug.Log("ехали назад");
            }   
        }

        //Debug.Log("Enter IdleState");
        if (!_idlePool.Has(_entity))
            _idlePool.Add(_entity);

        ref var fsmComp = ref _fsmPool.Get(_entity);
        fsmComp.CurrentStateName = "CarIdleState";
    }

    public override void ExitState(CarControllState nextState = null)
    {
        //Debug.Log("Exit IdleState");
        if (_idlePool.Has(_entity))
            _idlePool.Del(_entity);

        var type = nextState.GetType();
        ref var fsmComp = ref _fsmPool.Get(_entity);
        fsmComp.CurrentStateName = "ExitState CarIdleState";
    }
}
