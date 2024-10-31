using UnityEngine;

public class AttachedStrongObstacleMb : PhysicalObject, ICollisionInteractableWithThreshold
{
    [SerializeField] private Transform _centerOfMass;

    public bool HasFirstCollision { get; private set; }
    public int? SpeedThreshold => null;

    private void Start()
    {
        if (_rigidbody != null && _centerOfMass != null)
            _rigidbody.centerOfMass = _centerOfMass.localPosition;
    }

    public void OnFirstCollision()
    {
        HasFirstCollision = true;
        _rigidbody.isKinematic = false;
    }
}