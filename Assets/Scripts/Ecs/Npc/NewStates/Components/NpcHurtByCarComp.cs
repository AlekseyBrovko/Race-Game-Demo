using UnityEngine;

namespace Client
{
    public struct NpcHurtByCarComp 
    {
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public float Timer;
        public float Duration;
        public Transform TargetTransform;
    }
}