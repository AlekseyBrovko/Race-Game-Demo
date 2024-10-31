using UnityEngine;

public class DeathData : IDataForSetState
{
    public int KillerEntity { get; }
    public Vector3 DirectionAndForceForRagdoll { get; }
    public Enums.DamageType DamageType { get; }

    public DeathData(int killerEntity, Enums.DamageType damageType)
    {
        KillerEntity = killerEntity;
        DirectionAndForceForRagdoll = Vector3.zero;
        DamageType = damageType;
    }

    public DeathData(int killerEntity, Vector3 directionAndForceForRagdoll)
    {
        KillerEntity = killerEntity;
        DirectionAndForceForRagdoll = directionAndForceForRagdoll;
    }
}