using UnityEngine;

namespace Client
{
    public struct SpawnPickupEvent
    {
        public Vector3 Position;
        public Enums.PickupType PickupType;
    }
}