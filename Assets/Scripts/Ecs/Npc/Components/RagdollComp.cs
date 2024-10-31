using UnityEngine;

namespace Client
{
    struct RagdollComp
    { 
        public RagdollMb RagdollMb;
        public Rigidbody MainRigidbody;
        public Transform Transform;
    }
    struct RagdollCoolDownComp
    {
        public float Timer;
    }
}