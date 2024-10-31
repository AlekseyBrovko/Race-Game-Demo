using UnityEngine;

namespace Client
{
    struct CarSpawnEvent 
    {
        public Vector3? Position;
        public Quaternion? Rotation;
        public ICarSo CarSo;
    }
}