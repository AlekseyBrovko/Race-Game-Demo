using Client;
using UnityEngine;

public interface IThrowObject
{
    public string Id { get; }
    public int Entity { get; }
    public INpcMb Owner { get; }
    public Transform Transform { get; }

    public Rigidbody Rigidbody { get; }

    public void Init(GameState state, int entity);
    public void OnSpawn();
    public void OnThrowObject();
    public void OnLostObject();
    public void OnSetObjectInHand(INpcMb npcOwner);
    public void DestroyImmediately();

    public Vector3 OffsetPosition { get; }
    public Vector3 OffsetRotation { get; }
}

public interface ITwoHandsThrowingObject : IThrowObject
{
    public Transform LeftHandPlace { get; }
    public Transform RightHandPlace { get; }
}