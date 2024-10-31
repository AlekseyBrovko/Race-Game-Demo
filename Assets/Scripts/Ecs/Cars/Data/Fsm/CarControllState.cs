using Client;
using Leopotam.EcsLite;

public abstract class CarControllState
{
    public CarControllState(CarControllFsm fsm, int entity, GameState state)
    {
        _fsm = fsm;
        _entity = entity;
        _state = state;
        _world = _state.EcsWorld;

        _movingForwardPool = _world.GetPool<CarIsMovingForwardComp>();
        _movingBackPool = _world.GetPool<CarIsMovingBackwardComp>();
        _idlePool = _world.GetPool<CarIsIdleComp>();
        _flyPool = _world.GetPool<CarIsFlyComp>();
        _driftIsPool = _world.GetPool<CarIsDriftComp>();
        _fsmPool = _world.GetPool<CarFsmControllComp>();

        OnInit();
    }

    protected CarControllFsm _fsm;
    protected GameState _state;
    protected EcsWorld _world;
    protected int _entity;

    protected EcsPool<CarIsMovingForwardComp> _movingForwardPool;
    protected EcsPool<CarIsMovingBackwardComp> _movingBackPool;
    protected EcsPool<CarIsIdleComp> _idlePool;
    protected EcsPool<CarIsFlyComp> _flyPool;
    protected EcsPool<CarIsDriftComp> _driftIsPool;
    protected EcsPool<CarFsmControllComp> _fsmPool;

    public virtual void OnInit() { }
    public virtual void EnterState(CarControllState previousState = null) { }
    public virtual void ExitState(CarControllState nextState = null) { }
}