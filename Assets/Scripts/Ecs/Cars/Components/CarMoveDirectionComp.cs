using UnityEngine;

namespace Client
{
    struct CarMoveDirectionComp
    {
        public Vector3 DirectionByRb;
        public float AngleBetweenRbAndForwardDir;
        public float DotProductBetweenRbAndForwardDir;
    }
}