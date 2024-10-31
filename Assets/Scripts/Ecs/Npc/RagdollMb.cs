using Client;
using Leopotam.EcsLite;
using UnityEngine;

public class RagdollMb : MonoBehaviour, IPoolObjectMb
{
    public int Entity { get; private set; }
    public PoolObject PoolObject { get; private set; }
    public GameObject GameObject => this.gameObject;

    [field: SerializeField] public Rigidbody MainRigidbody { get; private set; }

    [SerializeField] private Collider[] _colliders;
    [SerializeField] private Rigidbody[] _bodies;
    [SerializeField] private Joint[] _joints;
    [SerializeField] private RagdollHelper _ragdollHelper;

    public void SetTransforms(RagdollHelper ragdollHelper)
    {
        _ragdollHelper.Main.position = ragdollHelper.Main.position;
        _ragdollHelper.Main.rotation = ragdollHelper.Main.rotation;

        for (int i = 0; i < _ragdollHelper.BodyParts.Length; i++)
        {
            _ragdollHelper.BodyParts[i].position = ragdollHelper.BodyParts[i].position;
            _ragdollHelper.BodyParts[i].rotation = ragdollHelper.BodyParts[i].rotation;
        }
    }

    public void MakeKinematic()
    {
        foreach (var rb in _bodies)
        {
            rb.detectCollisions = false;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        foreach (var col in _colliders)
        {
            col.enabled = false;
        }

        foreach (var joint in _joints)
        {
            joint.enableCollision = false;
        }
    }

    public void MakePhysical()
    {
        foreach (var rb in _bodies)
        {
            rb.detectCollisions = true;
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        foreach (var col in _colliders)
        {
            col.enabled = true;
        }

        foreach (var joint in _joints)
        {
            joint.enableCollision = true;
        }
    }

    private EcsWorld _world;
    private EcsPool<RagdollInitEvent> _initPool;
    public void InitObjectOfPool(PoolObject poolObject, GameState state)
    {
        PoolObject = poolObject;
        _world = state.EcsWorld;
        _initPool = _world.GetPool<RagdollInitEvent>();
        Entity = _world.NewEntity();
        ref var initComp = ref _initPool.Add(Entity);
        initComp.RagdollMb = this;
        MakeKinematic();
    }

    public void ReturnObjectInPool()
    {
        this.gameObject.SetActive(false);
        MakeKinematic();

        this.transform.SetParent(PoolObject.Parent);
        PoolObject.IsReadyToWork = true;
    }

    public void OnSpawn()
    {
        this.gameObject.SetActive(true);
        MakePhysical();
    }
}