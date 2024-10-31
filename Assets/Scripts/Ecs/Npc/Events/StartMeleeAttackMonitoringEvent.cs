using UnityEngine;

namespace Client
{
    public struct StartMeleeAttackMonitoringEvent 
    {
        public Transform AttackPoint;
        public float Radius;
    }
}