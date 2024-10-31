using UnityEngine;

[CreateAssetMenu(fileName = "RideAllOnMapMissionPart", menuName = "Mission/RideAllOnMapMissionPart", order = 2)]
public class RideAllOnMapMissionPart : RideByCheckpointsMission
{
    public override Enums.MissionType MissionType => Enums.MissionType.RideByCheckpointsAllOnMap;
}