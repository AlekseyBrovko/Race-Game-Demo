using Client;
using Leopotam.EcsLite;

public class CarMoveBackwardState : CarControllState
{
    protected EcsPool<StartBackDriveEvent> _startBackDrivePool;
    protected EcsPool<StopBackDriveEvent> _stopBackDrivePool;
    public CarMoveBackwardState(CarControllFsm fsm, int entity, GameState state) : base(fsm, entity, state) { }

    public override void OnInit()
    {
        _startBackDrivePool = _world.GetPool<StartBackDriveEvent>();
        _stopBackDrivePool = _world.GetPool<StopBackDriveEvent>();
    }

    public override void EnterState(CarControllState previousState = null)
    {
        //Debug.Log("Enter MoveBackState");
        if (!_movingBackPool.Has(_entity))
            _movingBackPool.Add(_entity);

        _startBackDrivePool.Add(_entity);

        ref var fsmComp = ref _fsmPool.Get(_entity);
        fsmComp.CurrentStateName = "CarMoveBackwardState";
    }

    public override void ExitState(CarControllState nextState = null)
    {
        //Debug.Log("Exit MoveBackState");
        if (_movingBackPool.Has(_entity))
            _movingBackPool.Del(_entity);

        _stopBackDrivePool.Add(_entity);

        ref var fsmComp = ref _fsmPool.Get(_entity);
        fsmComp.CurrentStateName = "ExitState CarMoveBackwardState";
    }
}
