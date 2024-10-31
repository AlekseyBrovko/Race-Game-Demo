namespace Client
{
    struct MissionPartComp
    {
        public MissionPartBase MissionPart;
        public MissionPartSaveDataBase MissionPartSaveData;
    }

    struct MissionPartOnTimeComp
    {
        public int PreviousUITimerValue;
        public float Timer;
        public MissionPartBase MissionPart;
        public MissionPartSaveDataBase MissionPartSaveData;
    }

    struct MissionPartWithCheckpointsComp
    {
        public int Counter;
        public RidePartSaveData RideMissionPartSaveData;
        public RideByCheckpointsMission RideMissionPart;
    }

    struct MissionPartOneByOneComp { }

    struct MissionPartAllOnMapComp { }

    struct MissionFailedEvent { }
    struct ShowMissionBrifEvent { }
    struct ContinueAfterBrifEvent { }

    struct MissionPartOnKillComp
    {
        public KillByNameMissionPart KillMissionPart;
        public KillPartSaveData KillMissionPartSaveData;
    }

    struct MissionPartOnKillOnZoneComp
    {
        public string ZoneName;
        public int ItemOnMinimapId;
    }

    struct MissionPartWithCollectablesComp
    {
        public int Counter;
        public CollectMissionPart CollectMissionPart;
        public CollectPartSaveData CollectMissionPartSaveData;
    }

    struct MissionPartCompleteEvent
    {

    }

    struct MissionCompleteEvent
    {
    }

    struct MissionCompleteComp
    {
        public float Timer;
    }

    struct MissionCompleteAfterPauseEvent
    {

    }
}