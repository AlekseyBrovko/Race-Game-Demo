using UnityEngine;

public interface ITriggerInteractable
{
    public bool IsPhysical { get; }
    public Rigidbody Rigidbody { get; }
}