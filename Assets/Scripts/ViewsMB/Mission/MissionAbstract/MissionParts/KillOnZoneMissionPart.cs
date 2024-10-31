using UnityEngine;

[CreateAssetMenu(fileName = "KillOnZoneMission", menuName = "Mission/KillOnZoneMission", order = 2)]
public class KillOnZoneMissionPart : KillByNameMissionPart
{
    public override Enums.MissionType MissionType => Enums.MissionType.KillOnZone;
    [field: SerializeField] public string ZoneName { get; private set; }
}