using Client;
using Leopotam.EcsLite;
using System.Collections.Generic;
using UnityEngine;

public abstract class PhysicalObject : MonoBehaviour, IPhysicalObject
{
    [Header("PhysicalObject")]
    [SerializeField] protected Rigidbody _rigidbody;
    [SerializeField] protected BoxCollider _triggerCollider;

    public int Entity { get; private set; }
    public bool IsPhysical { get; private set; }
    public bool HasInited { get; private set; }
    public Rigidbody Rigidbody => _rigidbody;
    public BoxCollider TriggerCollider => _triggerCollider;
    public Transform Transform => this.transform;
    public virtual Enums.PhysicalInteractableType PhysicalInteractableType { get; protected set; } = Enums.PhysicalInteractableType.Wood;

    protected EcsWorld _world;
    protected EcsPool<PhysicalObjectTriggerEvent> _objectTriggerEventPool;
    protected EcsPool<PhysicalObjectCollisionEvent> _objectCollisionEventPool;

    public virtual void InitOnFirstTriggerWithCar(EcsWorld world, int entity)
    {
        _world = world;
        Entity = entity;
        HasInited = true;
        _objectTriggerEventPool = _world.GetPool<PhysicalObjectTriggerEvent>();
        _objectCollisionEventPool = _world.GetPool<PhysicalObjectCollisionEvent>();
    }

    public virtual bool OnBeganPhysical()
    {
        _triggerCollider.enabled = true;
        IsPhysical = true;
        _rigidbody.isKinematic = false;
        return true;
    }

    public virtual void OnEndPhysical()
    {
        _triggerCollider.enabled = true;
        IsPhysical = false;
    }

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

    private List<GameObject> _onCollisionEnterGos = new List<GameObject>();
    protected virtual void OnCollisionEnter(Collision collision)
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
        }
    }

    public void ResetCollisionsTemp()
    {
        _onCollisionEnterGos.Clear();
        _onTriggerEnterGos.Clear();
    }
}