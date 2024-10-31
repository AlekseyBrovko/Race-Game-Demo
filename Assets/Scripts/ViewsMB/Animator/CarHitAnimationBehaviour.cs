using Leopotam.EcsLite;
using UnityEngine;

public class CarHitAnimationBehaviour : StateMachineBehaviour
{
    private int _entity;
    private EcsPool<NpcCarHitAnimationStopEvent> _stopEventPool;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimationHandlerMb handlerMb = animator.gameObject.GetComponent<AnimationHandlerMb>();
        _stopEventPool = handlerMb.GameState.EcsWorld.GetPool<NpcCarHitAnimationStopEvent>();
        _entity = handlerMb.Entity;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) =>
        _stopEventPool.Add(_entity);
}