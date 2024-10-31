using UnityEngine;

namespace Client
{
    struct NpcDamageEvent
    {
        public int DamagedEntity;
        public int DamagerEntity;
        public float DamageValue;
        public Enums.DamageType DamageType;
        public Vector3 PointOfForce;
    }
}