using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public struct NpcMeleeAttackComp 
    {
        public bool IsAttacking;

        public Transform AttackPoint;
        public float Radius;

        public int HitCollidersAmount;
        public Collider[] HitColliders;
        public List<Transform> InjuredTransforms;
    }
}