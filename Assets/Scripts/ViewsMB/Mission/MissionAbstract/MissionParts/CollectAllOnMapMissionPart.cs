using UnityEngine;

[CreateAssetMenu(fileName = "CollectAllOnMapMission", menuName = "Mission/CollectAllOnMapMission", order = 2)]
public class CollectAllOnMapMissionPart : CollectMissionPart
{
    public override Enums.MissionType MissionType => Enums.MissionType.CollectAllOnMap;
}