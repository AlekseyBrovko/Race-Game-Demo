using Client;
using Leopotam.EcsLite;

public class CarDriftingState : CarControllState
{
    private EcsPool<CarDriftComp> _driftPool;

    public CarDriftingState(CarControllFsm fsm, int entity, GameState state) : base(fsm, entity, state) { }

    public override void OnInit()
    {
        _driftPool = _world.GetPool<CarDriftComp>();
    }

    public override void EnterState(CarControllState previousState = null)
    {
        //Debug.Log("EnterState Drift State");
        if (!_driftIsPool.Has(_entity))
            _driftIsPool.Add(_entity);

        _driftPool.Add(_entity);

        ref var fsmComp = ref _fsmPool.Get(_entity);
        fsmComp.CurrentStateName = "CarDriftingState";
    }

    public override void ExitState(CarControllState nextState = null)
    {
        //Debug.Log("ExitState Drift State");
        if (_driftIsPool.Has(_entity))
            _driftIsPool.Del(_entity);

        _driftPool.Del(_entity);

        ref var fsmComp = ref _fsmPool.Get(_entity);
        fsmComp.CurrentStateName = "ExitState CarDriftingState";
    }
}