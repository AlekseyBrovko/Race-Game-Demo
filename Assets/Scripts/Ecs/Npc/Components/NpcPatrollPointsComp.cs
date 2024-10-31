using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    struct NpcPatrollPointsComp
    {
        public List<Transform> PatrollPoints;
        public Transform CurrentPoint;
        public Transform PreviousPoint;
    }
}