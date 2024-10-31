using System;
using UnityEngine;

public class TalkBehaviour : StateMachineBehaviour
{
    public event Action OnExit;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) =>
        OnExit?.Invoke();
}