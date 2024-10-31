using Client;
using Leopotam.EcsLite;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class CarMb : MonoBehaviour, ICar, IEcsEntityMb, IVisible, IPhysicalInteractable
{
    public int Entity => _entity;
    public WheelDriveType WheelDriveType => _wheelDriveType;
    public CarType CarType => _carType;
    public float MotorPower => _motorPower;
    public BoxCollider MainTriggerCollider => _mainTriggerCollider;
    public Transform CenterOfMass => _centerOfMass;
    public Rigidbody Rigidbody => _rigidbody;
    public Wheel[] Wheels => _wheels;
    public ParticleSystem[] StopFlareParticles => _stopFlareParticles;
    public ParticleSystem[] BackDriveFlareParticles => _backDriveFlareParticles;
    public ParticleSystem[] DeathSmokeParticles => _deathSmokeParticles;
    public float TurningRadius => _turningRadius;
    public float SteeringSpeed => _steeringSpeed;
    public float BreakPower => _breakPower;
    public float MaxSpeed => _maxSpeed;
    public int TransmissionGearsAmount => _transmissionGears;
    public Transform LookPoint => _npcLookPoint;
    public Transform Transform => this.transform;
    public float DriftHelpIndex => _driftHelpIndex;
    public float HandBreakPower => _handBreakPower;
    public EntityType EntityType => Enums.EntityType.Car;
    public PhysicalInteractableType PhysicalInteractableType => Enums.PhysicalInteractableType.Metal;
    public GameObject ArmorGameObject => _armorGameObject;

    [field: SerializeField] public CarMotorType CarMotorType { get; private set; }


    [SerializeField] private WheelDriveType _wheelDriveType;
    [SerializeField] private CarType _carType;
    [SerializeField] private BoxCollider _mainTriggerCollider;
    [SerializeField] private Transform _centerOfMass;
    [SerializeField] private Transform _npcLookPoint;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private GameObject _armorGameObject;
    [SerializeField] private Wheel[] _wheels;
    [SerializeField] private ParticleSystem[] _stopFlareParticles;
    [SerializeField] private ParticleSystem[] _backDriveFlareParticles;
    [SerializeField] private ParticleSystem[] _deathSmokeParticles;
    [SerializeField] private float _motorPower;
    [SerializeField] private float _turningRadius;
    [SerializeField] private float _steeringSpeed;
    [SerializeField] private float _driftHelpIndex = 0.7f;
    [SerializeField] private float _breakPower;
    [SerializeField] private float _handBreakPower = 0;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private int _transmissionGears;

    private int _entity;
    private GameState _state;
    private EcsWorld _world;
    private EcsPool<CarTriggerEvent> _triggerPool;
    private EcsPool<CarCollisionEvent> _collisionPool;
    private EcsPool<CarTriggerExitEvent> _triggerExitPool;
    private EcsPool<PickupEvent> _pickupPool;
    private EcsPool<CarCollisionSoundEvent> _soundPool;

    public void Init(GameState state, int entity)
    {
        _state = state;
        _entity = entity;
        _world = state.EcsWorld;
        _triggerPool = _world.GetPool<CarTriggerEvent>();
        _collisionPool = _world.GetPool<CarCollisionEvent>();
        _triggerExitPool = _world.GetPool<CarTriggerExitEvent>();
        _pickupPool = _world.GetPool<PickupEvent>();
        _soundPool = _world.GetPool<CarCollisionSoundEvent>();

        IDump dump = GetComponentInChildren<IDump>();
        if (dump != null)
            dump.Entity = _entity;
    }

    public void SetShopState()
    {
        _rigidbody.isKinematic = true;
        foreach (var wheel in _wheels)
        {
            wheel.WheelController.enabled = false;
            wheel.EffectsMb.gameObject.SetActive(false);
        }
    }

    private List<GameObject> _onTriggerEnterTempGos = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IPhysicalInteractable physicalMb))
        {
            ref var triggerComp = ref _triggerPool.Add(_world.NewEntity());
            triggerComp.CarEntity = _entity;
            triggerComp.Interactable = physicalMb;
        }
        else if (other.gameObject.TryGetComponent(out PickupMb pickupMb))
        {
            if (!pickupMb.HasPickup)
            {
                pickupMb.HasPickup = true;

                ref var pickupComp = ref _pickupPool.Add(pickupMb.SpawnerEntity);
                pickupComp.Type = pickupMb.PickupType;
                pickupComp.PositionOfPickup = pickupMb.transform.position;
                pickupMb.DestroyPickup();
            }
        }
    }

    private List<GameObject> _onTriggerExitTempGos = new List<GameObject>();
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IPhysicalInteractable physicalMb))
        {
            ref var triggerExitComp = ref _triggerExitPool.Add(_world.NewEntity());
            triggerExitComp.CarEntity = _entity;
            triggerExitComp.Interactable = physicalMb;
        }
    }

    private List<GameObject> _onCollisionEnterTempGos = new List<GameObject>();
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IPhysicalInteractable physicalMb)
            && !_onCollisionEnterTempGos.Contains(collision.gameObject))
        {
            _onCollisionEnterTempGos.Add(collision.gameObject);
            ref var collisionComp = ref _collisionPool.Add(_world.NewEntity());
            collisionComp.CarEntity = _entity;
            collisionComp.Interactable = physicalMb;

            InvokeColisionSoundEvent(collision.contacts[0].point, physicalMb.PhysicalInteractableType);
        }
        else
        {
            InvokeColisionSoundEvent(collision.contacts[0].point, PhysicalInteractableType.Default);
        }
    }

    private void InvokeColisionSoundEvent(Vector3 pos, PhysicalInteractableType type)
    {
        ref var soundComp = ref _soundPool.Add(_world.NewEntity());
        soundComp.Position = pos;
        soundComp.PhysicalInteractableType = type;
    }

    public void ResetCollisionsTemp()
    {
        _onTriggerEnterTempGos.Clear();
        _onTriggerExitTempGos.Clear();
        _onCollisionEnterTempGos.Clear();
    }
}