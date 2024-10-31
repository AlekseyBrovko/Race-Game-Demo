using Client;
using UnityEngine;

public class SimpleZombieAnimationHandler : AnimationHandlerMb
{
    private int[] _attackTriggers;
    public override void Init(GameState state, int entity, Animator animator)
    {
        base.Init(state, entity, animator);
        int twoHandsAttack = Animator.StringToHash("TwoHandsAttack");
        int rightHandsAttack = Animator.StringToHash("RightHandAttack");
        //int headAttack = Animator.StringToHash("HeadAttack");
        int rightLeftKick = Animator.StringToHash("RightLegKick");
        int rightLegHardKick = Animator.StringToHash("RightLegHardKick");
        int rightHandTopDownPunch = Animator.StringToHash("RightHandTopDownPunch");

        _attackTriggers = new int[] { twoHandsAttack, rightHandsAttack, /*headAttack,*/
            rightLeftKick, rightLegHardKick, rightHandTopDownPunch };
    }

    public override void Attack() =>
        _animator.SetTrigger(_attackTriggers[Random.Range(0, _attackTriggers.Length)]);
}