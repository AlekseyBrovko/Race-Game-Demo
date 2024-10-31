using UnityEngine;

[CreateAssetMenu(fileName = "CollectOneByOneMission", menuName = "Mission/CollectOneByOneMission", order = 2)]
public class CollectOneByOneMissionPart : CollectMissionPart
{
    public override Enums.MissionType MissionType => Enums.MissionType.CollectOneByOne;
}
