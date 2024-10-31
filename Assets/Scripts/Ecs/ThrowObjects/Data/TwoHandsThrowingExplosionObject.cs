using Client;
using Leopotam.EcsLite;
using System.Collections.Generic;
using UnityEngine;

public class TwoHandsThrowingExplosionObject : TwoHandsThrowingObject, IPhysicalObject
{
    protected EcsPool<ExplosionThrowingObjectEvent> _explosionPool;
    protected EcsPool<ThrowObjectDelayBeforeDeleteComp> _deletePool;

    protected EcsPool<PhysicalObjectComp> _physicalPool;
    protected EcsPool<PhysicalObjectTriggerEvent> _objectTriggerEventPool;
    protected EcsPool<PhysicalObjectCollisionEvent> _objectCollisionEventPool;

    [SerializeField] private BoxCollider _triggerCollider;
    public Enums.PhysicalInteractableType PhysicalInteractableType => Enums.PhysicalInteractableType.Metal;
    public BoxCollider TriggerCollider => _triggerCollider;
    public bool IsPhysical { get; private set; }
    public bool HasInited { get; private set; }

    private bool _canBePhysical;

    public override void Init(GameState state, int entity)
    {
        base.Init(state, entity);

        _explosionPool = state.EcsWorld.GetPool<ExplosionThrowingObjectEvent>();
        _deletePool = state.EcsWorld.GetPool<ThrowObjectDelayBeforeDeleteComp>();

        _physicalPool = state.EcsWorld.GetPool<PhysicalObjectComp>();
        _objectTriggerEventPool = state.EcsWorld.GetPool<PhysicalObjectTriggerEvent>();
        _objectCollisionEventPool = state.EcsWorld.GetPool<PhysicalObjectCollisionEvent>();
    }

    private void InitLikePhysicalObject()
    {
        HasInited = true;
        ref var physicalComp = ref _physicalPool.Add(Entity);
        physicalComp.PhysicalMb = this;
    }

    public override void OnThrowObject()
    {
        base.OnThrowObject();
    }

    public override void OnLostObject()
    {
        base.OnLostObject();
        _canBePhysical = true;

        InitLikePhysicalObject();

        _deletePool.Add(Entity);
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        _triggerCollider.enabled = false;
        _canBePhysical = false;
        IsPhysical = false;
        HasInited = false;
    }

    private List<GameObject> _onCollisionEnterGos = new List<GameObject>();
    private List<GameObject> _onTriggerEnterGos = new List<GameObject>();
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (IsPhysical)
        {
            if (other.TryGetComponent(out IPhysicalInteractable interactableMb)
                && !_onTriggerEnterGos.Contains(other.gameObject))
            {
                _onTriggerEnterGos.Add(other.gameObject);
                ref var objectTriggerComp = ref _objectTriggerEventPool.Add(_world.NewEntity());
                objectTriggerComp.FlyingObject = this;
                objectTriggerComp.Interactable = interactableMb;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsPhysical)
        {
            if (collision.gameObject.TryGetComponent(out IEcsEntityMb entityMb))
                WorkWithPhysicalCollision(collision, entityMb);
        }
        else
        {
            if (collision.gameObject.TryGetComponent(out IEcsEntityMb entityMb))
                if (entityMb.Entity == _npcOwner.Entity)
                    return;
            Explosion();
        }
    }

    private void WorkWithPhysicalCollision(Collision collision, IEcsEntityMb entityMb)
    {
        if (collision.gameObject.TryGetComponent(out IPhysicalInteractable interactableMb)
                && !_onCollisionEnterGos.Contains(collision.gameObject))
        {
            _onCollisionEnterGos.Add(collision.gameObject);
            ref var objectCollisionComp = ref _objectCollisionEventPool.Add(_world.NewEntity());
            objectCollisionComp.FlyingObject = this;
            objectCollisionComp.Interactable = interactableMb;
        }
    }

    private void Explosion()
    {
        if (_isAttacking)
        {
            ref var explosionComp = ref _explosionPool.Add(Entity);
            explosionComp.ThrowObject = this;
            _isAttacking = false;
        }
    }

    public void InitOnFirstTriggerWithCar(EcsWorld world, int entity) { }

    public bool OnBeganPhysical()
    {
        if (_canBePhysical)
        {
            _triggerCollider.enabled = true;
            IsPhysical = true;
        }
        return _canBePhysical;
    }

    public void OnEndPhysical()
    {
        if (_canBePhysical)
        {
            _triggerCollider.enabled = true;
            IsPhysical = false;
        }
    }

    public void ResetCollisionsTemp()
    {
        _onCollisionEnterGos.Clear();
        _onTriggerEnterGos.Clear();
    }
}