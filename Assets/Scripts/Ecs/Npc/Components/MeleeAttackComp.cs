using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    struct MeleeAttackComp 
    {
        public Transform AttackPoint;
        public float Radius;

        public int HitCollidersAmount;
        public Collider[] HitColliders;
        public List<Transform> InjuredTransforms;
    }
}