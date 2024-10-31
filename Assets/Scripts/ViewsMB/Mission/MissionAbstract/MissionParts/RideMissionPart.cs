using UnityEngine;

[CreateAssetMenu(fileName = "RideOneByOneMissionPart", menuName = "Mission/RideOneByOneMissionPart", order = 2)]
public class RideOneByOneMissionPart : RideByCheckpointsMission
{
    public override Enums.MissionType MissionType => Enums.MissionType.RideByCheckpointsOneByOne;
}