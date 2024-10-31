using Client;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RangeThrowingNpcMb : NpcMb, IRangeThrowingNpc
{
    public float RangeOfAttack => _rangeOfAttack;
    public float RangeAttackCoolDown => _rangeAttackCoolDown;
    public float DistanceOfMeleeAttack => _distanceOfMeleeAttack;
    public float DefaultRunSpeed => _runSpeed;
    public bool CanMeleeAttack { get; set; }
    public bool CanRun { get; set; }
    public IThrowObject ThrowObjectInHands { get; set; }
    public ITwoHandsThrowingObject TwoHandsThrowingObject { get; set; }
    public bool CanThrowObject => ThrowObjectInHands != null;
    [field: SerializeField] public string ThrowObjectId { get; private set; }
    public GameObjectsPool PoolOfThrowObjects { get; set; }

    [Header("Range Region")]
    [SerializeField] private float _rangeOfAttack;
    [SerializeField] private float _rangeAttackCoolDown;
    [SerializeField] private float _distanceOfMeleeAttack;

    [SerializeField] private Rig _handsRig;
    [SerializeField] private RigBigObjectInHandsHandler _twoHandsRigHandler;
    [SerializeField] private GameObject _throwObjectPrefab;

    private EcsPool<ThrowObjectInitEvent> _throwObjectInitPool;
    private EcsPool<NpcSetRunSpeedEvent> _setRunSpeedEvent;

    private Coroutine _delayBeforeGetObjectInHandCor;
    private Coroutine _throwAnimationCor;
    private Coroutine _getInHandAnimationCor;

    private float _durationOfGetObjectInHand = 0.5f;
    private float _durationOfGetInHandObjectAnimation = 0.2f;

    private float _throwAnimationDuration = 0.3f;

    protected virtual void StartTimerForSetObjectInHand() =>
        _delayBeforeGetObjectInHandCor = StartCoroutine(DelayBeforeGetObjectInHands());


    //TODO переписать на системы
    protected virtual IEnumerator DelayBeforeGetObjectInHands()
    {
        //TODO это значение нужно пробрасывать, работает в связке с атак кулдаун
        yield return new WaitForSeconds(
            _durationOfGetObjectInHand - _durationOfGetInHandObjectAnimation);

        StartGetObjectInHandAnimation();

        yield return new WaitForSeconds(_durationOfGetInHandObjectAnimation);

        TakeObjectForRangeAttack();
    }

    public override void InitObjectOfPool(PoolObject poolObject, GameState state)
    {
        base.InitObjectOfPool(poolObject, state);

        _throwObjectInitPool = _world.GetPool<ThrowObjectInitEvent>();
        _setRunSpeedEvent = _world.GetPool<NpcSetRunSpeedEvent>();

        _twoHandsRigHandler.Init(state, Entity);
    }

    public override void SetRunSpeed()
    {
        if (CanRun)
            RunSpeed = _runSpeed;
        else
            RunSpeed = _walkSpeed;

        if (!_setRunSpeedEvent.Has(Entity))
            _setRunSpeedEvent.Add(Entity);
    }

    public void OnRangeAttack()
    {
        CanRun = true;
        ThrowObjectInHands = null;
        TwoHandsThrowingObject = null;
        SetRunSpeed();
        StartTimerForSetObjectInHand();
        StartThrowAnimation();
    }

    public override void OnSpawn()
    {
        base.OnSpawn();

        _handsRig.weight = 0;
        StartTimerForSetObjectInHand();
    }

    public void TakeObjectForRangeAttack()
    {
        //GameObject throwGO = Instantiate(_throwObjectPrefab, this.transform);

        //TODO get from pool
        GameObject throwGO = PoolOfThrowObjects.GetFromPool();
        throwGO.transform.SetParent(this.transform);

        ThrowObjectInHands = throwGO.GetComponent<IThrowObject>();
        ThrowObjectInHands.OnSpawn();
        throwGO.SetActive(true);
        throwGO.transform.localPosition = ThrowObjectInHands.OffsetPosition;
        throwGO.transform.localRotation = Quaternion.Euler(ThrowObjectInHands.OffsetRotation);

        ITwoHandsThrowingObject twoHandsObj = ThrowObjectInHands as ITwoHandsThrowingObject;
        TwoHandsThrowingObject = twoHandsObj;

        _twoHandsRigHandler.OnTakeObjectInHands(twoHandsObj);
        twoHandsObj.OnSetObjectInHand(this);

        CanRun = false;

        ref var throwInitComp = ref _throwObjectInitPool.Add(_state.EcsWorld.NewEntity());
        throwInitComp.ThrowGo = throwGO;

        SetRunSpeed();
    }

    public override void OnDeathEvent()
    {
        base.OnDeathEvent();
        _twoHandsRigHandler.TurnOffRig();

        if (ThrowObjectInHands != null)
        {
            ThrowObjectInHands.OnLostObject();
            ThrowObjectInHands = null;
            TwoHandsThrowingObject = null;
        }

        if (_delayBeforeGetObjectInHandCor != null)
            StopCoroutine(_delayBeforeGetObjectInHandCor);

        if (_throwAnimationCor != null)
            StopCoroutine(_throwAnimationCor);

        if (_getInHandAnimationCor != null)
            StopCoroutine(_getInHandAnimationCor);
    }

    public override void ReturnObjectInPool()
    {
        base.ReturnObjectInPool();

        if (ThrowObjectInHands != null)
        {
            ThrowObjectInHands.DestroyImmediately();
            ThrowObjectInHands = null;
            TwoHandsThrowingObject = null;
        }

        if (_delayBeforeGetObjectInHandCor != null)
            StopCoroutine(_delayBeforeGetObjectInHandCor);

        if (_throwAnimationCor != null)
            StopCoroutine(_throwAnimationCor);

        if (_getInHandAnimationCor != null)
            StopCoroutine(_getInHandAnimationCor);
    }

    public void OnStartAiming()
    {
        //присели, начинаем целиться
        Debug.Log("присели, начинаем целиться");
    }

    protected virtual void StartThrowAnimation() =>
        _throwAnimationCor = StartCoroutine(ThrowAnimationCor());

    private IEnumerator ThrowAnimationCor()
    {
        float timer = _throwAnimationDuration;
        while (_handsRig.weight != 0)
        {
            timer -= Time.deltaTime;
            _handsRig.weight = Mathf.Lerp(1, 0, 1f - timer / _throwAnimationDuration);
            if (_handsRig.weight < 0)
                _handsRig.weight = 0;
            yield return null;
        }
    }

    protected virtual void StartGetObjectInHandAnimation() =>
        _getInHandAnimationCor = StartCoroutine(GetObjectCor());

    private IEnumerator GetObjectCor()
    {
        float timer = _durationOfGetInHandObjectAnimation;
        while (_handsRig.weight != 1)
        {
            timer -= Time.deltaTime;
            _handsRig.weight = Mathf.Lerp(0f, 1f, 1f - timer / _durationOfGetInHandObjectAnimation);
            if (_handsRig.weight > 1)
                _handsRig.weight = 1;
            yield return null;
        }
    }

    public override void OnStartHurt()
    {
        base.OnStartHurt();

        if (_throwAnimationCor != null)
            StopCoroutine(_throwAnimationCor);

        if (_getInHandAnimationCor != null)
            StopCoroutine(_getInHandAnimationCor);
    }

    public override void OnStopHurt()
    {
        base.OnStopHurt();

        if (_throwAnimationCor != null)
            StopCoroutine(_throwAnimationCor);

        if (_getInHandAnimationCor != null)
            StopCoroutine(_getInHandAnimationCor);
    }

    public override void Update()
    {
        base.Update();

        HandleRig();
    }

    private void HandleRig()
    {
        if (TwoHandsThrowingObject != null)
            _twoHandsRigHandler.HandleRig(TwoHandsThrowingObject);
    }
}