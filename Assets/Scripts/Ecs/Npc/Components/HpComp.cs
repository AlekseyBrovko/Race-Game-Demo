using FMOD.Studio;
using UnityEngine;

namespace Client
{
    struct HpComp
    {
        public float HpValue;
        public float FullHpValue;
    }
    struct CarSoundComp
    {
        public Transform CarTransform;
        public Rigidbody MainRigidbody;
        public EventInstance EventInstance;
    }
}