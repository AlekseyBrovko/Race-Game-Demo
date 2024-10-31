using UnityEngine;

namespace Client
{
    struct Sound3DEvent
    {
        public Enums.SoundEnum SoundType;
        public Vector3 Position;
        public Transform Transform;
        public Rigidbody MainRigidbody;
    }
}