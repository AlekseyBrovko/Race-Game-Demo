using Client;
using UnityEngine;
using UnityEngine.AI;

public interface INpcMb
{
    public string Id { get; }
    public string RagdollId { get; }
    public int Entity { get; }
    public int Hp { get; }
    public float WalkSpeed { get; }
    public float RunSpeed { get; set; }

    public Collider MainCollider { get; }
    public Transform Transform { get; }
    public GameObject GameObject { get; }
    public RagdollHelper RagdollHelper { get; }
    public NavMeshAgent Agent { get; }
    public Animator Animator { get; }
    public Enums.NpcType NpcType { get; set; }
    public string ZoneName { get; }

    public void Attack();
    public void SetRunSpeed();
    public void OnDeathEvent();
    public void OnStartHurt();
    public void OnStopHurt();
    public void Update();
}