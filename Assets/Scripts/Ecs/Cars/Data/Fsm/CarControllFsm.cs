using Client;
using Leopotam.EcsLite;
using System;
using System.Collections.Generic;

public class CarControllFsm
{
    public CarControllState CurrentState { get; private set; }

    private Dictionary<Type, CarControllState> _states = new Dictionary<Type, CarControllState>();

    private GameState _state;
    private EcsWorld _world;
    private int _entity;

    public CarControllFsm(GameState state, int entity)
    {
        _state = state;
        _entity = entity;
        _world = state.EcsWorld;

        AddState(new CarIdleState(this, _entity, _state));
        AddState(new CarMoveForwardState(this, _entity, _state));
        AddState(new CarMoveBackwardState(this, _entity, _state));
        AddState(new CarFlyState(this, _entity, _state));
        AddState(new CarDriftingState(this, _entity, _state));
        SetState<CarIdleState>();
    }

    public void SetIdleState() =>
        SetState<CarIdleState>();

    public void SetMoveForwardState() =>
        SetState<CarMoveForwardState>();

    public void SetMoveBackState() =>
        SetState<CarMoveBackwardState>();

    public void SetFlyState() =>
        SetState<CarFlyState>();

    public void SetDriftState() =>
        SetState<CarDriftingState>();

    private void AddState(CarControllState state) =>
        _states.Add(state.GetType(), state);

    private void SetState<T>() where T : CarControllState
    {
        var type = typeof(T);

        if (CurrentState != null && CurrentState.GetType() == type)
            return;

        if (_states.TryGetValue(type, out var newState))
        {
            var oldState = CurrentState;
            CurrentState?.ExitState(newState);
            CurrentState = newState;
            CurrentState.EnterState(oldState);
        }
    }
}