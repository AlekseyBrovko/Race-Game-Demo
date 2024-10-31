using Client;

public class CarMoveForwardState : CarControllState
{
    public CarMoveForwardState(CarControllFsm fsm, int entity, GameState state) : base(fsm, entity, state) { }

    public override void EnterState(CarControllState previousState = null)
    {
        //Debug.Log("Enter MoveForwardState");
        if (!_movingForwardPool.Has(_entity))
            _movingForwardPool.Add(_entity);

        ref var fsmComp = ref _fsmPool.Get(_entity);
        fsmComp.CurrentStateName = "CarMoveForwardState";
    }

    public override void ExitState(CarControllState nextState = null)
    {
        //Debug.Log("Exit MoveForwardState");
        if (_movingForwardPool.Has(_entity))
            _movingForwardPool.Del(_entity);

        ref var fsmComp = ref _fsmPool.Get(_entity);
        fsmComp.CurrentStateName = "EnterState CarMoveForwardState";
    }
}
