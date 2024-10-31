namespace Client
{
    struct SpawnCheckpointByMissionPoint
    {
        public MissionPoint MissionPoint;
        public int MissionPartId;
        public bool FinishCheckpoint;
    }

    struct SpawnMissionCollectableEvent
    {
        public int MissionPartId;
        public MissionPoint CollectPoint;
        public MissionCollectableMb CollectablePrefab;
    }

    struct MissionCollectableComp
    {
        public MissionCollectableMb CollectableMb;
    }
}