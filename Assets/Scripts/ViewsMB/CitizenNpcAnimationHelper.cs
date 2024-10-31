using System.Collections.Generic;
using UnityEngine;

public class CitizenNpcAnimationHelper : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private StartAnimationType _animationType;

    private bool _handleTalkAnimation;
    private bool _handleIdleWithWeaponAnimation;
    private bool _handleIdleAnimation;

    private int _idleTypeBHash = Animator.StringToHash("IdleB");
    private int _idleWithLoopHash = Animator.StringToHash("LoopIdle");

    private int _changePoseHash = Animator.StringToHash("ChangePose");

    private int _sitOnChairAHash = Animator.StringToHash("SitOnChairA");
    private int _sitOnChairBHash = Animator.StringToHash("SitOnChairB");

    private int _sitOnGroundAHash = Animator.StringToHash("SitOnGroundA");
    private int _sitOnGroundBHash = Animator.StringToHash("SitOnGroundB");

    private int _riffleIdleHash = Animator.StringToHash("Riffle");
    private int _assaultIdleHash = Animator.StringToHash("Assault");
    private int _handgunIdleHash = Animator.StringToHash("Handgun");
    private int _meleeIdleHash = Animator.StringToHash("Melee");
    private int _lookAroundHash = Animator.StringToHash("LookAround");

    private int _changeTalkHash = Animator.StringToHash("ChangeTalk");
    private int _talkValueHash = Animator.StringToHash("TalkValue");

    private int _talkActionHash = Animator.StringToHash("TalkAction");
    private int _talkActionValueHash = Animator.StringToHash("TalkActionValue");

    private TalkBehaviour[] _talkBehaviours;

    private void Start()
    {
        switch (_animationType)
        {
            case StartAnimationType.IdleB:
                _animator.SetTrigger(_idleTypeBHash);
                break;

            case StartAnimationType.IdleWithLoop:
                _animator.SetTrigger(_idleWithLoopHash);
                _handleIdleAnimation = true;
                break;

            case StartAnimationType.SitOnChairA:
                _animator.SetTrigger(_sitOnChairAHash);
                break;

            case StartAnimationType.SitOnChairB:
                _animator.SetTrigger(_sitOnChairBHash);
                break;

            case StartAnimationType.SitOnGroundA:
                _animator.SetTrigger(_sitOnGroundAHash);
                break;

            case StartAnimationType.SitOnGroundB:
                _animator.SetTrigger(_sitOnGroundBHash);
                break;

            case StartAnimationType.RiffleIdleState:
                _animator.SetTrigger(_riffleIdleHash);
                _handleIdleWithWeaponAnimation = true;
                break;

            case StartAnimationType.AssaultIdleState:
                _animator.SetTrigger(_assaultIdleHash);
                _handleIdleWithWeaponAnimation = true;
                break;

            case StartAnimationType.HandgunIdle:
                _animator.SetTrigger(_handgunIdleHash);
                _handleIdleWithWeaponAnimation = true;
                break;

            case StartAnimationType.MeleeCombatIdle:
                _animator.SetTrigger(_meleeIdleHash);
                break;

            case StartAnimationType.Talk:
                _talkBehaviours = _animator.GetBehaviours<TalkBehaviour>();
                foreach (var behaviour in _talkBehaviours)
                    behaviour.OnExit += TalkBehaviour_OnExit;

                _animator.SetInteger(_talkValueHash, Random.Range(1, 3));
                _animator.SetTrigger(_changeTalkHash);
                _handleTalkAnimation = true;
                break;
        }

        _animator.speed = Random.Range(0.8f, 1.2f);
    }

    private void Update()
    {
        if (_handleIdleWithWeaponAnimation)
            HandleIdleWithWeaponAnimation();

        if (_handleTalkAnimation)
            HandleTalkAnimation();

        if (_handleIdleAnimation)
            HandleIdleAnimation();
    }

    private enum StartAnimationType
    {
        Idle, IdleB, IdleWithLoop, SitOnChairA, SitOnChairB, SitOnGroundA,
        SitOnGroundB, RiffleIdleState, AssaultIdleState, HandgunIdle, MeleeCombatIdle, Talk
    }

    private bool _exitPlaned;
    private float _timer = 5f;
    private void HandleTalkAnimation()
    {
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            _exitPlaned = true;
            _animator.SetInteger(_talkActionValueHash, Random.Range(1, 5));
            _animator.SetTrigger(_talkActionHash);
            _timer = _lookAroundMinDiration + Random.Range(0f, _lookAroundDelta);
        }
    }

    private void TalkBehaviour_OnExit()
    {
        if (!_exitPlaned)
        {
            _animator.SetInteger(_talkValueHash, Random.Range(1, 3));
            _animator.SetTrigger(_changeTalkHash);
        }
        _exitPlaned = false;
    }

    private float _lookAroundMinDiration = 5f;
    private float _lookAroundDelta = 7f;
    private void HandleIdleWithWeaponAnimation()
    {
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            _animator.SetTrigger(_lookAroundHash);
            _timer = _lookAroundMinDiration + Random.Range(0f, _lookAroundDelta);
        }
    }

    private float _poseMinDuration = 4f;
    private float _poseDelta = 4f;
    private void HandleIdleAnimation()
    {
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            _animator.SetTrigger(_changePoseHash);
            _timer = _poseMinDuration + Random.Range(0f, _poseDelta);
        }
    }

    private void OnDestroy()
    {
        if (_handleTalkAnimation)
        {
            foreach (var behaviour in _talkBehaviours)
                behaviour.OnExit -= TalkBehaviour_OnExit;
        }
    }
}