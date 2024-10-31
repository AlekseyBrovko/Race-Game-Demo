using Client;
using Leopotam.EcsLite;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public abstract class TwoHandsThrowingObject : MonoBehaviour, ITwoHandsThrowingObject, IPoolObjectMb
{
    public string Id => _id;
    public int Entity => _entity;
    public Rigidbody Rigidbody => _rigidbody;
    public Transform LeftHandPlace => _leftHandTarget;
    public Transform RightHandPlace => _rightHandTarget;
    public Vector3 OffsetPosition => _offsetPosition;
    public Vector3 OffsetRotation => _offsetRotation;
    public Transform Transform => this.transform;
    public INpcMb Owner => _npcOwner;

    public PoolObject PoolObject { get; private set; }

    public GameObject GameObject => this.gameObject;

    [SerializeField] protected string _id;
    [SerializeField] protected Transform _leftHandTarget;
    [SerializeField] protected Transform _rightHandTarget;

    [SerializeField] private Vector3 _offsetPosition;
    [SerializeField] private Vector3 _offsetRotation;

    [SerializeField] private Rigidbody _rigidbody;

    protected GameState _state;
    protected EcsWorld _world;
    protected int _entity;
    protected bool _isAttacking;
    protected INpcMb _npcOwner;

    public virtual void Init(GameState state, int entity)
    {
        _world = state.EcsWorld;
        _state = state;
        _entity = entity;
    }

    public virtual void OnSetObjectInHand(INpcMb npcOwner)
    {
        _npcOwner = npcOwner;
        _rigidbody.isKinematic = true;
        //_isAttacking = false;
    }

    public virtual void OnThrowObject()
    {
        _isAttacking = true;
        UnparentAndWorkWithObject();
    }

    public virtual void OnLostObject()
    {
        UnparentAndWorkWithObject();
    }

    protected virtual void UnparentAndWorkWithObject()
    {
        this.transform.SetParent(null);
        _rigidbody.isKinematic = false;
    }

    public void DestroyImmediately()
    {
        ReturnObjectInPool();
    }

    public void InitObjectOfPool(PoolObject poolObject, GameState state) =>
        PoolObject = poolObject;

    public virtual void OnSpawn()
    {
        _isAttacking = false;
    }

    public void ReturnObjectInPool() 
    {
        this.gameObject.SetActive(false);
        this.transform.SetParent(PoolObject.Parent);
        PoolObject.IsReadyToWork = true;

        //TODO зарефакторить 
        _world.DelEntity(_entity);
        //ReturnObjectInPool sw.Elapsed = 00:00:00.0000355
    }
}