using UnityEngine;

namespace Client
{
    struct RideMissionCheckpointEvent
    {
        public int MissionPartId;
        public int CheckpointEntity;
        public Vector3 Position;
    }
}