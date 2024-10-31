using Leopotam.EcsLite;
using UnityEngine;

public class MeleeAttackAnimationBehaviour : StateMachineBehaviour
{
    private int _entity;
    private EcsPool<NpcMeleeAttackAnimationStopEvent> _stopAnimationPool;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimationHandlerMb handlerMb = animator.gameObject.GetComponent<AnimationHandlerMb>();
        _stopAnimationPool = handlerMb.GameState.EcsWorld.GetPool<NpcMeleeAttackAnimationStopEvent>();
        _entity = handlerMb.Entity;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.isActiveAndEnabled)
            _stopAnimationPool.Add(_entity);
    }
}