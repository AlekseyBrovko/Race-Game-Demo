using Client;

public class CarFlyState : CarControllState
{
    public CarFlyState(CarControllFsm fsm, int entity, GameState state) : base(fsm, entity, state) { }

    public override void EnterState(CarControllState previousState = null)
    {
        if (!_flyPool.Has(_entity))
            _flyPool.Add(_entity);

        ref var fsmComp = ref _fsmPool.Get(_entity);
        fsmComp.CurrentStateName = "CarFlyState";
    }

    public override void ExitState(CarControllState nextState = null)
    {
        if (_flyPool.Has(_entity))
            _flyPool.Del(_entity);

        ref var fsmComp = ref _fsmPool.Get(_entity);
        fsmComp.CurrentStateName = "ExitState CarFlyState";
    }
}
