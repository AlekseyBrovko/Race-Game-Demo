using Client;
using Leopotam.EcsLite;
using UnityEngine;

public class HurtAnimationBehaviour : StateMachineBehaviour
{
    private GameState _state;
    private EcsWorld _world;
    private int _entity;

    public void Init(GameState state, int entity)
    {
        _state = state;
        _world = state.EcsWorld;
        _entity = entity;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}