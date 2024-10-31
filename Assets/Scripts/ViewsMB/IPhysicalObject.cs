using Leopotam.EcsLite;
using UnityEngine;

/// <summary>
/// Для объектов, которые можно подвинуть, которые будут сбивать другие объекты.
/// </summary>
public interface IPhysicalObject : IPhysicalInteractable
{
    public int Entity { get; }
    public Transform Transform { get; }
    public Rigidbody Rigidbody { get; }
    public BoxCollider TriggerCollider { get; }
    public bool IsPhysical { get; }
    public bool HasInited { get; }
    public void InitOnFirstTriggerWithCar(EcsWorld world, int entity);
    public bool OnBeganPhysical();
    public void OnEndPhysical();
    public void ResetCollisionsTemp();
}

/// <summary>
/// Для объектов, у которых есть особое событие при первой коллизии.
/// </summary>
public interface ICollisionInterractable : IPhysicalObject
{
    public bool HasFirstCollision { get; }
    public void OnFirstCollision();
}

/// <summary>
/// Для объектов, у которых есть порог скорости для первого взаимодействия.
/// </summary>
public interface ICollisionInteractableWithThreshold : IPhysicalObject
{
    public bool HasFirstCollision { get; }
    public int? SpeedThreshold { get; }
    public void OnFirstCollision();
}

/// <summary>
/// Для объектов, которые нужно сбивать на определенной скорости
/// </summary>
public interface ITriggerInteractableWithThreshold : IPhysicalObject
{
    public bool HasFirstTrigger { get; }
    public int? SpeedThreshold { get; }
    public void OnFirstTrigger();
}