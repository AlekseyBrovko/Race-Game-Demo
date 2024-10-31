using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    struct NpcLookAroundComp
    {
        public float TimerOfCheck;
        public List<Transform> Targets;
        public Transform NearestTarget;
    }
}