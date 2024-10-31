using UnityEngine;

namespace Client
{
    public struct PhysicalObjectTriggerEvent
    {
        public IPhysicalObject FlyingObject;
        public IPhysicalInteractable Interactable;
    }

    public struct PhysicalObjectCollisionEvent
    {
        public IPhysicalObject FlyingObject;
        public IPhysicalInteractable Interactable;
    }

    public struct PhysicalObjectComp
    {
        public IPhysicalObject PhysicalMb;
    }

    struct PhysicalObjectIsFlyingComp
    {
        public float Timer;
        public Transform ObjectTransform;
        public Rigidbody Rigidbody;
        public BoxCollider TriggerCollider;
        public Vector3 ColliderStartPos;
        public float SpeedKmH;
        public float PreviousFrameSpeedKmH;
    }

    struct PhysicalObjectStartFlyEvent
    {
        public IPhysicalObject PhysicalMb;
    }

    public struct PhysicalObjectIsMovingByCarComp 
    {
        public int CarEntity;
    }
}