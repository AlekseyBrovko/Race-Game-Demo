using UnityEngine;

namespace Client
{
    struct CarComp
    {
        public ICar CarMb;
        public ICameraOffset CameraOffsetMb;
        public Wheel[] Wheels;

        public AnimationCurve ThrottleCurve;
        public int TransmissionGearsAmount;

        public Rigidbody Rigidbody;
        public Transform CarTransform;

        public float WidthWheelBase;
        public float LengthWheelBase;

        public float DriftHelpIndex;

        public ParticleSystem[] DeathSmokeParticles;
        public GameObject ArmorGameObject;

        public CarCharacteristics CarCharacteristic;
    }
}