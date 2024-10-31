using Client;
using Leopotam.EcsLite;
using System.Collections.Generic;
using UnityEngine;

public class TwoHandsThrowingKinematicObject : TwoHandsThrowingObject, IPhysicalObject
{
    protected EcsPool<CollisionWithThrowingObjectEvent> _collisionEventPool;
    protected EcsPool<ThrowObjectDelayBeforeDeleteComp> _deletePool;

    protected EcsPool<PhysicalObjectComp> _physicalPool;
    protected EcsPool<PhysicalObjectTriggerEvent> _objectTriggerEventPool;
    protected EcsPool<PhysicalObjectCollisionEvent> _objectCollisionEventPool;

    public override void Init(GameState state, int entity)
    {
        base.Init(state, entity);

        _collisionEventPool = state.EcsWorld.GetPool<CollisionWithThrowingObjectEvent>();
        _deletePool = state.EcsWorld.GetPool<ThrowObjectDelayBeforeDeleteComp>();
        _physicalPool = state.EcsWorld.GetPool<PhysicalObjectComp>();
        _objectTriggerEventPool = state.EcsWorld.GetPool<PhysicalObjectTriggerEvent>();
        _objectCollisionEventPool = state.EcsWorld.GetPool<PhysicalObjectCollisionEvent>();

        InitLikePhysicalObject();
    }

    private void InitLikePhysicalObject()
    {
        _isAttacking = true;
        IsPhysical = true;
        HasInited = true;

        ref var physicalComp = ref _physicalPool.Add(Entity);
        physicalComp.PhysicalMb = this;
    }

    public override void OnSetObjectInHand(INpcMb npcOwner)
    {
        base.OnSetObjectInHand(npcOwner);

        _damagedEntities.Clear();
        _damagedEntities.Add(_npcOwner.Entity);
    }

    protected override void UnparentAndWorkWithObject()
    {
        base.UnparentAndWorkWithObject();

        _deletePool.Add(Entity);
    }

    public override void OnLostObject()
    {
        base.OnLostObject();

        _isAttacking = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsPhysical)
        {
            if (collision.gameObject.TryGetComponent(out IPhysicalInteractable interactableMb)
                && !_onCollisionEnterGos.Contains(collision.gameObject))
            {
                _onCollisionEnterGos.Add(collision.gameObject);
                ref var objectCollisionComp = ref _objectCollisionEventPool.Add(_world.NewEntity());
                objectCollisionComp.FlyingObject = this;
                objectCollisionComp.Interactable = interactableMb;
            }
            if (_isAttacking)
            {
                CheckOwner(collision, out IEcsEntityMb entityMb);
                WorkWithDefaultLayer(collision);
                WorkWithCrateCollision(entityMb);
            }
        }
    }

    List<int> _damagedEntities = new List<int>();
    private void WorkWithCrateCollision(IEcsEntityMb entityMb)
    {
        if (entityMb != null && !_damagedEntities.Contains(entityMb.Entity))
        {
            ref var collisionEventComp = ref _collisionEventPool.Add(entityMb.Entity);
            collisionEventComp.NpcOwner = _npcOwner;
            collisionEventComp.ThrowObject = this;
            _damagedEntities.Add(entityMb.Entity);
        }
    }

    private void CheckOwner(Collision collision, out IEcsEntityMb entityMbret)
    {
        entityMbret = default;
        if (collision.gameObject.TryGetComponent(out IEcsEntityMb entityMb))
        {
            entityMbret = entityMb;
            if (_npcOwner.Entity == entityMb.Entity)
                return;
        }
    }

    private void WorkWithDefaultLayer(Collision collision)
    {
        if (collision.gameObject.layer == 0)
            _isAttacking = false;
    }

    public Enums.PhysicalInteractableType PhysicalInteractableType => Enums.PhysicalInteractableType.Wood;
    public BoxCollider TriggerCollider => _triggerCollider;

    public bool IsPhysical { get; private set; }


    public bool HasInited { get; private set; }


    [SerializeField] private BoxCollider _triggerCollider;

    public void InitOnFirstTriggerWithCar(EcsWorld world, int entity)
    {
    }

    public bool OnBeganPhysical()
    {
        _triggerCollider.enabled = true;
        IsPhysical = true;
        return true;
    }

    public void OnEndPhysical()
    {
        _triggerCollider.enabled = true;
        IsPhysical = false;
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

    public void ResetCollisionsTemp()
    {
        _onCollisionEnterGos.Clear();
        _onTriggerEnterGos.Clear();
    }
}
