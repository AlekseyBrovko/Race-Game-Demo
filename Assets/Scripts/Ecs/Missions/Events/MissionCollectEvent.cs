using UnityEngine;

namespace Client
{
    struct MissionCollectEvent
    {
        public int MissionPartId;
        public int CollectableEntity;
        public Vector3 Position;
    }
}