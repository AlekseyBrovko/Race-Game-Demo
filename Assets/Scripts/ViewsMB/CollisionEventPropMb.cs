using UnityEngine;
using UnityEngine.Events;

public class CollisionEventPropMb : PhysicalObject, ICollisionInterractable
{
    [SerializeField] private UnityEvent _onCollision;

    public bool HasFirstCollision { get; private set; }

    public void OnFirstCollision()
    {
        HasFirstCollision = true;
        _onCollision.Invoke();
    }
}
