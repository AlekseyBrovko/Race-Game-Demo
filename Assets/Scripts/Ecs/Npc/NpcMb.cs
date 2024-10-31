using Client;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.AI;

public class NpcMb : MonoBehaviour, INpcMb, IEnemy, IEcsEntityMb,
    IPoolObjectMb, ISpawned, IVisible, IPhysicalInteractable
{
    public int Entity { get; private set; }
    public int Hp => _hp;
    public PoolObject PoolObject { get; private set; }
    public float WalkSpeed => _walkSpeed;
    public float RunSpeed { get; set; }
    public NavMeshAgent Agent => _agent;
    public Animator Animator => _animator;
    public int SpawnerEntity { get; set; }
    public string EnemyId => _enemyId;
    public Enums.NpcType NpcType { get; set; }
    public Transform LookPoint => this.transform;
    public GameObject GameObject => this.gameObject;
    public bool IsFlying { get; private set; }
    public string Id => _enemyId;
    public string RagdollId => _ragdollId;
    public RagdollHelper RagdollHelper => _ragdollHelper;
    public Transform Transform => this.transform;
    public Enums.EntityType EntityType => Enums.EntityType.Npc;
    public string ZoneName { get; set; }
    public Enums.PhysicalInteractableType PhysicalInteractableType => Enums.PhysicalInteractableType.Flash;
    public Collider MainCollider => _mainCollider;

    [Header("Npc Region")]
    [SerializeField] private string _enemyId;
    [SerializeField] private string _ragdollId;
    [SerializeField] private int _hp = 100;
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider _mainCollider;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private AnimationHandlerMb _animationHandler;
    [SerializeField] private RagdollHelper _ragdollHelper;

    [SerializeField] protected float _runSpeed;
    [SerializeField] protected float _walkSpeed;

    protected GameState _state;
    protected EcsWorld _world;

    private EcsPool<NpcStartInObjectPoolSystemsEvent> _deactivatePool;
    private EcsPool<NpcStartSpawnSystemEvent> _spawnPool;

    public virtual void InitObjectOfPool(PoolObject poolObject, GameState state)
    {
        _state = state;
        _world = state.EcsWorld;
        Entity = _world.NewEntity();
        PoolObject = poolObject;

        _spawnPool = _world.GetPool<NpcStartSpawnSystemEvent>();
        _deactivatePool = _world.GetPool<NpcStartInObjectPoolSystemsEvent>();

        EcsPool<NpcInitEvent> initPool = _world.GetPool<NpcInitEvent>();

        ref var initComp = ref initPool.Add(Entity);
        initComp.NpcGo = this.gameObject;
        initComp.NpcMb = this;

        _animationHandler.Init(state, Entity, _animator);
    }

    public virtual void SetRunSpeed() =>
        RunSpeed = _runSpeed;

    public virtual void Attack() =>
        _animationHandler.Attack();

    public virtual void ReturnObjectInPool()
    {
        //TODO перенести это тоже отсюда
        this.gameObject.SetActive(false);

        _animationHandler.gameObject.transform.localPosition = Vector3.zero;
        _animationHandler.gameObject.transform.rotation = this.transform.rotation;

        this.transform.SetParent(PoolObject.Parent);
        PoolObject.IsReadyToWork = true;
    }

    //TODO сюда закинуть всё что связано со смертью
    public virtual void OnDeathEvent()
    {

    }

    public virtual void OnSpawn()
    {
        //TODO вот это тоже перенести

        _spawnPool.Add(Entity);
        this.gameObject.SetActive(true);
        _mainCollider.enabled = true;
        _agent.enabled = true;
    }

    public virtual void OnStartHurt() { }
    public virtual void OnStopHurt() { }
    public virtual void Update() { }
}